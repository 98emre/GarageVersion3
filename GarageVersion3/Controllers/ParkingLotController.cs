using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using System.Drawing;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using GarageVersion3.Helpers;

namespace GarageVersion3.Controllers
{
    public class ParkingLotController : Controller
    {
        private readonly GarageVersion3Context _context;
        private int maxParkingSize;

        public ParkingLotController(GarageVersion3Context context)
        {
            maxParkingSize = 25;
            _context = context;
        }

        [HttpGet]
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
                    VehicleId = pt.VehicleId,
                    User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})",
                    VehicleViewModel = new VehicleViewModel
                    {
                        Id = pt.Vehicle.Id,
                        RegistrationNumber = pt.Vehicle.RegistrationNumber,
                        Brand = pt.Vehicle.Brand,
                        VehicleModel = pt.Vehicle.VehicleModel,
                        NrOfWheels = pt.Vehicle.NrOfWheels,
                        VehicleType = pt.Vehicle.VehicleType.Type,
                        Color = pt.Vehicle.Color,
                        MaxParkingSize = maxParkingSize
                    }
                }).ToListAsync();

            ViewBag.Vehicles = maxParkingSize - _context.ParkingLot.Count();
                
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Vehicles = _context.ParkingLot.ToList().Count();

            if (ViewBag.Vehicles == maxParkingSize)
            {
                return RedirectToAction(nameof(Index));
            }

            var vehicles = await _context.Vehicle
                 .Where(v => !_context.ParkingLot.Any(pl => pl.VehicleId == v.Id))
                 .Select(v => new VehicleViewModel
                 {
                     Id = v.Id,
                     RegistrationNumber = v.RegistrationNumber,
                     User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                     VehicleType = v.VehicleType.Type,
                     MaxParkingSize = maxParkingSize
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
                ViewBag.Vehicles = _context.ParkingLot.ToList().Count();

                if (availableSpot == -1 || ViewBag.Vehicles == maxParkingSize)
                {
                    ModelState.AddModelError(string.Empty, "No available parking spots.");
                    return RedirectToAction(nameof(Index));
                }

                var parkingLot = new ParkingLot
                {
                    AvailableParkingSpot = false,
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
                   VehicleId = pt.VehicleId,
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
                var parkingLot = _context.ParkingLot.Find(id);

                if (parkingLot == null)
                {
                    return NotFound();
                }

                ReceiptHelper helper = new ReceiptHelper(_context, parkingLot);
                helper.CheckoutVehicle();

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
        public async Task<IActionResult> FilterIndex(string firstName, string lastName)
        {
            ViewBag.Vehicles = maxParkingSize - _context.ParkingLot.ToList().Count();

            var query = _context.ParkingLot.AsQueryable();
            ModelState.Remove("firstName");
            ModelState.Remove("lastName");
           

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                TempData["SearchMessage"] = "Please provide input for at least one search criteria";
                TempData["SearchStatus"] = "alert alert-warning";
                var empyList = new List<ParkingLotViewModel>();
                return View("Index", empyList);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(u => u.Vehicle.User.FirstName.Trim().ToUpper().Equals(firstName.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(u => u.Vehicle.User.LastName.Trim().ToUpper().Equals(lastName.ToUpper().Trim()));
            }

            var searchResults = await query
                .Include(pt => pt.Vehicle)
                .ThenInclude(pt => pt.User)
                .Select(pt => new ParkingLotViewModel
                {
                    Id = pt.Id,
                    VehicleId = pt.Vehicle.Id,
                    RegistrationNumber = pt.Vehicle.RegistrationNumber,
                    ParkingSpot = pt.ParkingSpot,
                    Checkin = pt.Checkin,
                    User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})"
                }).ToListAsync();

            TempData["SearchMessage"] = (searchResults.Count == 0) ? "Could not find the receipt for the user" : "Search was successful";
            TempData["SearchStatus"] = (searchResults.Count == 0) ? "alert alert-warning" : "alert alert-success";

        
            return View("Index", searchResults);
        }

        [HttpGet]
        public async Task<IActionResult> FilterCreate(string firstName, string lastName)
        {
            ViewBag.Vehicles = maxParkingSize - _context.ParkingLot.ToList().Count();
            var query = _context.Vehicle.AsQueryable();

            ModelState.Remove("firstName");
            ModelState.Remove("lastName");

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                TempData["SearchMessage"] = "Please provide input for at least one search criteria";
                TempData["SearchStatus"] = "alert alert-warning";
                var empyList = new List<VehicleViewModel>();
                return View("Create", empyList);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(u => u.User.FirstName.Trim().ToUpper().Equals(firstName.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(u => u.User.LastName.Trim().ToUpper().Equals(lastName.ToUpper().Trim()));
            }

            var searchResults = await query
                .Where(pt => !_context.ParkingLot.Any(pl => pl.VehicleId == pt.Id))
                .Select(pt => new VehicleViewModel
                {
                    Id = pt.Id,
                    VehicleType = pt.VehicleType.Type,
                    RegistrationNumber = pt.RegistrationNumber,
                    User = $"{pt.User.FirstName} {pt.User.LastName} ({pt.User.PersonalIdentifyNumber})"
                }).ToListAsync();

            TempData["SearchMessage"] = (searchResults.Count == 0) ? "Could not find the receipt for the user" : "Search was successful";
            TempData["SearchStatus"] = (searchResults.Count == 0) ? "alert alert-warning" : "alert alert-success";

            return View("Create", searchResults);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll(bool status)
        {
            var message = status ? "There are no vehicles to choose from" : "There are no vehicles in the parking lot";
            var count = status ? _context.ParkingLot.Count() : _context.Vehicle.Where(pt => !_context.ParkingLot.Any(v => v.VehicleId == pt.Id)).Count();

            TempData["SearchMessage"] = (count == 0) ? message : "Showing all vehicles was successful";
            TempData["SearchStatus"] = (count == 0) ? "alert alert-warning" : "alert alert-success";

            return RedirectToAction(status ? nameof(Index) : nameof(Create));

        }

        private async Task<int> GetAvailableParkingSpot()
        {
            var parkingLot = await _context.ParkingLot.OrderBy(pl => pl.ParkingSpot).ToListAsync();

            if(parkingLot.Count() == maxParkingSize)
            {
                return -1;
            }

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
