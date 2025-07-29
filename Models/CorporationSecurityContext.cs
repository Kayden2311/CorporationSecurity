using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CorporationSecurity.Models;

public partial class CorporationSecurityContext : DbContext
{
    public CorporationSecurityContext()
    {
    }

    public CorporationSecurityContext(DbContextOptions<CorporationSecurityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Control> Controls { get; set; }

    public virtual DbSet<Risk> Risks { get; set; }

    public virtual DbSet<RiskCategory> RiskCategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Asset__3214EC07DD9ACD6E");

            entity.ToTable("Asset");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Assets)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asset__CategoryI__46E78A0C");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Assets)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Asset__CreatedBy__47DBAE45");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC07FE15E451");

            entity.ToTable("AuditLog");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLog__UserId__59063A47");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC074F83A317");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Name, "UQ__Category__737584F61D5E11FB").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Control>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Control__3214EC078BB71112");

            entity.ToTable("Control");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Effectiveness).HasMaxLength(20);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Controls)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Control__Created__5535A963");

            entity.HasOne(d => d.Risk).WithMany(p => p.Controls)
                .HasForeignKey(d => d.RiskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Control__RiskId__5441852A");
        });

        modelBuilder.Entity<Risk>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Risk__3214EC07C0940868");

            entity.ToTable("Risk");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Mitigation).HasMaxLength(500);

            entity.HasOne(d => d.Asset).WithMany(p => p.Risks)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Risk__AssetId__4D94879B");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Risks)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Risk__CreatedBy__4F7CD00D");

            entity.HasOne(d => d.RiskCategory).WithMany(p => p.Risks)
                .HasForeignKey(d => d.RiskCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Risk__RiskCatego__4E88ABD4");
        });

        modelBuilder.Entity<RiskCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RiskCate__3214EC07A765B48C");

            entity.ToTable("RiskCategory");

            entity.HasIndex(e => e.Name, "UQ__RiskCate__737584F61EFF6A2B").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07D8BC8EE9");

            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ__Role__737584F6C88B789F").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0710AA7C79");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E4B0831F6A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D105345B44AA28").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(256)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
