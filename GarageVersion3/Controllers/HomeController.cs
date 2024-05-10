using GarageVersion3.Data;
using GarageVersion3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GarageVersion3.Controllers
{
    public class HomeController : Controller
    {
        private readonly GarageVersion3Context _context;

        public HomeController(GarageVersion3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var parkedVehicles = await _context.ParkingLot
                .Include(v => v.Vehicle)
                .ThenInclude(v => v.VehicleType)
                .ToListAsync();

            var vehicleTypeCount = new Dictionary<string, int>();

            foreach (var vehicle in parkedVehicles)
            {
                if (vehicle.Vehicle.VehicleType != null)
                {
                    if (!vehicleTypeCount.ContainsKey(vehicle.Vehicle.VehicleType.Type))
                    {
                        vehicleTypeCount[vehicle.Vehicle.VehicleType.Type] = 0;
                    }

                    vehicleTypeCount[vehicle.Vehicle.VehicleType.Type]++;
                };
            }

            var totalWheels = parkedVehicles.Sum(v => v.Vehicle.NrOfWheels);
            var totalRevenue = _context.Receipt.Sum(r => r.Price);

            ViewBag.vehicleTypeCount = vehicleTypeCount;
            ViewBag.TotalWheels = totalWheels;
            ViewBag.TotalRevenue = totalRevenue.ToString("#,##0.00");

            return View();
        }
    }
}
