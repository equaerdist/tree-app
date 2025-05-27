using Microsoft.EntityFrameworkCore;
using tree_api.Database.Models;

namespace tree_api.Database;

internal class DbCtx : DbContext
{
    public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<LogEntry> Journal => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>()
            .HasOne(n => n.Parent)
            .WithMany(n => n.Children)
            .HasForeignKey(n => n.ParentId);

        modelBuilder.Entity<Node>()
            .HasIndex(n => new { n.TreeId, n.ParentId, n.Name })
            .IsUnique();
    }
}
