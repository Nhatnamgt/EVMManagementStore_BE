using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class DebtReportDTO
    {
        public string CustomerOrDealerName { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal OutstandingDebt => TotalOrderAmount - TotalPaid;
    }

}
