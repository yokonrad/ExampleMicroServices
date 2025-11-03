using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Tests.Examples;

public class ExampleDbContext(DbContextOptions<ExampleDbContext> options) : DbContext(options)
{
    public DbSet<Example> Examples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Example>().HasKey(t => t.Guid);
        modelBuilder.Entity<Example>().Property(t => t.Guid).IsRequired();
    }
}