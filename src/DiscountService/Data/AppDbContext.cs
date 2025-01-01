using DiscountService.Entities;
using Microsoft.EntityFrameworkCore;


namespace DiscountService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }  

    public DbSet<Discount> Discounts { get; set; }
}