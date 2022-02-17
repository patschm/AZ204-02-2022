using System;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions options): base(options)
        {           
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductGroupProduct> ProductGroupProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(em=>{
                em.HasKey(p=>p.ID);
            });
            modelBuilder.Entity<Product>(em=>{
                em.HasKey(p=>p.ID);
                em.HasOne(p=>p.Brand).WithMany().HasForeignKey(p=>p.BrandID);
                em.HasMany(p=>p.ProductGroups).WithOne(p=>p.Product);
            });
            modelBuilder.Entity<ProductGroup>(em=>{
                em.HasKey(p=>p.ID);
                em.HasMany(p=>p.Products).WithOne(p=>p.ProductGroup);
            });
            modelBuilder.Entity<ProductGroupProduct>(em=>{
                em.HasKey(p=>new{p.ProductID, p.ProductGroupID});
            });
        }
    }
}
