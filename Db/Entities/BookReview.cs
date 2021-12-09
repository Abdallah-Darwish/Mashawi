using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class BookReview
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<BookReview> b)
    {
        b.HasKey(s => s.Id);
        b.HasOne(s => s.Book)
            .WithMany(b => b.Reviews)
            .HasForeignKey(s => s.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(s => s.User)
            .WithMany(s => s.Reviews)
            .HasForeignKey(s => s.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Content)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Rating)
            .IsRequired();
        b.HasCheckConstraint($"CK_{nameof(BookReview)}_{nameof(Rating)}", $"\"{nameof(Rating)}\" >= 0 AND {nameof(Rating)} <= 5");
    }
}