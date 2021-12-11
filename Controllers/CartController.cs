using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.WishList;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public CartController(AppDbContext dbContext, IMapper mapper)
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
        var items = await _dbContext.CartsItems
            .Where(r => r.CustomerId == userId)
            .Skip(dto.Offset)
            .Take(dto.Count)
            .OrderBy(d => d.Id)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(items);
    }
    [LoggedInFilter]
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CartItemDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] int[] ids)
    {
        var cart = _dbContext.CartsItems
            .Include(w => w.Book)
            .Include(w => w.Customer)
            .AsQueryable();

        var existingItems = cart.Where(a => ids.Contains(a.Id));
        var nonExistingItems = ids.Except(existingItems.Select(a => a.Id)).ToArray();

        if (nonExistingItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following items don't exist.",
                    Data = new() { ["NonExistingCartItems"] = nonExistingItems }
                });
        }
        var userId = this.GetUser()!.Id;
        var notOwnedItems = await existingItems
            .Where(i => i.CustomerId != userId)
            .ToArrayAsync()
            .ConfigureAwait(false);
        if (notOwnedItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorDto
                {
                    Description = "You don't own the following items.",
                    Data = new() { ["NotOwnedCartItems"] = notOwnedItems }
                });
        }
        return Ok(_mapper.ProjectTo<CartItemDto>(existingItems));
    }
    [LoggedInFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(CartItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartItemDto>> Create([FromBody] CreateCartItemDto dto)
    {
        var user = this.GetUser()!;
        CartItem entity = new()
        {
            CustomerId = user.Id,
            BookId = dto.BookId,
            Quantity = dto.Quantity
        };
        await _dbContext.CartsItems.AddAsync(entity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id } }, _mapper.Map<BookReviewDto>(entity));
    }
    [LoggedInFilter]
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        var items = _dbContext.CartsItems.AsQueryable();

        var existingItems = items.Where(a => ids.Contains(a.Id));
        var nonExistingItems = ids.Except(existingItems.Select(a => a.Id)).ToArray();

        if (nonExistingItems.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following cart items don't exist.",
                    Data = new() { ["NonExistingCartItems"] = nonExistingItems }
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
                Description = "You don't own the following cart items.",
                Data = new() { ["NotOwnedCartItems"] = notOwnedItems }
            });
        }

        _dbContext.CartsItems.RemoveRange(existingItems);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [LoggedInFilter]
    [HttpPatch("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateCartItemDto update)
    {
        var item = await _dbContext.CartsItems.FindAsync(update.Id).ConfigureAwait(false);
        if (update.Quantity != null)
        {
            item.Quantity = update.Quantity.Value;
        }
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
}