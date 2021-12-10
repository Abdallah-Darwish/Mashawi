using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.BooksReviews;

public class BookReviewDto
{
    public int Id { get; set; }
    public BookMetadataDto Book { get; set; }
    public UserMetadataDto User { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
}