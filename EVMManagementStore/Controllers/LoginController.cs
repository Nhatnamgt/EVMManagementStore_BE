using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LoginResponse>.BadRequestResponse("Dữ liệu không hợp lệ"));
            }

            var response = await _loginService.Login(request);

            if (response == null)
            {
                return Unauthorized(ApiResponse<LoginResponse>.UnauthorizedResponse("Email hoặc mật khẩu không đúng"));
            }

            return Ok(ApiResponse<LoginResponse>.OkResponse(response, "Đăng nhập thành công"));
        }
    }
}
