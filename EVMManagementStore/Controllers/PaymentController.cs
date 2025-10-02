using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using EVMManagementStore.Service.Service.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _paymentService.GetAllPaymentsAsync();
            return Ok(ApiResponse<List<PaymentDTO>>.OkResponse(appointments, "Lấy danh sách quản lí Thanh toán thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _paymentService.GetPaymentsByIdAsync(id);
            if (appointment == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thông tin sách quản lí thanh toán"));

            return Ok(ApiResponse<PaymentDTO>.OkResponse(appointment, "Lấy thông tin thanh toán thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _paymentService.CreatePaymentAsync(dto);
            return Ok(ApiResponse<PaymentDTO>.OkResponse(created, "Tạo thông tin thanh toán thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _paymentService.UpdatePaymentAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy thông tin thanh toán"));

            return Ok(ApiResponse<PaymentDTO>.OkResponse(updated, "Cập nhật thông tin thanh toán thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy thông tin thanh toán"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa thông tin thanh toán thành công"));
        }
    }
}
