using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class TestDriveAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public int VehicleId { get; set; }
        public string Address { get; set; } 
        public string Username { get; set; }
        public string VehicleName { get; set; }
    }
}
