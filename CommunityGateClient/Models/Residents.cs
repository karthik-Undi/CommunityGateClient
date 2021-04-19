using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommunityGateClient.Models
{
    public class Residents
    {
        public int ResidentId { get; set; }
        public string ResidentName { get; set; }
        public int? ResidentHouseNo { get; set; }
        public string ResidentType { get; set; }
        public string ResidentMobileNo { get; set; }
        public string ResidentEmail { get; set; }
        public string ResidentPassword { get; set; }
        public string IsApproved { get; set; }

        [RegularExpression("^[1-9]+[1-9]*$", ErrorMessage = "Please enter a value greater than zero")]
        public int? ResidentWallet { get; set; }

    }
}
