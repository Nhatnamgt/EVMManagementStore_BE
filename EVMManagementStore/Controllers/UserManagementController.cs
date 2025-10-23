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
            return Ok(ApiResponse<List<UserDTO>>.OkResponse(users.ToList(), "Danh s�ch t�i kho?n"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _usermanagementService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Kh�ng t�m th?y ng??i d�ng"));

            return Ok(ApiResponse<UserDTO>.OkResponse(user, "Th�ng tin ng??i d�ng"));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO dto)
        {
            var newUser = await _usermanagementService.CreateUserAsync(dto);
            return Ok(ApiResponse<UserDTO>.OkResponse(newUser, "T?o t�i kho?n th�nh c�ng"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO dto)
        {
            var updated = await _usermanagementService.UpdateUserAsync(id, dto);
            if (updated == null)
                return NotFound(ApiResponse<string>.NotFoundResponse("Kh�ng t�m th?y ng??i d�ng"));

            return Ok(ApiResponse<UserDTO>.OkResponse(updated, "C?p nh?t th�nh c�ng"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _usermanagementService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.NotFoundResponse("Kh�ng t�m th?y ng??i d�ng"));

            return Ok(ApiResponse<string>.OkResponse("X�a t�i kho?n th�nh c�ng"));
        }
    }
}
