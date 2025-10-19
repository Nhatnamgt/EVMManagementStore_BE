using EVMManagementStore.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public partial class PromotionDTO
    {
        public int PromotionId { get; set; }

        public int UserId { get; set; }

        public string PromotionCode { get; set; }

        public string OptionName { get; set; }

        public decimal? OptionValue { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

    }
}
