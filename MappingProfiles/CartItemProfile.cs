using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class CartItemProfile : Profile
{
    public CartItemProfile()
    {
        CreateMap<CartItem, CartItemDto>();
    }
}