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

        // ✅ Lấy toàn bộ danh sách tồn kho (hiển thị đầy đủ thông tin xe)
        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();
            var inventories = await _unitOfWork.InventoryRepository.GetAllAsync();

            // 🔹 Kiểm tra xem có xe nào mới mà chưa có record trong inventory chưa
            foreach (var v in vehicles)
            {
                var existingInventory = inventories.FirstOrDefault(i => i.VehicleId == v.VehicleId);
                if (existingInventory == null)
                {
                    var newInventory = new Inventory
                    {
                        VehicleId = v.VehicleId,
                        Quantity = 0 // Mặc định chưa nhập hàng
                    };

                    await _unitOfWork.InventoryRepository.AddAsync(newInventory);
                }
            }

            // 🔹 Lưu lại nếu có thêm mới Inventory
            await _unitOfWork.SaveAsync();

            // Lấy lại danh sách sau khi cập nhật
            inventories = await _unitOfWork.InventoryRepository.GetAllAsync();

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

        // ✅ Lấy tồn kho theo VehicleId
        public async Task<InventoryDTO?> GetInventoryByVehicleIdAsync(int vehicleId)
        {
            var inventory = (await _unitOfWork.InventoryRepository
                .FindIncludeAsync(i => i.VehicleId == vehicleId, i => i.Vehicle)).FirstOrDefault();

            if (inventory == null)
            {
                // 🔹 Nếu chưa có tồn kho, tự tạo mới
                var newInventory = new Inventory
                {
                    VehicleId = vehicleId,
                    Quantity = 0
                };
                await _unitOfWork.InventoryRepository.AddAsync(newInventory);
                await _unitOfWork.SaveAsync();

                inventory = newInventory;
            }

            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
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
        public async Task<InventoryDTO> CreateInventoryAsync(int vehicleId, int quantity)
        {
            // Check vehicle tồn tại
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("Không tìm thấy xe để thêm vào kho.");

            // Check nếu inventory đã tồn tại
            var existingInv = (await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == vehicleId)).FirstOrDefault();
            if (existingInv != null)
                throw new InvalidOperationException("Xe này đã có trong kho.");

            // Tạo mới inventory
            var newInventory = new Inventory
            {
                VehicleId = vehicleId,
                Quantity = quantity
            };

            await _unitOfWork.InventoryRepository.AddAsync(newInventory);

            // Cập nhật status cho vehicle
            vehicle.Status = quantity > 0 ? "Còn hàng" : "Hết hàng";
            _unitOfWork.VehicleRepository.Update(vehicle);

            await _unitOfWork.SaveAsync();

            return new InventoryDTO
            {
                InventoryId = newInventory.InventoryId,
                VehicleId = vehicle.VehicleId,
                Type = vehicle.Type,
                Model = vehicle.Model,
                Version = vehicle.Version,
                Color = vehicle.Color,
                Price = vehicle.Price,
                Distance = vehicle.Distance,
                Timecharging = vehicle.Timecharging,
                Speed = vehicle.Speed,
                Image1 = vehicle.Image1,
                Image2 = vehicle.Image2,
                Image3 = vehicle.Image3,
                Quantity = newInventory.Quantity,
                Status = vehicle.Status
            };
        }


        // ✅ Cập nhật số lượng tồn kho
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
