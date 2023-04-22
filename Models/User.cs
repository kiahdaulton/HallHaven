using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class User
    {
        public User()
        {
            Forms = new HashSet<Form>();
            MatchUser1s = new HashSet<Match>();
            MatchUser2s = new HashSet<Match>();
        }

        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? GenderId { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? ProfileBio { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsHidden { get; set; }

        public virtual Gender? Gender { get; set; }
        public virtual ICollection<Form> Forms { get; set; }
        public virtual ICollection<Match> MatchUser1s { get; set; }
        public virtual ICollection<Match> MatchUser2s { get; set; }
    }
}
