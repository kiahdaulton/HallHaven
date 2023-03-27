using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class Dorm
    {
        public Dorm()
        {
            Forms = new HashSet<Form>();
        }

        public int DormId { get; set; }
        public string DormName { get; set; } = null!;
        public int GenderId { get; set; }
        public int CreditHourId { get; set; }

        public virtual CreditHour CreditHour { get; set; } = null!;
        public virtual Gender Gender { get; set; } = null!;
        public virtual ICollection<Form> Forms { get; set; }
    }
}
