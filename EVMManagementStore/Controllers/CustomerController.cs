using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(ApiResponse<List<CustomerDTO>>.OkResponse(customers, "Lấy danh sách khách hàng thành công"));
        }

        [HttpGet("GetCustomerById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomersByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy khách hàng"));

            return Ok(ApiResponse<CustomerDTO>.OkResponse(customer, "Lấy thông tin khách hàng thành công"));
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> Create([FromBody] CustomerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _customerService.CreateCustomersAsync(dto);
            return Ok(ApiResponse<CustomerDTO>.OkResponse(created, "Tạo khách hàng thành công"));
        }

        [HttpPut("UpdateCustomer/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _customerService.UpdateCustomersAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy khách hàng"));

            return Ok(ApiResponse<CustomerDTO>.OkResponse(updated, "Cập nhật khách hàng thành công"));
        }

        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _customerService.DeleteCustomersAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy khách hàng"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa khách hàng thành công"));
        }
    }
}
