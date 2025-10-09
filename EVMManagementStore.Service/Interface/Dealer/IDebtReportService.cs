using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IDebtReportService
    {
        Task<List<DebtReportDTO>>GetCustomerDebtReportAsync();
        Task<List<DebtReportDTO>> GetDealerDebtReportAsync();
    }
}
