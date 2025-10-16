using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.Dealer
{
    public class AuthService : IAuthService
    {
        private readonly EVMManagementStoreContext _context;
        private readonly IConfiguration _config;

        public AuthService(EVMManagementStoreContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // Lấy user + role từ DB
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
                return null;

            string roleName = user.Role?.RoleName;

            // Kiểm tra role hợp lệ
            var validRoles = new[] { "admin", "dealer", "evm_staff", "customer" };
            if (string.IsNullOrEmpty(roleName) || !validRoles.Contains(roleName))
                return null;

            var token = GenerateJwtToken(user.Email, roleName);

            return new LoginResponse
            {
                Token = token,
                Role = roleName
            };
        }

        private string GenerateJwtToken(string email, string role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role ?? "User")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int expiresInMinutes = 60; // default 1h
            if (!string.IsNullOrEmpty(jwtSettings["ExpiresInMinutes"]))
                int.TryParse(jwtSettings["ExpiresInMinutes"], out expiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
