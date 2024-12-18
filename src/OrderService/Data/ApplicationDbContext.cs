using Microsoft.EntityFrameworkCore;

namespace OrderService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    
}