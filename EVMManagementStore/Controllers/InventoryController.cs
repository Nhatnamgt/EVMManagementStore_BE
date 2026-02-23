using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVMManagementStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "evm_staff")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _inventoryService.GetAllInventoriesAsync();
            if (result == null || !result.Any())
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có dữ liệu tồn kho."));

            return Ok(ApiResponse<List<InventoryDTO>>.OkResponse(result.ToList(),
                "Lấy danh sách tồn kho thành công."));
        }

        [HttpGet("{inventoryId}")]
        public async Task<IActionResult> GetById(int inventoryId)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(inventoryId);
            if (result == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy tồn kho với ID đã cho."));

            return Ok(ApiResponse<InventoryDTO>.OkResponse(result, "Lấy thông tin tồn kho thành công."));
        }

        [HttpPut("{inventoryId}")]
        public async Task<IActionResult> Update(int inventoryId, [FromBody] int quantity)
        {
            var updated = await _inventoryService.UpdateInventoryAsync(inventoryId, quantity);
            return Ok(ApiResponse<InventoryDTO>.OkResponse(updated, "Cập nhật số lượng tồn kho thành công."));
        }

        [HttpDelete("{inventoryId}")]
        public async Task<IActionResult> DeleteInventoryColor(int inventoryId)
        {
            var ok = await _inventoryService.DeleteInventoryAsync(inventoryId);
            if (!ok)
                return BadRequest(ApiResponse<string>.BadRequestResponse(
                    "Không thể xoá kho này. Vui lòng kiểm tra số lượng tồn kho."));

            return Ok(ApiResponse<string>.OkResponse(
                "Xóa màu khỏi tồn kho thành công và đã cập nhật lại màu xe."));
        }

        [HttpPost("dispatch")]
        public async Task<IActionResult> Dispatch([FromBody] DispatchRequest req)
        {
            await _inventoryService.DispatchVehicleAsync(req);

            return Ok(ApiResponse<string>.OkResponse("Điều phối xe thành công."));
        }
    }
}
