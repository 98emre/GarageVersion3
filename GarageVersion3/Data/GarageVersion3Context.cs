using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GarageVersion3.Models;

namespace GarageVersion3.Data
{
    public class GarageVersion3Context : DbContext
    {
        public GarageVersion3Context (DbContextOptions<GarageVersion3Context> options)
            : base(options)
        {
        }

        public DbSet<GarageVersion3.Models.ParkingLot> ParkingLot { get; set; } = default!;
        public DbSet<GarageVersion3.Models.Receipt> Receipt { get; set; } = default!;
        public DbSet<GarageVersion3.Models.User> User { get; set; } = default!;
        public DbSet<GarageVersion3.Models.Vehicle> Vehicle { get; set; } = default!;
        public DbSet<GarageVersion3.Models.VehicleType> VehicleType { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VehicleType>().HasData(
                new VehicleType { Id = 1, Type = "Car", ParkingSize = 1 },
                new VehicleType { Id = 2, Type = "Bus", ParkingSize = 1 },
                new VehicleType { Id = 3, Type = "Truck", ParkingSize = 2 },
                new VehicleType { Id = 4, Type = "Boat", ParkingSize = 3 },
                new VehicleType { Id = 5, Type = "Airplane", ParkingSize = 3 }
            );
        }
    }
}
