using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public partial class HouseList
    {
        public HouseList()
        {
            Residents = new HashSet<Residents>();
        }

        public int HouseId { get; set; }
        public string IsFree { get; set; }

        public virtual ICollection<Residents> Residents { get; set; }
    }
}
