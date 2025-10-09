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

            var groupedRevenue = orders
                .GroupBy(o => new { o.UserId, o.User.Username })
                .Select(g => new RevenueDTO
                {
                    SalespersonName = g.Key.Username,
                    TotalOrders = g.Count(),
                    TotalSales = g.Sum(o => o.TotalAmount),
                    FirstOrderDate = g.Min(o => o.OrderDate ?? DateTime.MinValue),
                    LastOrderDate = g.Max(o => o.OrderDate ?? DateTime.MinValue)
                })
                .ToList();

            return groupedRevenue;
        }

        public async Task<RevenueDTO> GetRevenueByDealerAsync(int dealerId)
        {
            var orders = await _unitOfWork.OrderRepository.FindIncludeAsync(o => o.UserId == dealerId, o => o.User);

            if (orders == null || !orders.Any())
                return null;

            var dealer = orders.First().User;

            return new RevenueDTO
            {
                SalespersonName = dealer.Username,
                TotalOrders = orders.Count(),
                TotalSales = orders.Sum(o => o.TotalAmount),
                FirstOrderDate = orders.Min(o => o.OrderDate ?? DateTime.MinValue),
                LastOrderDate = orders.Max(o => o.OrderDate ?? DateTime.MinValue)
            };
        }
    }
}
