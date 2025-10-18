using EVMManagementStore.Models;
using EVMManagementStore.Repository.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using EVMManagementStore.Service.Service.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationController : ControllerBase
    {
        private readonly ISaleManagementService _saleManagement;
        public QuotationController(ISaleManagementService saleManagement)
        {
            _saleManagement = saleManagement;
        }

        [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> CreateQuotation([FromBody] QuotationDTO dto)
        {
            var quotations = await _saleManagement.CreateQuotationAsync(dto);
            return Ok(ApiResponse<QuotationDTO>.OkResponse(quotations, "Tạo báo giá thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetQuotation()
        {
            var quotations = await _saleManagement.GetAllQuotationsAsync();
            if (quotations == null || !quotations.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có báo giá trong danh sách"));
            }
            return Ok(ApiResponse<List<QuotationDTO>>.OkResponse(quotations.ToList(), "Lấy danh sách báo giá thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuotationById(int id)
        {
            var quotations = await _saleManagement.GetQuotationByIdAsync(id);
            if (quotations == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy báo giá với ID đã cho"));
            }
            return Ok(ApiResponse<QuotationDTO>.OkResponse(quotations, "Lấy thông tin báo giá thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotation(int id, [FromBody] QuotationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var quotation = await _saleManagement.UpdateQuotationAsync(id, dto);
            if (quotation == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy báo giá"));

            return Ok(ApiResponse<QuotationDTO>.OkResponse(quotation, "Cập nhật báo giá thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(int id, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var upload = await _saleManagement.UploadFiles(id, attachmentFile, attachmentImage);
            return Ok(ApiResponse<QuotationDTO>.OkResponse(upload, "Upload báo giá thành công"));
        }


        [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotation(int id)
        {
            var success = await _saleManagement.DeleteQuotationAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy báo giá"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa báo giá thành công"));
        }      
    }
}
