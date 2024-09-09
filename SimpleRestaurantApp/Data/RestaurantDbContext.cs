using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SimpleRestaurantApp.Models;

namespace SimpleRestaurantApp.Data;

public partial class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Food> Foods { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__customer__3213E83F347EB869");

            entity.ToTable("customers");

            entity.HasIndex(e => e.Email, "UQ__customer__AB6E61649264FDDE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phonenumber");
        });

        modelBuilder.Entity<Food>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__foods__3213E83FF7D4BB27");

            entity.ToTable("foods");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__transact__3213E83FBF8B4FB5");

            entity.ToTable("transactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Idcustomers).HasColumnName("idcustomers");
            entity.Property(e => e.Idfood).HasColumnName("idfood");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Totalprice).HasColumnName("totalprice");
            entity.Property(e => e.Transactiondate)
                .HasColumnType("datetime")
                .HasColumnName("transactiondate");

            entity.HasOne(d => d.IdcustomersNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Idcustomers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__idcus__619B8048");

            entity.HasOne(d => d.IdfoodNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Idfood)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__transacti__idfoo__628FA481");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
