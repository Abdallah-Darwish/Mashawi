using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.WishList;
public class WishListItemDto
{
    public int Id { get; set; }
    public UserMetadataDto Customer { get; set; }
    public BookMetadataDto Book { get; set; }
}