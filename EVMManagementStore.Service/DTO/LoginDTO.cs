using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.DTO
{

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must be a valid Gmail address ending with @gmail.com")]
        public string Email { get; set; }

        [Required]
        [MaxLength(6, ErrorMessage = "Password must not exceed 6 characters")]
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
