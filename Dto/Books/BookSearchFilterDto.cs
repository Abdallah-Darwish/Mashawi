using Mashawi.Db.Entities;
using Mashawi.Dto.Authors;

namespace Mashawi.Dto.Books;
/// <summary>
/// Results will be ordered by InStock Rating PublishDate 
/// </summary>
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
    public BookGenre[]? Genres { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
    public bool OnlyInStock { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
}