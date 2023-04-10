using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                var students = _context.Users.ToList();

                // display users by gender to home view
                var usersByGender = _context.Users
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender).Where(g => g.Gender.Gender1 == gender).ToList();

                return View(usersByGender);
            }
            else
            {
                // display default view if user is not currently logged in
                return View();
            }
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