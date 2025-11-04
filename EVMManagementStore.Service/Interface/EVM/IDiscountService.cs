using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountDTO>> GetAllAsync();
        Task<DiscountDTO?> GetByIdAsync(int id);
        Task<DiscountDTO> CreateAsync(DiscountDTO dto);
        Task<DiscountDTO?> UpdateAsync(int id, DiscountDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApplyDiscountToVehicleAsync(int vehicleId, int discountId);
        Task<bool> RemoveDiscountFromVehicleAsync(int vehicleId);
        decimal CalculateFinalPrice(EVMManagementStore.Repository.Models.Vehicle vehicle);
    }
}
