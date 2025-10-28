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

        public async Task<IEnumerable<SalesReportDTO>> GetAllSalesReportsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {

            var users = await _unitOfWork.UserRepository.GetAllIncludeAsync(u => u.Role);
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();


            if (fromDate.HasValue && toDate.HasValue)
            {
                orders = orders.Where(o =>
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value.Date >= fromDate.Value.Date &&
                    o.OrderDate.Value.Date <= toDate.Value.Date).ToList();
            }


            var dealers = users.Where(u =>
                u.RoleId == 2 ||
                (u.Role != null && u.Role.RoleName.ToLower() == "dealer"))
                .ToList();


            var reports = dealers.Select(dealer =>
            {
                var dealerOrders = orders.Where(o => o.UserId == dealer.UserId).ToList();

                var joined = from o in dealerOrders
                             join v in vehicles on o.VehicleId equals v.VehicleId
                             select v;

                return new SalesReportDTO
                {
                    CompanyName = dealer.CompanyName,
                    Address = dealer.Address,
                    TotalOrders = dealerOrders.Count,
                    TotalSales = dealerOrders.Sum(o => o.FinalPrice),
                    BestSellingModel = joined.GroupBy(v => v.Model)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault(),
                    BestSellingType = joined.GroupBy(v => v.Type)
                                            .OrderByDescending(g => g.Count())
                                            .Select(g => g.Key)
                                            .FirstOrDefault(),
                    BestSellingColor = joined.GroupBy(v => v.Color)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault()
                };
            }).ToList();

            var allOrders = orders.ToList();
            var allVehicles = (from o in allOrders
                               join v in vehicles on o.VehicleId equals v.VehicleId
                               select v).ToList();

            var totalReport = new SalesReportDTO
            {
                CompanyName = "Tổng hợp toàn hệ thống",
                Address = "Tất cả khu vực",
                TotalOrders = allOrders.Count,
                TotalSales = allOrders.Sum(o => o.FinalPrice),
                BestSellingModel = allVehicles.GroupBy(v => v.Model)
                                              .OrderByDescending(g => g.Count())
                                              .Select(g => g.Key)
                                              .FirstOrDefault(),
                BestSellingType = allVehicles.GroupBy(v => v.Type)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault(),
                BestSellingColor = allVehicles.GroupBy(v => v.Color)
                                              .OrderByDescending(g => g.Count())
                                              .Select(g => g.Key)
                                              .FirstOrDefault()
            };

            reports.Add(totalReport);
            return reports;
        }
    }
}

