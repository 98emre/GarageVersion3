using GarageVersion3.Data;
using GarageVersion3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageVersion3.Controllers
{
    public class VehicleTypeController : Controller
    {
        private readonly GarageVersion3Context _context;

        public VehicleTypeController(GarageVersion3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleType vehicleType)
        {
            var existingType = await _context.VehicleType.FirstOrDefaultAsync(vt => vt.Type.Trim().ToUpper() == vehicleType.Type.ToUpper());

            if (existingType != null)
            {
                ModelState.AddModelError(nameof(vehicleType.Type), "A vehicle type with this name already exists");
            }

            if (ModelState.IsValid)
            {
                _context.Add(vehicleType);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create","Vehicles");
            }
            return View(vehicleType);
        }
    }
}
