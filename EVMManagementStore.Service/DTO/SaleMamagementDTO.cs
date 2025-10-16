using EVMManagementStore.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{
    public class QuotationDTO
    {
        public int QuotationId { get; set; }

        public int UserId { get; set; }

        public int VehicleId { get; set; }

        public DateTime? QuotationDate { get; set; }

        public decimal BasePrice { get; set; }

        public decimal? Discount { get; set; }

        public decimal FinalPrice { get; set; }
        public string AttachmentImage { get; set; }

        public string AttachmentFile { get; set; }

        public string Status { get; set; }

    }
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int? QuotationId { get; set; }

        public int UserId { get; set; }

        public int VehicleId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string Status { get; set; }
        public string AttachmentImage { get; set; }

        public string AttachmentFile { get; set; }

        public decimal TotalAmount { get; set; }
    }


    public class SalesContractDTO
    {
        public int SalesContractId { get; set; }

        public int OrderId { get; set; }

        public DateTime? ContractDate { get; set; }

        public string Terms { get; set; }

        public string SignedByDealer { get; set; }

        public string CustomerName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string PaymentMethod { get; set; }

        public string Address { get; set; }

        public string Cccd { get; set; }

        public string ContractImage { get; set; }
    }


    public class DealerOrderDTO
    {
        public int DealerOrderId { get; set; }

        public int UserId { get; set; }

        public int VehicleId { get; set; }

        public int Quantity { get; set; }

        public DateTime? OrderDate { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public decimal TotalAmount { get; set; }

    }
}