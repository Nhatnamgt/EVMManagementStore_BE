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


        public async Task<VehicleDTO?> UpdateVehicleAsync(int id, VehicleDTO dto)
        {
            var existing = await _unitOfWork.VehicleRepository.GetByIdAsync(id);
            if (existing == null) return null;

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

            var updatedColorsRaw = dto.Color
                           .Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(c => c.Trim())
                           .ToList();

            var updatedColorsLower = updatedColorsRaw
                .Select(c => c.ToLower())
                .ToList();
            var existingInventories = await _unitOfWork.InventoryRepository
                 .FindAsync(i => i.VehicleId == id);

            var existingColorsLower = existingInventories
                .Select(i => i.Color.ToLower())
                .ToList();

            foreach (var color in updatedColorsRaw)
            {
                if (!existingColorsLower.Contains(color.ToLower()))
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
                if (!updatedColorsLower.Contains(inv.Color.ToLower()))
                {
                    _unitOfWork.InventoryRepository.Remove(inv);
                }
            }

            // ================================
            // 🔥 CẬP NHẬT FINAL PRICE (Nếu có discount)
            // ================================
            if (existing.DiscountId != null)
            {
                var discount = await _unitOfWork.DiscountsRepository.GetByIdAsync(existing.DiscountId.Value);

                if (discount != null)
                    existing.FinalPrice = _discountService.CalculateFinalPrice(existing, discount);
                else
                    existing.FinalPrice = existing.Price;
            }
            else
            {
                existing.FinalPrice = existing.Price;
            }

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

            // 🔥 XOÁ HẾT INVENTORY LIÊN QUAN TRƯỚC
            var inventories = await _unitOfWork.InventoryRepository.FindAsync(i => i.VehicleId == id);
            foreach (var inv in inventories)
                _unitOfWork.InventoryRepository.Remove(inv);

            // XOÁ XE
            _unitOfWork.VehicleRepository.Remove(v);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
