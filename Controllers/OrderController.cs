using AutoMapper;
using Mashawi.Db;
using Mashawi.Db.Entities;
using Mashawi.Dto;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Orders;
using Mashawi.Services.UserSystem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mashawi.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;
    public OrderController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    [LoggedInFilter]
    [HttpPost("GetAll")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    public async Task<ActionResult<int[]>> GetAll([FromBody] GetAllDto dto)
    {
        var user = this.GetUser()!;
        var orders = await _dbContext.Orders
            .Where(o => o.CustomerId == user.Id)
            .Skip(dto.Offset)
            .Take(dto.Count)
            .OrderByDescending(d => d.CreationDate)
            .Select(a => a.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        return Ok(orders);
    }
    [LoggedInFilter]
    [HttpPost("Get")]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OrderDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromBody] int[] ids, bool metadata = false)
    {
        var orders = _dbContext.Orders
            .Include(o => o.ShippingAddress)
            .AsQueryable();
        if (!metadata)
        {
            orders = orders.Include(o => o.Items);
        }

        var existingOrders = orders.Where(a => ids.Contains(a.Id));
        var nonExistingOrders = ids.Except(existingOrders.Select(a => a.Id)).ToArray();

        if (nonExistingOrders.Length > 0)
        {
            return StatusCode(StatusCodes.Status404NotFound,
                new ErrorDto
                {
                    Description = "The following orders don't exist.",
                    Data = new() { ["NonExistingOrders"] = nonExistingOrders }
                });
        }
        var user = this.GetUser()!;
        var notOwnedOrders = await existingOrders
            .Where(o => o.CustomerId != user.Id)
            .Select(o => o.Id)
            .ToArrayAsync()
            .ConfigureAwait(false);
        if (notOwnedOrders.Length > 0)
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorDto
                {
                    Description = "You don't own the following orders.",
                    Data = new() { ["NotOwnedOrders"] = notOwnedOrders }
                });
        }
        if (metadata)
        {
            return Ok(_mapper.Map<OrderMetadataDto>(existingOrders));
        }
        return Ok(_mapper.Map<OrderDto>(existingOrders));
    }
    [LoggedInFilter]
    [HttpPost("Create")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        var user = this.GetUser()!;
        var cart = await _dbContext.CartsItems
            .Include(i => i.Book)
            .Where(i => i.CustomerId == user.Id)
            .ToArrayAsync();

        Order entity = new()
        {
            CreationDate = DateTime.UtcNow.Date,
            CustomerId = user.Id,
            Items = cart.Select(i =>
            new OrderItem
            {
                BookId = i.BookId,
                Quantity = i.Quantity,
                UnitPrice = i.Book.Price
            }).ToArray(),
            ShippingAddress = new()
            {
                BuildingNumber = dto.ShippingAddress.BuildingNumber,
                City = dto.ShippingAddress.City,
                FlatNumber = dto.ShippingAddress.FlatNumber,
                Neighborhood = dto.ShippingAddress.Neighborhood,
                Street = dto.ShippingAddress.Street,
            },
            Total = cart.Sum(i => i.Book.Price * i.Quantity)
        };

        await _dbContext.Orders.AddAsync(entity).ConfigureAwait(false);
        _dbContext.CartsItems.RemoveRange(cart);
        foreach (var i in entity.Items)
        {
            var book = await _dbContext.Books.FindAsync(i.BookId).ConfigureAwait(false);
            book.Sold += i.Quantity;
            book.Stock -= i.Quantity;
        }
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        return CreatedAtAction(nameof(Get), new { ids = new int[] { entity.Id }, metadata = false }, _mapper.Map<OrderDto>(entity));
    }
}