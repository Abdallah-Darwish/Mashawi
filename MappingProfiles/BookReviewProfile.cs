using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.BooksReviews;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class BookReviewProfile : Profile
{
    public BookReviewProfile()
    {
        CreateMap<BookReview, BookReviewDto>();
    }
}