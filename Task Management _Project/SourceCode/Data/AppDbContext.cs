using InternalProcessMgmt.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalProcessMgmt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext ( DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<TaskHistory>()
                .ToTable("TaskHistory");

            builder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
            builder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            builder.Entity<TaskItem>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskHistory>()
    .HasKey(h => h.HistoryId);  

            builder.Entity<TaskHistory>()
                .HasOne(h => h.TaskItem)
                .WithMany()
                .HasForeignKey(h => h.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
