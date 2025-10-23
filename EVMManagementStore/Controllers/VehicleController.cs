using Azure;
using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IEVMVehicleService _evmVehicleService;

        public VehicleController(IVehicleService vehicleService, IEVMVehicleService evmVehicleService)
        {
            _vehicleService = vehicleService;
            _evmVehicleService = evmVehicleService;
        }
        [Authorize(Roles = "dealer,evm_staff,admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehicle();
            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có xe trong danh sách"));
            }
            return Ok(ApiResponse<List<VehicleDTO>>.OkResponse(vehicles.ToList(), "Lấy danh sách xe thành công"));
        }
        [Authorize(Roles = "dealer,evm_staff,admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicles = await _vehicleService.GetVehicleById(id);
            if (vehicles == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe với ID đã cho"));
            }
            return Ok(ApiResponse<VehicleDTO>.OkResponse(vehicles, "Lấy thông tin xe thành công"));
        }

        [Authorize(Roles = "dealer")]
        [HttpGet("CompareCar")]
        public async Task<IActionResult> CompareVehicles(int vehicleId1, int vehicleId2)
        {
            var comparison = await _vehicleService.CompareVehicles(vehicleId1, vehicleId2);
            if (comparison == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe để so sánh"));
            }
            return Ok(ApiResponse<VehicleComparisonDTO>.OkResponse(comparison, "So sánh xe thành công"));
        }

        [Authorize(Roles = "dealer,evm_staff")]
        [HttpGet("Sreach")]
        public async Task<IActionResult> SearchVehicle(string search)
        {
            var searchs = await _vehicleService.SearchVehicle(search);
            if (searchs == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe"));
            }
            return Ok(ApiResponse<List<VehicleDTO>>.OkResponse(searchs, "search xe thành công"));
        }

        [Authorize(Roles = "evm_staff, admin")]
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleDTO dto)
        {
            var created = await _evmVehicleService.CreateVehicleAsync(dto);
            return Ok(ApiResponse<VehicleDTO>.OkResponse(created, "Thêm xe mới thành công."));
        }

        [Authorize(Roles = "evm_staff, admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] VehicleDTO dto)
        {
            var updated = await _evmVehicleService.UpdateVehicleAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe cần cập nhật."));

            return Ok(ApiResponse<VehicleDTO>.OkResponse(updated, "Cập nhật xe thành công."));
        }

        [Authorize(Roles = "evm_staff, admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var deleted = await _evmVehicleService.DeleteVehicleAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe cần xóa."));

            return Ok(ApiResponse<string>.OkResponse("Xóa xe thành công."));
        }
    }
}
