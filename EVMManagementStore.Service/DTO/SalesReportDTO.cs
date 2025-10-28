namespace EVMManagementStore.Service.DTO
{
    public class SalesReportDTO
    {
        public string CompanyName { get; set; }  
        public string Address { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? FirstOrderDate { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}
