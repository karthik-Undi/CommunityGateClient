using System;
using System.Collections.Generic;

namespace CommunityGateClient.Models
{
    public partial class Complaints
    {
        public int ComplaintId { get; set; }
        public int? ResidentId { get; set; }
        public string ComplaintRegarding { get; set; }
        public string ComplaintStatus { get; set; }

        public virtual Residents Resident { get; set; }
    }
}
