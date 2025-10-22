using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public partial class DeliveryDTO
    {
        public int DeliveryId { get; set; }

        public int UserId { get; set; }

        public int OrderId { get; set; }

        public int VehicleId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string DeliveryStatus { get; set; }

        public string Notes { get; set; }

    }
}
