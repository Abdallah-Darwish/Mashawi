using Mashawi.Dto.OrdersAddresses;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Orders;
public class OrderDto : OrderMetadataDto
{
    public OrderItemDto[] Items { get; set; }
}