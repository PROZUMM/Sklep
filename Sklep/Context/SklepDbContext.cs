using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Sklep.Context
{
    public class SklepDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=sklepDb;trusted_connection=true;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().
                HasMany(u => u.Orders).WithOne(u => u.User).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().
                HasOne(u => u.Cart).WithOne(u => u.User).HasForeignKey<User>(e => e.CartId);
            modelBuilder.Entity<Order>().
                HasMany(o => o.OrderedItems).WithMany(o => o.Orders);
            base.OnModelCreating(modelBuilder);
        }
    }
}