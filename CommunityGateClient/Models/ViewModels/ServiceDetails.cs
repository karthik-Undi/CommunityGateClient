using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesAPI.Models.ViewModels
{
    public class ServiceDetails
    {


        public int ServiceId { get; set; }
        public string ServiceType { get; set; }
        public DateTime? AppointmentTime { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceMessage { get; set; }
        public int? ServicePrice { get; set; }
        public int? EmployeeId { get; set; }
        public int? ResidentId { get; set; }
        public string EmployeeName { get; set; }
        public int? EmployeeRating { get; set; }



    }
}
