using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageVersion3.Controllers
{
    public class ParkingLotController : Controller
    {
        private readonly GarageVersion3Context _context;

        public ParkingLotController(GarageVersion3Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _context.ParkingLot
                .Include(pt => pt.Vehicle)
                .ThenInclude(pt => pt.User)
                .Select(pt => new ParkingLotViewModel
                {
                    Id = pt.Id,
                    RegistrationNumber = pt.Vehicle.RegistrationNumber,
                    ParkingSpot = pt.ParkingSpot,
                    Checkin = pt.Checkin,
                    User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})"
                }).ToListAsync();
                
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vehicles = await _context.Vehicle
                .Where(v => !_context.ParkingLot.Any(pl => pl.VehicleId == v.Id))
                 .Select(v => new VehicleViewModel
                 {
                     Id = v.Id,
                     RegistrationNumber = v.RegistrationNumber,
                     User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                     VehicleType = v.VehicleType.Type,
                 }).ToListAsync();

            return View(vehicles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParkingLotViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var availableSpot = await GetAvailableParkingSpot();

                if (availableSpot == -1)
                {
                    ModelState.AddModelError(string.Empty, "No available parking spots.");
                    return View(viewModel);
                }

                var parkingLot = new ParkingLot
                {
                    AvailableParkingSpot = true,
                    Checkin = DateTime.Now,
                    VehicleId = viewModel.VehicleId,
                    ParkingSpot = availableSpot
                };

                _context.Add(parkingLot);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ParkingLot
               .Where(pt => pt.Id == id)
               .Include(pt => pt.Vehicle)
               .ThenInclude(pt => pt.User)
               .Select(pt => new ParkingLotViewModel
               {
                   Id = pt.Id,
                   User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})",
                   Checkin = DateTime.Now,
                   VehicleViewModel = new VehicleViewModel
                   {
                       RegistrationNumber = pt.Vehicle.RegistrationNumber,
                       Brand = pt.Vehicle.Brand,
                       VehicleModel = pt.Vehicle.VehicleModel,
                       NrOfWheels = pt.Vehicle.NrOfWheels,
                       VehicleType = pt.Vehicle.VehicleType.Type,
                       Color = pt.Vehicle.Color
                   },
                   ParkingSpot = pt.ParkingSpot
               }).FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ParkingLot
                .Where(pt => pt.Id == id)
                .Select(pt => new ParkingLotViewModel
                {
                    Id = pt.Id,
                    RegistrationNumber = pt.Vehicle.RegistrationNumber,
                    User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})",
                    Checkin = pt.Checkin,
                    ParkingSpot = pt.ParkingSpot
                }).FirstOrDefaultAsync();
               
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var parkingLot = await _context.ParkingLot.FindAsync(id);

                if (parkingLot == null)
                {
                    return NotFound();
                }

                _context.ParkingLot.Remove(parkingLot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occured while trying to remove a vehicle from parking lot");
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<int> GetAvailableParkingSpot()
        {
            var parkingLot = await _context.ParkingLot.OrderBy(pl => pl.ParkingSpot).ToListAsync();

            int nextSpot = 1;

            foreach (var spot in parkingLot)
            {
                if (spot.ParkingSpot != nextSpot)
                {
                    return nextSpot;
                }

                nextSpot++;
            }

            return nextSpot;
        }

    }
}
