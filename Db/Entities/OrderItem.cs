using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int Quantity { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public decimal UnitPrice { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<OrderItem> b)
    {
        b.HasKey(s => s.Id);
        b.Property(s => s.UnitPrice)
            .IsRequired()
            .IsUnicode();
        b.HasOne(s => s.Order)
            .WithMany(s => s.Items)
            .HasForeignKey(s => s.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.Book)
            .WithMany()
            .HasForeignKey(s => s.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasCheckConstraint($"CK_{nameof(OrderItem)}_{nameof(UnitPrice)}", $"\"{nameof(UnitPrice)}\" > 0");
    }
    public static void CreateSeed(SeedingContext ctx)
    {
        Random rand = new();
        var books = ctx.Books.ToArray();
        foreach (var order in ctx.Orders)
        {
            var itemsCount = rand.Next(1, books.Length);
            for (int i = 0; i < itemsCount; i++)
            {
                var book = rand.NextElementAndSwap(books, books.Length - (i + 1));
                OrderItem item = new()
                {
                    BookId = book.Id,
                    OrderId = order.Id,
                    Quantity = rand.Next(1, 10),
                    UnitPrice = rand.Next(1, (int)book.Price),
                    Id = ctx.OrdersItems.Count + 1
                };
                order.Total += item.Quantity * item.UnitPrice;
                ctx.OrdersItems.Add(item);
            }
        }
    }
}