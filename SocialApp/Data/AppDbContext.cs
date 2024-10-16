using Microsoft.EntityFrameworkCore;
using SocialApp.Models;

namespace SocialApp.Data;

// This class is your application's DbContext, which serves as the main connection between your C# models and the database.
// It inherits from Entity Framework's DbContext class, allowing you to work with the database using C# models.
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();  // Enforces uniqueness on Email column
    }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<PostModel> Posts { get; set; }
    public DbSet<CommentModel> Comments { get; set; }
}
