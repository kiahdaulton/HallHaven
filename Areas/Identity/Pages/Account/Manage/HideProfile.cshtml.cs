// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace HallHaven.Areas.Identity.Pages.Account.Manage
{
    public class HideProfileModel : PageModel
    {
        private readonly UserManager<HallHavenUser> _userManager;
        private readonly SignInManager<HallHavenUser> _signInManager;
        private readonly HallHavenContext _context;

        public HideProfileModel(
            UserManager<HallHavenUser> userManager,
            SignInManager<HallHavenUser> signInManager,
            HallHavenContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>    
            public bool HideProfile { get; set; }
        }

        public void OnGet()
        {
        }

        //    public async Task<IActionResult> OnPostHideProfileAsync()
        //    {
        //        bool hideProfile = bool.Parse(Request.Form["hideProfile"]);
        //        bool hideProfile2 = Input.HideProfile;

        //        if (!ModelState.IsValid)
        //        {
        //            return Page();
        //        }
        //        var user = await _userManager.GetUserAsync(User);
        //        var customId = user.CustomUserId;


        //        // get the current user from the database
        //        List<User> currentUser = _context.Users.Where(c => c.UserId == customId)
        //                .Include(f => f.Forms.Where(f => f.UserId == customId))
        //                .Include(f => f.MatchUser1s)
        //                .Include(f => f.MatchUser2s)
        //                .Include(u => u.Gender).ToList();

        //        //set isHidden to the selected user value
        //        if (currentUser.Count != 0)
        //        {
        //            currentUser.First().IsHidden = hideProfile;
        //            await _context.SaveChangesAsync();

        //            return Page();

        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Profile saved");
        //            return Page();
        //        }
        //    }    

        //}

        //[HttpPost]
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    // get the current user from the database
        //    var user = await _userManager.GetUserAsync(User);
        //    var customId = user.CustomUserId;
        //    var currentUser = await _context.Users
        //        .Include(u => u.Gender)
        //        .Include(f => f.Forms)
        //        .Include(f => f.MatchUser1s)
        //        .Include(f => f.MatchUser2s)
        //        .FirstOrDefaultAsync(c => c.UserId == customId);

        //    // set isHidden to the selected user value
        //    if (currentUser != null)
        //    {
        //        currentUser.IsHidden = Input.HideProfile;
        //        await _context.SaveChangesAsync();

        //        return RedirectToPage();
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Error hiding profile.");
        //        return Page();
        //    }
        //}

    }
}
