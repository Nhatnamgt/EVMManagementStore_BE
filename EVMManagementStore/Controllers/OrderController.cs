using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly ISaleManagementService _saleManagement;
        public OrderController(ISaleManagementService saleManagement)
        {
            _saleManagement = saleManagement;
        }

   //     [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var order = await _saleManagement.GetAllOrdersAsync();
            if (order == null || !order.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có đơn hàng trong danh sách"));
            }
            return Ok(ApiResponse<List<OrderDTO>>.OkResponse(order.ToList(), "Lấy danh sách đơn hàng thành công"));
        }

    //    [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _saleManagement.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy đơn hàng với ID đã cho"));
            }
            return Ok(ApiResponse<OrderDTO>.OkResponse(order, "Lấy thông tin đơn hàng thành công"));
        }

    //    [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var order = await _saleManagement.UpdateOrderAsync(id, dto);
            if (order == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy đơn hàng"));

            return Ok(ApiResponse<OrderDTO>.OkResponse(order, "Cập nhật đơn hàng thành công"));
        }

        //     [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDTO dto)
        {
            var order = await _saleManagement.CreateOrderAsync(dto);
            return Ok(ApiResponse<OrderDTO>.OkResponse(order, "Tạo đơn hàng thành công"));
        }

        //       [Authorize(Roles = "dealer")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(int id, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var orderupload = await _saleManagement.UploadFilesOrder(id, attachmentFile, attachmentImage);
            return Ok(ApiResponse<OrderDTO>.OkResponse(orderupload, "Upload đơn hàng thành công"));
        }

  //      [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _saleManagement.DeleteOrderAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy đơn hàng"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa đơn hàng thành công"));
        }
    }
}
