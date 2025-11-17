using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;
using NuGet.Protocol.Plugins;
using System.Data.Entity;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using VSP_HealthExam.Web.Services;
using VSP_HealthExam.Web.ViewModels;
using VSP_HealthExam.Web.Services.Register;

namespace VSP_HealthExam.Web.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUser> _userManager;
		private SignInManager<AppUser> _signInManager;
		private RoleManager<IdentityRole> _roleManager;
		private Entity.ApplicationDbContext _context;
        private I_KhamSucKhoe _I_KhamSucKhoe;
        private I_ChuyenDeNuRegister _chuyenDeNuRegister;
        private IStringLocalizer<SharedResource> _loc;
		private IConfiguration _configuration;
        private SubDbContext _subContext;
		public AccountController(UserManager<AppUser> userManager,
								SignInManager<AppUser> signInManager,
								RoleManager<IdentityRole> roleManager,
                                Entity.ApplicationDbContext context,
								IStringLocalizer<SharedResource> Loc,
								IConfiguration configuration,
                                I_KhamSucKhoe khamSucKhoe,
                                I_ChuyenDeNuRegister chuyenDeNuRegister,
                                SubDbContext subContext)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_context = context;
			_loc = Loc;
			_configuration = configuration;
            _I_KhamSucKhoe = khamSucKhoe;
            _chuyenDeNuRegister = chuyenDeNuRegister;
            _subContext = subContext;
        }
		public IActionResult Index()
		{
			return View();
		}

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Validate if the culture is one of the supported cultures
            var supportedCultures = new[] { "vi", "ru" };  // Example of supported cultures
            if (!supportedCultures.Contains(culture))
            {
                return View("Error");  // Redirect to home if invalid culture is selected
            }

            // Store the selected culture in a cookie to persist the culture across requests
            Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                   new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }

               );
            // Chỉ cho phép redirect nội bộ
            if (!Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [HttpGet]
		public ActionResult Login()
        {
            LoginViewModel model = new();
			return PartialView("Login", model);
		}
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel login)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errorMessage = "Invalid username or password.";
        //        return BadRequest(new LoginResponse { Success = false, Error = errorMessage, result = 0, Token = "" });
        //    }

        //    // Ensure roles exist in AspNetRoles
        //    //if (!await _roleManager.RoleExistsAsync("ADMIN"))
        //    //{
        //    //    await _roleManager.CreateAsync(new IdentityRole("ADMIN"));
        //    //}
        //    //if (!await _roleManager.RoleExistsAsync("MEDIC"))
        //    //{
        //    //    await _roleManager.CreateAsync(new IdentityRole("MEDIC"));
        //    //}

        //    // Core system login
        //    var userfindByCore = await _userManager.FindByNameAsync(login.DanhSo);
        //    if (userfindByCore != null)
        //    {
        //        var user = await _KSK_BenhNhan_Full_Dao.Login(login.DanhSo, login.Password);
        //        if (user.result == 1)
        //        {
        //            await _signInManager.SignOutAsync(); // Clear existing cookies

        //            // Cookie claims
        //            var cookieClaims = new List<Claim>
        //            {
        //                new Claim("DanhSo", login.DanhSo),
        //                new Claim(ClaimTypes.NameIdentifier, userfindByCore.Id),
        //                new Claim(ClaimTypes.Name, login.DanhSo)
        //            };

        //            // Load existing roles
        //            var roles = await _userManager.GetRolesAsync(userfindByCore);
        //            if (roles == null || !roles.Any())
        //            {
        //                // Fallback: Manually query AspNetUserRoles for debugging
        //                try
        //                {
        //                        var sqlRoleNames = await _context.Database
        //                            .SqlQueryRaw<string>("SELECT r.Name FROM AspNetUserRoles ur JOIN AspNetRoles r ON ur.RoleId = r.Id WHERE ur.UserId = {0}", userfindByCore.Id)
        //                            .ToListAsync();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"Manual Query Failed: {ex.Message}, StackTrace: {ex.StackTrace}");
        //                }
        //            }
        //            else
        //            {
        //                foreach (var role in roles)
        //                {
        //                    cookieClaims.Add(new Claim(ClaimTypes.Role, role));
        //                }
        //            }

        //            // Sign in with cookie
        //            var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        //            var principal = new ClaimsPrincipal(identity);
        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        //            {
        //                IsPersistent = false,
        //                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
        //                IssuedUtc = DateTimeOffset.UtcNow
        //            });
        //            user.user.MatKhau= ""; // Clear password before returning user data 
        //            return Ok(new LoginResponse { Success = true, Token = "", result = 1, user = user.user });
        //        }
        //        else
        //        {
        //            var errorMessage = "Invalid username or password.";
        //            return Ok(new LoginResponse { Success = false, Error = errorMessage, result = 0, Token = "" });
        //        }
        //    }
        //    else
        //    {
        //        // Legacy system login
        //        var user = await _KSK_BenhNhan_Full_Dao.Login(login.DanhSo, login.Password);
        //        if (user == null || user.result != 1)
        //        {
        //            var errorMessage = "Invalid username or password.";
        //            return Ok(new LoginResponse { Success = false, Error = errorMessage, result = 0, Token = "" });
        //        }

        //        var appUser = await _userManager.FindByNameAsync(login.DanhSo);
        //        if (appUser == null)
        //        {
        //            appUser = new AppUser
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                DanhSo = user.user.DanhSo,
        //                FullName = user.user.HoTen,
        //                UserName = user.user.DanhSo,
        //                Email = user.user.Email,
        //                NormalizedUserName = user.user.DanhSo.ToUpper()
        //            };
        //            var createResult = await _userManager.CreateAsync(appUser);
        //            if (!createResult.Succeeded)
        //            {
        //                var errorMessage = "Failed to create user: " + string.Join(", ", createResult.Errors.Select(e => e.Description));
        //                return Ok(new LoginResponse { Success = false, Error = errorMessage, result = 0, Token = "" });
        //            }
        //        }

        //        await _signInManager.SignOutAsync(); // Clear existing cookies

        //        // Cookie claims
        //        var cookieClaims = new List<Claim>
        //{
        //    new Claim("DanhSo", login.DanhSo),
        //    new Claim(ClaimTypes.NameIdentifier, appUser.Id),
        //    new Claim(ClaimTypes.Name, login.DanhSo)
        //};

        //        var roles = await _userManager.GetRolesAsync(appUser);
        //        if (roles == null || !roles.Any())
        //        {
        //            // Fallback: Manually query AspNetUserRoles for debugging
        //            try
        //            {
        //                var roleNames = await _context.UserRoles
        //                    .Where(ur => ur.UserId == appUser.Id)
        //                    .Join(_context.Roles,
        //                        ur => ur.RoleId,
        //                        r => r.Id,
        //                        (ur, r) => r.Name)
        //                    .ToListAsync();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Manual Role Query Failed: {ex.Message}, StackTrace: {ex.StackTrace}");
        //            }
        //        }
        //        else
        //        {
        //            foreach (var role in roles)
        //            {
        //                cookieClaims.Add(new Claim(ClaimTypes.Role, role));
        //            }
        //        }

        //        // Sign in with cookie
        //        var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);
        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        //        {
        //            IsPersistent = false,
        //            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
        //            IssuedUtc = DateTimeOffset.UtcNow
        //        });
        //        user.user.MatKhau = ""; // Clear password before returning user data
        //        return Ok(new LoginResponse { Success = true, Token = "", result = 1, user = user.user });
        //    }
        //}
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, error = "Invalid username or password." });
            }

            // Kiểm tra user có trong bảng đăng ký chuyên đề nữ không
            var nuRegister = await _chuyenDeNuRegister.GetByDanhSoAsync(login.DanhSo);
            if (nuRegister != null)
            {
                // Kiểm tra mật khẩu cho user chuyên đề nữ
                if (nuRegister.Password != login.Password)
                {
                    return Json(new { success = false, error = "Invalid username or password." });
                }

                if (nuRegister.TrangThaiPheDuyet == 2)
                {
                    // Bị từ chối
                    await _signInManager.SignOutAsync();
                    return Json(new { success = false, error = "Tài khoản của bạn đã bị từ chối." });
                }
                else if (nuRegister.TrangThaiPheDuyet == 0)
                {
                    // Chưa duyệt
                    await _signInManager.SignOutAsync();
                    return Json(new { success = false, error = "Tài khoản của bạn đang chờ phê duyệt. Vui lòng quay lại sau!" });
                }
                else if (nuRegister.TrangThaiPheDuyet == 1)
                {
                    // Đã duyệt - cho phép đăng nhập trực tiếp từ bảng Register
                    await _signInManager.SignOutAsync(); // Clear existing cookies

                    // Nếu FullName null hoặc rỗng, gán giá trị mặc định
                    var fullName = string.IsNullOrWhiteSpace(nuRegister.FullName) ? "Chuyên đề nữ" : nuRegister.FullName;

                    var cookieClaims = new List<Claim>
                    {
                        new Claim("DanhSo", login.DanhSo),
                        new Claim(ClaimTypes.NameIdentifier, nuRegister.Id.ToString()),
                        new Claim(ClaimTypes.Name, login.DanhSo),
                        new Claim("UserType", "ChuyenDeNu"),
                        new Claim("FullName", fullName)
                    };

                    var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                        IssuedUtc = DateTimeOffset.UtcNow
                    });

                    return Json(new { success = true, redirectUrl = Url.Action("MainPage", "Home"), claims = cookieClaims.Select(c => new { c.Type, c.Value }) });
                }
            }

            // Core system login - chỉ cho user có trong AppUser
            var userfindByCore = await _userManager.FindByNameAsync(login.DanhSo);
            if (userfindByCore != null)
            {
                // Kiểm tra password bằng ASP.NET Identity
                var passwordCheck = await _userManager.CheckPasswordAsync(userfindByCore, login.Password);
                if (passwordCheck)
                {
                    await _signInManager.SignOutAsync(); // Clear existing cookies

                    // Cookie claims
                    var cookieClaims = new List<Claim>
                    {
                        new Claim("DanhSo", login.DanhSo),
                        new Claim(ClaimTypes.NameIdentifier, userfindByCore.Id),
                        new Claim(ClaimTypes.Name, login.DanhSo),
                        new Claim("UserType", "NhanVien")
                    };
                    // Load existing roles
                    var roles = await _userManager.GetRolesAsync(userfindByCore);
                    if (roles != null && roles.Any())
                    {
                        foreach (var role in roles)
                        {
                            cookieClaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    // Sign in with cookie
                    var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                        IssuedUtc = DateTimeOffset.UtcNow
                    });                   
                    return Json(new { success = true, redirectUrl = Url.Action("MainPage", "Home") });
                }
                else
                {
                    return Json(new { success = false, error = "Invalid username or password." });
                }
            }
            else
            {
                // Legacy system login - chỉ cho user không có trong AppUser
                var user = await _I_KhamSucKhoe.GetInfoNhanVien(login.DanhSo, login.Password);
                if (user == null)
                {
                    return Json(new { success = false, error = "Invalid username or password." });
                }

                // Tìm trong AppUser, nếu chưa có thì tạo mới
                var appUser = await _userManager.FindByNameAsync(login.DanhSo);
                if (appUser == null)
                {
                    // Lấy thông tin từ _I_KhamSucKhoe (đã kiểm tra password ở trên)
                    if (user != null && !string.IsNullOrEmpty(login.Password))
                    {
                        var newUser = new AppUser
                        {
                            UserName = user.DanhSo,
                            DanhSo = user.DanhSo,
                            Email = user.Email,
                            FullName = user.HoTen,
                            NormalizedUserName = user.DanhSo?.ToUpper()
                        };
                        var createResult = await _userManager.CreateAsync(newUser, login.Password);
                        if (!createResult.Succeeded)
                        {
                            return Json(new { success = false, error = "Failed to create user account." });
                        }
                        // Lấy lại user vừa tạo để có ID
                        appUser = await _userManager.FindByNameAsync(login.DanhSo);
                        if (appUser == null)
                        {
                            return Json(new { success = false, error = "User created but not found." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, error = "Invalid user information." });
                    }
                }
                
                await _signInManager.SignOutAsync(); // Clear existing cookies

                // Cookie claims
                var cookieClaims = new List<Claim>
                {
                    new Claim("DanhSo", login.DanhSo),
                    new Claim(ClaimTypes.NameIdentifier, appUser?.Id ?? login.DanhSo),
                    new Claim(ClaimTypes.Name, login.DanhSo),
                    new Claim("UserType", "NhanVien")
                };

                // Sign in with cookie
                var identity = new ClaimsIdentity(cookieClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    IssuedUtc = DateTimeOffset.UtcNow
                });
                
                return Json(new { success = true, redirectUrl = Url.Action("MainPage", "Home") });
            }
        }
       


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
		{
			var userType = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
			
			// Chỉ xử lý claims cho user trong AppUser
			if (userType != "ChuyenDeNu" && userType != "Legacy")
			{
				var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
				if (user != null)
				{
					var userClaims = await _userManager.GetClaimsAsync(user);
					await _userManager.RemoveClaimsAsync(user, userClaims);
				}
			}
			
			await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
		}
	}
}
