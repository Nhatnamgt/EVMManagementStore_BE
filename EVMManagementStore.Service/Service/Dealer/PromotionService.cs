using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.Dealer
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<PromotionDTO>> GetAllPromotionsAsync()
        {
            var promotions = await _unitOfWork.PromotionRepository.GetAllAsync();
            return promotions.Select(p => new PromotionDTO
            {
                PromotionId = p.PromotionId,
                UserId = p.UserId,
                PromotionCode = p.PromotionCode,
                OptionName = p.OptionName,
                OptionValue = p.OptionValue,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList();
        }
        public async Task<PromotionDTO> GetPromotionByIdAsync(int id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null) return null;

            return new PromotionDTO
            {
                PromotionId = promotion.PromotionId,
                UserId = promotion.UserId,
                PromotionCode = promotion.PromotionCode,
                OptionName = promotion.OptionName,
                OptionValue = promotion.OptionValue,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }

        public async Task<PromotionDTO> CreatePromotionAsync(PromotionDTO promotionDto)
        {
            var promotion = new Promotion
            {
                PromotionId = promotionDto.PromotionId,
                UserId = promotionDto.UserId,
                PromotionCode = promotionDto.PromotionCode,
                OptionName = promotionDto.OptionName,
                OptionValue = promotionDto.OptionValue,
                StartDate = promotionDto.StartDate,
                EndDate = promotionDto.EndDate
            };

            await _unitOfWork.PromotionRepository.AddAsync(promotion);
            await _unitOfWork.SaveAsync();

            promotionDto.PromotionId = promotion.PromotionId; // cập nhật ID sau khi tạo
            return promotionDto;
        }
        public async Task<PromotionDTO> UpdatePromotionAsync(int id, PromotionDTO promotionDto)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null) return null;

            promotion.UserId = promotionDto.UserId;
            promotion.PromotionCode = promotionDto.PromotionCode;
            promotion.OptionName = promotionDto.OptionName;
            promotion.OptionValue = promotionDto.OptionValue;   
            promotion.StartDate = promotionDto.StartDate;
            promotion.EndDate = promotionDto.EndDate;

            await _unitOfWork.PromotionRepository.UpdateAsync(promotion);
            await _unitOfWork.SaveAsync();

            return promotionDto;
        }
        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promotion = await _unitOfWork.PromotionRepository.GetByIdAsync(id);
            if (promotion == null) return false;

            await _unitOfWork.PromotionRepository.RemoveAsync(promotion);
            await _unitOfWork.SaveAsync();
            return true;
        }

    }
}
