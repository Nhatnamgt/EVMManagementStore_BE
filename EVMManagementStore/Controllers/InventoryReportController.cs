using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryReportController : ControllerBase
    {
        private readonly IInventoryReportService _inventoryReportService;

        public InventoryReportController(IInventoryReportService inventoryReportService)
        {
            _inventoryReportService = inventoryReportService;
        }

        // ✅ Báo cáo dispatch trong khoảng thời gian
        [Authorize(Roles = "evm_staff")]
        [HttpGet("dispatch-report")]
        public async Task<IActionResult> GetDispatchReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = await _inventoryReportService.GetDispatchReportAsync(fromDate, toDate);
            if (result == null || !result.Any())
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có dữ liệu dispatch trong khoảng thời gian này"));

            return Ok(ApiResponse<List<InventoryReportDTO>>.OkResponse(result.ToList(), "Lấy báo cáo dispatch thành công"));
        }
    }
}
