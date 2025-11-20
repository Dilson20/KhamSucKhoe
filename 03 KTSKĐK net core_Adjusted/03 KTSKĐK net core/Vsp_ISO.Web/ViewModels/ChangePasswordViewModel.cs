using System.ComponentModel.DataAnnotations;

namespace VSP_HealthExam.Web.ViewModels
{
	public class ChangePasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; } = default!;
		[Required]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; } = default!;
		[Required]
		[DataType(DataType.Password)]
		[Compare("NewPassword")]
		public string ConfirmPassword { get; set; } = default!;
	}
}
