using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; }
    public int ShippingAddressId { get; set; }
    public OrderAddress ShippingAddress { get; set; }
    public DateTime CreationDate { get; set; }
    public decimal Total { get; set; }
    public ICollection<OrderItem> Items { get; set; }

    public static void ConfigureEntity(EntityTypeBuilder<Order> b)
    {
        b.HasKey(s => s.Id);
        b.HasOne(s => s.Customer)
            .WithMany(b => b.Orders)
            .HasForeignKey(s => s.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.ShippingAddress)
            .WithOne()
            .HasForeignKey<Order>(o => o.ShippingAddressId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.CreationDate)
            .IsRequired();
        b.Property(s => s.Total)
            .IsRequired();
        b.HasCheckConstraint($"CK_{nameof(Order)}_{nameof(Total)}", $"\"{nameof(Total)}\" > 0");
    }
}