using System.Reflection;
using Mashawi.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }


    public const string EntityConfigurationMethodName = "ConfigureEntity";

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<BookReview> BooksReviews { get; set; }
    public DbSet<CartItem> CartsItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderAddress> OrdersAddresses { get; set; }
    public DbSet<OrderItem> OrdersItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WishListItem> WhishListItems { get; set; }

    private static MethodInfo? FindEntityConfigurationMethod(Type t)
    {
        var typeMethods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (var m in typeMethods)
        {
            if (m.Name != EntityConfigurationMethodName) { continue; }

            if (m.ReturnType != typeof(void)) { continue; }

            if (m.IsGenericMethod) { continue; }

            if (!m.IsStatic) { continue; }

            if (!m.IsPublic) { continue; }

            var parameters = m.GetParameters();
            if (parameters.Length != 1) { continue; }

            if (!parameters[0].ParameterType.IsGenericType) { continue; }

            var typeBuilder = typeof(EntityTypeBuilder<>).MakeGenericType(t);
            if (parameters[0].ParameterType != typeBuilder) { continue; }

            return m;
        }

        return null;
    }

    private static bool IsEntity(Type t) => FindEntityConfigurationMethod(t) != null;

    private static Type[] GetAllEntitiesInAssembly(Assembly asm)
    {
        var types = asm.GetExportedTypes();
        return types.Where(IsEntity).ToArray();
    }

    ///<inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var modelBuilderEntityMethod = modelBuilder
            .GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(m => m.IsGenericMethod && m.Name == nameof(ModelBuilder.Entity));

        var entitiesTypes = GetAllEntitiesInAssembly(Assembly.GetExecutingAssembly());
        foreach (var type in entitiesTypes)
        {
            var configMethod = FindEntityConfigurationMethod(type)!;

            object entityTypeBuilder = modelBuilderEntityMethod.MakeGenericMethod(type).Invoke(modelBuilder, null)!;
            configMethod.Invoke(null, new object[] { entityTypeBuilder });
        }
    }
}