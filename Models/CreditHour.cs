using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class CreditHour
    {
        public CreditHour()
        {
            Dorms = new HashSet<Dorm>();
            Forms = new HashSet<Form>();
        }

        public int CreditHourId { get; set; }
        public string CreditHourName { get; set; } = null!;
        public string Classification { get; set; } = null!;

        public virtual ICollection<Dorm> Dorms { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
    }
}
