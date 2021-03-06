using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public class Payments
    {
        public int PaymentId { get; set; }
        public string PaymentFor { get; set; }
        public int? Amount { get; set; }
        public int? ResidentId { get; set; }
        public int? EmployeeId { get; set; }
        public string PaymentStatus { get; set; }
        public int? ServiceId { get; set; }
    }
}
