namespace VSP_HealthExam.Web.ViewModels
{
	public class MailSettings
	{
		public string Email { get; set; } = default!;
		public string? Password { get; set; }
		public string Host { get; set; } = default!;
		public int Port { get; set; }
		public bool SSL { get; set; }
		public bool Active { get; set; }
	}
}
