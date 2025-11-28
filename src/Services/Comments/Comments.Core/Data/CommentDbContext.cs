using Comments.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Comments.Core.Data;

public class CommentDbContext(DbContextOptions<CommentDbContext> options) : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>().HasKey(c => c.Guid);
        modelBuilder.Entity<Comment>().Property(c => c.Guid).IsRequired();
        modelBuilder.Entity<Comment>().Property(c => c.PostGuid).IsRequired();
        modelBuilder.Entity<Comment>().Property(c => c.Text).IsRequired();
        modelBuilder.Entity<Comment>().Property(t => t.Visible).IsRequired();
    }
}