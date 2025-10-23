using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _usermanagementService;

        public UserManagementController(IUserManagementService usermanagementService)
        {
            _usermanagementService = usermanagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _usermanagementService.GetAllUsersAsync();
            return Ok(ApiResponse<List<UserDTO>>.OkResponse(users.ToList(), "Danh sách tài kho?n"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usermanagementService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm th?y ng??i dùng"));

            return Ok(ApiResponse<UserDTO>.OkResponse(user, "Thông tin ng??i dùng"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO dto)
        {
            var newUser = await _usermanagementService.CreateUserAsync(dto);
            return Ok(ApiResponse<UserDTO>.OkResponse(newUser, "T?o tài kho?n thành công"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO dto)
        {
            var updated = await _usermanagementService.UpdateUserAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm th?y ng??i dùng"));

            return Ok(ApiResponse<UserDTO>.OkResponse(updated, "C?p nh?t thành công"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _usermanagementService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.NotFoundResponse("Không tìm th?y ng??i dùng"));

            return Ok(ApiResponse<string>.OkResponse("Xóa tài kho?n thành công"));
        }
    }
}
