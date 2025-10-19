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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<PaymentDTO>> GetAllPaymentsAsync()
        {
            var payments = await _unitOfWork.PaymentRepository.GetAllIncludeAsync(p => p.Order);

            return payments.Select(p => new PaymentDTO
            {
                PaymentId = p.PaymentId,
                OrderId = p.OrderId,
                PaymentDate = p.PaymentDate,
                Amount = p.Order != null ? p.Order.TotalAmount : 0,
                Method = p.Method,
                Status = p.Status
            }).ToList();
        }
        public async Task<PaymentDTO> GetPaymentsByIdAsync(int paymentid)
        {
            var payment = (await _unitOfWork.PaymentRepository.FindIncludeAsync(p => p.PaymentId == paymentid, p => p.Order)).FirstOrDefault();

            if (payment == null) return null;

            return new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Order != null ? payment.Order.TotalAmount : 0,
                Method = payment.Method,
                Status = payment.Status
            };
        }
        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(paymentDto.OrderId);
            if (order == null) return null;

            var payment = new Payment
            {
                OrderId = paymentDto.OrderId,
                PaymentDate = paymentDto.PaymentDate,
                Amount = order.TotalAmount,
                Method = paymentDto.Method,
                Status = paymentDto.Status
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            paymentDto.PaymentId = payment.PaymentId;
           
            return new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status
            };
        }
        public async Task<PaymentDTO> UpdatePaymentAsync(int id, PaymentDTO paymentDto)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null) return null;

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(paymentDto.OrderId);
            if (order == null)
                throw new KeyNotFoundException($"OrderId {paymentDto.OrderId} not found.");

            payment.OrderId = paymentDto.OrderId;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.Method = paymentDto.Method;
            payment.Status = paymentDto.Status;

            await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveAsync();

            return new PaymentDTO
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentDate = payment.PaymentDate,
                Method = payment.Method,
                Status = payment.Status
            };
        }
        public async Task<bool> DeletePaymentAsync(int paymentid)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentid);
            if (payment == null) return false;

            await _unitOfWork.PaymentRepository.RemoveAsync(payment);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
