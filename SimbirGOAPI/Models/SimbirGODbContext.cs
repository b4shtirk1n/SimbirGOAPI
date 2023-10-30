using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SimbirGOAPI.Models;

public partial class SimbirGODbContext : DbContext
{
    public SimbirGODbContext()
    {
    }

    public SimbirGODbContext(DbContextOptions<SimbirGODbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Rent> Rents { get; set; }

    public virtual DbSet<RentType> RentTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transport> Transports { get; set; }

    public virtual DbSet<TransportType> TransportTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:SimbirGODb");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Color_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Rent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rent_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.TransportNavigation).WithMany(p => p.Rents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Transport_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Rents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RentType_fkey");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Rents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_fkey");
        });

        modelBuilder.Entity<RentType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("RentType_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transport_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.TransportOwnerNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Owner_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Transports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("TransportType_fkey");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.TransportUserNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_fkey");
        });

        modelBuilder.Entity<TransportType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransportType_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Role_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
