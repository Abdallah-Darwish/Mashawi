using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.WishList;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class WishListController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public WishListController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    [LoggedInFilter]
    [HttpPost("GetAll")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<int[]>> GetAll([FromBody] GetAllDto dto)
    {
        var userId = this.GetUser()!.Id;
        var reviews = await _dbContext.WhishListItems
            .Where(r => r.CustomerId == userId)
            .Skip(dto.Offset)
            .Take(dto.Count)
            .OrderBy(d => d.Id)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(reviews);
    }
    [LoggedInFilter]
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(WishListItemDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] int[] ids)
    {
        var wishlist = _dbContext.WhishListItems
            .Include(w => w.Book)
            .Include(w => w.Customer)
            .AsQueryable();

        var existingItems = wishlist.Where(a => ids.Contains(a.Id));
        var nonExistingItems = ids.Except(existingItems.Select(a => a.Id)).ToArray();

        if (nonExistingItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following items don't exist.",
                    Data = new() { ["NonExistingWishListItems"] = nonExistingItems }
                });
        }
        var userId = this.GetUser()!.Id;
        var notOwnedItems = await existingItems
            .Where(i => i.CustomerId != userId)
            .Select(i => i.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        if (notOwnedItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorDto
                {
                    Description = "You don't own the following items.",
                    Data = new() { ["NotOwnedItems"] = notOwnedItems }
                });
        }
        return Ok(_mapper.ProjectTo<WishListItemDto>(existingItems));
    }
    [LoggedInFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(WishListItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookReviewDto>> Create([FromQuery] int bookId)
    {
        var book = await _dbContext.Books.FindAsync(bookId).ConfigureAwait(false);
        if (book == null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "There is no book with the following id.",
                    Data = new() { ["BookId"] = bookId }
                });
        }
        var user = this.GetUser()!;

        if (await _dbContext.WhishListItems.AnyAsync(i => i.CustomerId == user.Id && i.BookId == bookId).ConfigureAwait(false))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorDto
                {
                    Description = "You already have the following book in your wish list.",
                    Data = new() { ["BookId"] = bookId }
                });
        }
        WishListItem entity = new()
        {
            CustomerId = user.Id,
            BookId = bookId
        };
        await _dbContext.WhishListItems.AddAsync(entity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id } }, _mapper.Map<WishListItemDto>(entity));
    }
    [LoggedInFilter]
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        var items = _dbContext.WhishListItems.AsQueryable();

        var existingItems = items.Where(a => ids.Contains(a.Id));
        var nonExistingItems = ids.Except(existingItems.Select(a => a.Id)).ToArray();

        if (nonExistingItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following wish list items don't exist.",
                    Data = new() { ["NonExistingWishListItems"] = nonExistingItems }
                });
        }
        var user = this.GetUser()!;

        var notOwnedItems = await existingItems
            .Where(r => r.CustomerId != user.Id)
            .Select(r => r.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);

        if (notOwnedItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
            new ErrorDto
            {
                Description = "You don't own the following wish list items.",
                Data = new() { ["NotOwnedWishListItems"] = notOwnedItems }
            });
        }

        _dbContext.WhishListItems.RemoveRange(existingItems);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [LoggedInFilter]
    [HttpGet("IsInWishList")]
    public async Task<ActionResult<bool>> IsInWishList([FromQuery] int bookId)
    {
        var user = this.GetUser()!;
        return await _dbContext.WhishListItems
            .AnyAsync(wi => wi.CustomerId == user.Id && wi.BookId == bookId)
            .ConfigureAwait(false);
    }
}
