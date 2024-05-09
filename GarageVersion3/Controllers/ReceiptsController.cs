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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace GarageVersion3.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly GarageVersion3Context _context;

        public ReceiptsController(GarageVersion3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string selectedUserPersonalNr)
        {
            var userList = await _context.User.ToListAsync();
            TempData["Users"] = userList;
            if (selectedUserPersonalNr == null || selectedUserPersonalNr == "")
            {
                var viewModel = await _context.Receipt
                    .Include(r => r.User)
                    .Select(r => new ReceiptViewModel
                    {
                        Id = r.Id,
                        User = r.User,
                        CheckIn = r.CheckIn,
                        CheckOutDate = r.CheckOut,
                        Price = r.Price,
                        ParkingNumber = r.ParkingNumber
                    }).ToListAsync();

                return View(viewModel);
            }
            else
            {

                var user = _context.User.FirstOrDefault(u => u.PersonalIdentifyNumber == selectedUserPersonalNr);
                if (user == null)
                {
                    // Handle case where user is not found
                    return NotFound();
                }

                var userReceipts = await _context.Receipt.Where(u => u.UserId == user.Id).Select(u => new ReceiptViewModel
                {
                    Id = u.Id,
                    User = u.User,
                    CheckIn = u.CheckIn,
                    CheckOutDate = u.CheckOut,
                    Price = u.Price,
                    ParkingNumber = u.ParkingNumber
                }).ToListAsync();

                return View(userReceipts);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.Receipt
                .Include(r => r.User)
                .Select(r => new ReceiptViewModel
                {
                    Id = r.Id,
                    User = r.User,
                    CheckIn = r.CheckIn,
                    CheckOutDate = r.CheckOut,
                    Price = r.Price,
                    ParkingNumber = r.ParkingNumber
                })
                .FirstOrDefaultAsync(r => r.Id == id);

            if(viewModel == null)
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

            var receipt = await _context.Receipt
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

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

        [HttpGet]
        public async Task<IActionResult> Filter(string firstName, string lastName)
        {
            var query = _context.Receipt.AsQueryable();

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                TempData["SearchMessage"] = "Please provide input for at least one search criteria";
                TempData["SearchStatus"] = "alert alert-warning";
                var empyList = new List<ReceiptViewModel>();
                return View("Index", empyList);
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(u => u.User.FirstName.Replace(" ", "").Replace(" ", "").Trim().ToUpper().Equals(firstName.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(u => u.User.LastName.Replace(" ", "").Trim().ToUpper().Equals(lastName.Replace(" ", "").ToUpper().Trim()));
            }

            var searchResults = await query
                .Include(r => r.User)
                .Select(r => new ReceiptViewModel
                {
                    Id = r.Id,
                    User = r.User,
                    CheckIn = r.CheckIn,
                    CheckOutDate = r.CheckOut,
                    Price = r.Price,
                    ParkingNumber = r.ParkingNumber
                }).ToListAsync();

            TempData["SearchMessage"] = (searchResults.Count() == 0) ? "No receipts where found on the search user" : "Search was successful";
            TempData["SearchStatus"] = (searchResults.Count() == 0) ? "alert alert-warning" : "alert alert-success";

            return View("Index", searchResults);
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            TempData["SearchMessage"] = (_context.Receipt.Count() == 0) ? "There are no receipts" : "Showing all receipts was successful";
            TempData["SearchStatus"] = (_context.Receipt.Count() == 0) ? "alert alert-warning" : "alert alert-success";

            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipt.Any(e => e.Id == id);
        }
    }
}
