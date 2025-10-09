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
    public class DebtReportService : IDebtReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DebtReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DebtReportDTO>> GetCustomerDebtReportAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync();
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var report = (from o in orders
                          join u in users on o.UserId equals u.UserId
                          join p in payments on o.OrderId equals p.OrderId into paymentGroup
                          select new DebtReportDTO
                          {
                              CustomerOrDealerName = u.FullName ?? u.CompanyName ?? "Unknown",
                              TotalOrderAmount = o.TotalAmount,
                              TotalPaid = paymentGroup.Sum(x => x.Amount)
                          })
                          .GroupBy(x => x.CustomerOrDealerName)
                          .Select(g => new DebtReportDTO
                          {
                              CustomerOrDealerName = g.Key,
                              TotalOrderAmount = g.Sum(x => x.TotalOrderAmount),
                              TotalPaid = g.Sum(x => x.TotalPaid)
                          })
                          .ToList();

            return report;
        }
        public async Task<List<DebtReportDTO>> GetDealerDebtReportAsync()
        {
            var dealerOrders = await _unitOfWork.DealerOrderRepository.GetAllAsync();
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            // Nếu có Payment riêng cho Dealer thì dùng thêm
            // var dealerPayments = await _unitOfWork.PaymentRepository.GetAllAsync();

            var report = (from d in dealerOrders
                          join u in users on d.UserId equals u.UserId
                          select new DebtReportDTO
                          {
                              CustomerOrDealerName = u.CompanyName ?? u.FullName ?? "Unknown",
                              TotalOrderAmount = d.TotalAmount,
                              TotalPaid = 0 // Nếu chưa có bảng Payment riêng cho Dealer
                          })
                          .GroupBy(x => x.CustomerOrDealerName)
                          .Select(g => new DebtReportDTO
                          {
                              CustomerOrDealerName = g.Key,
                              TotalOrderAmount = g.Sum(x => x.TotalOrderAmount),
                              TotalPaid = g.Sum(x => x.TotalPaid)
                          })
                          .ToList();

            return report;
        }

    }
}
