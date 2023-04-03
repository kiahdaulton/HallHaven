using System;
using System.Collections.Generic;

namespace HallHaven.Models
{
    public partial class Match
    {
        public int MatchId { get; set; }
        public int? User1Id { get; set; }
        public string? ApplicationUser1Id { get; set; }
        public int? User2Id { get; set; }
        public string? ApplicationUser2Id { get; set; }
        public bool IsMatched { get; set; }
        public float SimilarityPercentage { get; set; }

        public virtual User? User1 { get; set; }
        public virtual User? User2 { get; set; }
    }
}
