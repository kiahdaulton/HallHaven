using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class Gender
    {
        public Gender()
        {
            Dorms = new HashSet<Dorm>();
            Users = new HashSet<User>();
        }

        public int GenderId { get; set; }
        public string Gender1 { get; set; } = null!;

        public virtual ICollection<Dorm> Dorms { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
