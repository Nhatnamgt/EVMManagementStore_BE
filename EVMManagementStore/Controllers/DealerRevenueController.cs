using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealerRevenueController : ControllerBase
    {
        private readonly IDealerRevenueService _dealerRevenueService;

        public DealerRevenueController(IDealerRevenueService dealerRevenueService)
        {
            _dealerRevenueService = dealerRevenueService;
        }

              [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetDealerRevenue()
        {
            var revenue = await _dealerRevenueService.GetAllDealersRevenueAsync();
            if (revenue == null || !revenue.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có báo cáo doanh thu trong danh sách"));
            }
            return Ok(ApiResponse<List<RevenueDTO>>.OkResponse(revenue.ToList(), "Lấy báo cáo doanh thu thành công"));
        }
        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDealerRevenueById(int id)
        {
            var revenue = await _dealerRevenueService.GetRevenueByDealerAsync(id);
            if (revenue == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm báo cáo doanh thu ID đã cho"));
            }
            return Ok(ApiResponse<RevenueDTO>.OkResponse(revenue, "Lấy báo cáo doanh thu thành công"));
        }
    }
}
