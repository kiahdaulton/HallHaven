using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

        public IActionResult Index()
        {

           // var userId = _userManager.GetUserId(HttpContext.User);

            var userId = User.GetLoggedInUserId<string>();
            //var userGender = _userManager.GetLoggedInUserGender(User);
            //var users = GetUsers();
            // get all users
            //var users = _context.Users.ToList();
            //var user = await _userManager.GetUserAsync(User);
            // get gender of logged in user
            // display roommates if logged in user gender equals gender of each user in the list
            //return View();

            var users = _userManager.Users.ToList();
            return View(users);

            // this gets and displays username/email
            //return Content(this.User.GetLoggedInUserName());
        }

        public IActionResult UsersList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult GetUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // will give the user's Email
            var userGender = User.FindFirstValue(ClaimTypes.Gender); // get user gender

            // use extension class 
            var userId2 = User.GetLoggedInUserId<string>(); // Specify the type of your UserId;
            var userName2 = User.GetLoggedInUserName();
            var userEmail2 = User.GetLoggedInUserEmail();
            return View(userId);
        }

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

    public class OtherClass
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OtherClass(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void GetUsers()
        {
            //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _httpContextAccessor.HttpContext.User.GetLoggedInUserId<string>(); // Specify the type of your UserId;
        }
    }
}