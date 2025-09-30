using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Dealer
{
    public class SaleManagementService : ISaleManagementService
    {
        private readonly IUnitOfWork _unitOfWork; 
        public SaleManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<DealerOrderDTO> CreateDealerOrderAsync(DealerOrderDTO dealerorderDTO)
        {
            if (dealerorderDTO == null)
                throw new ArgumentNullException(nameof(dealerorderDTO));

            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(dealerorderDTO.VehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"VehicleId {dealerorderDTO.VehicleId} not found.");

            var dealerOrder = new DealerOrder
            {
                UserId = dealerorderDTO.UserId,
                VehicleId = dealerorderDTO.VehicleId,
                Quantity = dealerorderDTO.Quantity,
                OrderDate = dealerorderDTO.OrderDate ?? DateTime.UtcNow,
                Status = string.IsNullOrEmpty(dealerorderDTO.Status) ? "Pending" : dealerorderDTO.Status,
                PaymentStatus = string.IsNullOrEmpty(dealerorderDTO.PaymentStatus) ? "Unpaid" : dealerorderDTO.PaymentStatus,
                TotalAmount = dealerorderDTO.TotalAmount,
            };

            await _unitOfWork.DealerOrderRepository.AddAsync(dealerOrder);
            await _unitOfWork.SaveAsync();

            return new DealerOrderDTO
            {
                DealerOrderId = dealerOrder.DealerOrderId,
                UserId = dealerOrder.UserId,
                VehicleId = dealerOrder.VehicleId,
                Quantity = dealerOrder.Quantity,
                OrderDate = dealerOrder.OrderDate,
                Status = dealerOrder.Status,
                PaymentStatus = dealerOrder.PaymentStatus,
                TotalAmount = dealerOrder.TotalAmount
            };
        }


        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO)
        {
            if (orderDTO == null)
                throw new ArgumentNullException(nameof(orderDTO));

            //decimal totalAmount = orderDTO.TotalAmount;
            //if (orderDTO.QuotationId.HasValue)
            //{
            //    var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(orderDTO.QuotationId.Value);
            //    if (quotation == null)
            //        throw new KeyNotFoundException($"QuotationId {orderDTO.QuotationId.Value} not found.");

            //    totalAmount = quotation.FinalPrice;
            //}

            var order = new Order
            {
                QuotationId = orderDTO.QuotationId,
                UserId = orderDTO.UserId,
                VehicleId = orderDTO.VehicleId,
                OrderDate = orderDTO.OrderDate ?? DateTime.UtcNow,
                Status = string.IsNullOrEmpty(orderDTO.Status) ? "Pending" : orderDTO.Status,
                TotalAmount = orderDTO.TotalAmount  
            };

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return new OrderDTO
            {
                OrderId = order.OrderId,
                QuotationId = order.QuotationId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
        }

        public async Task<QuotationDTO> CreateQuotationAsync(QuotationDTO quotationDTO)
        {
            if (quotationDTO == null)
                throw new ArgumentNullException(nameof(quotationDTO));

            var quotation = new Quotation
            {
                UserId = quotationDTO.UserId,
                VehicleId = quotationDTO.VehicleId,
                QuotationDate = quotationDTO.QuotationDate ?? DateTime.UtcNow,
                BasePrice = quotationDTO.BasePrice,
                Discount = quotationDTO.Discount,
                Status = string.IsNullOrEmpty(quotationDTO.Status) ? "Pending" : quotationDTO.Status
            };

            decimal discountValue = quotation.Discount ?? 0;
            quotation.FinalPrice = quotation.BasePrice - discountValue;

            await _unitOfWork.QuotationRepository.AddAsync(quotation);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = quotation.QuotationId,
                UserId = quotation.UserId,
                VehicleId = quotation.VehicleId,
                QuotationDate = quotation.QuotationDate,
                BasePrice = quotation.BasePrice,
                Discount = quotation.Discount,
                FinalPrice = quotation.FinalPrice,
                Status = quotation.Status
            };
        }

    }
}
