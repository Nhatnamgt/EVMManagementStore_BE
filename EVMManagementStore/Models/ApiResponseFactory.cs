namespace EVMManagementStore.Models
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(T data, string message = "Thành công", int status = 200)
            => new ApiResponse<T> { Data = data, Status = status, Message = message };

        public static ApiResponse<T> Fail<T>(string message, int status = 400)
            => new ApiResponse<T> { Data = default, Status = status, Message = message };
    }

}
