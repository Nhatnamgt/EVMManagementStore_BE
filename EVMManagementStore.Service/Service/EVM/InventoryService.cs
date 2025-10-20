using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.EntityFrameworkCore;
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

        // ✅ Lấy toàn bộ kho + thông tin xe
        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            var inventories = await _unitOfWork.InventoryRepository
                .GetAllIncludeAsync(i => i.Vehicle);

            return inventories.Select(i => new InventoryDTO
            {
                InventoryId = i.InventoryId,
                VehicleId = i.VehicleId,
                Model = i.Vehicle.Model,
                Color = i.Vehicle.Color,
                Price = i.Vehicle.Price,
                Quantity = i.Quantity,
                Status = i.Quantity > 0 ? "Còn hàng" : "Hết hàng"
            }).ToList();
        }

        // ✅ Lấy kho theo VehicleId
        public async Task<InventoryDTO?> GetInventoryByVehicleIdAsync(int vehicleId)
        {
            var inventory = await _unitOfWork.InventoryRepository
                .FindIncludeAsync(i => i.VehicleId == vehicleId, i => i.Vehicle);

            var inv = inventory.FirstOrDefault();
            if (inv == null) return null;

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = inv.VehicleId,
                Model = inv.Vehicle.Model,
                Color = inv.Vehicle.Color,
                Price = inv.Vehicle.Price,
                Quantity = inv.Quantity,
                Status = inv.Quantity > 0 ? "Còn hàng" : "Hết hàng"
            };
        }

        // ✅ Cập nhật số lượng xe trong kho
        public async Task<InventoryDTO> UpdateInventoryAsync(int vehicleId, int quantity)
        {
            var inventory = (await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == vehicleId)).FirstOrDefault();
            if (inventory == null)
                throw new KeyNotFoundException("Không tìm thấy kho cho xe này");

            inventory.Quantity = quantity;

            // cập nhật trạng thái xe theo số lượng
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.Status = quantity > 0 ? "Còn hàng" : "Hết hàng";
                _unitOfWork.VehicleRepository.Update(vehicle);
            }

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveAsync();

            return new InventoryDTO
            {
                InventoryId = inventory.InventoryId,
                VehicleId = inventory.VehicleId,
                Model = vehicle?.Model ?? "",
                Color = vehicle?.Color ?? "",
                Price = vehicle?.Price ?? 0,
                Quantity = inventory.Quantity,
                Status = vehicle?.Status ?? "Không xác định"
            };
        }

        // ✅ Dispatch (xuất xe cho đại lý)
        public async Task<bool> DispatchVehicleAsync(DispatchRequest request)
        {
            var inventory = (await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == request.VehicleId)).FirstOrDefault();
            if (inventory == null) return false;

            if (inventory.Quantity < request.Quantity)
                throw new InvalidOperationException("Không đủ xe trong kho để điều phối");

            inventory.Quantity -= request.Quantity;

            // cập nhật trạng thái xe
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(request.VehicleId);
            if (vehicle != null)
            {
                vehicle.Status = inventory.Quantity > 0 ? "Còn hàng" : "Hết hàng";
                _unitOfWork.VehicleRepository.Update(vehicle);
            }

            _unitOfWork.InventoryRepository.Update(inventory);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
