using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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


        }
    }   
}
