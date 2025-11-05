using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.Data;

public class PostDbContext(DbContextOptions<PostDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().HasKey(t => t.Guid);
        modelBuilder.Entity<Post>().Property(t => t.Guid).IsRequired();
        modelBuilder.Entity<Post>().Property(t => t.Title).IsRequired();
        modelBuilder.Entity<Post>().Property(t => t.Text).IsRequired();
        modelBuilder.Entity<Post>().Property(t => t.Visible).IsRequired();
    }
}