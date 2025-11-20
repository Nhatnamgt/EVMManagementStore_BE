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
            // Lấy dữ liệu
            var users = await _unitOfWork.UserRepository.GetAllIncludeAsync(u => u.Role);
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();

            // Lọc theo thời gian
            if (fromDate.HasValue && toDate.HasValue)
            {
                orders = orders.Where(o =>
                    o.OrderDate.HasValue &&
                    o.OrderDate.Value.Date >= fromDate.Value.Date &&
                    o.OrderDate.Value.Date <= toDate.Value.Date).ToList();
            }

            // Lọc danh sách đại lý (dealer)
            var dealers = users.Where(u =>
                u.RoleId == 2 ||
                (u.Role != null && u.Role.RoleName.ToLower() == "dealer"))
                .ToList();

            // =============================
            //  REPORT THEO TỪNG DEALER
            // =============================
            var reports = dealers.Select(dealer =>
            {
                // Lấy đơn hàng của dealer
                var dealerOrders = orders.Where(o => o.UserId == dealer.UserId).ToList();

                // Join với Vehicle để lấy model/type
                var joined = from o in dealerOrders
                             join v in vehicles on o.VehicleId equals v.VehicleId
                             select new { o, v };

                return new SalesReportDTO
                {
                    CompanyName = dealer.CompanyName,
                    Address = dealer.Address,

                    TotalOrders = dealerOrders.Count,
                    TotalSales = dealerOrders.Sum(o => o.FinalPrice),

                    // Model bán chạy nhất
                    BestSellingModel = joined
                        .GroupBy(x => x.v.Model)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault(),

                    // Type bán chạy nhất
                    BestSellingType = joined
                        .GroupBy(x => x.v.Type)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault(),

                    // 🔥 Color bán chạy nhất (đúng màu thực tế trong Order)
                    BestSellingColor = dealerOrders
                        .GroupBy(o => o.Color)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault()
                };
            }).ToList();

            // =============================
            //  REPORT TỔNG HỢP TOÀN HỆ THỐNG
            // =============================
            var allOrders = orders.ToList();
            var allJoined = from o in allOrders
                            join v in vehicles on o.VehicleId equals v.VehicleId
                            select new { o, v };

            var totalReport = new SalesReportDTO
            {
                CompanyName = "Tổng hợp toàn hệ thống",
                Address = "Tất cả khu vực",

                TotalOrders = allOrders.Count,
                TotalSales = allOrders.Sum(o => o.FinalPrice),

                BestSellingModel = allJoined
                    .GroupBy(x => x.v.Model)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault(),

                BestSellingType = allJoined
                    .GroupBy(x => x.v.Type)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault(),

                // 🔥 Lấy màu thực tế từ ORDER của toàn hệ thống
                BestSellingColor = allOrders
                    .GroupBy(o => o.Color)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };

            reports.Add(totalReport);
            return reports;
        }
    }
}
