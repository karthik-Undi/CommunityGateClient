using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommunityGateClient.Models
{
    public class Employees
    {

        public int EmployeeId { get; set; }
        [Required]

        public string EmployeeName { get; set; }
        [Required]

        public string EmployeeDept { get; set; }
        [Required]

        public string EmployeeMobile { get; set; }
        [Required]

        public string EmployeeEmail { get; set; }
        [Required]

        public string EmployeePassword { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("EmployeePassword", ErrorMessage = "Passwords Donot Match")]
        public string ConfirmPassword { get; set; }


        public string IsApproved { get; set; }

        [RegularExpression("^[1-9]+[0-9]*$", ErrorMessage = "Please enter a value greater than zero")]
        public int? EmployeeWallet { get; set; }

        public int? EmployeeRating { get; set; }

    }
}
