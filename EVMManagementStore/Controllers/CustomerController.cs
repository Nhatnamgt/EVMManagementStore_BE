using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(ApiResponse<List<CustomerDTO>>.OkResponse(customers, "Lấy danh sách khách hàng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomersByIdAsync(id);
            if (customer == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy khách hàng"));

            return Ok(ApiResponse<CustomerDTO>.OkResponse(customer, "Lấy thông tin khách hàng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _customerService.CreateCustomersAsync(dto);
            return Ok(ApiResponse<CustomerDTO>.OkResponse(created, "Tạo khách hàng thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _customerService.UpdateCustomersAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy khách hàng"));

            return Ok(ApiResponse<CustomerDTO>.OkResponse(updated, "Cập nhật khách hàng thành công"));
        }
    }
}
