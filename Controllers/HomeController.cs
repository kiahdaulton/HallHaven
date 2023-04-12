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

        public async Task<IActionResult> IndexAsync()
        {
            // current user Guid
            var userId = User.GetLoggedInUserId<string>();

            // if user is logged in
            if (userId != null)
            {
                // get logged in user
                var user = await _userManager.GetUserAsync(User);
                //var customUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                // get logged in user's gender
                var gender = user.Gender;

                // get list of hall haven context users
                //var students = _context.Users.ToList();


                // display users by gender to home view
                var usersByGender = _context.Users
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender).Where(g => g.Gender.Gender1 == gender).ToList();

                var existingModelData = usersByGender; // Retrieve the data for the existing user model


                // populate formViewModel
                var dorms = _context.Dorms.ToList();
                var creditHours = _context.CreditHours.ToList();
                var majors = _context.Majors.ToList();
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
                    Users = existingModelData,
                    FormViewModel = formViewModelData
                };

                return View(homeViewModel);

            }
            return View();
        }

            //    var userId = User.GetLoggedInUserId<string>();

            //    // if user is logged in
            //    if (userId != null)
            //    {
            //        // get logged in user
            //        var user = await _userManager.GetUserAsync(User);
            //        //var customUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //        // get logged in user's gender
            //        var gender = user.Gender;

            //        // Retrieve the selected values from the form
            //        int selectedDormId = formViewModelData.SelectedDormId;
            //        int selectedCreditHourId = formViewModelData.SelectedCreditHourId;
            //        int selectedMajorId = formViewModelData.SelectedMajorId;

            //        // Get the list of users filtered by the selected values
            //        var usersByGender = _context.Users
            //            .Include(f => f.Forms)
            //            .Include(f => f.MatchUser1s)
            //            .Include(f => f.MatchUser2s)
            //            .Include(u => u.Gender)
            //            .Where(g => g.Gender.Gender1 == gender);

            //        if (selectedDormId != 0)
            //        {
            //            usersByGender = usersByGender.Where(u => u.Forms.First().DormId == selectedDormId);
            //        }

            //        if (selectedCreditHourId != 0)
            //        {
            //            usersByGender = usersByGender.Where(u => u.Forms.First().CreditHourId == selectedCreditHourId);
            //        }

            //        if (selectedMajorId != 0)
            //        {
            //            usersByGender = usersByGender.Where(u => u.Forms.First().MajorId == selectedMajorId);
            //        }

            //        var usersByGenderList = usersByGender.ToList();

            //        var homeViewModel = new HomeViewModel
            //        {
            //            Users = usersByGenderList,
            //            FormViewModel = formViewModelData
            //        };

            //        return View(homeViewModel);
            //    }
            //}

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