using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDriveAppointmentController : ControllerBase
    {
        private readonly ITestDriveAppointmentService _testDriveAppointmentService; 

        public TestDriveAppointmentController(ITestDriveAppointmentService testDriveAppointmentService)
        {
            _testDriveAppointmentService = testDriveAppointmentService; 
        }

   //     [Authorize(Roles = "dealer")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _testDriveAppointmentService.GetTestDriveAppointmenAsync();
            return Ok(ApiResponse<List<TestDriveAppointmentDTO>>.OkResponse(appointments, "Lấy danh sách lịch hẹn thành công"));
        }

   //     [Authorize(Roles = "dealer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _testDriveAppointmentService.GetTestDriveAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy thông tin lịch hẹn"));

            return Ok(ApiResponse<TestDriveAppointmentDTO>.OkResponse(appointment, "Lấy thông tin lịch hẹn thành công"));
        }

   //     [Authorize(Roles = "dealer")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TestDriveAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var created = await _testDriveAppointmentService.CreateTestDriveAppointmentAsync(dto);
            return Ok(ApiResponse<TestDriveAppointmentDTO>.OkResponse(created, "Tạo lịch hẹn thành công"));
        }

     //   [Authorize(Roles = "dealer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TestDriveAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.BadRequestResponse("Dữ liệu không hợp lệ"));

            var updated = await _testDriveAppointmentService.UpdateTestDriveAppointmentAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy lịch hẹn"));

            return Ok(ApiResponse<TestDriveAppointmentDTO>.OkResponse(updated, "Cập nhật lịch hẹn thành công"));
        }

     //   [Authorize(Roles = "dealer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _testDriveAppointmentService.DeleteTestDriveAppointmentAsync(id);
            if (!success)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy lịch hẹn"));

            return Ok(ApiResponse<string>.OkResponse(null, "Xóa lịch hẹn thành công"));
        }
    }
}