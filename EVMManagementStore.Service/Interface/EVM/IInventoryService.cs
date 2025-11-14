using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync();
        Task<InventoryDTO?> GetInventoryByIdAsync(int inventoryId);
        Task<InventoryDTO> UpdateInventoryAsync(int inventoryId, int quantity);
        Task<bool> DispatchVehicleAsync(DispatchRequest request);
    }
}
