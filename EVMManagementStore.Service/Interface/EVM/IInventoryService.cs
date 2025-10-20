using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync();
        Task<InventoryDTO?> GetInventoryByVehicleIdAsync(int vehicleId);
        Task<InventoryDTO> UpdateInventoryAsync(int vehicleId, int quantity);
        Task<bool> DispatchVehicleAsync(DispatchRequest request);
    }
}
