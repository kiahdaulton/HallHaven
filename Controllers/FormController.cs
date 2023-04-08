﻿using System;
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

                // display dorms by gender to form view
                // where Gender GenderId equals User GenderId 
                // just include current user in the form

                //var hallHavenContext = _context.Forms.Include(f => f.CreditHour).Include(f => f.Dorm).Where(u => u.Dorm.GenderId == Form.GenderId)
                //    .Include(f => f.Major).Include(f => f.User).Where(c => c.UserId == customId);

                //return View(await hallHavenContext.ToListAsync());

                // bind form values
                // ORIGINAL
                var hallHavenContext = _context.Forms.Include(f => f.CreditHour).Include(f => f.Dorm).Include(f => f.Major).Include(f => f.User);
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
            var identityUser = await _userManager.GetUserAsync(User);
            var customId = identityUser.CustomUserId;

            var currentUser = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);
            if (currentUser != null)
            {
                // get gender id of current user
                int customGender = currentUser.Gender.GenderId;
                // get credit hours
                var creditHours = _context.CreditHours.ToList();

                //// Populate CreditHours dropdown
                //ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName");

                //// Populate Dorms dropdown initially with all dorms
                //var allDorms = await _context.Dorms.ToListAsync();
                //ViewData["DormId"] = new SelectList(allDorms, "DormId", "DormName");


                ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName");
                // display dorms by user's gender
                ViewData["DormId"] = new SelectList(_context.Dorms.Where(d => d.GenderId == customGender).OrderBy(d => d.DormId), "DormId", "DormName");
                ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName").OrderBy(x => x.Text);
                ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            }

            return View();
        }

        // POST: Form/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormId,DormId,UserId,MajorId,CreditHourId,GenderId,IsCandiateStudent,IsStudentAthlete,NeatnessLevel,VisitorLevel,FitnessLevel,AcademicLevel,SocialLevel,SharingLevel,BackgroundNoiseLevel,BedTimeRanking,ModestyLevel,NumberOfBelongings")] Form form)
        {
            if (ModelState.IsValid)
            {
                _context.Add(form);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName", form.CreditHourId);
            ViewData["DormId"] = new SelectList(_context.Dorms, "DormId", "DormName", form.DormId);
            ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName", form.MajorId).OrderBy(x => x.Text);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", form.UserId);

            return View(form);
        }

        // get dorms by credit hours
        public async Task<IActionResult> GetDormsByCreditHourAsync(int selectedCreditHourId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var customId = identityUser.CustomUserId;

            var currentUser = _context.Users.Include(u => u.Gender).FirstOrDefault(u => u.UserId == customId);
            if (currentUser != null)
            {
                // get gender id of current user
                int customGender = currentUser.Gender.GenderId;

                var dorms = _context.Dorms
                    .Where(d => d.GenderId == customGender && d.CreditHourId == selectedCreditHourId)
                    .OrderBy(d => d.DormId)
                    .ToList();

                var dormsSelectList = new SelectList(dorms, "DormId", "DormName");


                return Json(dormsSelectList);
            }

            return NotFound();
        }

        // GET: Form/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Forms == null)
            {
                return NotFound();
            }

            var form = await _context.Forms.FindAsync(id);
            if (form == null)
            {
                return NotFound();
            }
            ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName", form.CreditHourId);
            ViewData["DormId"] = new SelectList(_context.Dorms, "DormId", "DormName", form.DormId);
            ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName", form.MajorId).OrderBy(x => x.Text); ;
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", form.UserId);
            return View(form);
        }

        // POST: Form/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormId,DormId,UserId,MajorId,CreditHourId,GenderId,IsCandiateStudent,IsStudentAthlete,NeatnessLevel,VisitorLevel,FitnessLevel,AcademicLevel,SocialLevel,SharingLevel,BackgroundNoiseLevel,BedTimeRanking,ModestyLevel,NumberOfBelongings")] Form form)
        {
            if (id != form.FormId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(form);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormExists(form.FormId))
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
            ViewData["CreditHourId"] = new SelectList(_context.CreditHours, "CreditHourId", "CreditHourName", form.CreditHourId);
            ViewData["DormId"] = new SelectList(_context.Dorms, "DormId", "DormName", form.DormId);
            ViewData["MajorId"] = new SelectList(_context.Majors, "MajorId", "MajorName", form.MajorId).OrderBy(x => x.Text); ;
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", form.UserId);
            return View(form);
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