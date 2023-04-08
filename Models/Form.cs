﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HallHaven.Models
{
    public partial class Form
    {
        public int FormId { get; set; }

        [Display(Name = "Current Dorm")]
        [Required(ErrorMessage = "Please select your current dormitory")]
        public int? DormId { get; set; }

        // hide this in form view
        public int? UserId { get; set; }

        [Display(Name = "Current Major")]
        [Required(ErrorMessage = "Please enter your current major")]
        public int? MajorId { get; set; }

        // credit hours can be null or zero for incoming students
        // link number of credit hours to classificaton ex: freshman, sophomore, etc.
        [Display(Name = "Credit Hours")]
        [Required(ErrorMessage = "Please enter your current number of credit hours")]
        public int? CreditHourId { get; set; }

        public int? GenderId { get; set; }

        [Display(Name = "Are you currently an incoming student?")]
        [Required(ErrorMessage = "Please select if you are currently an incoming student")]
        public bool? IsCandiateStudent { get; set; }

        [Display(Name = "Are you currently a student athlete?")]
        [Required(ErrorMessage = "Please select if you are currently a student athlete")]
        public bool? IsStudentAthlete { get; set; }

        [Display(Name = "How neat would you like your room to be?")]
        [Required(ErrorMessage = "Please select how neat you would like your room to be")]
        public int? NeatnessLevel { get; set; }

        [Display(Name = "how often would you like to have visitors in your room?")]
        [Required(ErrorMessage = "Please select how often you would like to have visitors in your room")]
        public int? VisitorLevel { get; set; }

        [Display(Name = "How much do you value fitness and athletic activities?")]
        [Required(ErrorMessage = "Please select how much you value fitness and athletic activities")]
        public int? FitnessLevel { get; set; }

        [Display(Name = "How much do you value your academic studies?")]
        [Required(ErrorMessage = "Please select how much you value your academic studies")]
        public int? AcademicLevel { get; set; }

        [Display(Name = "How much do you value social activities?")]
        [Required(ErrorMessage = "Please select how much you value social activities")]
        public int? SocialLevel { get; set; }

        [Display(Name = "How much do you value sharing personal belongings?")]
        [Required(ErrorMessage = "Please select how much you value sharing personal belongings")]
        public int? SharingLevel { get; set; }

        [Display(Name = "What level of noise do you prefer in your room while studying?")]
        [Required(ErrorMessage = "Please select the level of noise you prefer in your room while studying")]
        public int? BackgroundNoiseLevel { get; set; }

        [Display(Name = "What is the average time you plan to sleep on a weekday?")]
        [Required(ErrorMessage = "Please select the average time you go to sleep")]
        public int? BedTimeRanking { get; set; }

        [Display(Name = "How modest you are in your dorm room?")]
        [Required(ErrorMessage = "Please select how modest you are in your dorm room")]
        public int? ModestyLevel { get; set; }

        [Display(Name = "What is the average number of belongings you own?")]
        [Required(ErrorMessage = "Please select the average number of belongings you own")]
        public int? NumberOfBelongings { get; set; }


        public virtual CreditHour CreditHour { get; set; } = null!;
        public virtual Dorm Dorm { get; set; } = null!;
        public virtual Major Major { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
