namespace EVMManagementStore.Service.DTO
{
    public class DiscountDTO
    {
        public int DiscountId { get; set; }
        public int UserId { get; set; }
        public string DiscountCode { get; set; }
        public string DiscountName { get; set; }
        public string DiscountType { get; set; } // amount | percent
        public decimal DiscountValue { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } // ACTIVE | EXPIRED
    }
}