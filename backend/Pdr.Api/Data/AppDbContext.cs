using Microsoft.EntityFrameworkCore;
using Pdr.Api.Models;

namespace Pdr.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<Engagement> Engagements => Set<Engagement>();
    public DbSet<EngagementStatus> EngagementStatuses => Set<EngagementStatus>();
    public DbSet<EngagementType> EngagementTypes => Set<EngagementType>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).HasColumnName("id");
            entity.Property(r => r.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id");
            entity.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            
            // Performance Indexes matching PostgreSQL schema
            entity.HasIndex(u => u.Email, "idx_users_email").IsUnique();
            entity.HasIndex(u => u.RoleId, "idx_users_role_id");

            entity.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
            entity.Property(u => u.RoleId).HasColumnName("role_id").IsRequired();
            entity.Property(u => u.IsAdmin).HasColumnName("isAdmin").IsRequired();
            entity.Property(u => u.FailedAttemptCount).HasColumnName("failed_attempt_count").HasDefaultValue(0);
            entity.Property(u => u.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<EngagementStatus>(entity =>
        {
            entity.HasKey(s => s.StatusId);

            entity.Property(s => s.StatusName)
                .HasMaxLength(50)
                .IsRequired();
        });


        modelBuilder.Entity<EngagementType>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Engagement>(entity =>
        {
            entity.HasKey(e => e.EngagementId);
            entity.Property(e => e.ClientName).HasMaxLength(200).IsRequired();

            entity.HasOne(e => e.EngagementType)
                  .WithMany()
                  .HasForeignKey(e => e.EngagementTypeId);

            entity.HasOne(e => e.Status)
                  .WithMany(s => s.Engagements)
                  .HasForeignKey(e => e.StatusId);
        });

    }
}
