using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace EVMManagementStore.Service.Service.EVM
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllIncludeAsync(u => u.Role);

            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                RoleId = u.RoleId,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                CompanyName = u.CompanyName,
                Password = null 
            }).ToList();
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                RoleId = user.RoleId,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                CompanyName = user.CompanyName,
                Password = null
            };
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO dto)
        {
            string hashedPassword = string.IsNullOrWhiteSpace(dto.Password)
                ? null
                : BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                CompanyName = dto.CompanyName
            };

            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.SaveAsync();

            dto.UserId = newUser.UserId;
            dto.Password = null;
            return dto;
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UserDTO dto)
        {
            var existing = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Username = dto.Username;
            existing.Email = dto.Email;
            existing.FullName = dto.FullName;
            existing.Phone = dto.Phone;
            existing.Address = dto.Address;
            existing.CompanyName = dto.CompanyName;
            existing.RoleId = dto.RoleId;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _unitOfWork.UserRepository.Update(existing);
            await _unitOfWork.SaveAsync();

            dto.Password = null;
            return dto;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) return false;

            _unitOfWork.UserRepository.Remove(user);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
