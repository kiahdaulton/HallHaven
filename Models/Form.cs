using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HallHaven.Models
{
    public partial class Form
    {
        public int FormId { get; set; }

        [Display(Name = "Current Dorm")]
        public int DormId { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; } = null!;

        [Display(Name = "Major Id")]
        public int MajorId { get; set; }

        [Display(Name = "Credit Hours")]
        public int CreditHourId { get; set; }

        [Display(Name = "Gender")]
        public int GenderId { get; set; }

        [Required(ErrorMessage = "Please select if you are a transfer student or an incoming freshman")]
        public bool IsCandiateStudent { get; set; }

        [Required(ErrorMessage = "Please select if you are currently a student athlete")]
        public bool IsStudentAthlete { get; set; }

        [Required(ErrorMessage = "Please select a level of neatness for the prospective roommate")]
        public int NeatnessLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of potential visitors for the prospective roommate")]
        public int VisitorLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of fitness for the prospective roommate")]
        public int FitnessLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of academic focus for the prospective roommate")]
        public int AcademicLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of sociability for the prospective roommate")]
        public int SocialLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of sharing belonging for the prospective roommate")]
        public int SharingLevel { get; set; }

        [Required(ErrorMessage = "Please select a level of background noise preferred for the prospective roommate")]
        public int BackgroundNoiseLevel { get; set; }

        [Required(ErrorMessage = "Please select a preferred time to sleep for the prospective roommate")]
        public int BedTimeRanking { get; set; }

        [Required(ErrorMessage = "Please select a level of modesty for the prospective roommate")]
        public int ModestyLevel { get; set; }

        [Required(ErrorMessage = "Please select an estimated number of belongings for the prospective roommate")]
        public int NumberOfBelongings { get; set; }

        public virtual CreditHour CreditHour { get; set; } = null!;
        public virtual Dorm Dorm { get; set; } = null!;
        public virtual Major Major { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
