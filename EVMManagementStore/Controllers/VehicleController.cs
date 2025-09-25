using Azure;
using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }
        [Authorize(Roles = "dealer")]
        [HttpGet("GetVehicle")]
        public async Task<IActionResult> GetVehicle()
        {         
            var vehicles = await _vehicleService.GetAllVehicle();
            if (vehicles == null || !vehicles.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có xe trong danh sách"));
            }
            return Ok(ApiResponse<List<VehicleDTO>>.OkResponse(vehicles.ToList(), "Lấy danh sách xe thành công"));
        }
        [Authorize(Roles = "dealer")]
        [HttpGet("GetVehicleById/{id}")]
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
        [HttpGet("Compare")]
        public async Task<IActionResult> Compare(int vehicleId1, int vehicleId2)
        {
            var comparison = await _vehicleService.CompareVehicles(vehicleId1, vehicleId2);
            if (comparison == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm thấy xe để so sánh"));
            }
            return Ok(ApiResponse<VehicleComparisonDTO>.OkResponse(comparison, "So sánh xe thành công"));
        }
    }
}
