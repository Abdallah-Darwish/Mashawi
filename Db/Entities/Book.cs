using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public enum BookLanguage { Arabic, English }
public enum BookGenre { Fantasy, Romance }
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public decimal Price { get; set; }
    public float RatersCount { get; set; }
    public float RatingSum { get; set; }
    public float Rating => RatersCount == 0 ? 0 : RatingSum / RatersCount;
    public bool IsUsed { get; set; }
    public string Description { get; set; }
    public BookLanguage Language { get; set; }
    public DateTime PublishDate { get; set; }
    public BookGenre Genre { get; set; }
    public int Stock { get; set; }
    public ICollection<BookReview> Reviews { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<Book> b)
    {
        b.HasKey(s => s.Id);
        b.Ignore(s => s.Rating);
        b.Property(s => s.Title)
            .IsUnicode()
            .IsRequired();
        b.Property(s => s.Isbn)
            .IsRequired();
        b.HasOne(s => s.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(s => s.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Price)
            .IsRequired();
        b.Property(s => s.RatersCount)
            .IsRequired()
            .HasDefaultValue(0);
        b.Property(s => s.RatingSum)
            .IsRequired()
            .HasDefaultValue(0);
        b.Property(s => s.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);
        b.Property(s => s.Description)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Language)
            .IsRequired()
            .HasConversion<byte>();
        b.Property(s => s.PublishDate)
            .IsRequired();
        b.Property(s => s.Genre)
            .IsRequired()
            .HasConversion<byte>();
        b.Property(s => s.Stock)
            .IsRequired();

        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(RatersCount)}", $"\"{nameof(RatersCount)}\" >= 0");
        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(RatingSum)}", $"\"{nameof(RatingSum)}\" >= 0");
        b.HasCheckConstraint($"CK_{nameof(Book)}_{nameof(Stock)}", $"\"{nameof(Stock)}\" >= 0");
    }

}