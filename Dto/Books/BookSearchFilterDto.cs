using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;

namespace Mashawi.Dto.Books;
public enum BookSortingAttribute { MostSelling, Rating, PublishDate, AddedDate }
public class BookSearchFilterDto
{
    public string? IsbnMask { get; set; }
    public string? TitleMask { get; set; }
    public int[]? Authors { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public float? MinRating { get; set; }
    public float? MaxRating { get; set; }
    public bool? IsUsed { get; set; }
    public BookLanguage[]? Languages { get; set; }
    public DateTime? MinPublishDate { get; set; }
    public DateTime? MaxPublishDate { get; set; }

    
    public DateTime? MinAddedDate { get; set; }
    public DateTime? MaxAddedDate { get; set; }
    public BookGenre[]? Genres { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
    public bool OnlyInStock { get; set; }
    public BookSortingAttribute[]? SortingMethod { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
}