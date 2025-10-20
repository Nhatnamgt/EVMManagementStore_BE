using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [Authorize(Roles = "admin,evm_staff")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();

            if (inventories == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có xe nào trong kho."));

            return Ok(ApiResponse<IEnumerable<InventoryDTO>>.OkResponse(inventories, "Lấy danh sách tồn kho thành công."));
        }

        [Authorize(Roles = "admin,evm_staff")]
        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetByVehicleId(int vehicleId)
        {
            var inventory = await _inventoryService.GetInventoryByVehicleIdAsync(vehicleId);
            if (inventory == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe trong kho."));

            return Ok(ApiResponse<InventoryDTO>.OkResponse(inventory, "Lấy thông tin tồn kho thành công."));
        }

        [Authorize(Roles = "admin,evm_staff")]
        [HttpPut("{vehicleId}/update")]
        public async Task<IActionResult> UpdateQuantity(int vehicleId, [FromBody] int quantity)
        {
            if (quantity < 0)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Số lượng không hợp lệ."));

            var updated = await _inventoryService.UpdateInventoryAsync(vehicleId, quantity);
            return Ok(ApiResponse<InventoryDTO>.OkResponse(updated, "Cập nhật số lượng tồn kho thành công."));
        }

        [Authorize(Roles = "admin,evm_staff")]
        [HttpPost("dispatch")]
        public async Task<IActionResult> Dispatch([FromBody] DispatchRequest request)
        {
            if (request.Quantity <= 0)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Số lượng điều phối phải lớn hơn 0."));

            var success = await _inventoryService.DispatchVehicleAsync(request);
            if (!success)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Không thể điều phối xe."));

            return Ok(ApiResponse<string>.OkResponse("Điều phối xe thành công."));
        }
    }
}
