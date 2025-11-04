using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.EVM
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DiscountDTO>> GetAllAsync()
        {
            var discounts = await _unitOfWork.DiscountRepository.GetAllAsync();

            return discounts.Select(d => new DiscountDTO
            {
                DiscountId = d.DiscountId,
                UserId = d.UserId,
                DiscountCode = d.DiscountCode,
                DiscountName = d.DiscountName,
                DiscountType = d.DiscountType,
                DiscountValue = d.DiscountValue,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Status = d.Status
            }).ToList();
        }

        public async Task<DiscountDTO?> GetByIdAsync(int id)
        {
            var d = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
            if (d == null) return null;

            return new DiscountDTO
            {
                DiscountId = d.DiscountId,
                UserId = d.UserId,
                DiscountCode = d.DiscountCode,
                DiscountName = d.DiscountName,
                DiscountType = d.DiscountType,
                DiscountValue = d.DiscountValue,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                Status = d.Status
            };
        }

        public async Task<DiscountDTO> CreateAsync(DiscountDTO dto)
        {
            var entity = new Discount
            {
                UserId = dto.UserId,
                DiscountCode = dto.DiscountCode,
                DiscountName = dto.DiscountName,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status ?? "ACTIVE"
            };

            await _unitOfWork.DiscountRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            dto.DiscountId = entity.DiscountId;
            return dto;
        }

        public async Task<DiscountDTO?> UpdateAsync(int id, DiscountDTO dto)
        {
            var existing = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.DiscountName = dto.DiscountName;
            existing.DiscountType = dto.DiscountType;
            existing.DiscountValue = dto.DiscountValue;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.Status = dto.Status;

            _unitOfWork.DiscountRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.DiscountRepository.Remove(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        // ⭐ Áp dụng giảm giá vào xe
        public async Task<bool> ApplyDiscountToVehicleAsync(int vehicleId, int discountId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            var discount = await _unitOfWork.DiscountRepository.GetByIdAsync(discountId);

            if (vehicle == null || discount == null)
                return false;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (discount.StartDate > today || discount.EndDate < today)
                throw new Exception("Discount is not active or has expired.");

            decimal finalPrice = vehicle.Price;

            switch (discount.DiscountType?.ToLower())
            {
                case "percent":
                    finalPrice -= vehicle.Price * (discount.DiscountValue / 100);
                    break;

                case "amount":
                    finalPrice -= discount.DiscountValue;
                    break;

                default:
                    throw new Exception("Invalid discount type.");
            }

            if (finalPrice < 0)
                finalPrice = 0;

            vehicle.FinalPrice = finalPrice;
            vehicle.DiscountId = discountId;

            _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveAsync();

            return true;
        }
        public async Task<bool> RemoveDiscountFromVehicleAsync(int vehicleId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            vehicle.DiscountId = null;
            vehicle.FinalPrice = vehicle.Price;

            _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveAsync();

            return true;
        }
        public decimal CalculateFinalPrice(Vehicle vehicle)
        {
            if (vehicle.DiscountId == null)
                return vehicle.Price; // Không có giảm giá → trả giá gốc

            var discount = _unitOfWork.DiscountRepository.GetByIdAsync(vehicle.DiscountId).Result;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // Nếu giảm giá chưa bắt đầu hoặc đã hết hạn → không áp dụng nữa
            if (discount == null || discount.StartDate > today || discount.EndDate < today)
                return vehicle.Price;

            decimal finalPrice = vehicle.Price;

            // Nếu giảm theo % → giảm theo phần trăm
            switch (discount.DiscountType?.ToLower())
            {
                case "percent":
                    finalPrice -= vehicle.Price * (discount.DiscountValue / 100);
                    break;

                // Nếu giảm số tiền cụ thể → trừ thẳng
                case "amount":
                    finalPrice -= discount.DiscountValue;
                    break;
            }

            // Không để giá âm
            return finalPrice < 0 ? 0 : finalPrice;
        }
    }
}
