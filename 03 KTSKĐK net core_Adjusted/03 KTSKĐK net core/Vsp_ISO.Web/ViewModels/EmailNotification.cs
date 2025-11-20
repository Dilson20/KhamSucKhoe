namespace VSP_HealthExam.Web.ViewModels
{
	public class EmailNotification
	{
		public string? UserId { get; set; } = default!;
		public string? UserName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string? Photo { get; set; }
		public bool? ReceiveEmailNewHSE { get; set; }
		public bool? ReceiveEmailFeedback { get; set; }
	}
}
