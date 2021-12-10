using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Dto.BooksReviews;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class BookReviewController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public BookReviewController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    [HttpPost("GetAll")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<int[]>> GetAll([FromBody] GetAllBookReviewsDto dto)
    {
        var userId = this.GetUser()?.Id ?? -1;
        var reviews = await _dbContext.BooksReviews
            .Where(r => r.BookId == dto.Id)
            .Skip(dto.Offset)
            .Take(dto.Count)
            //My review first
            .OrderBy(d => d.UserId == userId ? 0 : 1)
            .OrderBy(d => d.Id)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(reviews);
    }
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BookReviewDto[]), StatusCodes.Status200OK)]
    public IActionResult Get([FromBody] int[] ids)
    {
        var reviews = _dbContext.BooksReviews
            .Include(r => r.User)
            .Include(r => r.Book)
            .AsQueryable();

        var existingReviews = reviews.Where(a => ids.Contains(a.Id));
        var nonExistingReviews = ids.Except(existingReviews.Select(a => a.Id)).ToArray();

        if (nonExistingReviews.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following reviews don't exist.",
                    Data = new() { ["NonExistingReviews"] = nonExistingReviews }
                });
        }
        return Ok(_mapper.ProjectTo<BookReviewDto>(existingReviews));
    }
    [LoggedInFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(BookReviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookReviewDto>> Create([FromBody] CreateBookReviewDto dto)
    {
        var user = this.GetUser()!;
        BookReview entity = new()
        {
            BookId = dto.BookId,
            UserId = user.Id,
            Content = dto.Content,
            Rating = dto.Rating
        };
        await _dbContext.BooksReviews.AddAsync(entity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id } }, _mapper.Map<BookReviewDto>(entity));
    }
    [LoggedInFilter]
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        var reviews = _dbContext.BooksReviews.AsQueryable();

        var existingReviews = reviews.Where(a => ids.Contains(a.Id));
        var nonExistingReviews = ids.Except(existingReviews.Select(a => a.Id)).ToArray();

        if (nonExistingReviews.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following reviews don't exist.",
                    Data = new() { ["NonExistingReviews"] = nonExistingReviews }
                });
        }
        var user = this.GetUser()!;
        if (!user.IsAdmin)
        {
            var notOwnedReviews = await existingReviews
                .Where(r => r.UserId != user.Id)
                .Select(r => r.Id)
                .ToArrayAsync()
                .ConfigureAwait(false);

            if (notOwnedReviews.Length > 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorDto
                {
                    Description = "You don't own the following reviews.",
                    Data = new() { ["NotOwnedReviews"] = notOwnedReviews }
                });
            }
        }
        _dbContext.BooksReviews.RemoveRange(existingReviews);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [AdminFilter]
    [HttpPatch("Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateBookReviewDto update)
    {
        var review = await _dbContext.BooksReviews.FindAsync(update.Id).ConfigureAwait(false);
        if (update.Content != null)
        {
            review.Content = update.Content;
        }
        if (update.Rating != null)
        {
            review.Rating = update.Rating.Value;
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }

}