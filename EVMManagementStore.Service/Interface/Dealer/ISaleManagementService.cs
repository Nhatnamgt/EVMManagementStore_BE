using EVMManagementStore.Service.Dealer;
using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface ISaleManagementService
    {
        Task<QuotationDTO> CreateQuotationAsync(QuotationDTO quotationDTO);
        Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO);
        Task<DealerOrderDTO> CreateDealerOrderAsync(DealerOrderDTO dealerorderDTO);
        Task<SalesContractDTO> CteateSaleContractAsync(SalesContractDTO salesContractDTO);   


    }
}
