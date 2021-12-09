namespace Mashawi.Dto.OrdersAddresses;
public class CreateOrderAddressDto
{
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int BuildingNumber { get; set; }
    public int FlatNumber { get; set; }
}