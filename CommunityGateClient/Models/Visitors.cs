using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CommunityGateClient.Models
{
    public class Visitors
    {
        public int VisitorID { get; set; }

        [Required(ErrorMessage ="Please Enter Visitor Name")]
        [DisplayName("Visitor Name")]
        public string VisitorName { get; set; }

        [Required(ErrorMessage = "Please Enter Reason For Visit")]
        [DisplayName("Reason For Visit")]
        public string VisitorResaon { get; set; }

        [Required(ErrorMessage = "Please Enter Visit Scheduled at")]
        [DisplayName("Visit Scheduled at")]
        public DateTime? VisitStartTime { get; set; }

        [Required(ErrorMessage = "Please Enter Visit End Time")]
        [DisplayName("Visit End Time")]
        public DateTime? VisitEndTime { get; set; }
        public int? ResidentId { get; set; }
    }
}
