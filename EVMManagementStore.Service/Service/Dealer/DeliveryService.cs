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
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;   
        public DeliveryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<DeliveryDTO>> GetAllDeliveriesAsync()
        {
            var deliveries = await _unitOfWork.DeliveryRepository.GetAllAsync();

            return deliveries.Select(d => new DeliveryDTO
            {
                DeliveryId = d.DeliveryId,
                UserId = d.UserId,  
                OrderId = d.OrderId,
                VehicleId = d.VehicleId,
                DeliveryDate = d.DeliveryDate,
                DeliveryStatus = d.DeliveryStatus,
                Notes = d.Notes
            }).ToList();
        }

        public async Task<DeliveryDTO> GetDeliveryByIdAsync(int deliveryId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            if (delivery == null) return null;

            return new DeliveryDTO
            {
                DeliveryId = delivery.DeliveryId,
                UserId = delivery.UserId,
                OrderId = delivery.OrderId,
                VehicleId = delivery.VehicleId,
                DeliveryDate = delivery.DeliveryDate,
                DeliveryStatus = delivery.DeliveryStatus,
                Notes = delivery.Notes
            };
        }

        public async Task<DeliveryDTO> CreateDeliveryAsync(DeliveryDTO deliveryDto)
        {
            var delivery = new Delivery
            {
                OrderId = deliveryDto.OrderId,
                UserId = deliveryDto.UserId,        
                VehicleId = deliveryDto.VehicleId,
                DeliveryDate = deliveryDto.DeliveryDate,
                DeliveryStatus = deliveryDto.DeliveryStatus,
                Notes = deliveryDto.Notes
            };

            await _unitOfWork.DeliveryRepository.AddAsync(delivery);
            await _unitOfWork.SaveAsync();

            deliveryDto.DeliveryId = delivery.DeliveryId;
            return deliveryDto;
        }

        public async Task<DeliveryDTO> UpdateDeliveryAsync(int deliveryId, DeliveryDTO deliveryDto)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            if (delivery == null) return null;

            delivery.OrderId = deliveryDto.OrderId;
            delivery.UserId = deliveryDto.UserId;   
            delivery.VehicleId = deliveryDto.VehicleId;
            delivery.DeliveryDate = deliveryDto.DeliveryDate;
            delivery.DeliveryStatus = deliveryDto.DeliveryStatus;
            delivery.Notes = deliveryDto.Notes;

            await _unitOfWork.DeliveryRepository.UpdateAsync(delivery);
            await _unitOfWork.SaveAsync();

            return deliveryDto;
        }
        public async Task<bool> DeleteDeliveryAsync(int deliveryId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            if (delivery == null) return false;

            await _unitOfWork.DeliveryRepository.RemoveAsync(delivery);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
