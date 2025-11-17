using Microsoft.AspNetCore.Components.Web;

namespace VSP_HealthExam.Web.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public int result { get; set; }
    }
}
