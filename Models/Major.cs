using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class Major
    {
        public Major()
        {
            Forms = new HashSet<Form>();
        }

        public int MajorId { get; set; }
        public string MajorName { get; set; } = null!;

        public virtual ICollection<Form> Forms { get; set; }
    }
}
