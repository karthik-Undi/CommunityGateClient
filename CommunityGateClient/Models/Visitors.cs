using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public class Visitors
    {
        public int VisitorId { get; set; }
        public string VisitorName { get; set; }
        public string VisitorResaon { get; set; }
        public DateTime? VisitStartTime { get; set; }
        public DateTime? VisitEndTime { get; set; }
        public int? ResidentId { get; set; }
        public string VisitorEntryStatus { get; set; }
        public int? EmployeeId { get; set; }
    }
}
