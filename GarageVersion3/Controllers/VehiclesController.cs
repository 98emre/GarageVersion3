using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;
using System.Drawing.Drawing2D;
using System.Drawing;
using GarageVersion3.Helpers;

namespace GarageVersion3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly GarageVersion3Context _context;

        public VehiclesController(GarageVersion3Context context)
        {
            _context = context;
        }


        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var viewModel = await _context.Vehicle
                .Include(v => v.User)
                .Include(v => v.VehicleType)
                .Include(v => v.ParkingLot)
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    RegistrationNumber = v.RegistrationNumber,
                    User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                    VehicleType = v.VehicleType.Type,
                    ParkingSpot = v.ParkingLot.ParkingSpot,
                    CheckInTime = v.ParkingLot.Checkin
                }).ToListAsync();

            return View(viewModel);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.Vehicle
                .Where(v => v.Id == id)
                .Include(v => v.User)
                .Include(v => v.VehicleType)
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                    VehicleType = v.VehicleType.Type,
                    RegistrationNumber = v.RegistrationNumber,
                    Brand = v.Brand,
                    Color = v.Color,
                    VehicleModel = v.VehicleModel,
                    NrOfWheels = v.NrOfWheels,
                    ParkingSpot = v.ParkingLot.ParkingSpot,
                    CheckInTime = v.ParkingLot.Checkin
                }).FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            DropdownDataLists();

            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var availableSpot = await GetAvailableParkingSpot();

                if (availableSpot == -1)
                {
                    ModelState.AddModelError(string.Empty, "No available parking spots.");
                    DropdownDataLists();
                    return View(viewModel);
                }

                var vehicle = new Vehicle
                {
                    VehicleTypeId = viewModel.VehicleTypeId,
                    UserId = viewModel.UserId,
                    RegistrationNumber = viewModel.RegistrationNumber,
                    Brand = viewModel.Brand,
                    Color = viewModel.Color,
                    VehicleModel = viewModel.VehicleModel,
                    NrOfWheels = viewModel.NrOfWheels,
                };

                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                var parkingLot = new ParkingLot
                {
                    VehicleId = vehicle.Id,
                    ParkingSpot = availableSpot,
                    Checkin = DateTime.Now,
                    AvailableParkingSpot = true
                };

                _context.Add(parkingLot);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            DropdownDataLists();
            return View(viewModel);
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

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var viewModel = new VehicleViewModel
            {
                Id = vehicle.Id,
                VehicleTypeId = vehicle.VehicleTypeId,
                UserId = vehicle.UserId,
                RegistrationNumber = vehicle.RegistrationNumber,
                Brand = vehicle.Brand,
                Color = vehicle.Color,
                VehicleModel = vehicle.VehicleModel,
                NrOfWheels = vehicle.NrOfWheels
            };


            DropdownDataLists();
            return View(viewModel);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleViewModel viewModel)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    var vehicle = await _context.Vehicle.FindAsync(id);

                    if (vehicle == null)
                    {
                        return NotFound();
                    }

                    vehicle.VehicleTypeId = viewModel.VehicleTypeId;
                    vehicle.UserId = viewModel.UserId;
                    vehicle.RegistrationNumber = viewModel.RegistrationNumber;
                    vehicle.Brand = viewModel.Brand;
                    vehicle.Color = viewModel.Color;
                    vehicle.VehicleModel = viewModel.VehicleModel;
                    vehicle.NrOfWheels = viewModel.NrOfWheels;

                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            DropdownDataLists();
            return View(viewModel);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FirstOrDefaultAsync(v => v.Id == id);
            var vehicleType = await _context.VehicleType.FirstOrDefaultAsync(vt => vt.Id == vehicle.VehicleTypeId);
            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == vehicle.UserId);


            if (vehicle == null || vehicleType == null || user == null)
            {
                return NotFound();
            }


            var viewModel = new VehicleViewModel
            {
                Id = vehicle.Id,
                VehicleTypeId = vehicleType.Id,
                VehicleType = vehicleType.Type,
                UserId = vehicle.UserId,
                User = $"{user.FirstName} {user.LastName} ({user.PersonalIdentifyNumber})",
                RegistrationNumber = vehicle.RegistrationNumber,
                Brand = vehicle.Brand,
                Color = vehicle.Color,
                VehicleModel = vehicle.VehicleModel,
                NrOfWheels = vehicle.NrOfWheels
            };

            return View(viewModel);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vehicle = await _context.Vehicle.FindAsync(id);
                if (vehicle == null)
                {
                    return NotFound();
                }

                ReceiptHelper helper = new ReceiptHelper(_context, id);
                ReceiptViewModel receiptVM = helper.CheckoutVehicle();
                return View("~/Views/Receipts/Details.cshtml", receiptVM);
            }

            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error occured while trying to remove a vehicle");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            var vehicles = await _context.Vehicle
                .Include(v => v.VehicleType)
                .Include(v => v.User)
                .Include(v => v.ParkingLot)
                .ToListAsync();

            if (vehicles.Count() == 0)
            {
                TempData["SortOnEmptyList"] = "The list is empty";
                return RedirectToAction(nameof(Index));
            }

            switch (sortOrder)
            {
                case "VehicleType":
                    vehicles = vehicles.OrderBy(v => v.VehicleType.Type).ToList();
                    TempData["Sort"] = "Vehicle type sort was done";
                    break;
                case "RegistrationNumber":
                    vehicles = vehicles.OrderBy(v => v.RegistrationNumber).ToList();
                    TempData["Sort"] = "Registration number sort was done";
                    break;
                case "User":
                    vehicles = vehicles.OrderBy(v => v.User.FirstName).ToList();
                    TempData["Sort"] = "User first name sort was done";
                    break;

                case "ParkingSpot":
                    vehicles = vehicles.OrderBy(v => v.ParkingLot.ParkingSpot).ToList();
                    TempData["Sort"] = "Parking Spot sort was done";
                    break;

                case "CheckInTime":
                    vehicles = vehicles.OrderBy(v => v.ParkingLot.Checkin).ToList();
                    TempData["Sort"] = "Check in sort was done";
                    break;

                default:
                    vehicles = vehicles.OrderBy(v => v.Id).ToList();
                 break;
            }

            var sortedVehicles = vehicles
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType.Type,
                            RegistrationNumber = v.RegistrationNumber,
                            User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                            UserId = v.UserId,
                            ParkingSpot = v.ParkingLot.ParkingSpot,
                            CheckInTime = v.ParkingLot.Checkin,
                            VehicleTypeId = v.VehicleTypeId
                        }).ToList();

            return View("Index", sortedVehicles);
        }

        
        [HttpGet]
        public async Task<IActionResult> Filter(string registrationNumber, string color, string brand)
        {
            if (string.IsNullOrEmpty(registrationNumber) && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(brand))
            {
                TempData["SearchFail"] = "Please provide input for at least one search criteria";
                var empyList = new List<VehicleViewModel>();
                return View("Index", empyList);
            }

            var query = _context.Vehicle.AsQueryable();

            if (!string.IsNullOrEmpty(registrationNumber))
            {
                query = query.Where(v => v.RegistrationNumber.Equals(registrationNumber.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(v => v.Color.Equals(color.Trim()));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v => v.Brand.Equals(brand.Trim()));
            }

            var search = await query
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType.Type,
                            RegistrationNumber = v.RegistrationNumber,
                            User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                            ParkingSpot = v.ParkingLot.ParkingSpot,
                            CheckInTime = v.ParkingLot.Checkin
                        }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "No vehicles found";
            }

            else
            {
                TempData["SearchSuccess"] = "Search was successful";
            }

            return View("Index", search);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            var query = _context.Vehicle.AsQueryable();
            var search = await query
                      .Select(v => new VehicleViewModel
                      {
                          Id = v.Id,
                          VehicleType = v.VehicleType.Type,
                          RegistrationNumber = v.RegistrationNumber,
                          User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                          ParkingSpot = v.ParkingLot.ParkingSpot,
                          CheckInTime = v.ParkingLot.Checkin
                      }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "There is no vehicles in the system";
            }

            else
            {
                TempData["SearchSuccess"] = "Showing all vehicles was successful";
            }

            return View("Index", search);
        }


        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }


        private void DropdownDataLists()
        {
            var users = _context.User.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName} ({u.PersonalIdentifyNumber})",
                Value = u.Id.ToString()
            });


            ViewData["Users"] = users;

            var vehicleTypes = _context.VehicleType.Select(vt => new SelectListItem
            {
                Text = vt.Type,
                Value = vt.Id.ToString()
            }).ToList();

            ViewData["VehicleTypes"] = vehicleTypes;
        }


        [HttpGet]
        public IActionResult Statistics()
        {
            var parkedVehicles = _context.Vehicle.Include(v => v.VehicleType).ToList();

            var vehicleTypeCount = new Dictionary<string, int>();

            foreach (var vehicle in parkedVehicles)
            {
                if (vehicle.VehicleType != null)
                {
                    if (!vehicleTypeCount.ContainsKey(vehicle.VehicleType.Type))
                    {
                        vehicleTypeCount[vehicle.VehicleType.Type] = 0;
                    }

                    vehicleTypeCount[vehicle.VehicleType.Type]++;
                };
            }

            var totalWheels = parkedVehicles.Sum(v => v.NrOfWheels);

            ViewBag.VehicleType = vehicleTypeCount;
            ViewBag.TotalWheels = totalWheels;

            return View();
        }
    }
}
