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
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    RegistrationNumber = v.RegistrationNumber,
                    User = $"{v.User.FirstName} {v.User.LastName} ({v.User.BirthDate})",
                    VehicleType = v.VehicleType.Type
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
                    RegistrationNumber = v.RegistrationNumber,
                    User = $"{v.User.FirstName} {v.User.LastName} ({v.User.BirthDate})",
                    VehicleType = v.VehicleType.Type
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
                var vehicle = new Vehicle
                {
                    VehicleTypeId = viewModel.VehicleTypeId,
                    UserId = viewModel.UserId,
                    RegistrationNumber = viewModel.RegistrationNumber
                };

                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            DropdownDataLists();
            return View(viewModel);
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
                RegistrationNumber = vehicle.RegistrationNumber
            };

            DropdownDataLists();
            return View(viewModel);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  VehicleViewModel viewModel)
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
                User = $"{user.FirstName} {user.LastName} ({user.BirthDate})",
                RegistrationNumber = vehicle.RegistrationNumber
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

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }


        private void DropdownDataLists()
        {
            var users = _context.User.Select(u => new SelectListItem
            {
                Text = $"{u.FirstName} {u.LastName} ({u.BirthDate})",
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
    }
}
