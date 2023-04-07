// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using HallHaven.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HallHaven.Data;
using HallHaven.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using HallHaven.Controllers;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallHaven.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<HallHavenUser> _signInManager;
        private readonly UserManager<HallHavenUser> _userManager;
        private readonly IUserStore<HallHavenUser> _userStore;
        private readonly IUserEmailStore<HallHavenUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly HallHavenContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<HallHavenUser> userManager,
            IUserStore<HallHavenUser> userStore,
            SignInManager<HallHavenUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            HallHavenContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(255, ErrorMessage = "The first name field should have a maximum of 255 characters.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(255, ErrorMessage = "The last name field should have a maximum of 255 characters.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }


            [Required(ErrorMessage = "Please enter your gender")]
            [BindProperty]
            public string Gender { get; set; }

            //[Required(ErrorMessage = "Please select your profile picture")]
            ////[DataType(DataType.Upload)]
            //[Display(Name = "Profile Picture")]
            //public byte[] ProfilePicture { get; set; }


            [Required(ErrorMessage = "Please select a profile picture.")]
            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePictureFile { get; set; }

            public byte[] ProfilePicture { get; set; }


            //[ValidateNever]
            //public string? ImageUrl { get; set; }

            [Required(ErrorMessage = "Please enter your profile biography")]
            [Display(Name = "Profile Biography")]
            public string ProfileBio { get; set; }

            // custom user id to map to identity user
            public int? CustomUserId { get; set; } = null;


            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile ProfilePicture, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                var customUser = new User();
                var customUserGender = new Gender();

                if (Input.Gender == "Male")
                {
                    customUser.GenderId = 1;
                }
                // female
                else
                {
                    customUser.GenderId = 2;
                }

                customUser.FirstName = Input.FirstName;
                customUser.LastName = Input.LastName;
                customUser.Email = Input.Email;
                customUser.ProfileBio = Input.ProfileBio;


                // user table
                if (Input.ProfilePictureFile != null && Input.ProfilePictureFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Input.ProfilePictureFile.CopyTo(ms);
                        customUser.ProfilePicture = ms.ToArray();
                    }
                }


                // generate new user in user table
                _context.Add(customUser);
                await _context.SaveChangesAsync();


                // create identity user
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Gender = Input.Gender;
                //user.ProfilePicture = Input.ProfilePicture;
                user.ProfileBio = Input.ProfileBio;
                // set null customUserId to value of userId in user table
                user.CustomUserId = customUser.UserId;

                // identity table
                if (Input.ProfilePictureFile != null && Input.ProfilePictureFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Input.ProfilePictureFile.CopyTo(ms);
                        user.ProfilePicture = ms.ToArray();
                    }
                }

                // set identity email
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // create identity user
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // add profile image
                    // handle profile image file upload
                    //if (Input.ProfilePictureFile != null && ProfilePicture.Length > 0)
                    //{
                    //    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", Input.ProfilePictureFile.FileName);
                    //    using (var stream = new FileStream(imagePath, FileMode.Create))
                    //    {
                    //        await Input.ProfilePictureFile.CopyToAsync(stream);
                    //    }
                    //}

                    // userId is created
                    var userId = await _userManager.GetUserIdAsync(user);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<byte[]> ConvertFileToByteArray(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                return stream.ToArray();
            }
        }


        private HallHavenUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<HallHavenUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(HallHavenUser)}'. " +
                    $"Ensure that '{nameof(HallHavenUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<HallHavenUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<HallHavenUser>)_userStore;
        }
    }
}
