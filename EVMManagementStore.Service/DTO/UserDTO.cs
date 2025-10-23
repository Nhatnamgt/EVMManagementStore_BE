namespace EVMManagementStore.Service.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public int RoleId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string CompanyName { get; set; }
    }
}
