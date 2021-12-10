using AutoMapper;
using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;
using Mashawi.Dto.Books;
using Mashawi.Dto.Users;

namespace Mashawi.MappingProfiles;
public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookMetadataDto>();
        CreateMap<Book, BookDto>();
    }
}