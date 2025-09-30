using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using EVMManagementStore.Service.Service.Dealer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleManagementController : ControllerBase
    {
        private readonly ISaleManagementService _saleManagement;
        public SaleManagementController(ISaleManagementService saleManagement)
        {
            _saleManagement = saleManagement;
        }

        //      [Authorize(Roles = "dealer")]
        [HttpPost("CreateQuotation")]
        public async Task<IActionResult> CreateQuotation([FromBody] QuotationDTO dto)
        {
            var create = await _saleManagement.CreateQuotationAsync(dto);
            return Ok(ApiResponse<QuotationDTO>.OkResponse(create, "Lấy danh sách phản hồi thành công"));
        }

        //      [Authorize(Roles = "dealer")]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO dto)
        {
            var create = await _saleManagement.CreateOrderAsync(dto);
            return Ok(ApiResponse<OrderDTO>.OkResponse(create, "Lấy danh sách phản hồi thành công"));
        }

        //      [Authorize(Roles = "dealer")]
        [HttpPost("CreateDealerOrder")]
        public async Task<IActionResult> CreateDealerOrder([FromBody] DealerOrderDTO dto)
        {
            var create = await _saleManagement.CreateDealerOrderAsync(dto);
            return Ok(ApiResponse<DealerOrderDTO>.OkResponse(create, "Lấy danh sách phản hồi thành công"));
        }

    }
}
