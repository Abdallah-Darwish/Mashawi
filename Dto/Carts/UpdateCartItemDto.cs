using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Carts;
public class UpdateCartItemDto
{
    public int Id { get; set; }
    public int? Quantity { get; set; }
}