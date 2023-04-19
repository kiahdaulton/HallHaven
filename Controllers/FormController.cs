using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HallHaven.Data;
using HallHaven.Models;
using HallHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Scripting;
using static System.Collections.Specialized.BitVector32;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.SignalR;

namespace HallHaven.Controllers
{
    public class FormController : Controller
    {
        private readonly HallHavenContext _context;
        private readonly UserManager<HallHavenUser> _userManager;

        public FormController(HallHavenContext context, UserManager<HallHavenUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Form
        //public async Task<IActionResult> Index()
        //{
        //    var hallHavenContext = _context.Forms.Include(f => f.CreditHour).Include(f => f.Dorm).Include(f => f.Major).Include(f => f.User);
        //    return View(await hallHavenContext.ToListAsync());
        //}

        // GET: Forms
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.GetLoggedInUserId<string>();
            //GetHomeDropdowns();

            // if user is logged in
            if (userId != null)
            {
                // get logged in user
                var user = await _userManager.GetUserAsync(User);
                //var customUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                // get logged in user's id
                var customId = user.CustomUserId;

                // get hallhavencontext user by id
                var currentUser = _context.Users.Where(c => c.UserId == customId).ToList();

                // bind form values
                // ORIGINAL
                var hallHavenContext = _context.Forms
                    .Include(f => f.CreditHour)
                    .Include(f => f.Dorm)
                    .Include(f => f.Major)
                    .Include(f => f.User);
                return View(await hallHavenContext.ToListAsync());
            }
            else
            {
                // display default view if user is not currently logged in
                return View();
            }
        }

        // GET: Form/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Forms == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .Include(f => f.CreditHour)
                .Include(f => f.Dorm)
                .Include(f => f.Major)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FormId == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // GET: Form/Create
        //public IActionResult Create()
        public async Task<IActionResult> CreateAsync()
        {

            // get userId and populate form data
            var identityUser = await _userManager.GetUserAsync(User);
            var customId = identityUser.CustomUserId;

            var currentUser = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);
            if (currentUser != null)
            {
                // get gender id of current user
                int customGender = currentUser.Gender.GenderId;
                // get credit hours
                var creditHours = _context.CreditHours.ToList();

                // if credithours from credithour model of user equals 3 or 4 (junior or senior)
                // then display dorms from dorm model where credit hour equals 3 (junior AND senior dorms)

                ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName");
                // display dorms by user's gender
                ViewData["DormId"] = new SelectList(_context.Dorms.Where(d => d.GenderId == customGender).OrderBy(d => d.DormId), "DormId", "DormName");
                ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName").OrderBy(x => x.Text);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            }

            return View();
        }

        //POST: Form/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormId,DormId,UserId,MajorId,CreditHourId,GenderId,IsCandiateStudent,IsStudentAthlete,NeatnessLevel,VisitorLevel,FitnessLevel,AcademicLevel,SocialLevel,SharingLevel,BackgroundNoiseLevel,BedTimeRanking,ModestyLevel,NumberOfBelongings")] Form form)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var customId = identityUser.CustomUserId;

            //get currentUser related to identityUser table
            var currentUser = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);
            int customGender = currentUser.Gender.GenderId;
            if (currentUser != null)
            {
                // set userId from AspNetUsers loose foreign key instead of asking on the form
                form.UserId = (int)customId;


                // Check if a row with the same userId already exists in the database
                var existingForm = _context.Forms.FirstOrDefault(f => f.UserId == form.UserId);

                // if form has already been created
                if (existingForm != null)
                {
                    // A row with the same userId already exists, return an error message
                    ModelState.AddModelError("UserId", "You have already submitted a form. Please edit your existing form instead.");
                    // redisplay form if something went wrong
                    ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName");
                    // display dorms by user's gender
                    ViewData["DormId"] = new SelectList(_context.Dorms.Where(d => d.GenderId == customGender).OrderBy(d => d.DormId), "DormId", "DormName");
                    ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName").OrderBy(x => x.Text);
                    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
                    return View(form);
                }

                // save form
                // No existing row with the same userId, proceed with saving the form submission
                _context.Add(form);
                await _context.SaveChangesAsync();

                // begin matching algorithm
                // create new instance of match
                var match = new Match();

                // set user1 in form to logged in user id
                match.User1Id = form.UserId;
                // set user 1 as logged in user
                match.User1 = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);

