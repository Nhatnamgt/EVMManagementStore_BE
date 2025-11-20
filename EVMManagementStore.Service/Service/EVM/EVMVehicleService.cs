using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.EVM
{
    public class EVMVehicleService : IEVMVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;

        public EVMVehicleService(IUnitOfWork unitOfWork, IDiscountService discountService)
        {
            _unitOfWork = unitOfWork;
            _discountService = discountService;
        }

        public async Task<IEnumerable<VehicleDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();

            return vehicles.Select(v => new VehicleDTO
            {
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
                Status = v.Status
            }).ToList();
        }

        public async Task<VehicleDTO?> GetVehicleByIdAsync(int id)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (v == null) return null;

            return new VehicleDTO
            {
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
                Status = v.Status
            };
        }

        // ======================================
        // CREATE VEHICLE 
        // ======================================
        public async Task<VehicleDTO> CreateVehicleAsync(VehicleDTO dto)
        {
            var v = new Vehicle
            {
                Type = dto.Type,
                Model = dto.Model,
                Version = dto.Version,
                Color = dto.Color,
                Price = dto.Price,
                FinalPrice = dto.Price,
                DiscountId = null,
                Distance = dto.Distance,
                Timecharging = dto.Timecharging,
                Speed = dto.Speed,
                Image1 = dto.Image1,
                Image2 = dto.Image2,
                Image3 = dto.Image3,
                Status = dto.Status
            };

            await _unitOfWork.VehicleRepository.AddAsync(v);
            await _unitOfWork.SaveAsync();

            // 🔥 AUTO TẠO INVENTORY THEO MÀU
            var colors = dto.Color
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();

            foreach (var c in colors)
            {
                await _unitOfWork.InventoryRepository.AddAsync(new Inventory
                {
                    VehicleId = v.VehicleId,
                    Color = c,
                    Quantity = 0
                });
            }

            await _unitOfWork.SaveAsync();
            dto.VehicleId = v.VehicleId;

            return dto;
        }
        // ================================
        // UPDATE VEHICLE
        // ================================
        public async Task<VehicleDTO?> UpdateVehicleAsync(int id, VehicleDTO dto)
        {
            var existing = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (existing == null) return null;

            // Cập nhật các thuộc tính cơ bản
            existing.Type = dto.Type;
            existing.Model = dto.Model;
            existing.Version = dto.Version;
            existing.Color = dto.Color;
            existing.Price = dto.Price;
            existing.Distance = dto.Distance;
            existing.Timecharging = dto.Timecharging;
            existing.Speed = dto.Speed;
            existing.Image1 = dto.Image1;
            existing.Image2 = dto.Image2;
            existing.Image3 = dto.Image3;
            existing.Status = dto.Status;

            var updatedColors = dto.Color
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();

            var existingInventories = await _unitOfWork.InventoryRepository
                .FindAsync(i => i.VehicleId == id);

            var existingColors = existingInventories
                .Select(i => i.Color)
                .ToList();

            foreach (var color in updatedColors)
            {
                if (!existingColors.Any(x => x.Equals(color, StringComparison.OrdinalIgnoreCase)))
                {
                    await _unitOfWork.InventoryRepository.AddAsync(new Inventory
                    {
                        VehicleId = id,
                        Color = color,
                        Quantity = 0
                    });
                }
            }

            foreach (var inv in existingInventories)
            {
                if (!updatedColors.Any(x => x.Equals(inv.Color, StringComparison.OrdinalIgnoreCase)))
                {
                    if (inv.Quantity > 0)
                        throw new Exception($"Không thể xoá màu {inv.Color} vì vẫn còn xe trong kho.");

                    _unitOfWork.InventoryRepository.Remove(inv);
                }
            }

            // -------------------------
            // Cập nhật Final Price theo discount
            // -------------------------
            if (existing.DiscountId != null)
            {
                var discount = await _unitOfWork.DiscountsRepository.GetByIdAsync(existing.DiscountId.Value);
                existing.FinalPrice = discount != null
                    ? _discountService.CalculateFinalPrice(existing, discount)
                    : existing.Price;
            }
            else
            {
                existing.FinalPrice = existing.Price;
            }

            // Lưu lại thay đổi
            _unitOfWork.VehicleRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            return dto;
        }

        // ======================================
        // DELETE VEHICLE 
        // ======================================
        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (v == null) return false;

            var inventories = await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == id);
            
            if (inventories.Any(inv => inv.Quantity > 0))
                throw new Exception("Không thể xoá xe vì vẫn còn xe trong kho.");

            foreach (var inv in inventories)
                _unitOfWork.InventoryRepository.Remove(inv);

            _unitOfWork.VehicleRepository.Remove(v);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
