using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class VehicleDTO
    {
        public int VehicleId { get; set; }

        public string Type { get; set; }

        public string Model { get; set; }

        public string Version { get; set; }

        public string Color { get; set; }

        public decimal Price { get; set; }

        public string Distance { get; set; }

        public string Timecharging { get; set; }

        public string Speed { get; set; }


        public string Image1 { get; set; }

        public string Image2 { get; set; }

        public string Image3 { get; set; }

        public string Status { get; set; }
    }

    public class VehicleComparisonDTO
    {
        public VehicleDTO Vehicle1 { get; set; }
        public VehicleDTO Vehicle2 { get; set; }
        public decimal PriceDifference { get; set; }
        public string TypeComparison { get; set; }
        public string ModelComparison { get; set; }
        public string VersionComparison { get; set; }
        public string ColorComparison { get; set; }
        public string StatusComparison { get; set; }
        public string DistanceComparison { get; set; }
        public string SpeedComparison { get; set; }
        public string  TimeChargingComparison {get; set; }
    }

}
