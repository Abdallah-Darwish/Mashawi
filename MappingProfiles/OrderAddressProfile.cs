using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.OrdersAddresses;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class OrderAddressProfile : Profile
{
    public OrderAddressProfile()
    {
        CreateMap<OrderAddress, OrderAddressDto>();
    }
}