using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class Form
    {
        public int FormId { get; set; }
        public int DormId { get; set; }
        public string UserId { get; set; } = null!;
        public int MajorId { get; set; }
        public int CreditHourId { get; set; }
        public int GenderId { get; set; }
        public bool IsCandiateStudent { get; set; }
        public bool IsStudentAthlete { get; set; }
        public int NeatnessLevel { get; set; }
        public int VisitorLevel { get; set; }
        public int FitnessLevel { get; set; }
        public int AcademicLevel { get; set; }
        public int SocialLevel { get; set; }
        public int SharingLevel { get; set; }
        public int BackgroundNoiseLevel { get; set; }
        public int BedTimeRanking { get; set; }
        public int ModestyLevel { get; set; }
        public int NumberOfBelongings { get; set; }

        public virtual CreditHour CreditHour { get; set; } = null!;
        public virtual Dorm Dorm { get; set; } = null!;
        public virtual Major Major { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
