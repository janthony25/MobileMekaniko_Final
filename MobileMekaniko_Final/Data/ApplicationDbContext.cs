﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileMekaniko_Final.Models;

namespace MobileMekaniko_Final.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Make> Makes { get; set; }
        public DbSet<CarMake> CarMakes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<QuotationItem> QuotationItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table names to match DbSet names
            builder.Entity<Quotation>().ToTable("Quotations");
            builder.Entity<QuotationItem>().ToTable("QuotationItems");

            // One to Many Customer-Car
            builder.Entity<Customer>()
                .HasMany(c => c.Car)
                .WithOne(car => car.Customer)
                .HasForeignKey(car => car.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many to Many Car-Make
            builder.Entity<CarMake>()
                .HasKey(cm => new {cm.CarId, cm.MakeId});

            builder.Entity<CarMake>()
                .HasOne(cm => cm.Car)
                .WithMany(car => car.CarMake)
                .HasForeignKey(cm => cm.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CarMake>()
                .HasOne(cm => cm.Make)
                .WithMany(make => make.CarMake)
                .HasForeignKey(cm => cm.MakeId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Car-Invoice
            builder.Entity<Car>()
                .HasMany(car => car.Invoice)
                .WithOne(i => i.Car)
                .HasForeignKey(i => i.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Invoice-Invoice Item
            builder.Entity<Invoice>()
                .HasMany(i => i.InvoiceItem)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Car-Quotation
            builder.Entity<Car>()
                .HasMany(car => car.Quotation)
                .WithOne(q => q.Car)
                .HasForeignKey(q => q.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Quotation-Quotation Item
            builder.Entity<Quotation>()
                .HasMany(q => q.QuotationItem)
                .WithOne(qi => qi.Quotation)
                .HasForeignKey(qi => qi.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);
                
        }
    }   
}
