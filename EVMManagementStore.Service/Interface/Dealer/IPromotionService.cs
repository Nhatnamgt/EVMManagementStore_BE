using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IPromotionService
    {
        Task<List<PromotionDTO>> GetAllPromotionsAsync();
        Task<PromotionDTO> GetPromotionByIdAsync(int id);
        Task<PromotionDTO> CreatePromotionAsync(PromotionDTO promotion);
        Task<PromotionDTO> UpdatePromotionAsync(int id, PromotionDTO promotion);
        Task<bool> DeletePromotionAsync(int id);
    }
}
