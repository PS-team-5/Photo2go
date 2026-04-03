using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Model;

namespace Photo2GoAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<GeneratedRoute> GeneratedRoutes { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<GeneratedRoute>()
        .HasOne(route => route.User)
        .WithMany(user => user.GeneratedRoutes)
        .HasForeignKey(route => route.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Location>().HasData(
        new Location { 
            Id = 1, Name = "Gediminas Tower", ObjectType = "Castle", 
            ArchitectureStyle = "Gothic", Period = "14th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Historical Monument", BuildingMaterials = "Bricks, Stone", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 2, Name = "Vilnius Cathedral", ObjectType = "Church", 
            ArchitectureStyle = "Classicism", Period = "18th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Religious Object", BuildingMaterials = "Plaster, Bricks", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 3, Name = "St. Anne's Church", ObjectType = "Church", 
            ArchitectureStyle = "Flamboyant Gothic", Period = "15th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Religious Object", BuildingMaterials = "Clay, Bricks", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 4, Name = "Vilnius University", ObjectType = "University", 
            ArchitectureStyle = "Baroque/Renaissance", Period = "16th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Educational Institution", BuildingMaterials = "Masonry", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 5, Name = "Gate of Dawn", ObjectType = "City Gate", 
            ArchitectureStyle = "Renaissance", Period = "16th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Religious/Defensive", BuildingMaterials = "Stone, Bricks", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 6, Name = "St. Peter and St. Paul's Church", ObjectType = "Church", 
            ArchitectureStyle = "Baroque", Period = "17th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Religious Object", BuildingMaterials = "Masonry, Stucco", IsUnescoProtected = "No" 
        },
        new Location { 
            Id = 7, Name = "Presidential Palace", ObjectType = "Palace", 
            ArchitectureStyle = "Classicism", Period = "19th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Government Building", BuildingMaterials = "Masonry, Plaster", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 8, Name = "Vilnius Town Hall", ObjectType = "Town Hall", 
            ArchitectureStyle = "Classicism", Period = "18th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Local Government", BuildingMaterials = "Masonry", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 9, Name = "Bernardinai Garden", ObjectType = "Park", 
            ArchitectureStyle = "None", Period = "15th century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Recreational Area", BuildingMaterials = "Nature", IsUnescoProtected = "Yes" 
        },
        new Location { 
            Id = 10, Name = "Angel of Uzupis", ObjectType = "Sculpture", 
            ArchitectureStyle = "Modernism", Period = "21st century", 
            City = "Vilnius", Province = "Vilnius County", 
            Category = "Artistic Object", BuildingMaterials = "Bronze", IsUnescoProtected = "Yes" 
        }
    );
}
    
}
