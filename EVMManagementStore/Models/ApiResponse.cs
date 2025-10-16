using System.Net;

namespace EVMManagementStore.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(T? data, HttpStatusCode status, string? message)
        {
            Data = data;
            Status = (int) status;
            Message = message;
        }
        
        public static ApiResponse<T> OkResponse(T? data, string? mess)
        {
            return new ApiResponse<T>(data, HttpStatusCode.OK, mess);
        }
        public static ApiResponse<T> OkResponse(string? mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.OK, mess);
        }
    
        public static ApiResponse<T> CreateResponse(string? mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.Created, mess);
        }

        public static ApiResponse<T> UnauthorizedResponse(string mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.Unauthorized, mess);
        }

        public static ApiResponse<T> NotFoundResponse(string mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.NotFound, mess);
        }

        public static ApiResponse<T> BadRequestResponse(string mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.BadRequest, mess);
        }

        public static ApiResponse<T> InternalErrorResponse(string mess)
        {
            return new ApiResponse<T>(default, HttpStatusCode.InternalServerError, mess);
        }
    }

}
