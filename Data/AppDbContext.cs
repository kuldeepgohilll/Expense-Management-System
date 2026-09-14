using ExpenseManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Transaction>().HasOne(x => x.User).WithMany(x => x.Transactions)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Budget>().HasOne(x => x.User).WithMany(x => x.Budgets)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Budget>().HasIndex(x => new { x.UserId, x.Month }).IsUnique();
    }
}
