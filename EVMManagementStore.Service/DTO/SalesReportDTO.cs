namespace EVMManagementStore.Service.DTO
{
    public class SalesReportDTO
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }

        public string BestSellingModel { get; set; }
        public string BestSellingType { get; set; }
        public string BestSellingColor { get; set; }
    }
}
