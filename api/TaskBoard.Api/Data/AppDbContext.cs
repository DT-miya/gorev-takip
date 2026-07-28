
using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Data.Entities;

namespace TaskBoard.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(150).IsRequired();
            e.Property(p => p.Description).HasMaxLength(2000);

            e.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectMember>(e =>
        {
            e.Property(pm => pm.Role).HasMaxLength(20).IsRequired();
            e.HasIndex(pm => new {pm.ProjectId, pm.UserId}).IsUnique();


            e.HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pm => pm.User)
                .WithMany(u => u.Memberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

       

        modelBuilder.Entity<BoardColumn>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100).IsRequired();

            e.HasOne(c => c.Project)
            .WithMany(p => p.Columns)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.Priority).HasMaxLength(10).IsRequired();

            e.HasOne(t => t.Column)
             .WithMany(c => c.Tasks)
             .HasForeignKey(t => t.ColumnId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(t => t.Assignee)
             .WithMany()
             .HasForeignKey(t => t.AssigneeId)
             .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
