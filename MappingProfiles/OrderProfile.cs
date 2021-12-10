using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Carts;
using Mashawi.Dto.Orders;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<Order, OrderMetadataDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}