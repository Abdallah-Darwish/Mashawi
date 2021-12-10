using Mashawi.Dto.OrdersAddresses;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.Orders;
public class OrderMetadataDto
{
    public int Id { get; set; }
    public UserMetadataDto Customer { get; set; }
    public OrderAddressDto ShippingAddress { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Total { get; set; }
}