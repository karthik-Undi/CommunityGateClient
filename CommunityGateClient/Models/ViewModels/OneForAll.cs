using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityGateClient.Models.ViewModels
{
    public class OneForAll
    {
        public IEnumerable<Residents> residents { get; set; }
        public IEnumerable<Employees> employees { get; set; }
        public IEnumerable<Visitors> visitors { get; set; }
        public IEnumerable<Payments> payments { get; set; }
        public IEnumerable<Services> services { get; set; }
        public IEnumerable<DashBoardPosts> DashboardPosts { get; set; }
        public IEnumerable<FriendsAndFamily> friendsAndFamily { get; set; }
        public IEnumerable<HouseList> houseList { get; set; }
        public IEnumerable<Complaints> complaints { get; set; }
    }
}
