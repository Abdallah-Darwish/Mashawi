using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Carts;
public class CreateCartItemDto
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}