                // if logged in user exists
                if (match.User1 != null)
                {
                    // get logged in user's gender
                    var gender = match.User1.Gender.Gender1;

                    // get all users by gender
                    var usersByGender = _context.Users
                        .Include(f => f.Forms)
                        .Include(f => f.MatchUser1s)
                        .Include(f => f.MatchUser2s)
                        .Include(u => u.Gender)
                        .Where(g => g.Gender.Gender1 == gender).ToList();

                    // add a constraint where only one distinct userId1 and userId2 can be entered
                    // for each user in the list
                    foreach (User userByGender in usersByGender)
                    {
                        // ONLY DO MATCHING SEQUENCE IF USERS HAVE FILLED OUT A FORM
                        // does the user have an existing form?
                        var userByGenderForm = userByGender.Forms.Where(f => f.UserId == userByGender.UserId).ToList();

                        // if user isn't current user
                        if (userByGender.UserId != customId)
                        {
                            // user has an existing form
                            if (userByGenderForm.Count != 0)
                            {
                                // set matchId primary key to 0 to reset existing form
                                match.MatchId = 0;

                                // iterate through user by gender list

                                // set user 2 id as user id in list
                                match.User2Id = userByGender.UserId;
                                // set user 2 to user in list
                                match.User2 = userByGender;

                                // if the user has not already been matched (given a compatibility percentage)
                                // write update method if the value is equal to true and the user wants to update their compatibility form

                                // compare form fields
                                int equalFields = 0;
                                // subtract 2 for User and Form Id
                                int totalFields = Request.Form.Keys.Count - 2;

                                    // for each field in form
                                    // add special case for user id which is not shown to the user
                                    // add special cases for isCandiateStudent and IsStudentAthlete
                                    foreach (var fieldName in Request.Form.Keys)
                                    {
                                        if (fieldName == "UserId")
                                        {
                                            //skip  
                                        }
                                        if (fieldName == "FormId")
                                        {
                                            //skip
                                        }
                                        if (fieldName == "IsCandiateStudent")
                                        {
                                            // only match with incoming students
                                            // if selected is true
                                            // then only match with other candiate students
                                        }
                                        if (fieldName == "IsStudentAthlete")
                                        {
                                        // only match incoming student athletes with incoming student athletes
                                        // IsCandiateStudent IsStudentAthlete must both be true
                                        }

                                    // find current field value from user form
                                    var currentValue = form.GetType().GetProperty(fieldName)?.GetValue(form);
                                        // get value of user2's form of the same field
                                        var matchValue = match.User2.Forms.First().GetType().GetProperty(fieldName)?.GetValue(match.User2.Forms.First());

                                        // if the user's form field isn't empty and user2's form field isn't empty
                                        // and the user's form field is the SAME as user2's
                                        if (currentValue != null && matchValue != null && currentValue.Equals(matchValue))
                                        {
                                            // add to the equal fields variable
                                            equalFields++;
                                        }                                  
                                    }
                                    // similarity percentage is equal to the number of equal fields among users divided by the number of total fields in the form
                                    // SimilarityPercentage = (number of equal fields / total number of fields) *100
                                    float similarityPercentage = (float)equalFields / totalFields * 100;
                                    match.SimilarityPercentage = similarityPercentage;

                                    // save match for each user by gender
                                    _context.Add(match);
                                    await _context.SaveChangesAsync();                   
                            }
                        }
                    }
                }

