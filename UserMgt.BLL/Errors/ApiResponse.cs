namespace UserMgt.BLL.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
