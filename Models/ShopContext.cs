using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class ShopContext : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(a => a.Category).HasForeignKey(z => z.CategoryId);
            modelBuilder.Entity<Order>().HasMany(c => c.Products);
            modelBuilder.Entity<Order>().HasOne(o => o.User);
            modelBuilder.Entity<User>().HasMany(o => o.Orders).WithOne(u => u.User).HasForeignKey(u => u.UserId);
            modelBuilder.Seed();
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

