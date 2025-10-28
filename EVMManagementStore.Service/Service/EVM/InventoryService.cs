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

        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();
            var inventories = await _unitOfWork.InventoryRepository.GetAllAsync();

            var result = from v in vehicles
                         join i in inventories on v.VehicleId equals i.VehicleId into vi
                         from inv in vi.DefaultIfEmpty()
                         select new InventoryDTO
                         {
                             InventoryId = inv?.InventoryId ?? 0,
                             VehicleId = v.VehicleId,
                             Type = v.Type,
                             Model = v.Model,
                             Version = v.Version,
                             Color = v.Color,
                             Price = v.Price,
                             Distance = v.Distance,
                             Timecharging = v.Timecharging,
                             Speed = v.Speed,
                             Image1 = v.Image1,
                             Image2 = v.Image2,
                             Image3 = v.Image3,
                             Quantity = inv?.Quantity ?? 0,
                             Status = (inv?.Quantity ?? 0) > 0 ? "Còn hàng" : "Hết hàng"
                         };

            return result.ToList();
        }

        public async Task<InventoryDTO?> GetInventoryByVehicleIdAsync(int vehicleId)
        {
            var inventory = (await _unitOfWork.InventoryRepository
                .FindIncludeAsync(i => i.VehicleId == vehicleId, i => i.Vehicle)).FirstOrDefault();

            if (inventory == null) return null;

            var v = inventory.Vehicle;
            return new InventoryDTO
            {
                InventoryId = inventory.InventoryId,
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = v.Color,
                Price = v.Price,
                Distance = v.Distance,
                Timecharging = v.Timecharging,
                Speed = v.Speed,
                Image1 = v.Image1,
                Image2 = v.Image2,
                Image3 = v.Image3,
                Quantity = inventory.Quantity,
                Status = inventory.Quantity > 0 ? "Còn hàng" : "Hết hàng"
            };
        }

        public async Task<InventoryDTO> UpdateInventoryAsync(int vehicleId, int quantity)
        {
            var inventory = (await _unitOfWork.InventoryRepository
                .FindAsync(i => i.VehicleId == vehicleId)).FirstOrDefault();

            if (inventory == null)
                throw new KeyNotFoundException("Không tìm thấy kho cho xe này");

            inventory.Quantity = quantity;

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
                VehicleId = vehicle?.VehicleId ?? 0,
                Type = vehicle?.Type ?? "",
                Model = vehicle?.Model ?? "",
                Version = vehicle?.Version ?? "",
                Color = vehicle?.Color ?? "",
                Price = vehicle?.Price ?? 0,
                Distance = vehicle?.Distance ?? "",
                Timecharging = vehicle?.Timecharging ?? "",
                Speed = vehicle?.Speed ?? "",
                Image1 = vehicle?.Image1 ?? "",
                Image2 = vehicle?.Image2 ?? "",
                Image3 = vehicle?.Image3 ?? "",
                Quantity = inventory.Quantity,
                Status = vehicle?.Status ?? "Không xác định"
            };
        }


        public async Task<bool> DispatchVehicleAsync(DispatchRequest request)
        {
            var inventory = (await _unitOfWork.InventoryRepository
                .FindAsync(i => i.VehicleId == request.VehicleId)).FirstOrDefault();

            if (inventory == null)
                throw new KeyNotFoundException("Không tìm thấy xe trong kho");

            if (inventory.Quantity < request.Quantity)
                throw new InvalidOperationException("Không đủ xe trong kho để điều phối");

            inventory.Quantity -= request.Quantity;

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
