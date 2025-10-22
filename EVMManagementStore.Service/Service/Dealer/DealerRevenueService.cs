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
    public class DealerRevenueService : IDealerRevenueService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DealerRevenueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RevenueDTO>> GetAllDealersRevenueAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllIncludeAsync(o => o.User);

            var approvedOrders = orders
                .Where(o => o.Status != null && o.Status.ToUpper() == "APPROVED");

            var groupedRevenue = approvedOrders
                .GroupBy(o => new { o.UserId, o.User.Username })
                .Select(g => new RevenueDTO
                {
                    SalespersonName = g.Key.Username,
                    TotalOrders = g.Count(),
                    TotalSales = g.Sum(o => o.FinalPrice),
                    FirstOrderDate = g.Min(o => o.OrderDate ?? DateTime.MinValue),
                    LastOrderDate = g.Max(o => o.OrderDate ?? DateTime.MinValue)
                })
                .ToList();

            return groupedRevenue;
        }

        public async Task<RevenueDTO> GetRevenueByDealerAsync(int dealerId)
        {
            var approvedOrders = await _unitOfWork.OrderRepository.FindIncludeAsync(
                o => o.UserId == dealerId && o.Status != null && o.Status.ToUpper() == "APPROVED",
                o => o.User
            );

            if (approvedOrders == null || !approvedOrders.Any()) return null;

            var dealer = approvedOrders.First().User;

            return new RevenueDTO
            {
                SalespersonName = dealer.Username,
                TotalOrders = approvedOrders.Count(),
                TotalSales = approvedOrders.Sum(o => o.FinalPrice),
                FirstOrderDate = approvedOrders.Min(o => o.OrderDate ?? DateTime.MinValue),
                LastOrderDate = approvedOrders.Max(o => o.OrderDate ?? DateTime.MinValue)
            };
        }
    }

}
