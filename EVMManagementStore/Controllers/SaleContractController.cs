using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleContractController : Controller
    {
        private readonly ISaleManagementService _saleManagement;
        public SaleContractController(ISaleManagementService saleManagement)
        {
            _saleManagement = saleManagement;
        }

    //    [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> CteateSaleContract([FromBody] SalesContractDTO dto)
        {
            var salecontract = await _saleManagement.CteateSaleContractAsync(dto);
            return Ok(ApiResponse<SalesContractDTO>.OkResponse(salecontract, "Tạo hợp đồng bán hàng thành công"));
        }

    //    [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetVehicle()
        {
            var salecontract = await _saleManagement.GetAllSaleContractsAsync();
            if (salecontract == null || !salecontract.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có hợp đồng bán hàng trong danh sách"));
            }
            return Ok(ApiResponse<List<SalesContractDTO>>.OkResponse(salecontract.ToList(), "Lấy danh sách hợp đồng bán hàng thành công"));
        }

    //    [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleContractById(int id)
        {
            var salecontract = await _saleManagement.GetSaleContractByIdAsync(id);
            if (salecontract == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy hợp đồng bán hàng với ID đã cho"));
            }
            return Ok(ApiResponse<SalesContractDTO>.OkResponse(salecontract, "Lấy thông tin hợp đồng bán hàng thành công"));
        }

  //      [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleContract(int id, [FromBody] SalesContractDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var salecontract = await _saleManagement.UpdateSaleContractAsync(id, dto);
            if (salecontract == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy hợp đồng bán hàng"));

            return Ok(ApiResponse<SalesContractDTO>.OkResponse(salecontract, "Cập nhật hợp đồng bán hàng thành công"));
        }

     //   [Authorize(Roles = "dealer")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(int id, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var salecontractupload = await _saleManagement.UploadFilesSaleContract(id, attachmentFile, attachmentImage);
            return Ok(ApiResponse<SalesContractDTO>.OkResponse(salecontractupload, "Upload hợp đồng thành công"));
        }

     //   [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleContract(int id)
        {
            var success = await _saleManagement.DeleteSaleContractAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy hợp đồng bán hàng"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa hợp đồng bán hàng thành công"));
        }
    }
}
