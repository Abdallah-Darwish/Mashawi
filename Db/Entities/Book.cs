using Mashawi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkiaSharp;
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

    public static async Task CreateSeedFiles(IServiceProvider sp, SeedingContext seedingContext)
    {
        using SKPaint paint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill,
            HintingLevel = SKPaintHinting.Full,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center,
            TextSize = 10,
            StrokeWidth = 10
        };
        Random rand = new();
        var fileManager = sp.GetRequiredService<BookFileManager>();

        async Task GenerateBookPicture(Book b)
        {
            using SKBitmap bmp = new(200, 200);
            using (SKCanvas can = new(bmp))
            {
                can.Clear(new SKColor((uint)rand.Next(100, int.MaxValue)));
                can.Flush();
            }
            using var jpgData = bmp.Encode(SKEncodedImageFormat.Jpeg, 100);
            await using var jpgStream = jpgData.AsStream();
            await fileManager.SaveFile(b.Id, jpgStream).ConfigureAwait(false);
        }

        foreach (var group in seedingContext.Books)
        {
            await GenerateBookPicture(group).ConfigureAwait(false);
        }
    }
    public static void CreateSeed(SeedingContext ctx)
    {
        Random rand = new();
        var languages = Enum.GetValues<BookLanguage>();
        var genres = Enum.GetValues<BookGenre>();
        for (int i = 1; i <= 200; i++)
        {
            Book book = new()
            {
                Id = ctx.Books.Count + 1,
                AuthorId = rand.NextElement(ctx.Authors).Id,
                Description = rand.NextText(),
                Genre = rand.NextElement(genres),
                Isbn = $"{rand.NextNumber(3)}-{rand.NextNumber(5)}-",
                Language = rand.NextElement(languages),
                PublishDate = DateTime.Now - TimeSpan.FromDays(rand.Next(2, 3000)),
                Title = rand.NextText(),
                IsUsed = rand.NextBool(),
                Price = rand.Next(2, 100),
                Stock = rand.Next(300)
            };
            ctx.Books.Add(book);
        }
    }

}