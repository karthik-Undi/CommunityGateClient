using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class PaymentDetails
    {
        public int PaymentId { get; set; }
        public string PaymentFor { get; set; }
        public int? Amount { get; set; }
        public int? ResidentId { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PaymentStatus { get; set; }
        public int? ServiceId { get; set; }
        public string ServiceType { get; set; }
    }
}
