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
}