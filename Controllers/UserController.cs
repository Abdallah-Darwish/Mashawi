using AutoMapper;
using Mashawi.Db;
using Mashawi.Dto.Users;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager _userManager;
    private readonly IMapper _mapper;

    public UserController(AppDbContext dbContext, IMapper mapper, UserManager userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>Creates/Signsup a new user.</summary>
    /// <response code="201">Metadata of the newly created user.</response>
    [NotLoggedInFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> Create([FromBody] SignUpDto dto)
    {
        var user = await _userManager.SignUp(dto.Email, dto.Password, dto.Name, dto.Phone).ConfigureAwait(false);
        return StatusCode(StatusCodes.Status201Created, _mapper.Map<UserDto>(user));
    }

    /// <summary>
    /// Updates a user.
    /// </summary>
    /// <remarks>
    /// A user can update only himself.
    /// An instructor can update any user.
    /// IsInstructor will be considered only if the caller is an instructor.
    /// </remarks>
    /// <param name="update">The update to apply, null fields mean no update to this property.</param>
    [LoggedInFilter]
    [HttpPatch("Update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto update)
    {
        var user = await _dbContext.Users.FindAsync(update.Id).ConfigureAwait(false);
        await _userManager.UpdateUser(user.Id, update.Password, update.Name, user.IsAdmin ? update.Role : null, update.Phone).ConfigureAwait(false);

        if (update.Address != null)
        {
            if (update.Address.BuildingNumber != null)
            {
                user.Address.BuildingNumber = update.Address.BuildingNumber.Value;
            }
            if (update.Address.City != null)
            {
                user.Address.City = update.Address.City;
            }
            if (update.Address.FlatNumber != null)
            {
                user.Address.FlatNumber = update.Address.FlatNumber.Value;
            }
            if (update.Address.Neighborhood != null)
            {
                user.Address.Neighborhood = update.Address.Neighborhood;
            }
            if (update.Address.Street != null)
            {
                user.Address.Street = update.Address.Street;
            }
        }
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }

    /// <summary>
    /// Generates login cookie for a user to be used in subsequent requests.
    /// </summary>
    /// <remarks>
    /// A user can't be logged in before calling this method.
    /// </remarks>
    /// <response code="200">Successful login, will return the token and Create a set session cookie.</response>
    /// <response code="401">Invalid login credentials.</response>
    [NotLoggedInFilter]
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto info)
    {
        var userId = await _userManager.Login(info.Email, info.Password, Response.Cookies).ConfigureAwait(false);
        if (userId == null)
        {
            return Unauthorized("Invalid login credentials.");
        }

        var user = await _dbContext.Users
            .Include(u => u.Address)
            .FirstAsync(u => u.Id == userId)
            .ConfigureAwait(false);
        LoginResultDto result = new()
        {
            User = _mapper.Map<UserDto>(user),
            Token = user.Token!
        };
        return Ok(result);
    }
    /// <summary>
    /// Logs out the current user and removes his cookie.
    /// </summary>
    [LoggedInFilter]
    [HttpPost("Logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var user = this.GetUser()!;
        await _userManager.Logout(user, Response.Cookies).ConfigureAwait(false);
        return Ok();
    }
    /// <summary>
    /// Gets UserDto for the logged in user.
    /// </summary>
    /// <response code="200">Info about the logged in user.</response>
    [LoggedInFilter]
    [HttpGet("GetLoggedIn")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetLoggedIn()
    {
        var loggedInUser = this.GetUser()!;
        var user = await _dbContext.Users
            .Include(u => u.Address)
            .FirstAsync(u => u.Id == loggedInUser.Id)
            .ConfigureAwait(false);
        return Ok(_mapper.Map<UserDto>(user));
    }
}