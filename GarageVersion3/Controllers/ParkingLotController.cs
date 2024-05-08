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

        public ParkingLotController(GarageVersion3Context context)
        {
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
                    User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})",
                    VehicleViewModel = new VehicleViewModel
                    {
                        Id = pt.Vehicle.Id,
                        RegistrationNumber = pt.Vehicle.RegistrationNumber,
                        Brand = pt.Vehicle.Brand,
                        VehicleModel = pt.Vehicle.VehicleModel,
                        NrOfWheels = pt.Vehicle.NrOfWheels,
                        VehicleType = pt.Vehicle.VehicleType.Type,
                        Color = pt.Vehicle.Color
                    }
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
        public async Task<IActionResult> FilterIndex(string firstName, string lastName, string personalIdentifyNumber)
        {
            var query = _context.ParkingLot.AsQueryable();

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(personalIdentifyNumber))
            {
                TempData["SearchFail"] = "Please provide input for at least one search criteria";
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

            if (!string.IsNullOrEmpty(personalIdentifyNumber))
            {
                query = query.Where(u => u.Vehicle.User.PersonalIdentifyNumber.Equals(personalIdentifyNumber));
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

            if (searchResults.Count == 0)
            {
                TempData["SearchFail"] = "No users were found";
            }
            else
            {
                TempData["SearchSuccess"] = "Search was successful";
            }

            return View("Index", searchResults);
        }

        [HttpGet]
        public async Task<IActionResult> FilterCreate(string firstName, string lastName, string personalIdentifyNumber)
        {
            var query = _context.Vehicle.AsQueryable();

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(personalIdentifyNumber))
            {
                TempData["SearchFail"] = "Please provide input for at least one search criteria";
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

            if (!string.IsNullOrEmpty(personalIdentifyNumber))
            {
                query = query.Where(u => u.User.PersonalIdentifyNumber.Equals(personalIdentifyNumber));
            }

            var searchResults = await query
                .Where(pt => !_context.ParkingLot.Any(pl => pl.VehicleId == pt.Id))
                .Select(pt => new VehicleViewModel
                {
                    Id = pt.Id,
                    RegistrationNumber = pt.RegistrationNumber,
                    User = $"{pt.User.FirstName} {pt.User.LastName} ({pt.User.PersonalIdentifyNumber})"
                }).ToListAsync();

            if (searchResults.Count == 0)
            {
                TempData["SearchFail"] = "No users were found";
            }
            else
            {
                TempData["SearchSuccess"] = "Search was successful";
            }

            return View("Create", searchResults);
        }


        [HttpGet]
        public async Task<IActionResult> ShowAllIndex()
        {
            var query = _context.ParkingLot.AsQueryable();
            var search = await query
                      .Select(pt => new ParkingLotViewModel
                      {
                          Id = pt.Id,
                          RegistrationNumber = pt.Vehicle.RegistrationNumber,
                          User = $"{pt.Vehicle.User.FirstName} {pt.Vehicle.User.LastName} ({pt.Vehicle.User.PersonalIdentifyNumber})",
                          ParkingSpot = pt.ParkingSpot,
                          Checkin = pt.Checkin
                      }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "There are no vehicles in the system";
            }
            else
            {
                TempData["SearchSuccess"] = "Showing all vehicles was successful";
            }

            return View("Index", search);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllCreate()
        {
            var query = _context.Vehicle
                        .Where(v => !_context.ParkingLot.Any(pl => pl.VehicleId == v.Id))
                        .AsQueryable();

            var search = await query
                       .Select(v => new VehicleViewModel
                       {
                           Id = v.Id,
                           RegistrationNumber = v.RegistrationNumber,
                           User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})"
                       }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "There are no vehicles in the system";
            }
            else
            {
                TempData["SearchSuccess"] = "Showing all vehicles was successful";
            }

            return View("Create", search);
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
