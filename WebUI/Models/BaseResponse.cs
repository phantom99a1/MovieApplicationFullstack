namespace WebUI.Models
{
    public class BaseResponse
    {
        public bool Status { get; set; }

        public string Message { get; set; } = string.Empty;

        public object Data { get; set; } = new();
    }
}
