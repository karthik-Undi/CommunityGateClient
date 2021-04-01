using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class EmployeeRegistration
    {
        [Required]

        public string EmployeeName { get; set; }
        [Required]

        public string EmployeeDept { get; set; }
        [Required]
        [RegularExpression("^[0-9]{10}",
        ErrorMessage = "Mobile number should only have 10 integers")]
        public string EmployeeMobile { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmployeeEmail { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must contain atleast 8 characters")]
        public string EmployeePassword { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("EmployeePassword", ErrorMessage = "Passwords Donot Match")]
        public string ConfirmPassword { get; set; }
    }
}
