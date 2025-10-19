using EVMManagementStore.Service.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface ISaleManagementService
    {
        // Quotaion
        Task<QuotationDTO> UploadFiles(int quotationId, IFormFile attachmentFile, IFormFile attachmentImage);
        Task<List<QuotationDTO>> GetAllQuotationsAsync();
        Task<QuotationDTO> GetQuotationByIdAsync(int id);
        Task<QuotationDTO> CreateQuotationAsync(QuotationDTO quotationDTO);
        Task<QuotationDTO> UpdateQuotationAsync(int id, QuotationDTO dto);
        Task<bool> DeleteQuotationAsync(int id);

        // Order
        Task<OrderDTO> UploadFilesOrder(int orderid, IFormFile attachmentFile, IFormFile attachmentImage);
        Task<List<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO dto);
        Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO);
        Task<bool> DeleteOrderAsync(int id);

        // DealerOrder
        Task<List<DealerOrderDTO>> GetAllDealerOrdersAsync();
        Task<DealerOrderDTO> GetDealerOrderByIdAsync(int id);
        Task<DealerOrderDTO> CreateDealerOrderAsync(DealerOrderDTO dealerorderDTO);
        Task<DealerOrderDTO> UpdateDealerOrderAsync(int id, DealerOrderDTO dto);
        Task<bool> DeleteDealerOrderAsync(int id);

        // SaleContract
        Task<SalesContractDTO> UploadFilesSaleContract(int salecontractid, IFormFile attachmentFile, IFormFile attachmentImage);
        Task<List<SalesContractDTO>> GetAllSaleContractsAsync();
        Task<SalesContractDTO> GetSaleContractByIdAsync(int id);
        Task<SalesContractDTO> CteateSaleContractAsync(SalesContractDTO salesContractDTO);
        Task<SalesContractDTO> UpdateSaleContractAsync(int id, SalesContractDTO dto);
        Task<bool> DeleteSaleContractAsync(int id);
    }

}
