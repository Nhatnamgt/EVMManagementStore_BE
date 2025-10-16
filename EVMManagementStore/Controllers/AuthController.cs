using EVMManagementStore.Models;
using EVMManagementStore.Service.Dealer;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _loginService;

        public AuthController(IAuthService loginService)
        {
            _loginService = loginService;
        }
        //12345
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LoginResponse>.BadRequestResponse("Dữ liệu không hợp lệ"));
            }

            var response = await _loginService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized(ApiResponse<LoginResponse>.UnauthorizedResponse("Email hoặc mật khẩu không đúng"));
            }

            return Ok(ApiResponse<LoginResponse>.OkResponse(response, "Đăng nhập thành công"));
        }
    }
}
