using EVMManagementStore.Models;
using EVMManagementStore.Repository.Models;
using EVMManagementStore.Service.Dealer;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetVehicle()
        {
            var promotion = await _promotionService.GetAllPromotionsAsync();
            if (promotion == null || !promotion.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có promotion trong danh sách"));
            }
            return Ok(ApiResponse<List<PromotionDTO>>.OkResponse(promotion.ToList(), "Lấy danh sách promotion thành công"));
        }
        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy promotion với ID đã cho"));
            }
            return Ok(ApiResponse<PromotionDTO>.OkResponse(promotion, "Lấy thông tin promotion thành công"));
        }
        [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _promotionService.CreatePromotionAsync(dto);
            return Ok(ApiResponse<PromotionDTO>.OkResponse(created, "Tạo promotion thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _promotionService.UpdatePromotionAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy promotion"));

            return Ok(ApiResponse<PromotionDTO>.OkResponse(updated, "Cập nhật promotion thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _promotionService.DeletePromotionAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy promotion"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa promotion thành công"));
        }

    }

}
