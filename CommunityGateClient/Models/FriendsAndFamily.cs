using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public partial class FriendsAndFamily
    {
        public int FaFid { get; set; }
        public string FaFname { get; set; }
        public int? ResidentId { get; set; }
        public string Relation { get; set; }

        public virtual Residents Resident { get; set; }
    }
}
