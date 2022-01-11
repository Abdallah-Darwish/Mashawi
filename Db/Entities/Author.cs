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
        Author author1 = new(){
            Id=1,
            Name="Kristin Harmel"
        };

        Author author2=new(){
            Id=2,
            Name="Farah Heron"
        };

        Author author3 = new(){
            Id=3,
            Name="Sajni Patel"
        };

        Author author4 = new(){
            Id=4,
            Name="Sophie Cousens"
        };

        Author author5=new(){
            Id=5,
            Name="Casey McQuiston"
        };

        Author author6= new(){
            Id=6,
            Name="Talia Hibbert"
        };

        Author author7 = new(){
            Id=7,
            Name="Stephen King"
        };

        Author author8 = new(){
            Id=8,
            Name="J. R. R. Tolkien"
        };

        Author author9=new(){
            Id=9,
            Name="J.D. Salinger"
        };
        Author author10= new()      //5 books 
        {
            Id=10,
            Name="William Golding"
        };

        Author author11 =new()
        {
            Id=11,
            Name="Marisa Noelle"
        };
        Author author12=new()
        {
            Id=12,
            Name="M.L. Blackbird"
        };
        Author author13 =new(){
            Id=13,
            Name="Tom Clavin"
        };
        List<Author> authors= new(){author1,author2,author3,author4,author5,author6,author7,author8,author9,author10,author11,author12,author13};
        ctx.Authors.AddRange(authors);
        /*Random rand = new();
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
        }*/
    }
}