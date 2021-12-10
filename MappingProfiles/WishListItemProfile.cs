using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.Orders;
using Mashawi.Dto.Users;
using Mashawi.Dto.WishList;

namespace Mashawi.MappingProfiles;
public class WishListItemProfile : Profile
{
    public WishListItemProfile()
    {
        CreateMap<WishListItem, WishListItemDto>();
    }
}