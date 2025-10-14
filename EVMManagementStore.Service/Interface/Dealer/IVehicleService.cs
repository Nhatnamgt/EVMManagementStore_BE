using EVMManagementStore.Service.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDTO>> GetAllVehicle();
        Task<VehicleDTO?> GetVehicleById(int vehicleId);
        Task<VehicleComparisonDTO> CompareVehicles(int vehicleId1, int vehicleId2);
        Task<List<VehicleDTO>> SearchVehicle(string sreach);
    }
}
