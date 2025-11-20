using Microsoft.AspNetCore.Identity;

namespace VSP_HealthExam.Web.Entity
{
	public class AppUser : IdentityUser
	{
		public string? FullName { get; set; }
		public string? DanhSo { get; set; }

	}
}
