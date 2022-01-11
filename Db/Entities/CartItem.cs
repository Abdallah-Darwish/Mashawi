using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class CartItem
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int Quantity { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<CartItem> b)
    {
        b.HasKey(s => s.Id);
        b.HasOne(s => s.Customer)
            .WithMany(b => b.Cart)
            .HasForeignKey(s => s.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.Book)
            .WithMany()
            .HasForeignKey(s => s.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Quantity)
            .IsRequired();

        b.HasCheckConstraint($"CK_{nameof(CartItem)}_{nameof(Quantity)}", $"\"{nameof(Quantity)}\" >= 1");
        b.HasIndex(b => new { b.BookId, b.CustomerId })
            .IsUnique();
    }
    public static void CreateSeed(SeedingContext ctx)
    {
        Random rand = new();
        var books = ctx.Books.ToArray();
        foreach (var user in ctx.Users)
        {
            int itemsCount = rand.Next(books.Length);
            for (int i = 0; i < itemsCount; i++)
            {
                CartItem item = new()
                {
                    BookId = rand.NextElementAndSwap(books, books.Length - (i + 1)).Id,
                    CustomerId = user.Id,
                    Quantity = rand.Next(1, 5),
                    Id = ctx.CartsItems.Count + 1,
                };
                ctx.CartsItems.Add(item);
            }
        }
    }
}