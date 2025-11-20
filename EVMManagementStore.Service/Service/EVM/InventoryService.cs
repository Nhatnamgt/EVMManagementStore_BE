using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.EVM
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================================
        // GET ALL INVENTORIES (GROUP BY VEHICLE)
        // ================================
        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();

            // 🔥 Nhóm theo VehicleId 
            var inventories = (await _unitOfWork.InventoryRepository.GetAllAsync())
                .OrderBy(inv => inv.VehicleId)
                .ThenBy(inv => inv.Color)
                .ToList();

            return inventories.Select(inv =>
            {
                var v = vehicles.FirstOrDefault(x => x.VehicleId == inv.VehicleId);
                if (v == null) return null; // phòng crash dữ liệu

                return new InventoryDTO
                {
                    InventoryId = inv.InventoryId,
                    VehicleId = v.VehicleId,
                    Type = v.Type,
                    Model = v.Model,
                    Version = v.Version,
                    Distance = v.Distance,
                    Timecharging = v.Timecharging,
                    Speed = v.Speed,
                    Image1 = v.Image1,
                    Image2 = v.Image2,
                    Image3 = v.Image3,
                    Color = inv.Color,
                    Quantity = inv.Quantity,
                    Price = v.Price,
                    FinalPrice = v.FinalPrice ?? v.Price,
                    DiscountId = v.DiscountId,
                    Status = inv.Quantity > 0 ? "Còn hàng" : "Hết hàng"
                };
            })
            .Where(x => x != null)
            .ToList()!;
        }

        // ================================
        // GET INVENTORY BY ID
        // ================================
        public async Task<InventoryDTO?> GetInventoryByIdAsync(int inventoryId)
        {
            var inv = await _unitOfWork.InventoryRepository.GetByIdAsync(inventoryId);
            if (inv == null) return null;

            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(inv.VehicleId);

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = inv.Color,
                Quantity = inv.Quantity,
                Price = v.Price,
                FinalPrice = v.FinalPrice ?? v.Price,
                Image1 = v.Image1,
                Image2 = v.Image2,
                Image3 = v.Image3,
                DiscountId = v.DiscountId,
                Status = inv.Quantity > 0 ? "Còn hàng" : "Hết hàng"
            };
        }

        // ================================
        // UPDATE INVENTORY QUANTITY
        // ================================
        public async Task<InventoryDTO> UpdateInventoryAsync(int inventoryId, int quantity)
        {
            var inv = await _unitOfWork.InventoryRepository.GetByIdAsync(inventoryId);
            if (inv == null)
                throw new KeyNotFoundException("Inventory not found.");

            inv.Quantity = quantity;
            _unitOfWork.InventoryRepository.Update(inv);

            await _unitOfWork.SaveAsync();

            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(inv.VehicleId);

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = vehicle.VehicleId,
                Color = inv.Color,
                Quantity = inv.Quantity,
                Price = vehicle.Price,
                FinalPrice = vehicle.FinalPrice ?? vehicle.Price,
                Status = inv.Quantity > 0 ? "Còn hàng" : "Hết hàng",
                Image1 = vehicle.Image1,
                Image2 = vehicle.Image2,
                Image3 = vehicle.Image3
            };
        }

        // ================================
        // DISPATCH
        // ================================
        public async Task<bool> DispatchVehicleAsync(DispatchRequest request)
        {
            var inventory = (await _unitOfWork.InventoryRepository
                .FindAsync(i => i.VehicleId == request.VehicleId
                                && i.Color == request.Color))
                .FirstOrDefault();

            if (inventory == null)
                throw new KeyNotFoundException("Không tìm thấy tồn kho cho màu này.");

            if (inventory.Quantity < request.Quantity)
                throw new InvalidOperationException("Không đủ xe trong kho.");

            inventory.Quantity -= request.Quantity;
            _unitOfWork.InventoryRepository.Update(inventory);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
