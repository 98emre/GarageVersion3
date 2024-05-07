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
                .Select(v => new SelectListItem
                {
                    Text = $"{v.User.FirstName} {v.RegistrationNumber}",
                    Value = v.Id.ToString()
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

            var users = _context.User.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName} ({u.PersonalIdentifyNumber})",
                Value = u.Id.ToString()
            });

            ViewData["Users"] = users;

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
                   RegistrationNumber = pt.Vehicle.RegistrationNumber,
                   ParkingSpot = pt.ParkingSpot
               }).FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
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
                    UserId = pt.Vehicle.UserId,
                    ParkingSpot = pt.ParkingSpot
                }).FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }

            await GetUserVehicles(viewModel.UserId);
            await GetUsers();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ParkingLotViewModel parkingLotViewModel)
        {
            // 07/05/2024 00:26:07
            if (ModelState.IsValid)
            {
                try
                {
                    var parkingLot = await _context.ParkingLot
                        .Where(pt => pt.Id == id)
                        .FirstOrDefaultAsync();

                    if (parkingLot == null)
                    {
                        return NotFound();
                    }

                    parkingLot.VehicleId = parkingLotViewModel.VehicleId;
                   
                    _context.Update(parkingLot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();

                }

                return RedirectToAction(nameof(Index));
            }

            await GetUserVehicles(parkingLotViewModel.UserId);
            await GetUsers();
            return View(parkingLotViewModel);
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

        [HttpGet]
        public async Task<IActionResult> GetUserVehicles(int? userId)
        {
            var vehicles = await _context.Vehicle
                .Where(v => v.UserId == userId && !_context.ParkingLot.Any(pl => pl.VehicleId == v.Id))
                .Select(v => new SelectListItem
                {
                    Text = v.RegistrationNumber,
                    Value = v.Id.ToString()
                }).ToListAsync();

            ViewBag.Vehicles = vehicles.Count();

            return Json(vehicles);
        }

        private async Task GetUsers()
        {
            var users = await _context.User
                   .Select(u => new SelectListItem
                   {
                       Text = $"{u.FirstName} {u.LastName} ({u.PersonalIdentifyNumber})",
                       Value = u.Id.ToString()
                   }).ToListAsync();

            ViewBag.Users = users;
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
