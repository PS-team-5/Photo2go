using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Model;

namespace Photo2GoAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Location> Locations { get; set; }
}
