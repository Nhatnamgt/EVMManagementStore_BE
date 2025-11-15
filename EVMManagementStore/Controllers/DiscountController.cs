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
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        // GET All
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _discountService.GetAllAsync();
            return Ok(result);
        }

        // GET By id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _discountService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DiscountDTO dto)
        {
            var result = await _discountService.CreateAsync(dto);
            return Ok(result);
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DiscountDTO dto)
        {
            var result = await _discountService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _discountService.DeleteAsync(id);
            return result ? Ok("Deleted successfully") : NotFound();
        }

        // ✅ Áp dụng giảm giá lên xe
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyDiscount(int vehicleId, int discountId)
        {
            var result = await _discountService.ApplyDiscountToVehicleAsync(vehicleId, discountId);
            return result ? Ok("Discount applied successfully") : BadRequest("Failed to apply discount");
        }

        // ❌ Gỡ giảm giá khỏi xe
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveDiscount(int vehicleId)
        {
            var result = await _discountService.RemoveDiscountFromVehicleAsync(vehicleId);
            return result ? Ok("Discount removed successfully") : BadRequest("Failed to remove discount");
        }
    }
}
