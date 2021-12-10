using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawi.Db.Entities;
public class OrderAddress
{
    public int Id { get; set; }
    public string City { get; set; }
    public string Neighborhood { get; set; }
    public string Street { get; set; }
    public int BuildingNumber { get; set; }
    public int FlatNumber { get; set; }
    public static void ConfigureEntity(EntityTypeBuilder<OrderAddress> b)
    {
        b.HasKey(s => s.Id);
        b.Property(s => s.City)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Neighborhood)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.Street)
            .IsRequired()
            .IsUnicode();
        b.Property(s => s.BuildingNumber)
            .IsRequired();
        b.Property(s => s.FlatNumber)
            .IsRequired();
    }
    public static void CreateSeed(SeedingContext ctx)
    {

    }
}