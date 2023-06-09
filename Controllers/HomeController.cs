﻿using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using HallHaven.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static HallHaven.Models.ClaimsPrincipalExtensions;

namespace HallHaven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HallHavenContext _context;
        private readonly UserManager<HallHavenUser> _userManager;
        private readonly SendStudentEmail _sendStudentEmail;

        public HomeController(ILogger<HomeController> logger, HallHavenContext context, UserManager<HallHavenUser> userManager, SendStudentEmail sendStudentEmail)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _sendStudentEmail = sendStudentEmail;
        }

        // after submit of filter button
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync(FormViewModel formViewModel)
        {
            var userId = User.GetLoggedInUserId<string>();

            if (userId != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var customId = user.CustomUserId;
                var gender = user.Gender;

                List<User> usersByGender = _context.Users.Where(u => u.IsHidden == false)
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender)
                    .Where(g => g.Gender.Gender1 == gender)
                    // sort by highest overall sim percentage
                    // get logged in user's matches
                    .OrderByDescending(f => f.MatchUser2s.Where(u => u.User1Id == customId).Max(mu1 => mu1.SimilarityPercentage))
                    .ToList();

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
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.DormId == formViewModel.SelectedDormId)).ToList();
                }

                if (formViewModel.SelectedCreditHourId != 0)
                {
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.CreditHourId == formViewModel.SelectedCreditHourId)).ToList();
                }

                if (formViewModel.SelectedMajorId != 0)
                {
                    usersByGender = usersByGender.Where(u => u.Forms.Any(f => f.MajorId == formViewModel.SelectedMajorId)).ToList();
                }

                // get hallhavencontext user by id
                List<User> currentUser = _context.Users.Where(c => c.UserId == customId)
                    .Include(f => f.Forms.Where(f => f.UserId == customId))
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender)
                    .ToList();

                // get any user where they are an incoming student and they are an athlete
                var usersByGenderAndCandiate = usersByGender.Where(u => u.Forms.Any(f => f.IsCandiateStudent == true) && u.Forms.Any(f => f.IsStudentAthlete == false)).ToList();
                // get any user where they are an incoming student only
                var usersByGenderCandiateAthlete = usersByGender.Where(u => u.Forms.Any(f => f.IsCandiateStudent == true) && u.Forms.Any(f => f.IsStudentAthlete == true)).ToList();

                if (currentUser != null)
                {
                    // if current user's form is not empty
                    if (currentUser.First().Forms.Count() != 0)
                    {
                        // apply IsCandiateStudent and IsStudentAthlete filters from the form
                        // if current user is a candiate student
                        if (currentUser.First().Forms.First().IsCandiateStudent == true)
                        {
                            // get student atheletes
                            if (currentUser.First().Forms.First().IsStudentAthlete == true)
                            {
                                // new incoming student athlete
                                // if the student is a candiate student and a student athelete
                                // show students who are candiate students and are atheletes
                                usersByGender = usersByGenderCandiateAthlete;

                            }
                            else
                            {
                                // only show users that are candiate students AND not student atheletes
                                usersByGender = usersByGenderAndCandiate;
                            }
                        }
                    }
                    else
                    {
                        // current user's form is empty, so set the user model data to an empty list
                        // user should fill out a form first and not see any users
                        usersByGender = new List<User>();
                    }
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

        // load users
        public async Task<IActionResult> IndexAsync()
        {
            // current user Guid
            var userId = User.GetLoggedInUserId<string>();

            // if user is logged in
            if (userId != null)
            {
                // get logged in user
                var user = await _userManager.GetUserAsync(User);
                var customId = user.CustomUserId;

                // get logged in user's gender
                var gender = user.Gender;

                // get list of hall haven context users
                var users = _context.Users.Where(u => u.IsHidden == false)
                    .Include(f => f.Forms)
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender).Where(g => g.Gender.Gender1 == gender)
                    // sort by highest overall sim percentage
                    // get user1Id as the logged in user
                    .OrderByDescending(f => f.MatchUser2s.Where(u => u.User1Id == customId).Max(mu1 => mu1.SimilarityPercentage));

                if (users != null)
                {
                    var userModelData = await users.ToListAsync(); // Retrieve the data for the user model

                    // get logged in user by id
                    List<User> currentUser = _context.Users.Where(c => c.UserId == customId)
                        .Include(f => f.Forms.Where(f => f.UserId == customId))
                        .Include(f => f.MatchUser1s)
                        .Include(f => f.MatchUser2s)
                        .Include(u => u.Gender).ToList();

                    // get any user where IsCandiateStudent is true
                    var usersByGenderAndCandiate = userModelData.Where(u => u.Forms.Any(f => f.IsCandiateStudent == true) && u.Forms.Any(f => f.IsStudentAthlete == false)).ToList();

                    var usersByGenderCandiateAthlete = userModelData.Where(u => u.Forms.Any(f => f.IsCandiateStudent == true) && u.Forms.Any(f => f.IsStudentAthlete == true)).ToList();

                    if (currentUser != null)
                    {
                        // if current user's form is not empty
                        if (currentUser.First().Forms.Count() != 0)
                        {
                            // apply IsCandiateStudent and IsStudentAthlete filters from the form
                            // if current user is a candiate student
                            if (currentUser.First().Forms.First().IsCandiateStudent == true)
                            {
                                // get student atheletes
                                if (currentUser.First().Forms.First().IsStudentAthlete == true)
                                {
                                    // new incoming student athlete
                                    // if the student is a candiate student and a student athelete
                                    // show students who are candiate students and are atheletes
                                    userModelData = usersByGenderCandiateAthlete;

                                }
                                else
                                {
                                    // only show users that are candiate students AND not student atheletes
                                    userModelData = usersByGenderAndCandiate;
                                }
                            }
                        }
                        else
                        {
                            // current user's form is empty, so set the user model data to an empty list
                            // user should fill out a form first and not see any users
                            userModelData = new List<User>();
                        }
                    }

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

                    //int testSelectedDormId = homeViewModel.FormViewModel.SelectedDormId;

                    return View(homeViewModel);
                }
            }

            return View();
        }

        //hide the user profile in the account if true is selected
        [HttpPost]
        public IActionResult HideProfile(bool hideProfile)
        {

            string customId = Request.Form["customId"];
            int customIdInt = int.Parse(customId);

            try
            {
                // get currentUser
                List<User> currentUser = _context.Users.Where(c => c.UserId == customIdInt)
                    .Include(f => f.Forms.Where(f => f.UserId == customIdInt))
                    .Include(f => f.MatchUser1s)
                    .Include(f => f.MatchUser2s)
                    .Include(u => u.Gender).ToList();

                //set isHidden to the selected user value
                if (currentUser.Count != 0)
                {
                    //var test = hideProfile;
                    currentUser.First().IsHidden = hideProfile;
                    _context.SaveChanges();

                    return Redirect("/Identity/Account/Manage/HideProfile");

                }
                else
                {
                    return Redirect("/Identity/Account/Manage/HideProfile");
                }
            }
            catch (Exception ex)
            {
                // log the exception
                Console.WriteLine($"Error: {ex.Message}");

                // return a 500 error response
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendEmail(string recipient, string subject, string message)
        {
            if(string.IsNullOrEmpty(recipient))
            {
                //ModelState.AddModelError("UserId", "You have already submitted a form. Please edit your existing form instead.");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // pass in the user's data from the card to the email here
                await _sendStudentEmail.SendEmailAsync(recipient, subject, message);
                // close modal instead of redirect?
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult UsersList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public Task<IActionResult> Error()
        {
            return Task.FromResult<IActionResult>(View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }));
        }
    }
}