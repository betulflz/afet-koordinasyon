using AfetYonetim.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HelpRequest> HelpRequests => Set<HelpRequest>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<Region> Regions => Set<Region>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---- Global Query Filters (Soft Delete) ----
            builder.Entity<HelpRequest>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Assignment>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Announcement>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Region>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<AuditLog>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ApplicationUser>().HasQueryFilter(e => !e.IsDeleted);

            // ---- HelpRequest İlişkileri ----
            builder.Entity<HelpRequest>(entity =>
            {
                entity.HasOne(h => h.User)
                    .WithMany(u => u.HelpRequests)
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Region)
                    .WithMany(r => r.HelpRequests)
                    .HasForeignKey(h => h.RegionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ---- Assignment İlişkileri ----
            builder.Entity<Assignment>(entity =>
            {
                entity.HasOne(a => a.HelpRequest)
                    .WithMany(h => h.Assignments)
                    .HasForeignKey(a => a.HelpRequestId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Volunteer)
                    .WithMany(u => u.VolunteerAssignments)
                    .HasForeignKey(a => a.VolunteerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.AssignedByAdmin)
                    .WithMany(u => u.AdminAssignments)
                    .HasForeignKey(a => a.AssignedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- Announcement İlişkileri ----
            builder.Entity<Announcement>(entity =>
            {
                entity.HasOne(a => a.CreatedByAdmin)
                    .WithMany(u => u.Announcements)
                    .HasForeignKey(a => a.CreatedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- ApplicationUser İlişkileri ----
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.Region)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RegionId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ---- AuditLog İlişkileri ----
            builder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // BaseEntity türevleri
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = now;
                            break;
                        case EntityState.Modified:
                            baseEntity.UpdatedAt = now;
                            break;
                        case EntityState.Deleted:
                            // Hard delete yerine soft delete
                            entry.State = EntityState.Modified;
                            baseEntity.IsDeleted = true;
                            baseEntity.DeletedAt = now;
                            break;
                    }
                }

                // ApplicationUser (BaseEntity'den türemediği için ayrı)
                if (entry.Entity is ApplicationUser user)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            user.CreatedAt = now;
                            break;
                        case EntityState.Modified:
                            user.UpdatedAt = now;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            user.IsDeleted = true;
                            user.DeletedAt = now;
                            break;
                    }
                }
            }
        }
    }
}