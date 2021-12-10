using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthorController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public AuthorController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    [HttpPost("GetAll")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<int[]>> GetAll([FromBody] GetAllDto dto)
    {
        var authors = await _dbContext.Authors
            .Skip(dto.Offset)
            .Take(dto.Count)
            .OrderBy(d => d.Id)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(authors);
    }
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(AuthorDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] int[] ids)
    {
        var authors = _dbContext.Authors.AsQueryable();

        var existingAuthors = authors.Where(a => ids.Contains(a.Id));
        var nonExistingAuthors = ids.Except(existingAuthors.Select(a => a.Id)).ToArray();

        if (nonExistingAuthors.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following authors don't exist.",
                    Data = new() { ["NonExistingAuthors"] = nonExistingAuthors }
                });
        }
        return Ok(_mapper.ProjectTo<AuthorDto>(existingAuthors));
    }
    [AdminFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
    {
        Author entity = new()
        {
            Name = dto.Name
        };
        await _dbContext.Authors.AddAsync(entity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id } }, _mapper.Map<AuthorDto>(entity));
    }
    [AdminFilter]
    [HttpDelete("Delete")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        var authors = _dbContext.Authors.AsQueryable();

        var existingAuthors = authors.Where(a => ids.Contains(a.Id));
        var nonExistingAuthors = ids.Except(existingAuthors.Select(a => a.Id)).ToArray();

        if (nonExistingAuthors.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following authors don't exist.",
                    Data = new() { ["NonExistingAuthors"] = nonExistingAuthors }
                });
        }

        _dbContext.Authors.RemoveRange(existingAuthors);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return NoContent();
    }
    [AdminFilter]
    [HttpPatch("Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateAuthorDto update)
    {
        var author = await _dbContext.Authors.FindAsync(update.Id).ConfigureAwait(false);
        if (update.Name != null)
        {
            author.Name = update.Name;
        }

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }

}