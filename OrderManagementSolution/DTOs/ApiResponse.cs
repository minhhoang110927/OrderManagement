namespace OrderManagementSolution.DTOs
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Success") => new() { StatusCode = 200, Message = message, Data = data };
        public static ApiResponse<T> Error(string message) => new() { StatusCode = 400, Message = message };
    }
}
