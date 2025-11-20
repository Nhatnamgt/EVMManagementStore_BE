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
    public class InventoryReportService : IInventoryReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

 
        public async Task<IEnumerable<InventoryReportDTO>> GetDispatchReportAsync(DateTime fromDate, DateTime toDate)
        {
            var inventories = await _unitOfWork.InventoryRepository.GetAllAsync();
            var vehicles = await _unitOfWork.VehicleRepository.GetAllAsync();
            var dispatches = await _unitOfWork.DealerOrderRepository.GetAllAsync();
            var dealers = await _unitOfWork.UserRepository.GetAllAsync();

            int totalDays = (toDate - fromDate).Days;
            if (totalDays <= 0) totalDays = 1;


            var report = (from d in dispatches
                          join v in vehicles on d.VehicleId equals v.VehicleId
                          join inv in inventories on new { d.VehicleId, d.Color }
                                                   equals new { inv.VehicleId, inv.Color }
                          join u in dealers on d.UserId equals u.UserId
                          where d.OrderDate >= fromDate && d.OrderDate <= toDate
                          group new { d, v, inv, u } by new
                          {
                              d.UserId,
                              u.CompanyName,
                              v.VehicleId,
                              v.Type,
                              v.Model,
                              v.Version,
                              inv.Color  
                          }
                          into g
                          let dispatched = g.Sum(x => x.d.Quantity)
                          let remaining = g.FirstOrDefault()?.inv?.Quantity ?? 0
                          select new InventoryReportDTO
                          {
                              DealerId = g.Key.UserId,
                              CompanyName = g.Key.CompanyName,

                              VehicleId = g.Key.VehicleId,
                              Type = g.Key.Type,
                              Model = g.Key.Model,
                              Version = g.Key.Version,
                              Color = g.Key.Color,  

                              DispatchedQuantity = dispatched,
                              RemainingInStock = remaining,
                              ConsumptionRate = Math.Round((decimal)dispatched / totalDays, 2),

                              Status = remaining > 0 ? "Còn hàng" : "Hết hàng"
                          }).ToList();

            return report;
        }
    }
}
