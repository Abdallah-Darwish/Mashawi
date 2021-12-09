using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class WishListItem
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<WishListItem> b)
    {
        b.HasKey(s => s.Id);
        b.HasOne(s => s.Customer)
            .WithMany(b => b.WishList)
            .HasForeignKey(s => s.CustomerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.Book)
            .WithMany()
            .HasForeignKey(s => s.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}