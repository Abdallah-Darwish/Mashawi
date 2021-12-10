using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;

namespace Mashawi.Dto.Books;
public class CreateBookDto
{
    public string Isbn { get; set; }
    public string Title { get; set; }
    public int AuthorId { get; set; }
    public decimal Price { get; set; }
    public bool IsUsed { get; set; }
    public string Description { get; set; }
    public BookLanguage Language { get; set; }
    public DateTime PublishDate { get; set; }
    public BookGenre Genre { get; set; }
    public int Stock { get; set; }
}