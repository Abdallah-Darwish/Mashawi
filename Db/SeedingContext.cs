using Mashawi.Db.Entities;

namespace Mashawi.Db;
public class SeedingContext
{
    public List<Book> Books { get; } = new();
    public List<Author> Authors { get; } = new();
    public List<BookReview> BooksReviews { get; } = new();
    public List<CartItem> CartsItems { get; } = new();
    public List<Order> Orders { get; } = new();
    public List<OrderAddress> OrdersAddresses { get; } = new();
    public List<OrderItem> OrdersItems { get; } = new();
    public List<User> Users { get; } = new();
    public List<WishListItem> WhishListItems { get; } = new();
}