using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.Dto.BooksReviews;

public class UpdateBookReviewDto
{
    public int Id { get; set; }
    public float? Rating { get; set; }
    public string? Content { get; set; }
}