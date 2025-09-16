using Microsoft.EntityFrameworkCore;
using PostgreSqlJsonbDemo.Models;

namespace PostgreSqlJsonbDemo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasPrecision(18, 2);

            entity.Property(e => e.Specifications)
                .HasColumnType("jsonb");

            entity.Property(e => e.Metadata)
                .HasColumnType("jsonb");

            entity.Property(e => e.Tags)
                .HasColumnType("jsonb");

            entity.HasIndex(e => e.Name);

            entity.HasIndex(e => e.Specifications)
                .HasMethod("gin");

            entity.HasIndex(e => e.Tags)
                .HasMethod("gin");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.Property(e => e.Profile)
                .HasColumnType("jsonb");

            entity.Property(e => e.Preferences)
                .HasColumnType("jsonb");

            entity.Property(e => e.Address)
                .HasColumnType("jsonb");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasIndex(e => e.Profile)
                .HasMethod("gin");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.Property(e => e.Items)
                .HasColumnType("jsonb");

            entity.Property(e => e.ShippingAddress)
                .HasColumnType("jsonb");

            entity.Property(e => e.PaymentInfo)
                .HasColumnType("jsonb");

            entity.Property(e => e.OrderHistory)
                .HasColumnType("jsonb");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);

            entity.HasIndex(e => e.Items)
                .HasMethod("gin");
        });

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Level).HasMaxLength(20);
            entity.Property(e => e.Message).HasMaxLength(1000);

            entity.Property(e => e.Data)
                .HasColumnType("jsonb");

            entity.Property(e => e.Context)
                .HasColumnType("jsonb");

            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.Timestamp);

            entity.HasIndex(e => e.Data)
                .HasMethod("gin");
        });
    }
}