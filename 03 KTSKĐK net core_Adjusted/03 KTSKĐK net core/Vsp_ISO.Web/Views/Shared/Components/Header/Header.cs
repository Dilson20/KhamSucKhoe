using Microsoft.AspNetCore.Mvc;

namespace VSP_HealthExam.Web.Areas.Views.Shared.Components.Header
{
	[ViewComponent]
	public class Header : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}
}
