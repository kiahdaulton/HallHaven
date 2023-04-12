using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Claims;
using static HallHaven.Models.ClaimsPrincipalExtensions;

namespace HallHaven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HallHavenContext _context;
        private readonly UserManager<HallHavenUser> _userManager;

        public HomeController(ILogger<HomeController> logger, HallHavenContext context, UserManager<HallHavenUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync(FormViewModel formViewModel)
        {
            var userId = User.GetLoggedInUserId<string>();

            if (userId != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var gender = user.Gender;

                var usersByGender = _context.Users
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender)
                    .Where(g => g.Gender.Gender1 == gender);

                // populate formViewModel
                var dorms = await _context.Dorms.Where(g => g.Gender.Gender1 == gender).ToListAsync();
                var creditHours = await _context.CreditHours.ToListAsync();
                var majors = await _context.Majors.ToListAsync();

                formViewModel.Dorms = new SelectList(dorms, "DormId", "DormName");
                formViewModel.CreditHours = new SelectList(creditHours, "CreditHourId", "CreditHourName");
                formViewModel.Majors = new SelectList(majors, "MajorId", "MajorName");


                // apply filters
                if (formViewModel.SelectedDormId != 0)
                {
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.DormId == formViewModel.SelectedDormId));
                }

                if (formViewModel.SelectedCreditHourId != 0)
                {
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.CreditHourId == formViewModel.SelectedCreditHourId));
                }

                if (formViewModel.SelectedMajorId != 0)
                {
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.MajorId == formViewModel.SelectedMajorId));
                }


                var homeViewModel = new HomeViewModel
                {
                    Users = usersByGender.ToList(),
                    FormViewModel = formViewModel
                };

                return View(homeViewModel);
            }

            return View();
        }


        public async Task<IActionResult> IndexAsync()
        {
            // current user Guid
            var userId = User.GetLoggedInUserId<string>();

            // if user is logged in
            if (userId != null)
            {
                // get logged in user
                var user = await _userManager.GetUserAsync(User);

                // get logged in user's gender
                var gender = user.Gender;

                // get list of hall haven context users
                var users = _context.Users
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender).Where(g => g.Gender.Gender1 == gender);

                var userModelData = await users.ToListAsync(); // Retrieve the data for the user model

                // populate formViewModel
                var dorms = await _context.Dorms.Where(g => g.Gender.Gender1 == gender).ToListAsync();
                var creditHours = await _context.CreditHours.ToListAsync();
                var majors = await _context.Majors.ToListAsync();

                var dormsList = new SelectList(dorms, "DormId", "DormName");
                var creditHoursList = new SelectList(creditHours, "CreditHourId", "CreditHourName");
                var majorsList = new SelectList(majors, "MajorId", "MajorName");


                // add in selected values to search by
                var viewModel = new FormViewModel
                {
                    Dorms = dormsList,
                    CreditHours = creditHoursList,
                    Majors = majorsList
                };

                var formViewModelData = viewModel; // Retrieve the data for the form view model

                var homeViewModel = new HomeViewModel
                {
                    Users = userModelData,
                    FormViewModel = formViewModelData
                };

                int testSelectedDormId = homeViewModel.FormViewModel.SelectedDormId;

                return View(homeViewModel);
            }

            return View();
        }

        public IActionResult UsersList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}