using System;
using System.Collections.Generic;

namespace EVMManagementStore.Repository.Models
{
    public partial class Discount
    {
        public int DiscountId { get; set; }

        public int UserId { get; set; }

        public string DiscountCode { get; set; }

        public string DiscountName { get; set; }

        public string DiscountType { get; set; }   

        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } 

        public virtual User User { get; set; }

        // Nếu Vehicle có cột discount_id → liên kết 1 discount nhiều xe
        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}