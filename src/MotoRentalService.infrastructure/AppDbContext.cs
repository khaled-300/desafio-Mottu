using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;
using Serilog;

namespace MotoRentalService.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<DeliveryUser> DeliveryUser { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalPlans> RentalPlans { get; set; }
        public DbSet<RentalStatusHistory> RentalStatusHistories { get; set; }
        public DbSet<Users> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Motorcycle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Year).IsRequired();
                entity.Property(e => e.Model).IsRequired();
                entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.LicensePlate).IsUnique();
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });

            modelBuilder.Entity<RentalPlans>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DurationInDays).IsRequired();
                entity.Property(e => e.DailyRate).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.ExpectedEndDate).IsRequired();
                entity.Property(e => e.DailyRate).IsRequired().HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18, 2)");
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
                // Relationships
                entity.HasOne(r => r.Motorcycle)
                      .WithMany()
                      .HasForeignKey(r => r.MotorcycleId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.DeliveryPerson)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.RentalPlan)
                      .WithMany()
                      .HasForeignKey(r => r.RentalPlanId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<RentalStatusHistory>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Status).IsRequired();
                entity.Property(h => h.StatusChangedDate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Relationship with Rental table
                entity.HasOne(h => h.Rental)
                      .WithMany(r => r.StatusHistories)
                      .HasForeignKey(h => h.RentalId)
                      .IsRequired();
            });

            modelBuilder.Entity<DeliveryUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CNPJ).IsRequired().IsFixedLength(true).HasMaxLength(14);
                entity.HasIndex(e => e.CNPJ).IsUnique();
                entity.Property(e => e.DateOfBirth).IsRequired().HasColumnType("date");
                entity.Property(e => e.LicenseNumber).IsRequired();
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.Property(e => e.LicenseType).IsRequired();
                entity.Property(e => e.LicenseImageURL).IsRequired(false);
                entity.Property(e => e.LicenseImageFullName).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasConversion<string>();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired(false);
            });
        }

        public override int SaveChanges()
        {
            ApplyAuditRules();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditRules();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditRules()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {

                if (entry.State == EntityState.Modified)
                {
                    Log.Information($"Entity: {entry.Entity.GetType().Name}, UpdatedAt before set: {entry.CurrentValues["UpdatedAt"]}");
                    entry.CurrentValues["UpdatedAt"] = DateTime.UtcNow;
                    Log.Information($"Entity: {entry.Entity.GetType().Name}, UpdatedAt after set: {entry.CurrentValues["UpdatedAt"]}");
                }

                if (entry.State == EntityState.Added)
                {
                    Log.Information($"Entity: {entry.Entity.GetType().Name}, CreatedAt before set: {entry.CurrentValues["CreatedAt"]}");
                    entry.CurrentValues["CreatedAt"] = DateTime.UtcNow;
                    Log.Information($"Entity: {entry.Entity.GetType().Name}, CreatedAt after set: {entry.CurrentValues["CreatedAt"]}");
                }

                // Specific rules for DeliveryUser
                if (entry.Entity is DeliveryUser && entry.State == EntityState.Modified)
                {
                    var originalLicenseUrl = entry.OriginalValues["LicenseImageURL"] as string;
                    var currentLicenseUrl = entry.CurrentValues["LicenseImageURL"] as string;
                    if (originalLicenseUrl != currentLicenseUrl && !string.IsNullOrEmpty(currentLicenseUrl))
                    {
                        entry.CurrentValues["Status"] = UserStatus.Approved;
                    }
                }
            }
        }
    }
}
