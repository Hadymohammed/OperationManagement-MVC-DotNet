using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OperationManagement.Models;
using System;

namespace OperationManagement.Data
{
    public class AppDBContext :IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Identity
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            //Relations

            //Enterprise Has Staff
            modelBuilder.Entity<ApplicationUser>().HasOne(s => s.Enterprise).WithMany(e => e.Staff).HasForeignKey(s => s.EnterpriseId);
            //Enterprise Has Cagegories
            modelBuilder.Entity<Category>().HasOne(s => s.Enterprise).WithMany(e => e.Categories).HasForeignKey(s => s.EnterpriseId);
            //Enterprise Has Components
            modelBuilder.Entity<Component>().HasOne(s => s.Enterprise).WithMany(e => e.Components).HasForeignKey(s => s.EnterpriseId);
            //Component Has Photos
            modelBuilder.Entity<ComponentPhoto>().HasOne(s => s.Component).WithMany(e => e.Photos).HasForeignKey(s => s.ComponentId);
            //Enterprise Has Measurements
            modelBuilder.Entity<Measurement>().HasOne(s => s.Enterprise).WithMany(e => e.Measurements).HasForeignKey(s => s.EnterpriseId);
            //Enterprise Has DeliveryLocations
            modelBuilder.Entity<DeliveryLocation>().HasOne(s => s.Enterprise).WithMany(e => e.DeliveryLocations).HasForeignKey(s => s.EnterpriseId);
            //Enterprise Has Processes
            modelBuilder.Entity<Process>().HasOne(s => s.Enterprise).WithMany(e => e.Processes).HasForeignKey(s => s.EnterpriseId);
            //Enterprise Has ProcessCategories
            modelBuilder.Entity<ProcessCategory>().HasOne(s => s.Enterprise).WithMany(e => e.ProcessCategories).HasForeignKey(s => s.EnterpriseId);
            //Process Has Status
            modelBuilder.Entity<ProcessStatus>().HasOne(s => s.Process).WithMany(e => e.Statuses).HasForeignKey(s => s.ProcessId);
            //Process Has ProcessCategory
            modelBuilder.Entity<Process>().HasOne(s => s.Category).WithMany(e => e.Processes).HasForeignKey(s => s.CategoryId);
            //Enterprise Has Specifications
            modelBuilder.Entity<Specification>().HasOne(s => s.Enterprise).WithMany(e => e.Specifications).HasForeignKey(s => s.EnterpriseId);
            //Specification Has Status
            modelBuilder.Entity<SpecificationStatus>().HasOne(s => s.Specification).WithMany(e => e.Statuses).HasForeignKey(s => s.SpecificationId);
            //Specification Has Options
            modelBuilder.Entity<SpecificationOption>().HasOne(s => s.Specification).WithMany(e => e.Options).HasForeignKey(s => s.SpecificationId);
            //Enterprise Has Customers
            modelBuilder.Entity<Customer>().HasOne(s => s.Enterprise).WithMany(e => e.Customers).HasForeignKey(s => s.EnterpriseId);
            //Customer Has Contacts
            modelBuilder.Entity<CustomerContact>().HasOne(s => s.Customer).WithMany(e => e.Contacts).HasForeignKey(s => s.CustomerId);
            //Customer Has Orders
            modelBuilder.Entity<Order>().HasOne(s => s.Customer).WithMany(e => e.Orders).HasForeignKey(s => s.CustomerId);
            //Order Has DeliveryLocation
            modelBuilder.Entity<Order>().HasOne(s => s.DeliveryLocation).WithMany(e => e.Orders).HasForeignKey(s => s.DeliveryLocationId).OnDelete(DeleteBehavior.Restrict); ;
            //Order Has Products
            modelBuilder.Entity<Product>().HasOne(s => s.Order).WithMany(e => e.Products).HasForeignKey(s => s.OrderId).OnDelete(DeleteBehavior.Restrict); ;
            //Order Has Attachments
            modelBuilder.Entity<Attachment>().HasOne(s => s.Order).WithMany(e => e.Attachments).HasForeignKey(s => s.OrderId);
            //Product Has Category
            modelBuilder.Entity<Product>().HasOne(s => s.Category).WithMany(e => e.Products).HasForeignKey(s => s.CategoryId);
            //Product Has Components
            modelBuilder.Entity<ProductComponent>().HasOne(s => s.Product).WithMany(e => e.Components).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductComponent>().HasOne(s => s.Component).WithMany(e => e.Products).HasForeignKey(s => s.ComponentId).OnDelete(DeleteBehavior.Restrict); ;
            //Product Has Measurements
            modelBuilder.Entity<ProductMeasurement>().HasOne(s => s.Product).WithMany(e => e.Measurements).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductMeasurement>().HasOne(s => s.Measurement).WithMany(e => e.Products).HasForeignKey(s => s.MeasurementId).OnDelete(DeleteBehavior.Restrict); ;
            //Product Has Process
            modelBuilder.Entity<ProductProcess>().HasOne(s => s.Product).WithMany(e => e.Processes).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductProcess>().HasOne(s => s.Process).WithMany(e => e.Products).HasForeignKey(s => s.ProcessId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductProcess>().HasOne(s => s.Status).WithMany(e => e.Products).HasForeignKey(s => s.StatusId);
            //Product Has Specifications
            modelBuilder.Entity<ProductSpecification>().HasOne(s => s.Product).WithMany(e => e.Specifications).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductSpecification>().HasOne(s => s.Specification).WithMany(e => e.Products).HasForeignKey(s => s.SpecificationId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductSpecification>().HasOne(s => s.Status).WithMany(e => e.Products).HasForeignKey(s => s.StatusId).OnDelete(DeleteBehavior.Restrict); ;
            modelBuilder.Entity<ProductSpecification>().HasOne(s => s.Option).WithMany(e => e.Products).HasForeignKey(s => s.OptionId).OnDelete(DeleteBehavior.Restrict); ;

        }

        //Tables
        public DbSet<Enterprise> Enterprises { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentPhoto> ComponentPhotos { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<DeliveryLocation> DeliveryLocations { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessStatus> ProcessStatuses { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SpecificationStatus> SpecificationStatuses { get; set; }
        public DbSet<SpecificationOption> SpecificationOptions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductComponent> ProductComponents { get; set; }
        public DbSet<ProductMeasurement> ProductMeasurements { get; set; }
        public DbSet<ProductProcess> ProductProcesses { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<ProcessCategory> ProcessCategories { get; set; }
    }
}
