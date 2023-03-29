using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.NetworkInformation;

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
            //var currentUserId = _context.AspNetUsers.ToList();
            //var students = _context.Users.ToList();
            //var userId = _userManager.GetUserIdAsync(user);
            var users = _context.Users.ToList();
            return View(users);
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
}