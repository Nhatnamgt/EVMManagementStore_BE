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

            foreach (var v in vehicles)
            {
                if (!inventories.Any(i => i.VehicleId == v.VehicleId))
                {
                    await _unitOfWork.InventoryRepository.AddAsync(new Inventory
                    {
                        VehicleId = v.VehicleId,
                        Quantity = 0
                    });
                }
            }

            await _unitOfWork.SaveAsync();
            inventories = await _unitOfWork.InventoryRepository.GetAllAsync();

            return (from v in vehicles
                    join inv in inventories on v.VehicleId equals inv.VehicleId into vi
                    from i in vi.DefaultIfEmpty()
                    select new InventoryDTO
                    {
                        InventoryId = i?.InventoryId ?? 0,
                        VehicleId = v.VehicleId,
                        Type = v.Type,
                        Model = v.Model,
                        Version = v.Version,
                        Color = v.Color,
                        Price = v.Price,
                        FinalPrice = v.FinalPrice ?? v.Price, // ⭐ GIÁ SAU GIẢM
                        DiscountId = v.DiscountId,           // ⭐ ĐANG ÁP DỤNG GIẢM GIÁ
                        Distance = v.Distance,
                        Timecharging = v.Timecharging,
                        Speed = v.Speed,
                        Image1 = v.Image1,
                        Image2 = v.Image2,
                        Image3 = v.Image3,
                        Quantity = i?.Quantity ?? 0,
                        Status = (i?.Quantity ?? 0) > 0 ? "Còn hàng" : "Hết hàng"
                    }).ToList();
        }

        public async Task<InventoryDTO?> GetInventoryByVehicleIdAsync(int vehicleId)
        {
            var inv = (await _unitOfWork.InventoryRepository.FindIncludeAsync(i => i.VehicleId == vehicleId, i => i.Vehicle))
                .FirstOrDefault();

            if (inv == null)
            {
                inv = new Inventory { VehicleId = vehicleId, Quantity = 0 };
                await _unitOfWork.InventoryRepository.AddAsync(inv);
                await _unitOfWork.SaveAsync();
            }

            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = v.VehicleId,
                Type = v.Type,
                Model = v.Model,
                Version = v.Version,
                Color = v.Color,
                Price = v.Price,
                FinalPrice = v.FinalPrice ?? v.Price,
                DiscountId = v.DiscountId,
                Distance = v.Distance,
                Timecharging = v.Timecharging,
                Speed = v.Speed,
                Image1 = v.Image1,
                Image2 = v.Image2,
                Image3 = v.Image3,
                Quantity = inv.Quantity,
                Status = inv.Quantity > 0 ? "Còn hàng" : "Hết hàng"
            };
        }

        public async Task<InventoryDTO> CreateInventoryAsync(int vehicleId, int quantity)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Không tìm thấy xe.");

            var existing = (await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == vehicleId)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException("Xe này đã có trong kho.");

            var inv = new Inventory { VehicleId = vehicleId, Quantity = quantity };
            await _unitOfWork.InventoryRepository.AddAsync(inv);

            vehicle.Status = quantity > 0 ? "Còn hàng" : "Hết hàng";
            _unitOfWork.VehicleRepository.Update(vehicle);

            await _unitOfWork.SaveAsync();

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = vehicle.VehicleId,
                Type = vehicle.Type,
                Model = vehicle.Model,
                Version = vehicle.Version,
                Color = vehicle.Color,
                Price = vehicle.Price,
                FinalPrice = vehicle.FinalPrice ?? vehicle.Price,
                DiscountId = vehicle.DiscountId,
                Distance = vehicle.Distance,
                Timecharging = vehicle.Timecharging,
                Speed = vehicle.Speed,
                Image1 = vehicle.Image1,
                Image2 = vehicle.Image2,
                Image3 = vehicle.Image3,
                Quantity = inv.Quantity,
                Status = vehicle.Status
            };
        }

        public async Task<InventoryDTO> UpdateInventoryAsync(int vehicleId, int quantity)
        {
            var inv = (await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == vehicleId)).FirstOrDefault();
            if (inv == null)
                throw new KeyNotFoundException("Không tìm thấy tồn kho.");

            inv.Quantity = quantity;
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);

            vehicle.Status = quantity > 0 ? "Còn hàng" : "Hết hàng";
            _unitOfWork.VehicleRepository.Update(vehicle);
            _unitOfWork.InventoryRepository.Update(inv);
            await _unitOfWork.SaveAsync();

            return new InventoryDTO
            {
                InventoryId = inv.InventoryId,
                VehicleId = vehicle.VehicleId,
                Type = vehicle.Type,
                Model = vehicle.Model,
                Version = vehicle.Version,
                Color = vehicle.Color,
                Price = vehicle.Price,
                FinalPrice = vehicle.FinalPrice ?? vehicle.Price,
                DiscountId = vehicle.DiscountId,
                Distance = vehicle.Distance,
                Timecharging = vehicle.Timecharging,
                Speed = vehicle.Speed,
                Image1 = vehicle.Image1,
                Image2 = vehicle.Image2,
                Image3 = vehicle.Image3,
                Quantity = inv.Quantity,
                Status = vehicle.Status
            };
        }

        public async Task<bool> DeleteInventoryAsync(int inventoryId)
        {
            var inventory = await _unitOfWork.InventoryRepository.GetByIdAsync(inventoryId);
            if (inventory == null)
                throw new KeyNotFoundException("Không tìm thấy inventory cần xóa.");

            _unitOfWork.InventoryRepository.Remove(inventory);

            // Cập nhật trạng thái của xe về “Hết hàng”
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(inventory.VehicleId);
            if (vehicle != null)
            {
                vehicle.Status = "Hết hàng";
                _unitOfWork.VehicleRepository.Update(vehicle);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }


        // ✅ Dispatch xe tới đại lý (giảm số lượng trong kho)
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
