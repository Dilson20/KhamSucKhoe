using System.ComponentModel.DataAnnotations;

namespace VSP_HealthExam.Web.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "DanhSo must be not blank!")]
		public string DanhSo { get; set; } = default!;
		[Required(ErrorMessage = "Password must be not blank!")]
		[DataType(DataType.Password)]
		public string Password { get; set; } = default!;
		//public string? ReturnUrl { get; set; }	
	}
}
