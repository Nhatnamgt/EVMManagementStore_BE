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

        // ================================
        // AUTO UPDATE DISCOUNT STATUS
        // ================================
        private void AutoUpdateDiscountStatus(Discount d)
        {
            var today = DateTime.UtcNow.Date;

            if (d.EndDate < today)
                d.Status = "EXPIRED";
            else if (d.StartDate <= today)
                d.Status = "ACTIVE";
            else
                d.Status = "UPCOMING";
        }


        // ================================
        // GET ALL
        // ================================
        public async Task<IEnumerable<DiscountDTO>> GetAllAsync()
        {
            var discounts = await _unitOfWork.DiscountsRepository.GetAllAsync();

            foreach (var d in discounts)
                AutoUpdateDiscountStatus(d);

            await _unitOfWork.SaveAsync();

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

        // ================================
        // GET BY ID
        // ================================
        public async Task<DiscountDTO?> GetByIdAsync(int id)
        {
            var d = await _unitOfWork.DiscountsRepository.GetByIdAsync(id);
            if (d == null) return null;

            AutoUpdateDiscountStatus(d);
            await _unitOfWork.SaveAsync();

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

        // ================================
        // CREATE DISCOUNT
        // ================================
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
                EndDate = dto.EndDate
            };

            AutoUpdateDiscountStatus(entity);

            await _unitOfWork.DiscountsRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            dto.DiscountId = entity.DiscountId;
            dto.Status = entity.Status;
            return dto;
        }

        // ================================
        // UPDATE DISCOUNT
        // ================================
        public async Task<DiscountDTO?> UpdateAsync(int id, DiscountDTO dto)
        {
            var d = await _unitOfWork.DiscountsRepository.GetByIdAsync(id);
            if (d == null) return null;

            d.DiscountName = dto.DiscountName;
            d.DiscountType = dto.DiscountType;
            d.DiscountValue = dto.DiscountValue;
            d.StartDate = dto.StartDate;
            d.EndDate = dto.EndDate;

            AutoUpdateDiscountStatus(d);

            _unitOfWork.DiscountsRepository.Update(d);
            await _unitOfWork.SaveAsync();

            var vehicles = await _unitOfWork.VehicleRepository.FindAsync(v => v.DiscountId == id);

            foreach (var v in vehicles)
            {
                if (d.Status == "EXPIRED")
                {
                    v.DiscountId = null;
                    v.FinalPrice = v.Price;
                }
                else
                {
                    v.FinalPrice = CalculateFinalPrice(v, d);
                }

                _unitOfWork.VehicleRepository.Update(v);
            }

            await _unitOfWork.SaveAsync();

            return dto;
        }

        // ================================
        // DELETE
        // ================================
        public async Task<bool> DeleteAsync(int id)
        {
            var d = await _unitOfWork.DiscountsRepository.GetByIdAsync(id);
            if (d == null) return false;

            var vehicles = await _unitOfWork.VehicleRepository.FindAsync(v => v.DiscountId == id);

            foreach (var v in vehicles)
            {
                v.DiscountId = null;
                v.FinalPrice = v.Price;
                _unitOfWork.VehicleRepository.Update(v);
            }

            _unitOfWork.DiscountsRepository.Remove(d);
            await _unitOfWork.SaveAsync();

            return true;
        }

        // ================================
        // APPLY DISCOUNT TO VEHICLE
        // ================================
        public async Task<bool> ApplyDiscountToVehicleAsync(int vehicleId, int discountId)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            var d = await _unitOfWork.DiscountsRepository.GetByIdAsync(discountId);

            if (v == null || d == null)
                throw new Exception("Vehicle hoặc Discount không tồn tại.");

            AutoUpdateDiscountStatus(d);

           
            if (d.Status == "EXPIRED")
                throw new Exception("Discount đã hết hạn.");

       
            if (v.DiscountId != null)
                throw new Exception("Xe đã có discount. Không thể áp dụng thêm.");


            ValidateDiscountValue(v.Price, d);

            v.DiscountId = discountId;
            v.FinalPrice = CalculateFinalPrice(v, d);

            _unitOfWork.VehicleRepository.Update(v);
            await _unitOfWork.SaveAsync();

            return true;
        }

        // ================================
        // REMOVE DISCOUNT
        // ================================
        public async Task<bool> RemoveDiscountFromVehicleAsync(int vehicleId)
        {
            var v = await _unitOfWork.VehicleRepository.GetByIdAsync(vehicleId);
            if (v == null) return false;

            v.DiscountId = null;
            v.FinalPrice = v.Price;

            _unitOfWork.VehicleRepository.Update(v);
            await _unitOfWork.SaveAsync();

            return true;
        }

        // ================================
        // VALIDATION FOR VALUE
        // ================================
        private void ValidateDiscountValue(decimal price, Discount d)
        {
            if (d.DiscountType == "percent")
            {
                if (d.DiscountValue < 0 || d.DiscountValue > 100)
                    throw new Exception("Giá trị phần trăm phải từ 0–100%.");
            }
            else if (d.DiscountType == "amount")
            {
                if (d.DiscountValue < 0)
                    throw new Exception("Giá trị giảm không được âm.");

                if (d.DiscountValue > price)
                    throw new Exception("Số tiền giảm không được lớn hơn giá xe.");
            }
        }

        // ================================
        // FINAL PRICE CALCULATION
        // ================================
        public decimal CalculateFinalPrice(Vehicle v, Discount d)
        {
            ValidateDiscountValue(v.Price, d);

            decimal finalPrice = v.Price;

            if (d.DiscountType == "percent")
                finalPrice -= v.Price * (d.DiscountValue / 100m);

            else if (d.DiscountType == "amount")
                finalPrice -= d.DiscountValue;

            return finalPrice < 0 ? 0 : finalPrice;
        }
    }
}
