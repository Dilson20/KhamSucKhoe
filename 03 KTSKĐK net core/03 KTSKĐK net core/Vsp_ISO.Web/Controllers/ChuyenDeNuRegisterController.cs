using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSP_HealthExam.Web.EditModels.Register;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using VSP_HealthExam.Web.Services;
using VSP_HealthExam.Web.Services.Register;

namespace VSP_HealthExam.Web.Controllers
{
    public class ChuyenDeNuRegisterController : Controller
    {
        private readonly I_ChuyenDeNuRegister _registerService;
        private readonly I_KhamSucKhoe _khamSucKhoeService;

        public ChuyenDeNuRegisterController(I_ChuyenDeNuRegister registerService, I_KhamSucKhoe khamSucKhoeService)
        {
            _registerService = registerService;
            _khamSucKhoeService = khamSucKhoeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var donViList = await _khamSucKhoeService.GetAllDonVi();
            ViewBag.DonViList = donViList;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var registerList = await _registerService.GetAllAsync();
            return View(registerList);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] ChuyenDeNuRegister_EditModel model)
        {
            System.Diagnostics.Debug.WriteLine($"Create action called with DanhSo: {model?.DanhSoCu}");
            
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is valid");
                try
                {
                    // Tạm thời bypass validation để test
                    // var danhSoValid = await _registerService.IsDanhSoValidForRegistrationAsync(model.DanhSoCu);
                    // if (!danhSoValid)
                    // {
                    //     return Json(new { success = false, message = "Danh số đã tồn tại trong hệ thống hoặc không tìm thấy trong danh sách nhân viên!" });
                    // }

                    System.Diagnostics.Debug.WriteLine("Calling CreateAsync");
                    var result = await _registerService.CreateAsync(model);
                    System.Diagnostics.Debug.WriteLine($"CreateAsync result: {result}");
                    
                    if (result)
                    {
                        return Json(new { success = true, message = "Đăng ký thành công! Vui lòng chờ phê duyệt." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Có lỗi xảy ra khi tạo đăng ký! Có thể do: Danh số không tồn tại trong hệ thống hoặc đã có đăng ký trước đó." });
                    }
                }
                catch (Exception ex)
                {
                    // Log exception để debug
                    System.Diagnostics.Debug.WriteLine($"Create action error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Create action stack trace: {ex.StackTrace}");
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ModelState is invalid");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState error: {error}");
                }
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRegisters()
        {
            var registerList = await _registerService.GetAllAsync();
            return Json(registerList);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PheDuyet(int id, int trangThai, string? lyDoTuChoi = null)
        {
            try
            {
                bool result = false;
                if (trangThai == 1) // Phê duyệt
                {
                    result = await _registerService.ApproveAsync(id);
                }
                else if (trangThai == 2) // Từ chối
                {
                    result = await _registerService.RejectAsync(id, lyDoTuChoi);
                }
                
                if (result)
                {
                    var trangThaiText = trangThai switch
                    {
                        1 => "đã phê duyệt",
                        2 => "đã từ chối",
                        _ => "đã cập nhật"
                    };
                    return Json(new { success = true, message = $"Đăng ký {trangThaiText} thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy đăng ký cần phê duyệt hoặc có lỗi xảy ra!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _registerService.DeleteAsync(id);
                if (result)
                {
                    return Json(new { success = true, message = "Xóa đăng ký thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy đăng ký cần xóa!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Rejected()
        {
            return View();
        }
    }
} 