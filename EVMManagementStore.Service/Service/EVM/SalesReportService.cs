using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.EVM
{
    public class SalesReportService : ISalesReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SalesReportDTO>> GetAllSalesReportsAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            var dealers = users.Where(u =>
                u.RoleId == 2 ||
                (u.Role != null && u.Role.RoleName.ToLower() == "dealer"))
                .ToList();

            var reports = dealers.Select(dealer =>
            {
                var dealerOrders = orders.Where(o => o.UserId == dealer.UserId).ToList();

                return new SalesReportDTO
                {
                    CompanyName = dealer.CompanyName,
                    Address = dealer.Address,
                    TotalOrders = dealerOrders.Count,
                    TotalSales = dealerOrders.Sum(o => o.FinalPrice),
                    AverageOrderValue = dealerOrders.Any() ? dealerOrders.Average(o => o.FinalPrice) : 0,
                    FirstOrderDate = dealerOrders.Any() ? dealerOrders.Min(o => (DateTime?)o.OrderDate) : null,
                    LastOrderDate = dealerOrders.Any() ? dealerOrders.Max(o => (DateTime?)o.OrderDate) : null
                };
            }).ToList();

            return reports;
        }
    }
}
