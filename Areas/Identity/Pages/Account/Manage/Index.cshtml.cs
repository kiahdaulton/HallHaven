﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HallHaven.Areas.Identity.Data;
using HallHaven.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HallHaven.Models;
using Microsoft.EntityFrameworkCore;

namespace HallHaven.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<HallHavenUser> _userManager;
        private readonly SignInManager<HallHavenUser> _signInManager;
        private readonly HallHavenContext _context;

        public IndexModel(
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
            [Required(ErrorMessage = "Please enter your profile biography")]
            [Display(Name = "Profile Biography")]
            public string ProfileBio { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }

            [Display(Name = "Hide Profile")]
            public bool HideProfile { get; set; }

        }

        private async Task LoadAsync(HallHavenUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var profilePicture = user.ProfilePicture;

            Username = userName;

            Input = new InputModel
            {
                ProfileBio = user.ProfileBio,
                ProfilePicture = profilePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // add user profile picture
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
            }

            // save new profile bio
            if (Input.ProfileBio != user.ProfileBio)
            {
                user.ProfileBio = Input.ProfileBio;
            }

            // update identity user
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            // update user table

            // get logged in user's id
            var customId = user.CustomUserId;

            // get hallhavencontext user by id
            User currentUser = _context.Users.SingleOrDefault(c => c.UserId == customId);

            if (currentUser != null)
            {
                // save new profile bio
                if (Input.ProfileBio != currentUser.ProfileBio)
                {
                    currentUser.ProfileBio = Input.ProfileBio;

                }

                // add user profile picture
                if (Request.Form.Files.Count > 0)
                {
                    IFormFile file = Request.Form.Files.FirstOrDefault();
                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);             
                        currentUser.ProfilePicture = dataStream.ToArray();
                        
                    }
                }
            }

            // save hallhaven context user
            _context.Update(currentUser);
            await _context.SaveChangesAsync();

            // this method refreshes the user on screen
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
