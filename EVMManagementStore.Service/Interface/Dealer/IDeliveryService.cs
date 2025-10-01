using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDTO>> GetAllDeliveriesAsync();
        Task<DeliveryDTO> GetDeliveryByIdAsync(int deliveryId);
        Task<DeliveryDTO> CreateDeliveryAsync(DeliveryDTO deliveryDto);
        Task<DeliveryDTO> UpdateDeliveryAsync(int deliveryId, DeliveryDTO deliveryDto); 
        Task<bool> DeleteDeliveryAsync(int deliveryId); 
    }
}
