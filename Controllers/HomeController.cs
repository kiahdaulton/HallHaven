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

                // only show users that match the logged in user's gender in the view
                //var users = _userManager.Users.Where(g => g.Gender == gender).ToList();

                // get list of hall haven context users
                var students = _context.Users.ToList();

                // display users by gender to home view
                var usersByGender = _context.Users.Include(u => u.Gender).Where(g => g.Gender.Gender1 == gender).ToList();

                return View(usersByGender);
            }
            else
            {
                // display default view if user is not currently logged in
                return View();
            }     

            // this gets and displays username/email
            //return Content(this.User.GetLoggedInUserName());
        }

        public IActionResult UsersList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        //public IActionResult GetUsers()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
        //    var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
        //    var userEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
        //    var userGender = User.FindFirstValue(ClaimTypes.Gender); // get user gender

        //    // use extension class 
        //    var userId2 = User.GetLoggedInUserId<string>(); // Specify the type of your UserId;
        //    var userName2 = User.GetLoggedInUserName();
        //    var userEmail2 = User.GetLoggedInUserEmail();
        //    return View(userId);
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    //public class OtherClass
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    public OtherClass(IHttpContextAccessor httpContextAccessor)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //    }

    //    public void GetUsers()
    //    {
    //        //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId<string>(); // Specify the type of your UserId;
    //    }
    //}
}