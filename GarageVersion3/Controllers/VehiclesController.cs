using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageVersion3.Data;
using GarageVersion3.Models;
using GarageVersion3.Models.ViewModels;

namespace GarageVersion3.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly GarageVersion3Context _context;

        public VehiclesController(GarageVersion3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var viewModel = await _context.Vehicle
                .Include(v => v.User)
                .Include(v => v.VehicleType)
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    RegistrationNumber = v.RegistrationNumber,
                    User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                    VehicleType = v.VehicleType.Type,
                }).ToListAsync();

            return View(viewModel);
        }

        [HttpGet]
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
                    NrOfWheels = v.NrOfWheels
                }).FirstOrDefaultAsync();

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            DropdownDataLists();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                bool existingVehicle = _context.Vehicle.Any(v => v.RegistrationNumber.ToUpper().Trim().Replace(" ", "").Equals(viewModel.RegistrationNumber.Trim().ToUpper().Replace(" ", "")));
                int nrOfCharacters = viewModel.RegistrationNumber.ToUpper().Trim().Replace(" ", "").Count();

                  
                if (existingVehicle || nrOfCharacters < 6)
                {
                    ModelState.AddModelError("RegistrationNumber", existingVehicle ? "A vehicle with this registration number already exists":
                                                                                     "Registration number must be between 6 and 10");
                    DropdownDataLists();
                    return View();
                }

                var vehicle = new Vehicle
                {
                    VehicleTypeId = viewModel.VehicleTypeId,
                    UserId = viewModel.UserId,
                    RegistrationNumber = viewModel.RegistrationNumber.ToUpper().Trim().Replace(" ",""),
                    Brand = viewModel.Brand.Trim(),
                    Color = viewModel.Color.Trim(),
                    VehicleModel = viewModel.VehicleModel.Trim(),
                    NrOfWheels = viewModel.NrOfWheels,
                };

                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            DropdownDataLists();
            return View(viewModel);
        }

        [HttpGet]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VehicleViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var vehicle = await _context.Vehicle.FindAsync(id);

                    if (vehicle == null)
                    {
                        return NotFound();
                    }

                    var existingVehicleWithSameRegNumber = await _context.Vehicle
                        .Where(v => v.Id != id && v.RegistrationNumber.ToUpper().Trim().Replace(" ", "").Equals(viewModel.RegistrationNumber.ToUpper().Trim().Replace(" ", "")))
                        .FirstOrDefaultAsync();

                    int nrOfCharacters = viewModel.RegistrationNumber.ToUpper().Trim().Replace(" ", "").Count();

                    if (existingVehicleWithSameRegNumber != null || nrOfCharacters < 6)
                    {
                        ModelState.AddModelError("RegistrationNumber", (existingVehicleWithSameRegNumber != null) ? "A vehicle with this registration number already exists" : "Registration number must be between 6 and 10");
                        DropdownDataLists();
                        return View();
                    }

                    vehicle.VehicleTypeId = viewModel.VehicleTypeId;
                    vehicle.UserId = viewModel.UserId;
                    vehicle.RegistrationNumber = viewModel.RegistrationNumber.ToUpper().Trim().Replace(" ", "");
                    vehicle.Brand = viewModel.Brand.Trim();
                    vehicle.Color = viewModel.Color.Trim();
                    vehicle.VehicleModel = viewModel.VehicleModel.Trim();
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

        [HttpGet]
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

                _context.Vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

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
                    vehicles = vehicles
                        .OrderBy(v => v.User.FirstName.Substring(0, 2))
                        .ThenBy(v => v.User.FirstName)
                        .ToList();
                    TempData["Sort"] = "Owner sort was done";
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
                            VehicleTypeId = v.VehicleTypeId
                        }).ToList();

            return View("Index", sortedVehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string registrationNumber, string color, string brand, string vehicleType)
        {
            if (string.IsNullOrEmpty(registrationNumber) && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(brand) && string.IsNullOrEmpty(vehicleType))
            {
                TempData["SearchMessage"] = "Please provide input for at least one search criteria";
                TempData["SearchStatus"] = "alert alert-warning";
                return View("Index", new List<VehicleViewModel>());
            }

            var query = _context.Vehicle.AsQueryable();

            if (!string.IsNullOrEmpty(registrationNumber))
            {
                query = query.Where(v => v.RegistrationNumber.Replace(" ", "").ToUpper().Trim().Equals(registrationNumber.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(v => v.Color.Replace(" ", "").ToUpper().Trim().Equals(color.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v => v.Brand.Replace(" ", "").ToUpper().Trim().Equals(brand.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(vehicleType))
            {
                query = query.Where(v => v.VehicleType.Type.Replace(" ", "").Trim().ToUpper().Equals(vehicleType.Replace(" ", "").Trim().ToUpper()));
            }

            var search = await query
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType.Type,
                            Brand = v.Brand,
                            Color = v.Color,
                            NrOfWheels = v.NrOfWheels,
                            UserId = v.UserId,
                            RegistrationNumber = v.RegistrationNumber,
                            User = $"{v.User.FirstName} {v.User.LastName} ({v.User.PersonalIdentifyNumber})",
                        }).ToListAsync();

            TempData["SearchMessage"] = (search.Count == 0) ? "No vehicles could be found" : "Search was successful";
            TempData["SearchStatus"] = (search.Count == 0) ? "alert alert-warning" :"alert alert-success";

            return View("Index", search);
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            TempData["SearchMessage"] = (_context.Vehicle.ToList().Count == 0) ? "There are no vehicles in the system" : "Showing all vehicles was successful";
            TempData["SearchStatus"] = (_context.Vehicle.ToList().Count == 0) ? "alert alert-warning" : "alert alert-success";

            return RedirectToAction(nameof(Index));
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
            });

            ViewData["VehicleTypes"] = vehicleTypes;
        }
    }
}
