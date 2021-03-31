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
        public string ResidentMobileNo { get; set; }
        [Required(ErrorMessage = "Please Enter the Email")]
        [DisplayName("Resident Email")]
        public string ResidentEmail { get; set; }
        [Required(ErrorMessage = "Please Enter the Password")]
        [DisplayName("Resident Password")]
        public string ResidentPassword { get; set; }
        [Compare("ResidentPassword", ErrorMessage ="Passwords Do not Match")]
        [DisplayName("Confirm Password")]
        public string ResidentConfirmPassword { get; set; }
    }
}
