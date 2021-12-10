using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public enum UserRole : byte { Customer, Admin }
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? Token { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public int AddressId { get; set; }
    public OrderAddress Address { get; set; }
    public UserRole Role { get; set; }
    public bool IsCustomer => Role == UserRole.Customer;
    public bool IsAdmin => Role == UserRole.Admin;
    public ICollection<BookReview> Reviews { get; set; }
    public ICollection<CartItem> Cart { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<WishListItem> WishList { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<User> b)
    {
        b.HasKey(s => s.Id);
        b.Ignore(s => s.IsAdmin);
        b.Ignore(s => s.IsCustomer);
        b.Property(s => s.Email)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.PasswordHash)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Name)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Phone)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Token)
           .IsRequired()
           .IsUnicode();
        b.HasOne(s => s.Address)
            .WithOne()
            .IsRequired()
            .HasForeignKey<User>(s => s.AddressId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Property(s => s.Role)
            .IsRequired()
            .HasConversion<byte>();
        b.HasIndex(s => new { s.Email })
            .IsUnique();
    }
}