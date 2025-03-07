using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Models
{
    public class LoginResponse
    {
        public string accessToken { get; set; }

        public string tokenType { get; set; } = "Bearer";

        public List<Role> role { get; set; }

        public int ErrorCode { get; set; } = 200;
        public bool Status { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        // public User user { get; set; }
    }
}
