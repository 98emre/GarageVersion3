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

namespace GarageVersion3.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly GarageVersion3Context _context;

        public ReceiptsController(GarageVersion3Context context)
        {
            _context = context;
        }

        // GET: Receipts
        public async Task<IActionResult> Index()
        {
            var garageVersion3Context = _context.Receipt.Include(r => r.User);
            // Omvandla Receipt till ReceiptVM

            var viewModel = await _context.Receipt
                .Include(r => r.User)
                .Select(r => new ReceiptViewModel
                {
                    Id = r.Id,
                    User = r.User,
                    Checkin = r.CheckIn,
                    CheckoutDate = r.CheckOut,
                    Price = r.Price,
                    ParkingNumber = r.ParkingNumber
                }).ToListAsync();

            return View(viewModel);
        }

        // GET: Receipts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var receipt = _context.Receipt.Include(u => u.User).FirstOrDefault(v => v.Id == id);
            return View(receipt);
        }

        // GET: Receipts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipt
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        // POST: Receipts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt != null)
            {
                _context.Receipt.Remove(receipt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipt.Any(e => e.Id == id);
        }
    }
}
