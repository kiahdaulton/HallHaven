using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;

namespace HallHaven.Areas.Identity.Data;

// Add profile data for application users by adding properties to the HallHavenUser class
public class HallHavenUser : IdentityUser
{
    [PersonalData]
    public string FirstName { get; set; }

    [PersonalData]
    public string LastName { get; set; }

    [PersonalData]
    public string Gender { get; set; }

    [PersonalData]
    [Display(Name = "Profile Picture")]
    public byte[]? ProfilePicture { get; set; }

    [PersonalData]
    public string ProfileBio { get; set; }

    [PersonalData]
    public string DisplayName { get; set; }

    public int? CustomUserId { get; set; } = null;
}

