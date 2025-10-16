using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryController : ControllerBase
    {       
            private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

            [Authorize(Roles = "dealer")]
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var appointments = await _deliveryService.GetAllDeliveriesAsync();
                return Ok(ApiResponse<List<DeliveryDTO>>.OkResponse(appointments, "Lấy danh sách Theo dõi tình trạng giao xe thành công"));
            }

            [Authorize(Roles = "dealer")]
            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var appointment = await _deliveryService.GetDeliveryByIdAsync(id);
                if (appointment == null)
                    return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Theo dõi tình trạng giao xe"));

                return Ok(ApiResponse<DeliveryDTO>.OkResponse(appointment, "Lấy Theo dõi tình trạng giao xe thành công"));
            }

            [Authorize(Roles = "dealer")]
            [HttpPost]
            public async Task<IActionResult> Create([FromBody] DeliveryDTO dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

                var created = await _deliveryService.CreateDeliveryAsync(dto);
                return Ok(ApiResponse<DeliveryDTO>.OkResponse(created, "Tạo Theo dõi tình trạng giao xe thành công"));
            }

            [Authorize(Roles = "dealer")]
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] DeliveryDTO dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

                var updated = await _deliveryService.UpdateDeliveryAsync(id, dto);
                if (updated == null)
                    return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Theo dõi tình trạng giao xe"));

                return Ok(ApiResponse<DeliveryDTO>.OkResponse(updated, "Cập nhật Theo dõi tình trạng giao xe thành công"));
            }

            [Authorize(Roles = "dealer")]
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var success = await _deliveryService.DeleteDeliveryAsync(id);
                if (!success)
                    return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy Theo dõi tình trạng giao xe"));

                return Ok(ApiResponse<string>.OkResponse(null, "Xóa Theo dõi tình trạng giao xe thành công"));
            }
        }
    }
