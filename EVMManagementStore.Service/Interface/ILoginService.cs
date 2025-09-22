using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(LoginRequest request);
    }
}
