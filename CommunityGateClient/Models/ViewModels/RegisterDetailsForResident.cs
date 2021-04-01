using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class RegisterDetailsForResident
    {
        [Required(ErrorMessage ="Please Enter the Name")]
        [DisplayName("Resident Name")]
        public string ResidentName { get; set; }

        [Required(ErrorMessage = "Please Enter the House No.")]
        [DisplayName("Resident House No.")]
        public int? ResidentHouseNo { get; set; }

        [Required(ErrorMessage = "Please Enter the Resident Type")]
        [DisplayName("Resident Type")]
        public string ResidentType { get; set; }

        [Required(ErrorMessage = "Please Enter the Mobile No.")]
        [DisplayName("Resident Mobile No.")]
        [RegularExpression("^[0-9]{10}",
        ErrorMessage = "Mobile number should only have 10 integers")]
        public string ResidentMobileNo { get; set; }

        [Required(ErrorMessage = "Please Enter the Email")]
        [DisplayName("Resident Email")]
        [DataType(DataType.EmailAddress)]
        [Remote("DoesEmailExist", "Login", ErrorMessage = "EmailId already exists in database.")]
        public string ResidentEmail { get; set; }

        [Required(ErrorMessage = "Please Enter the Password")]
        [DisplayName("Resident Password")]
        [MinLength(8, ErrorMessage = "Password must contain atleast 8 characters")]
        public string ResidentPassword { get; set; }

        [Compare("ResidentPassword", ErrorMessage ="Passwords Do not Match")]
        [DisplayName("Confirm Password")]
        public string ResidentConfirmPassword { get; set; }

    }
}
