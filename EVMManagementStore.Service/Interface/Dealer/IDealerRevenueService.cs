using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface IDealerRevenueService
    {
        Task<List<RevenueDTO>> GetAllDealersRevenueAsync();
        Task<RevenueDTO> GetRevenueByDealerAsync(int dealerId);
    }
}
