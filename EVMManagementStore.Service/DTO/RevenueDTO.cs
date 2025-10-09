using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class RevenueDTO
    {
        public string SalespersonName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public DateTime? FirstOrderDate { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

}
