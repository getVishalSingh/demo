using EmpApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Emp> Emps => Set<Emp>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Emp>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Department).HasMaxLength(100).IsRequired();
            e.Property(x => x.Position).HasMaxLength(100);
            e.Property(x => x.Salary).HasColumnType("decimal(18,2)");
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
