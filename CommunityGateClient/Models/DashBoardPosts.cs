using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public class DashBoardPosts
    {
        public int DashItemId { get; set; }
        public string DashTitle { get; set; }
        public string DashType { get; set; }
        public string DashBody { get; set; }
        public string DashIntendedFor { get; set; }
        public int? ResidentId { get; set; }
        public DateTime? DashTime { get; set; }

        public virtual Residents Resident { get; set; }

    }
}
