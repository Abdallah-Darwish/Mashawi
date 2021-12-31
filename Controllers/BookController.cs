using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.WishList;
using Mashawi.Services;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly BookFileManager _bookFileManager;
    public BookController(AppDbContext dbContext, IMapper mapper, BookFileManager bookFileManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _bookFileManager = bookFileManager;
    }
    [HttpPost("GetAll")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<int[]>> GetAll([FromBody] GetAllDto dto)
    {
       // var userId = this.GetUser()!.Id;
        var books = await _dbContext.Books
            .Skip(dto.Offset)
            .Take(dto.Count)
            .OrderBy(d => d.Id)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(books);
    }
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BookMetadataDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BookDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] int[] ids, bool metadata = false)
    {
        var books = _dbContext.Books
            .AsQueryable();

        var existingBooks = books.Where(a => ids.Contains(a.Id));
        var nonExistingBooks = ids.Except(existingBooks.Select(a => a.Id)).ToArray();

        if (nonExistingBooks.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following books don't exist.",
                    Data = new() { ["NonExistingBooks"] = nonExistingBooks }
                });
        }
        if (metadata)
        {
            return Ok(_mapper.ProjectTo<BookMetadataDto>(existingBooks));
        }
        return Ok(_mapper.ProjectTo<BookDto>(existingBooks));
    }
    [AdminFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookReviewDto>> Create([FromBody] CreateBookDto dto)
    {
        var user = this.GetUser()!;
        Book entity = new()
        {
            AuthorId = dto.AuthorId,
            Description = dto.Description,
            Genre = dto.Genre,
            Isbn = dto.Isbn,
            IsUsed = dto.IsUsed,
            Language = dto.Language,
            Price = dto.Price,
            PublishDate = dto.PublishDate.Date.ToUniversalTime(),
            RatersCount = 0,
            RatingSum = 0,
            Title = dto.Title,
        };
        await _dbContext.Books.AddAsync(entity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id }, metadata = false }, _mapper.Map<BookDto>(entity));
    }
    [AdminFilter]
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        var books = _dbContext.Books.AsQueryable();

        var existingBooks = books.Where(a => ids.Contains(a.Id));
        var nonExistingBooks = ids.Except(existingBooks.Select(a => a.Id)).ToArray();

        if (nonExistingBooks.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following books don't exist.",
                    Data = new() { ["NonExistingBooks"] = nonExistingBooks }
                });
        }

        _dbContext.Books.RemoveRange(existingBooks);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [AdminFilter]
    [HttpPatch("Update")]
    public async Task<IActionResult> Update([FromBody] UpdateBookDto update)
    {
        var book = await _dbContext.Books.FindAsync(update.Id).ConfigureAwait(false);
        if (update.AuthorId != null)
        {
            book.AuthorId = update.AuthorId.Value;
        }
        if (update.Description != null)
        {
            book.Description = update.Description;
        }
        if (update.Genre != null)
        {
            book.Genre = update.Genre.Value;
        }
        if (update.IsUsed != null)
        {
            book.IsUsed = update.IsUsed.Value;
        }
        if (update.Language != null)
        {
            book.Language = update.Language.Value;
        }
        if (update.Price != null)
        {
            book.Price = update.Price.Value;
        }
        if (update.PublishDate != null)
        {
            book.PublishDate = update.PublishDate.Value;
        }
        if (update.Stock != null)
        {
            book.Stock = update.Stock.Value;
        }
        if (update.Title != null)
        {
            book.Title = update.Title;
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [HttpPost("Search")]
    public async Task<ActionResult<int[]>> Search([FromBody] BookSearchFilterDto filter)
    {
        var books = _dbContext.Books.AsQueryable();
        if (filter.Genres != null)
        {
            books = books.Where(b => filter.Genres.Contains(b.Genre));
        }
        if (filter.Authors != null)
        {
            books = books.Where(b => filter.Authors.Contains(b.AuthorId));
        }
        if (filter.IsbnMask != null)
        {
            books = books.Where(b => EF.Functions.Like(b.Isbn, filter.IsbnMask));
        }
        if (filter.IsUsed != null)
        {
            books = books.Where(b => b.IsUsed == filter.IsUsed);
        }
        if (filter.Languages != null)
        {
            books = books.Where(b => filter.Languages.Contains(b.Language));
        }
        if (filter.TitleMask != null)
        {
            books = books.Where(b => EF.Functions.Like(b.Title, filter.TitleMask));
        }
        if (filter.MinStock != null)
        {
            books = books.Where(b => b.Stock >= filter.MinStock);
        }
        if (filter.MaxStock != null)
        {
            books = books.Where(b => b.Stock <= filter.MaxStock);
        }
        if (filter.MinPrice != null)
        {
            books = books.Where(b => b.Price >= filter.MinPrice);
        }
        if (filter.MaxPrice != null)
        {
            books = books.Where(b => b.Price <= filter.MaxPrice);
        }
        if (filter.MinPublishDate != null)
        {
            books = books.Where(b => b.PublishDate >= filter.MinPublishDate);
        }
        if (filter.MaxPublishDate != null)
        {
            books = books.Where(b => b.PublishDate <= filter.MaxPublishDate);
        }
        if (filter.MinRating != null)
        {
            books = books.Where(b => b.Rating >= filter.MinRating);
        }
        if (filter.MaxRating != null)
        {
            books = books.Where(b => b.RatersCount <= filter.MaxRating);
        }
        if ((filter.SortingMethod?.Length ?? 0) > 0)
        {
            var orderedBooks = filter.SortingMethod[0] switch
            {
                BookSortingAttribute.MostSelling => books.OrderByDescending(a => a.Sold),
                BookSortingAttribute.PublishDate => books.OrderByDescending(a => a.PublishDate),
                                BookSortingAttribute.AddedDate => books.OrderByDescending(a => a.AddedDate),

                BookSortingAttribute.Rating => books.OrderByDescending(a => a.Rating),
            };
            foreach (var m in filter.SortingMethod.Skip(1))
            {
                orderedBooks = m switch
                {
                    BookSortingAttribute.MostSelling => orderedBooks.ThenByDescending(a => a.Sold),
                    BookSortingAttribute.PublishDate => orderedBooks.ThenByDescending(a => a.PublishDate),
                                        BookSortingAttribute.AddedDate => orderedBooks.ThenByDescending(a => a.AddedDate),

                    BookSortingAttribute.Rating => orderedBooks.ThenByDescending(a => a.Rating),
                };
            }
            books = orderedBooks;
        }
        return Ok(books.Select(b => b.Id));
    }
    [HttpGet("GetCover")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCover([FromQuery] int bookId)
    {
        var book = await _dbContext.Books.FindAsync(bookId).ConfigureAwait(false);
        if (book == null)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "There is no book with the following Id.",
                    Data = new Dictionary<string, object> { ["BookId"] = bookId }
                });
        }

        var bookCover = _bookFileManager.GetFile(book.Id);
        if (bookCover == null) { return NoContent(); }
        var result = File(bookCover, "image/jpeg");
        result.FileDownloadName = $"Book_{bookId}_Cover.jpg";
        return result;
    }
}