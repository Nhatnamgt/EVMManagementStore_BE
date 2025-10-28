using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IInventoryReportService
    {
        Task<IEnumerable<InventoryReportDTO>> GetDispatchReportAsync(DateTime fromDate, DateTime toDate);
    }
}
