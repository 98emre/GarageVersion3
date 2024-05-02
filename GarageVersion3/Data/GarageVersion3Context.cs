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
    }
}
