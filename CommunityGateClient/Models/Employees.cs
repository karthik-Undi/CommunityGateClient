using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public class Employees
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDept { get; set; }
        public string EmployeeMobile { get; set; }
        public string EmployeeEmail { get; set; }
        public string EmployeePassword { get; set; }
        public string IsApproved { get; set; }
        public int? EmployeeWallet { get; set; }
        public int? EmployeeRating { get; set; }
    }
}
