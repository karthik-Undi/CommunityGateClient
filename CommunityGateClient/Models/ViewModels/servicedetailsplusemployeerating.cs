using CommunityGateClient.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class servicedetailsplusemployeerating
    {
        public IEnumerable<ServiceDetails> serviceDetails { get; set; }
        public Employees employees { get; set; }
    }
}
