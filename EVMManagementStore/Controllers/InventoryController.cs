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
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllInventoriesAsync();
            return Ok(result);
        }

        [HttpGet("{inventoryId}")]
        public async Task<IActionResult> GetById(int inventoryId)
        {
            var result = await _service.GetInventoryByIdAsync(inventoryId);
            if (result == null)
                return NotFound("Không tìm thấy inventory.");

            return Ok(result);
        }

        [HttpPut("{inventoryId}")]
        public async Task<IActionResult> Update(int inventoryId, [FromBody] int quantity)
        {
            var result = await _service.UpdateInventoryAsync(inventoryId, quantity);
            return Ok(result);
        }

        [HttpPost("dispatch")]
        public async Task<IActionResult> Dispatch([FromBody] DispatchRequest req)
        {
            await _service.DispatchVehicleAsync(req);
            return Ok(new { message = "Điều phối thành công." });
        }
    }
}
