using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface ISalesReportService
    {
        Task<IEnumerable<SalesReportDTO>> GetAllSalesReportsAsync();
    }
}
