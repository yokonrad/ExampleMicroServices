using Microsoft.EntityFrameworkCore;
using Posts.Domain.Entities;

namespace Posts.Infrastructure.Data;

public class PostDbContext(DbContextOptions<PostDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>().HasKey(p => p.Guid);
        modelBuilder.Entity<Post>().Property(p => p.Guid).IsRequired();
        modelBuilder.Entity<Post>().Property(p => p.Title).IsRequired();
        modelBuilder.Entity<Post>().Property(p => p.Text).IsRequired();
        modelBuilder.Entity<Post>().Property(p => p.Visible).IsRequired();
    }
}