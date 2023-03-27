using Microsoft.EntityFrameworkCore;
using SmartVillages.Shared;
using SmartVillages.Shared.MarketplaceModels;
using SmartVillages.Shared.MessageModels;
using SmartVillages.Shared.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartVillages.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductCategory> ProductCategorys { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductRate> ProductRate { get; set; }
        
        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.Email, p.OIB })
                .IsUnique(true);
        }

        public DbSet<SmartVillages.Shared.UserModels.UserConnection> UserConnection { get; set; }

    }
}
