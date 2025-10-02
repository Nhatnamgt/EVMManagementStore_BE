using EVMManagementStore.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public string Method { get; set; }

        public string Status { get; set; }

    }
}