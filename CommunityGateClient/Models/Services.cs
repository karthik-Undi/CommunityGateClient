using System;
using System.Collections.Generic;


namespace CommunityGateClient.Models
{
    public partial class Services
    {
        public Services()
        {
            Payments = new HashSet<Payments>();
        }

        public int ServiceId { get; set; }
        public string ServiceType { get; set; }
        public DateTime? AppointmentTime { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceMessage { get; set; }
        public int? ServicePrice { get; set; }
        public int? EmployeeId { get; set; }
        public int? ResidentId { get; set; }

        public virtual Employees Employee { get; set; }
        public virtual Residents Resident { get; set; }
        public virtual ICollection<Payments> Payments { get; set; }
    }
}
