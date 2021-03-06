using Mashawi.Services.UserSystem;
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
    public static void CreateSeed(SeedingContext ctx)
    {
        OrderAddress address1 = new(){
            Id=1,
            City="Amman",
            Neighborhood="Jubeiha",
            Street="Hay Al-Rayyan",
            BuildingNumber=13,
            FlatNumber=1
        };
        User user1=new(){
            Id=1,
            AddressId=1,
            Name="Ammar AlZeer",
            Email="ammarzeer@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0799911996",
            Role=UserRole.Admin
        };
        //ctx.OrdersAddresses.Add(address1);
        //ctx.Users.Add(user1);

        OrderAddress address2 = new(){
            Id=2,
            City="Irbid",
            Neighborhood="Sama Al-Rousan",
            Street="Aiz Ben Hayef",
            BuildingNumber=10,
            FlatNumber=1
        };
        User user2= new(){
            Id=2,
            AddressId=2,
            Name="Housnie Al-Rousan",
            Email="housnie@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0795664711",
            Role=UserRole.Customer
        };

        OrderAddress address3=new(){
            Id=3,
            City="Amman",
            Neighborhood="Dahiat Alamir Hasan",
            Street="Omayah Al-Andalusi",
            BuildingNumber=15,
            FlatNumber=4
        };
        User user3=new(){
            Id=3,
            AddressId=3,
            Name="Mohammad Faroun",
            Email="faroun@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0775137187",
            Role=UserRole.Admin   
        };

        //ORDER ADDRESS AND USERS FOR BOOK REVIEWS
        OrderAddress tempAddress1 = new(){
            Id=4,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp1=new(){
            Id=4,
            AddressId=4,
            Name="Abdullah Darwish",
            Email="temp1@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0795294715",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress2 = new(){
            Id=5,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
         User temp2=new(){
            Id=5,
            AddressId=5,
            Name="Al-Bara'",
            Email="temp2@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0790094715",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress3 = new(){
            Id=6,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp3=new(){
            Id=6,
            AddressId=6,
            Name="Gustello",
            Email="temp3@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0790124717",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress4 = new(){
            Id=7,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp4=new(){
            Id=7,
            AddressId=7,
            Name="Tawfeeq",
            Email="temp4@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0790093815",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress5 = new(){
            Id=8,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp5=new(){
            Id=8,
            AddressId=8,
            Name="Ibrahim",
            Email="temp5@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0799294734",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress6 = new(){
            Id=9,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp6=new(){
            Id=9,
            AddressId=9,
            Name="Saif",
            Email="temp6@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0790029715",
            Role=UserRole.Customer
        };
          OrderAddress tempAddress7= new(){
            Id=10,
            City="Zarqa",
            Neighborhood="Al-Rusayfah ",
            Street="Hamdan Al Basheer",
            BuildingNumber=23,
            FlatNumber=3
        };
        User temp7=new(){
            Id=10,
            AddressId=10,
            Name="Haitham",
            Email="temp7@gmail.com",
            PasswordHash=UserManager.HashPassword("123456"),
            Phone="0797294715",
            Role=UserRole.Customer
        };
     
        List<OrderAddress> addresses=new List<OrderAddress>{address1,address2,address3,tempAddress1,tempAddress2,tempAddress3,tempAddress4,tempAddress5,tempAddress6,tempAddress7};
        List<User>users=new List<User>{user1,user2,user3,temp1,temp2,temp3,temp4,temp5,temp6,temp7};
        ctx.OrdersAddresses.AddRange(addresses);
        ctx.Users.AddRange(users);
      /*  Random rand = new();
        var firstNames = new string[]
        {
            "Abdallah", "Hashim", "Shatha", "Jannah", "Malik", "Basel", "Al-Bara", "Mohammad", "Aya", "Issra",
            "Huda", "Tuqa", "Deema"
        };
        var lastNames = new string[]
        {
            "Darwish", "Al-Mansour", "Shreim", "Barqawi", "Arabiat", "Azaizeh", "Zeer", "Faroun", "Abu-Rumman",
            "Allan", "Odeh"
        };
        for (char i = 'a'; i <= 'z'; i++)
        {
            OrderAddress address = new()
            {
                Id = ctx.OrdersAddresses.Count + 1,
                BuildingNumber = rand.Next(1, 100),
                FlatNumber = rand.Next(1, 100),
                City = rand.NextText(rand.Next(1, 10)),
                Neighborhood = rand.NextText(rand.Next(1, 10)),
                Street = rand.NextText(rand.Next(1, 10)),
            };
            User user = new()
            {
                Id = ctx.Users.Count + 1,
                Email = $"{i}@{i}.com",
                Name = $"{rand.NextElement(firstNames)} {rand.NextElement(lastNames)}",
                Phone = rand.NextNumber(10),
                PasswordHash = UserManager.HashPassword(".123456789a"),
                AddressId = address.Id,
                Role = UserRole.Customer
            };
            ctx.Users.Add(user);
            ctx.OrdersAddresses.Add(address);
        }
        for (int i = 0, e = 5; i < ctx.Users.Count && e > 0; i++)
        {
            ctx.Users[i].Role = UserRole.Admin;
            e--;
        }*/
    }
}