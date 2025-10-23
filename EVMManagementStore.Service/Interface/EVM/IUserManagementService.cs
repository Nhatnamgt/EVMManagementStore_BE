using EVMManagementStore.Service.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.EVM
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();

        Task<UserDTO?> GetUserByIdAsync(int id);


        Task<UserDTO> CreateUserAsync(UserDTO userDto);

        Task<UserDTO?> UpdateUserAsync(int id, UserDTO userDto);

        Task<bool> DeleteUserAsync(int id);
    }
}
