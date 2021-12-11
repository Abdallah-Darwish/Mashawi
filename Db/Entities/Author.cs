using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Book> Books { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<Author> b)
    {
        b.HasKey(s => s.Id);
        b.Property(s => s.Name)
            .IsUnicode()
            .IsRequired();
    }

    public static void CreateSeed(SeedingContext ctx)
    {
        Random rand = new();
        var firstNames = new string[]
        {
            "Abdallah", "Hashim", "Shatha", "Jannah", "Malik", "Basel", "Al-Bara", "Mohammad", "Aya", "Issra",
            "Huda", "Tuqa", "Deema"
        };
        var lastNames = new string[]
        {
            "Darwish", "Al-Mansour", "Shreim", "Barqawi", "Arabiat", "Azaizeh", "Zeer", "Faroun", "Abu-Rumman",
            "Allan", "Odeh"
        };
        int count = rand.Next(25, 50);
        for (int i = 0; i < count; i++)
        {
            Author author = new()
            {
                Id = ctx.Authors.Count + 1,
                Name = $"{rand.NextElement(firstNames)} {rand.NextElement(lastNames)}"
            };
            ctx.Authors.Add(author);
        }
    }
}