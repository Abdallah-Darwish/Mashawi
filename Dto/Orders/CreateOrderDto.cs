using Mashawi.Dto.OrdersAddresses;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Orders;
//Validate not empty cart
//Validate we have enough quantities
public class CreateOrderDto
{
    public CreateOrderAddressDto ShippingAddress { get; set; }
}