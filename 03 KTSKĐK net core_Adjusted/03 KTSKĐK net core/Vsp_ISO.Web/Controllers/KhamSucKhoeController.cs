using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Drawing;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using System.Text.Encodings.Web;
using VSP_HealthExam.Web.Services;
using Microsoft.Extensions.Options;
using VSP_HealthExam.Web.ViewModels;
using Microsoft.Extensions.Localization;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models.KhamSucKhoe;

namespace VSP_HealthExam.Web.Controllers
{
    [Authorize]
    public class KhamSucKhoeController : Controller
    {
        private readonly I_KhamSucKhoe _khamSucKhoeService;
        private readonly ILogger<KhamSucKhoeController> _logger;
        private readonly IMailServices _mailServices;
        private readonly MailSettings _mailSettings;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<AppUser> _userManager;
        
        public KhamSucKhoeController(I_KhamSucKhoe khamSucKhoeService, 
                                   ILogger<KhamSucKhoeController> logger, 
                                   IMailServices mailServices, 
                                   IOptions<MailSettings> mailSettings,
                                   IStringLocalizer<SharedResource> localizer,
                                   UserManager<AppUser> userManager)
        {
            _khamSucKhoeService = khamSucKhoeService;
            _logger = logger;
            _mailServices = mailServices;
            _mailSettings = mailSettings.Value;
            _localizer = localizer;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
            if (userType == "ChuyenDeNu")
            {
                // Trả về view riêng cho nhóm 3 (chuyên đề nữ)
                return View("Index_NuChuyenDe");
            }
            
            var user = User.Identity?.Name; 
            
            var userFindByCore = await _khamSucKhoeService.GetInfoNhanVienNOPassword(user ?? string.Empty);
            
            //Id TenNhom Color
            //1   Khám Sức khỏe định kỳ	#6aa84f
            //2   Khám Lãnh đạo	#f1c232
            //3   Khám chuyên đề nữ	#4a86e8
            //4   Khám chuyên đề tim mạch	#e06666
           
            if (userFindByCore == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user not found
            }else if( userFindByCore.Loai_CBCNV == 1 || userFindByCore.Loai_CBCNV == 2 || userFindByCore.Loai_CBCNV == 3)
            {
                userFindByCore?.NhomNhanVien_id?.Add(2);// add Lãnh đạo
            }
            else if (userFindByCore.GioiTinh == "Nữ")
            {
                userFindByCore?.NhomNhanVien_id?.Add(3);// add Giới tính Nữ
            }
            userFindByCore?.NhomNhanVien_id?.Add(1); // add Lấy mẫu máu định kỳ
            userFindByCore?.NhomNhanVien_id?.Add(4); // add Khám chuyên đề tim mạch
            userFindByCore?.NhomNhanVien_id?.Add(5); // add Tiêm vaccine
            userFindByCore?.NhomNhanVien_id?.Add(6); // add Khám sức khỏe định kỳ
            
            return View(userFindByCore);
        }
        [HttpGet]
        public async Task<IActionResult> GetDonVi()
        {
            try
            {
                var donViList = await _khamSucKhoeService.GetAllDonVi();
                return Json(donViList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading DonVi list: {ex.Message}");
                return Json(new List<DepartmentTree>()); // Return empty list on error
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminIndex()
        {
            try
            {
                var donViList = await _khamSucKhoeService.GetAllDonVi();
                return View("KSK_AdminIndex", donViList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading admin index: {ex.Message}");
                // Return view with empty list instead of crashing
                var emptyDonViList = new List<DepartmentTree>();
                return View("KSK_AdminIndex", emptyDonViList);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminCreateLichKSK()
        {
            try
            {
                var donViList = await _khamSucKhoeService.GetAllDonVi();
                return Json(donViList);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading DonVi list: {ex.Message}");
                return Json(new List<DepartmentTree>()); // Return empty list on error
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminCreateLichKSK([FromBody] Lich_KSK_EditModel model)
        {
            try
            {
                _logger.LogInformation("AdminCreateLichKSK called with model: {@Model}", model);
                
                if (ModelState.IsValid)
                {
                    // Kiểm tra thời gian hợp lệ
                    if (!model.IsValidTimeRange())
                    {
                        _logger.LogWarning("Invalid time range: Start={Start}, End={End}", model.ThoiGianBatDau, model.ThoiGianKetThuc);
                        return Json(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu!" });
                    }
                    
                    model.Creator = User.Identity?.Name ?? "Unknown";
                    var lichKham = await _khamSucKhoeService.CreateLichKhamSucKhoe(model);
                    _logger.LogInformation("Create successful for lichKham ID: {Id}", lichKham.Id);
                    return Json(new { success = true, data = lichKham, message = "Tạo lịch khám thành công!" });
                }
                
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState validation failed. Errors: {@Errors}", errors);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AdminCreateLichKSK: {Message}", ex.Message);
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminGetLoaiNhom()
        {

            var loaiNhomList = await _khamSucKhoeService.GetAllLoaiNhom();
            return Json(loaiNhomList);
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminGetLichKham()
        {
            var lichKhamList = await _khamSucKhoeService.GetAllLichKhamSucKhoe();
            return Json(
                lichKhamList.Select(x => new
                {
                    Id = x.Id,
                    DonViId= x.DonViId,
                    SoLuong =x.SoLuong,
                    GhiChu= x.GhiChu,
                    LoaiNhom =x.LoaiNhom,
                    start = x.ThoiGianBatDau.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = x.ThoiGianKetThuc.ToString("yyyy-MM-ddTHH:mm:ss"),
                }).ToList()
            );
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminUpdateLichKSK([FromBody] Lich_KSK_EditModel model)
        {
            try
            {
                _logger.LogInformation("AdminUpdateLichKSK called with model: {@Model}", model);
                
                if (ModelState.IsValid)
                {
                    // Kiểm tra thời gian hợp lệ
                    if (!model.IsValidTimeRange())
                    {
                        _logger.LogWarning("Invalid time range: Start={Start}, End={End}", model.ThoiGianBatDau, model.ThoiGianKetThuc);
                        return Json(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu!" });
                    }
                    
                    model.UpdatePerson = User.Identity?.Name ?? "Unknown";
                    var lichKham = await _khamSucKhoeService.UpdateLichKhamSucKhoe(model);
                    _logger.LogInformation("Update successful for lichKham ID: {Id}", lichKham.Id);
                    return Json(new { success = true, data = lichKham, message = "Cập nhật lịch khám thành công!" });
                }
                
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState validation failed. Errors: {@Errors}", errors);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AdminUpdateLichKSK: {Message}", ex.Message);
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminDeleteLichKSK([FromBody] int idLichKSK)
        {
            if (ModelState.IsValid)
            {
                var lichKham = await _khamSucKhoeService.DeleteLichKhamSucKhoe(idLichKSK);
                return Json(new { success = true, data = lichKham, message = "Xóa lịch khám thành công!" });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }

        [HttpGet]
        public async Task<IActionResult> GetLichKhamByDonVi(int donViId)
        {
            
            try
            {
                var lichKhamList = await _khamSucKhoeService.GetLichKhamByDonVi(donViId);
                return Json(lichKhamList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLichDaDangKy()
        {
            try
            {
                var danhSo = User.Claims.FirstOrDefault(c => c.Type == "DanhSo")?.Value;
                var userType = User.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
                if (string.IsNullOrEmpty(danhSo))
                    return BadRequest(new { error = _localizer["KSK.ErrorOccurred"] });
                var danhSachDangKy = await _khamSucKhoeService.GetLichDaDangKy(danhSo, _localizer);
                return Json(danhSachDangKy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLoaiNhomDaDangKy()
        {
            try
            {
                var danhSo = User.Claims.FirstOrDefault(c => c.Type == "DanhSo")?.Value;
                var userType = User.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
                if (string.IsNullOrEmpty(danhSo))
                    return BadRequest(new { error = "Không tìm thấy thông tin người dùng" });
                var loaiNhomDaDangKy = await _khamSucKhoeService.GetLoaiNhomDaDangKy(danhSo);
                return Json(loaiNhomDaDangKy);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNhanVienDangKy([FromBody] KSK_NhanVien_DangKy_EditModel model)
        {
            var danhSo = User.Claims.FirstOrDefault(c => c.Type == "DanhSo")?.Value;
            var userType = User.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;

            if (string.IsNullOrEmpty(danhSo))
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });

            if (userType == "ChuyenDeNu")
            {
                model.DanhSo = danhSo;
                model.DonViId = 0; // hoặc gán giá trị mặc định phù hợp
            }
            else
            {
                var userInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(danhSo);
                if (userInfo == null)
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                model.DonViId = userInfo.ID_DonVi ?? 0;
                model.DanhSo = userInfo.DanhSo ?? "0";
            }

            ModelState.Remove("DanhSo");
            ModelState.Remove("DonViId");
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem user đã đăng ký loại nhóm này chưa
                    var lich = await _khamSucKhoeService.GetLichKhamSucKhoeById(model.LichKhamSucKhoeId);
                    if (lich != null)
                    {
                        var daDangKy = await _khamSucKhoeService.KiemTraDaDangKyLoaiNhom(danhSo, lich.LoaiNhom);
                        if (daDangKy)
                        {
                            return Json(new { success = false, message = "Bạn đã đăng ký loại khám này rồi!" });
                        }
                    }

                    var lichKham = await _khamSucKhoeService.CreateNhanVienDangKy(model);

                    // Lấy thông tin nhân viên để gửi mail
                    var nhanVien = await _khamSucKhoeService.GetInfoNhanVienNOPassword(model.DanhSo);
                    
                    // Lấy thông tin loại nhóm để gửi email
                    var loaiNhom = await _khamSucKhoeService.GetLoaiNhomById(lich.LoaiNhom);
                    var tenLoaiNhom = loaiNhom?.TenNhom ?? "Khám sức khỏe";
                    
                    // Chỉ gửi email nếu service được kích hoạt
                    if (_mailSettings.Active && nhanVien != null && !string.IsNullOrEmpty(nhanVien.Email) && lich != null)
                    {
                        try
                        {
                            _logger.LogInformation("Đang gửi email thông báo đến: {Email}", nhanVien.Email);
                            await _mailServices.SendMailAsync(
                                nhanVien.Email,
                                nhanVien.GioiTinh ?? "",
                                nhanVien.HoTen ?? "",
                                nhanVien.DanhSo ?? "",
                                nhanVien.DonVi ?? "",
                                lich.Id,
                                lich.ThoiGianBatDau,
                                tenLoaiNhom,
                                lich.GhiChu ?? ""
                            );
                            _logger.LogInformation("Gửi email thành công");
                        }
                        catch (Exception emailEx)
                        {
                            _logger.LogError(emailEx, "Lỗi khi gửi email thông báo: {Message}", emailEx.Message);
                            // Không throw exception để không ảnh hưởng đến việc đăng ký
                        }
                    }
                    else
                    {
                        if (!_mailSettings.Active)
                        {
                            _logger.LogInformation("Email service không được kích hoạt, bỏ qua gửi email");
                        }
                        else
                        {
                            _logger.LogWarning("Không thể gửi email: Email={Email}, NhanVien={NhanVien}, Lich={Lich}", 
                                nhanVien?.Email, nhanVien != null, lich != null);
                        }
                    }
                    
                    return Json(new { success = true, data = lichKham, message = "Đăng ký thành công!" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors });
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> ExportKSKToExcel(DateTime? fromDate, DateTime? toDate, int? donviId, int? nhomId)
        {
            if (!fromDate.HasValue)
            {
                return BadRequest("Vui lòng chọn từ ngày!");
            }
            
            if (!toDate.HasValue)
            {
                return BadRequest("Vui lòng chọn đến ngày!");
            }
            
            if (!nhomId.HasValue)
            {
                return BadRequest("Vui lòng chọn nhóm!");
            }
            
            var fileBytes = await _khamSucKhoeService.ExportKSKToExcel(fromDate, toDate, donviId, nhomId);
            string fileName = $"DanhSachDangKy_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> ExportChuaDangKyKSKToExcel(int? donviId)
        {
            if (!donviId.HasValue)
            {
                return BadRequest("Vui lòng chọn đơn vị trước khi xuất Excel!");
            }
            
            var fileBytes = await _khamSucKhoeService.ExportChuaDangKyKSKToExcel(donviId);
            string fileName = $"DanhSachChuaDangKy_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetLichKhamByLoai(int loaiNhom)
        {
            try
            {
                var lichKhamList = await _khamSucKhoeService.GetAllLichKhamSucKhoe();
                var result = new List<object>();
                
                foreach (var lich in lichKhamList.Where(x => x.LoaiNhom == loaiNhom && x.ThoiGianBatDau > DateTime.Now).OrderBy(x => x.ThoiGianBatDau))
                {
                    // Đếm số lượng đã đăng ký cho lịch này
                    var soLuongDaDangKy = await _khamSucKhoeService.GetSoLuongDaDangKyByLichId(lich.Id);
                    
                    // Tính số slot trống
                    var soSlotTrong = lich.SoLuong - soLuongDaDangKy;
                    
                    result.Add(new {
                        id = lich.Id,
                        thoiGianKSK = lich.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm"),
                        donViName = lich.DonViId, // Nếu cần tên đơn vị, join thêm bảng đơn vị
                        soLuong = lich.SoLuong,
                        soLuongDaDangKy = soLuongDaDangKy,
                        soSlotTrong = soSlotTrong
                    });
                }
                
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> ViewNhanVienAll(int? donViId = null, string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("ViewNhanVienAll called - displaying current user info");
                
                // Get current logged-in user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("Current user not found");
                    return RedirectToAction("Login", "Account");
                }

                // Get user information from service
                var userInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(currentUser.DanhSo ?? string.Empty);
                
                if (userInfo == null)
                {
                    _logger.LogWarning("User information not found for DanhSo: {DanhSo}", currentUser.DanhSo);
                    return View(new List<View_NhanVien_All>());
                }

                // Return single user as a list for the view
                var singleUserList = new List<View_NhanVien_All> { userInfo };
                
                _logger.LogInformation("ViewNhanVienAll returned user info for: {DanhSo}", currentUser.DanhSo);
                
                return View(singleUserList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ViewNhanVienAll: {Message}", ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                _logger.LogInformation("Bắt đầu test email");
                
                // Lấy tên nhóm từ database để test
                var loaiNhom = await _khamSucKhoeService.GetLoaiNhomById(1); // Test với ID = 1
                var tenLoaiNhom = loaiNhom?.TenNhom ?? "Lấy mẫu máu định kỳ";
                
                await _mailServices.SendMailAsync(
                    "test@example.com", // Email test
                    "Nam",
                    "Nguyễn Văn Test",
                    "TEST001",
                    "Phòng IT",
                    1,
                    DateTime.Now,
                    tenLoaiNhom, // Sử dụng tên nhóm từ database
                    "Test ghi chú của TTYT" // Ghi chú test
                );
                return Json(new { success = true, message = "Test email thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test email thất bại: {Message}", ex.Message);
                return Json(new { success = false, message = $"Test email thất bại: {ex.Message}" });
            }
        }
    }
}