                return RedirectToAction("Index", "Home");
                //return RedirectToAction(nameof(Index));
            }

            // redisplay form if something went wrong
            ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName");
            // display dorms by user's gender
            ViewData["DormId"] = new SelectList(_context.Dorms.Where(d => d.GenderId == customGender).OrderBy(d => d.DormId), "DormId", "DormName");
            ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName").OrderBy(x => x.Text);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");

            return View(form);
        
        }

        [HttpGet]
        public IActionResult GetDormOptions(int creditHourId)
        {
            // if credit hours from credit hour model of user equals 3 or 4 (junior or senior)

            // Retrieve the dorm options from the database based on the selected credit hour value
            var dormOptions = _context.Dorms.Where(d => d.CreditHourId == creditHourId).Select(d => new SelectListItem
            {
                Value = d.DormId.ToString(),
                Text = d.DormName
            }).ToList();

            // Return the dorm options as a partial view
            return PartialView("_DormOptions", dormOptions);
        }

        // GET: Form/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Forms == null)
            {
                return NotFound();
            }
            // find form by form id
            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName", form.CreditHourId);
            ViewData["DormId"] = new SelectList(_context.Dorms, "DormId", "DormName", form.DormId);
            ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName", form.MajorId).OrderBy(x => x.Text);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", form.UserId);
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormId,DormId,UserId,MajorId,CreditHourId,GenderId,IsCandiateStudent,IsStudentAthlete,NeatnessLevel,VisitorLevel,FitnessLevel,AcademicLevel,SocialLevel,SharingLevel,BackgroundNoiseLevel,BedTimeRanking,ModestyLevel,NumberOfBelongings")] Form form)
        {

            var existingForm = _context.Forms.Find(form.FormId);
            if (existingForm == null)
            {
                return NotFound();
            }

            // update existing form with new data
            existingForm.DormId = form.DormId;
            existingForm.MajorId = form.MajorId;
            existingForm.CreditHourId = form.CreditHourId;
            existingForm.AcademicLevel = form.AcademicLevel;
            existingForm.SocialLevel = form.SocialLevel;
            existingForm.NeatnessLevel = form.NeatnessLevel;
            existingForm.FitnessLevel = form.FitnessLevel;
            existingForm.VisitorLevel = form.VisitorLevel;
            existingForm.BackgroundNoiseLevel = form.BackgroundNoiseLevel;
            existingForm.BedTimeRanking = form.BedTimeRanking;
            existingForm.IsCandiateStudent = form.IsCandiateStudent;
            existingForm.IsStudentAthlete = form.IsStudentAthlete;
            existingForm.NumberOfBelongings = form.NumberOfBelongings;
            existingForm.ModestyLevel = form.ModestyLevel;
            existingForm.SharingLevel = form.SharingLevel;

            await _context.SaveChangesAsync();


            // re-run algorithm with saved data

            // get user
            var identityUser = await _userManager.GetUserAsync(User);
            var customId = identityUser.CustomUserId;

            // get matches by logged in user
            var existingMatches = _context.Matches.Where(u => u.User1Id == customId).ToList();
            if (existingMatches == null)
            {
                return NotFound();
            }

            foreach (Match existingMatch in existingMatches)
            {

                // set user 1 as logged in user
                existingMatch.User1 = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);

                // if logged in user exists
                if (existingMatch.User1 != null)
                {
                    // get logged in user's gender
                    var gender = existingMatch.User1.Gender.Gender1;

                    // get all users by gender
                    var usersByGender = _context.Users
                        .Include(f => f.Forms)
                        .Include(f => f.MatchUser1s)
                        .Include(f => f.MatchUser2s)
                        .Include(u => u.Gender)
                        .Where(g => g.Gender.Gender1 == gender).ToList();

                    // add a constraint where only one distinct userId1 and userId2 can be entered
                    // for each user in the list
                    foreach (User userByGender in usersByGender)
                    {
                        // ONLY DO MATCHING SEQUENCE IF USERS HAVE FILLED OUT A FORM
                        // does the user have an existing form?
                        var userByGenderForm = userByGender.Forms.Where(f => f.UserId == userByGender.UserId).ToList();

                        // if user isn't current user
                        if (userByGender.UserId != customId)
                        {
                            // user has an existing form
                            if (userByGenderForm.Count != 0)
                            {
                                // compare updated form fields
                                int equalFields = 0;
                                // subtract 2 for User and Form Id
                                int totalFields = Request.Form.Keys.Count - 2;

                                // for each field in form
                                // add special case for user id which is not shown to the user
                                // add special cases for isCandiateStudent and IsStudentAthlete
                                foreach (var fieldName in Request.Form.Keys)
                                {
                                    if (fieldName == "UserId")
                                    {
                                        //skip  
                                    }
                                    if (fieldName == "FormId")
                                    {
                                        //skip
                                    }
                                    if (fieldName == "IsCandiateStudent")
                                    {
                                        // only match with incoming students
                                        // if selected is true
                                        // then only match with other candiate students
                                    }
                                    if (fieldName == "IsStudentAthlete")
                                    {
                                        // only match incoming student athletes with incoming student athletes
                                        // IsCandiateStudent IsStudentAthlete must both be true
                                    }
                                    // valid fieldName, run matching
                                    else
                                    {
                                        // find current field value from user form
                                        var currentValue = form.GetType().GetProperty(fieldName)?.GetValue(form);
                                        // get value of user2's form of the same field
                                        var matchValue = existingMatch.User2.Forms.First().GetType().GetProperty(fieldName)?.GetValue(existingMatch.User2.Forms.First());

                                        // if the user's form field isn't empty and user2's form field isn't empty
                                        // and the user's form field is the SAME as user2's
                                        if (currentValue != null && matchValue != null && currentValue.Equals(matchValue))
                                        {
                                            // add to the equal fields variable
                                            equalFields++;
                                        }
                                    }
                                }

                                // similarity percentage is equal to the number of equal fields among users divided by the number of total fields in the form
                                // SimilarityPercentage = (number of equal fields / total number of fields) *100
                                float similarityPercentage = (float)equalFields / totalFields * 100;
                                existingMatch.SimilarityPercentage = similarityPercentage;

                                // update the context with the new matches
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Form/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Forms == null)
            {
                return NotFound();
            }

            var form = await _context.Forms
                .Include(f => f.CreditHour)
                .Include(f => f.Dorm)
                .Include(f => f.Major)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FormId == id);
            if (form == null)
            {
                return NotFound();
            }

            return View(form);
        }

        // POST: Form/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Forms == null)
            {
                return Problem("Entity set 'HallHavenContext.Forms'  is null.");
            }
            var form = await _context.Forms.FindAsync(id);
            if (form != null)
            {
                _context.Forms.Remove(form);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormExists(int id)
        {
          return (_context.Forms?.Any(e => e.FormId == id)).GetValueOrDefault();
        }
    }
}