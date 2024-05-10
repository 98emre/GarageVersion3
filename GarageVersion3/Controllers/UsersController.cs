using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
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
        
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Vehicles)
                .ThenInclude(u => u.VehicleType)
                .FirstOrDefaultAsync(u => u.Id == id);

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var firstNameSize = viewModel.FirstName.Trim().Replace(" ", "").Count();
                var lastNameSize = viewModel.LastName.Trim().Replace(" ", "").Count();

                if (firstNameSize < 2 || lastNameSize < 2)
                {
                    ModelState.AddModelError((firstNameSize<2) ? "FirstName" : "LastName", "Must be atleast 2 characters");
                    return View();
                }

                User user = new User
                {
                    PersonalIdentifyNumber = viewModel.PersonalIdentifyNumber.Trim().Replace(" ", ""),
                    FirstName = viewModel.FirstName.Trim().Replace(" ", ""),
                    LastName = viewModel.LastName.Trim().Replace(" ","")
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                PersonalIdentifyNumber = user.PersonalIdentifyNumber.Trim(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var firstNameSize = viewModel.FirstName.Trim().Replace(" ", "").Count();
                    var lastNameSize = viewModel.LastName.Trim().Replace(" ", "").Count();

                    if (firstNameSize < 2 || lastNameSize < 2)
                    {
                        ModelState.AddModelError((firstNameSize < 2) ? "FirstName" : "LastName", "Must be atleast 2 characters");
                        return View(viewModel);
                    }

                    var user = await _context.User.FindAsync(id);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.Id = viewModel.Id;
                    user.FirstName = viewModel.FirstName.Trim().Replace(" ","");
                    user.LastName = viewModel.LastName.Trim().Replace(" ", ""); 
                    user.PersonalIdentifyNumber = viewModel.PersonalIdentifyNumber.Trim().Replace(" ", "");
                    
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

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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
                        .OrderBy(u => u.PersonalIdentifyNumber.Substring(0, 2)) 
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
            var query = _context.User.AsQueryable();

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && string.IsNullOrEmpty(personalIdentifyNumber))
            {
                TempData["SearchMessage"] = "Please provide input for at least one search criteria";
                TempData["SearchStatus"] = "alert alert-warning";
                return View("Index", new List<UserViewModel>());
            }
            
            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(v => v.FirstName.Replace(" ", "").Trim().ToUpper().Equals(firstName.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(v => v.LastName.Replace(" ", "").Trim().ToUpper().Equals(lastName.Replace(" ", "").ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(personalIdentifyNumber))
            {
                query = query.Where(v => v.PersonalIdentifyNumber.Replace(" ", "").Trim().Equals(personalIdentifyNumber.Replace(" ", "").Trim()));
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

            TempData["SearchMessage"] = (search.Count == 0) ? "No user could be found" : "Search was successful";
            TempData["SearchStatus"] = (search.Count == 0) ? "alert alert-warning" : "alert alert-success";

            return View("Index", search);
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            TempData["SearchMessage"] = (_context.User.ToList().Count() == 0) ? "There are no users in the system" : "Showing all users was successful";
            TempData["SearchStatus"] = (_context.User.ToList().Count() == 0) ? "alert alert-warning" : "alert alert-success";

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
