using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Services;
using VSP_HealthExam.Web.ViewModels;
using System.Text.Encodings.Web;

namespace VSP_HealthExam.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		
		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
			
		}
        [AllowAnonymous]
        public async Task<IActionResult> Index()
		{
            // Nếu đã đăng nhập, chuyển đến MainPage
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MainPage");
            }
			return View();
		}
        [AllowAnonymous]
        public async Task<IActionResult> About()
        {

            return View("About");
        }
        [Authorize]
        public async Task<IActionResult> MainPage()
        {
            return View();
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}