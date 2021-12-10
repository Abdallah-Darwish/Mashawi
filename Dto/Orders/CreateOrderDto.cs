using Mashawi.Dto.OrdersAddresses;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Orders;
//Validate not empty cart
public class CreateOrderDto
{
    public CreateOrderAddressDto ShippingAddress { get; set; }
}