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
    public class UsersController : Controller
    {
        private readonly GarageVersion3Context _context;

        public UsersController(GarageVersion3Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.User
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PersonalIdentifyNumber = u.PersonalIdentifyNumber,
                    NrOfVehicles = u.Vehicles.Count()
                }).ToListAsync();

            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Vehicles)
                .ThenInclude(v => v.VehicleType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonalIdentifyNumber = user.PersonalIdentifyNumber,
                NrOfVehicles = user.Vehicles?.Count() ?? 0,
                Vehicles = user.Vehicles
            };

            return View(viewModel);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                User createUser = new User
                {
                    PersonalIdentifyNumber = user.PersonalIdentifyNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                _context.Add(createUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonalIdentifyNumber = user.PersonalIdentifyNumber,
            };

            return View(viewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.User.FindAsync(id);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.FirstName = viewModel.FirstName;
                    user.LastName = viewModel.LastName; 
                    user.PersonalIdentifyNumber = viewModel.PersonalIdentifyNumber;
                    
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
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
            return View(viewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PersonalIdentifyNumber = user.PersonalIdentifyNumber
            };

            return View(viewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            var users = await _context.User
              .Select(u => new UserViewModel
              {
                  Id = u.Id,
                  FirstName = u.FirstName,
                  LastName = u.LastName,
                  PersonalIdentifyNumber = u.PersonalIdentifyNumber,
                  NrOfVehicles = u.Vehicles.Count()
              }).ToListAsync();

            if (users.Count() == 0)
            {
                TempData["SortOnEmptyList"] = "The user list is empty";
                return RedirectToAction(nameof(Index));
            }

            switch (sortOrder)
            {
                case "FirstName":
                    users = users
                        .OrderBy(u => u.FirstName.Substring(0, 2))
                        .ThenBy(u => u.FirstName)
                        .ToList();
                    TempData["Sort"] = "User first name sort was done";
                    break;

                case "LastName":
                    users = users
                        .OrderBy(u => u.LastName.Substring(0, 2))
                        .ThenBy(u => u.LastName)
                        .ToList();
                    TempData["Sort"] = "User last name sort was done";
                    break;

                case "PersonalIdentifyNumber":
                    users = users
                        .OrderBy(u => u.PersonalIdentifyNumber.Substring(0, 5))
                        .ThenBy(u => u.PersonalIdentifyNumber)
                        .ToList();
                    TempData["Sort"] = "Personal identify number with oldest sort was done";
                    break;

                case "NumberOfVehicles":
                    users = users.OrderBy(u => u.NrOfVehicles).ToList();
                    TempData["Sort"] = "Number of vehicles sort was done from lowest to highest ";
                    break;


                default:
                    users = users.OrderBy(v => v.Id).ToList();
                    break;
            }

            return View("Index", users);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string firstName, string lastName, string personalIdentifyNumber)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(personalIdentifyNumber))
            {
                TempData["SearchFail"] = "Please provide input for at least one search criteria";
                var empyList = new List<UserViewModel>();
                return View("Index", empyList);
            }

            var query = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(v => v.FirstName.ToUpper().Equals(firstName.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(v => v.LastName.ToUpper().Equals(lastName.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(personalIdentifyNumber))
            {
                query = query.Where(v => v.PersonalIdentifyNumber.Equals(personalIdentifyNumber.Trim()));
            }

            var search = await query
                        .Include(u => u.Vehicles)
                        .Select(u => new UserViewModel
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            PersonalIdentifyNumber = u.PersonalIdentifyNumber,
                            NrOfVehicles = u.Vehicles.Count()
                        }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "No User found";
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
            var query = _context.User.AsQueryable();
            var search = await query
                      .Select(u => new UserViewModel
                      {
                          Id = u.Id,
                          FirstName = u.FirstName,
                          LastName = u.LastName,
                          PersonalIdentifyNumber = u.PersonalIdentifyNumber,
                          NrOfVehicles = u.Vehicles.Count()
                          
                      }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "There are no users in the system";
            }

            else
            {
                TempData["SearchSuccess"] = "Showing all users was successful";
            }

            return View("Index", search);
        }
    }
}
