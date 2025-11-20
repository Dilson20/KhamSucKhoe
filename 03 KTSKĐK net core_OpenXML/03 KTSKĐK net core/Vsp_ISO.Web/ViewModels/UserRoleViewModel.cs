namespace VSP_HealthExam.Web.ViewModels
{
	public class UserRoleViewModel
	{
		public string UserId { get; set; } = default!;
		public string UserName { get; set; } = default!;
		public string? Photo { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsEditor { get; set; }

	}
}
