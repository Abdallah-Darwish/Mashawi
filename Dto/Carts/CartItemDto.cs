using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Carts;
public class CartItemDto
{
    public int Id { get; set; }
    public UserMetadataDto Customer { get; set; }
    public BookMetadataDto Book { get; set; }
    public int Quantity { get; set; }
}