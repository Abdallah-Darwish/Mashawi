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
    public static void CreateSeed(SeedingContext ctx)
    {
        Random rand = new();
        int id = 30;        // to not affect the user addresses for now
        foreach (var user in ctx.Users)
        {
            int ordersCount = rand.Next(5);
            for (int i = 0; i < ordersCount; i++)
            {
                OrderAddress address = new()
                {
                    Id = ctx.OrdersAddresses.Count + 1,
                    BuildingNumber = rand.Next(1, 100),
                    FlatNumber = rand.Next(1, 100),
                    City = rand.NextText(rand.Next(1, 10)),
                    Neighborhood = rand.NextText(rand.Next(1, 10)),
                    Street = rand.NextText(rand.Next(1, 10)),
                };
                Order order = new()
                {
                    CreationDate = DateTime.UtcNow - TimeSpan.FromDays(rand.Next(1, 300)),
                    CustomerId = user.Id,
                    Id = ctx.Orders.Count + 1,
                    ShippingAddressId = address.Id
                };
                ctx.OrdersAddresses.Add(address);
                ctx.Orders.Add(order);
            }
        }
    }
}