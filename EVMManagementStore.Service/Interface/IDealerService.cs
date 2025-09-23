using EVMManagementStore.Service.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface
{
    public interface IDealerService
    {
        Task<IEnumerable<VehicleDTO>> GetAllVehicle();
        Task<VehicleDTO?> GetVehicleById(int vehicleId);
        Task<VehicleComparisonDTO> CompareVehicles(int vehicleId1, int vehicleId2);
    }
}
