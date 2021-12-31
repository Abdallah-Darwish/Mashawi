using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;

namespace Mashawi.Dto.Books;
public class BookDto : BookMetadataDto
{
    public string Isbn { get; set; }
    public AuthorDto Author { get; set; }
    public decimal Price { get; set; }
    public float RatersCount { get; set; }
    public float RatingSum { get; set; }
    public bool IsUsed { get; set; }
    public string Description { get; set; }
    public BookLanguage Language { get; set; }
    public DateTime PublishDate { get; set; }
        public DateTime AddedDate { get; set; }

    public BookGenre Genre { get; set; }
    public int Stock { get; set; }
}