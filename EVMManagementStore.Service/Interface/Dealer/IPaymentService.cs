using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IPaymentService
    {
        Task<List<PaymentDTO>> GetAllPaymentsAsync();
        Task<List<PaymentDTO>> GetPaymentsByIdAsync(int paymentid);
        Task<PaymentDTO> CreatePaymentAsync(PaymentDTO paymentDto);
        Task<PaymentDTO> UpdatePaymentAsync(PaymentDTO paymentDTO);
        Task<bool> DeletePaymentAsync(int paymentid);
    }
}
