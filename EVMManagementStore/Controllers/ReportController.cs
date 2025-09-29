using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        //      [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _reportService.GetReportAsync();
            return Ok(ApiResponse<List<GetReportDTO>>.OkResponse(reports, "Lấy danh sách phản hồi thành công"));
        }

        //     [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy thông tin phản hồi"));

            return Ok(ApiResponse<GetReportDTO>.OkResponse(report, "Lấy thông tin phản hồi thành công"));
        }

        //     [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReportDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _reportService.CreateReportAsync(dto);
            return Ok(ApiResponse<ReportDTO>.OkResponse(created, "Tạo phản hồi thành công"));
        }

        //   [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReportDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _reportService.UpdateReportAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy phản hồi"));

            return Ok(ApiResponse<ReportDTO>.OkResponse(updated, "Cập nhật phản hồi thành công"));
        }

        //     [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _reportService.DeleteReportAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy phản hồi"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa phản hồi thành công"));
        }

    }
}
