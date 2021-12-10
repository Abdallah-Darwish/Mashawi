using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.BooksReviews;
//Checks user didnt review the book already
public class CreateBookReviewDto
{
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
}