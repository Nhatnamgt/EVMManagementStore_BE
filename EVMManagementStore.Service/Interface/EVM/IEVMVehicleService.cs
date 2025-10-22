using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IEVMVehicleService
    {
        Task<IEnumerable<VehicleDTO>> GetAllVehiclesAsync();
        Task<VehicleDTO?> GetVehicleByIdAsync(int id);
        Task<VehicleDTO> CreateVehicleAsync(VehicleDTO vehicleDto);
        Task<VehicleDTO?> UpdateVehicleAsync(int id, VehicleDTO vehicleDto);
        Task<bool> DeleteVehicleAsync(int id);
    }
}
