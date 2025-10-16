using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealerOrderController : Controller
    {
        private readonly ISaleManagementService _saleManagement;
        public DealerOrderController(ISaleManagementService saleManagement)
        {
            _saleManagement = saleManagement;
        }

        [Authorize(Roles = "dealer")]
        [HttpPost("CreateDealerOrder")]
        public async Task<IActionResult> CreateDealerOrder([FromBody] DealerOrderDTO dto)
        {
            var create = await _saleManagement.CreateDealerOrderAsync(dto);
            return Ok(ApiResponse<DealerOrderDTO>.OkResponse(create, "Đặt xe từ hãng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GeDealerOrder()
        {
            var dealerorder = await _saleManagement.GetAllDealerOrdersAsync();
            if (dealerorder == null || !dealerorder.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có Đặt xe từ hãng trong danh sách"));
            }
            return Ok(ApiResponse<List<DealerOrderDTO>>.OkResponse(dealerorder.ToList(), "Lấy danh sách Đặt xe từ hãng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDealerOrderById(int id)
        {
            var dealerorder = await _saleManagement.GetDealerOrderByIdAsync(id);
            if (dealerorder == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Đặt xe từ hãng với ID đã cho"));
            }
            return Ok(ApiResponse<DealerOrderDTO>.OkResponse(dealerorder, "Lấy thông tin Đặt xe từ hãng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDealerOrder(int id, [FromBody] DealerOrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var dealerorder = await _saleManagement.UpdateDealerOrderAsync(id, dto);
            if (dealerorder == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Đặt xe từ hãng"));

            return Ok(ApiResponse<DealerOrderDTO>.OkResponse(dealerorder, "Cập nhật Đặt xe từ hãng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDealerOrder(int id)
        {
            var success = await _saleManagement.DeleteDealerOrderAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Đặt xe từ hãng"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa Đặt xe từ hãng thành công"));
        }
    }
}
