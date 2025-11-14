using FitHub.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Users
            b.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.UserId);
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Role).HasMaxLength(20);
                e.HasOne(x => x.Customer)
                 .WithOne(c => c.User)
                 .HasForeignKey<Customer>(c => c.UserId);
            });

            // Customers
            b.Entity<Customer>(e =>
            {
                e.ToTable("Customers");
                e.HasKey(x => x.CustomerId);
                e.HasIndex(x => x.Email);
            });

            // Products
            b.Entity<Product>(e =>
            {
                e.ToTable("Products");
                e.HasKey(x => x.ProductId);
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            });

            // Orders
            b.Entity<Order>(e =>
            {
                e.ToTable("Orders");
                e.HasKey(x => x.OrderId);
                e.Property(x => x.Status).HasMaxLength(20);
                e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
                e.HasMany(x => x.Items)
                 .WithOne(i => i.Order)
                 .HasForeignKey(i => i.OrderId);
            });

            // OrderItems
            b.Entity<OrderItem>(e =>
            {
                e.ToTable("OrderItems");
                e.HasKey(x => x.OrderItemId);
                e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                e.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId);
            });
        }
    }
}
