using Mashawi.Dto.Books;

namespace Mashawi.Dto.Orders;
public class OrderItemDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public BookMetadataDto Book { get; set; }
    public decimal UnitPrice { get; set; }
}