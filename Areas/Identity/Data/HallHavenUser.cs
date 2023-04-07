using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
    [NotMapped]
    public IFormFile ProfilePictureFile { get; set; }

    [PersonalData]
    public byte[] ProfilePicture { get; set; }

    [PersonalData]
    public string ProfileBio { get; set; }

    [PersonalData]
    public string DisplayName { get; set; }

    public int? CustomUserId { get; set; } = null;
}

