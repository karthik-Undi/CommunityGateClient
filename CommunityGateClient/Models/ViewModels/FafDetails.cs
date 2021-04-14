using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class FafDetails
    {
        public int FaFid { get; set; }
        public int? ResidentHouseNo { get; set; }
        public string ResidentName { get; set; }
        public string FaFname { get; set; }
        public int? ResidentId { get; set; }
        public string Relation { get; set; }
        public string ResidentMobileNo { get; set; }
    }
}
