using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using System.Text.Encodings.Web;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.BaoHiem;
using VSP_HealthExam.Web.Services.BaoHiem;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using VSP_HealthExam.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VSP_HealthExam.Web.Controllers
{
    [Authorize]
    public class NgoaiVSPController : Controller
    {
        private readonly I_Document_BH _documentService;
        private readonly ApplicationDbContext _db;
        private readonly I_KhamSucKhoe _khamSucKhoeService;

        public NgoaiVSPController(I_Document_BH documentService, ApplicationDbContext db, I_KhamSucKhoe khamSucKhoeService)
        {
            _documentService = documentService;
            _db = db;
            _khamSucKhoeService = khamSucKhoeService;
        }

        public async Task<IActionResult> Index()
        {
            var language = HttpContext.Features?.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;

            if (language == null)
            {
                Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("vi")),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
                language = "vi";
            }
            // Get DanhSo from logged-in user claims
            string? danhSo = User.FindFirstValue("DanhSo");
            if (string.IsNullOrEmpty(danhSo))
            {
                // Handle missing DanhSo, e.g., redirect or show error
                return RedirectToAction("Login", "Account");
            }

            // Get model from service
            List<Document_BH>? documents = await _documentService.GetDocument_BHByDanhSo(danhSo);

            return View(documents);
        }
        [HttpGet]
        public async Task<IActionResult> CreateDocument_BH()
        {
            string? danhSo = User.FindFirstValue("DanhSo");
            if (string.IsNullOrEmpty(danhSo))
            {
                // Handle missing DanhSo, e.g., redirect or show error
                return RedirectToAction("Login", "Account");
            }
            Document_BH_EditModel model = new Document_BH_EditModel
            {
                DanhSo = danhSo,             
            };
            return View(model);   
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDocument_BH(Document_BH_EditModel model)
        {
            if (!Document_BH_EditModelValidator.ValidateTextList(model.FileNames))
            {
                ModelState.AddModelError("FileNames", "Tên file chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!Document_BH_EditModelValidator.ValidateTextList(model.Descriptions))
            {
                ModelState.AddModelError("Descriptions", "Mô tả chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!ModelState.IsValid)
            {
                // Collect all error messages from the model state
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors = errorMessages });
            }
            model.DateUpload = DateTime.Now;
            // Đã xóa HtmlEncoder.Default.Encode khi lưu dữ liệu user
            foreach (var file in model.FilesAttachments)
            {
                if (file != null)
                {
                    file.FileName = HtmlEncoder.Default.Encode(file.FileName ?? string.Empty);
                    file.Description = HtmlEncoder.Default.Encode(file.Description ?? string.Empty);
                }
            }
            var language = HttpContext.Features?.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name ?? "vi";
            var result = await _documentService.CreateDocument_BHWithAttachment(model, language);

            if (result)
            {
                return Json(new { success = true, message = "Tạo hồ sơ thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Tạo hồ sơ thất bại." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateDocument_BH(int id)
        {
            string? danhSo = User.FindFirstValue("DanhSo");
            if (string.IsNullOrEmpty(danhSo))
            {
                // Handle missing DanhSo, e.g., redirect or show error
                return RedirectToAction("Login", "Account");
            }
            Document_BH model = await _documentService.GetDocument_BHById(id);
            Document_BH_EditModel editModel = new Document_BH_EditModel
            {
                Id = model.Id,
                DanhSo = model.DanhSo,
                DateUpload = model.DateUpload,
                TenBenhVien = model.TenBenhVien,
                ThongTin = model.ThongTin,
                NgayBatDauKham = model.NgayBatDauKham,
                NgayKetThucKham = model.NgayKetThucKham,
                LoaiHinhDieuTri = model.LoaiHinhDieuTri,
                HinhThucKCB = model.HinhThucKCB,
                DongYSuDung = model.DongYSuDung.ToString(),
                FilesAttachments = model.FilesAttachments.ToList(),
                
            };
            return View(editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDocument_BH(Document_BH_EditModel model)
        {
            if (!Document_BH_EditModelValidator.ValidateTextList(model.FileNames))
            {
                ModelState.AddModelError("FileNames", "Tên file chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!Document_BH_EditModelValidator.ValidateTextList(model.Descriptions))
            {
                ModelState.AddModelError("Descriptions", "Mô tả chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!ModelState.IsValid)
            {
                // Collect all error messages from the model state
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors = errorMessages });
            }
            model.DateUpload = DateTime.Now;
            // Đã xóa HtmlEncoder.Default.Encode khi lưu dữ liệu user
            foreach (var file in model.FilesAttachments)
            {
                if (file != null)
                {
                    file.FileName = HtmlEncoder.Default.Encode(file.FileName ?? string.Empty);
                    file.Description = HtmlEncoder.Default.Encode(file.Description ?? string.Empty);
                }
            }
            var result = await _documentService.UpdateDocument_BH(model);

            if (result)
            {
                return Json(new { success = true, message = "Cập nhật hồ sơ thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Cập nhật hồ sơ thất bại." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDocument_BHByDateUpload(string? startDate, string? endDate, string?danhSo)
        {
            DateOnly? startDateOnly = null;
            DateOnly? endDateOnly = null;
            
            // Parse date strings (accept both dd/MM/yyyy and yyyy-MM-dd formats)
            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateOnly.TryParseExact(startDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedStartDate))
                {
                    startDateOnly = parsedStartDate;
                }
                else if (DateOnly.TryParseExact(startDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedStartDate2))
                {
                    startDateOnly = parsedStartDate2;
                }
                else
                {
                    return Json(new { success = false, message = "Định dạng ngày bắt đầu không hợp lệ (dd/MM/yyyy hoặc yyyy-MM-dd)" });
                }
            }
            
            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateOnly.TryParseExact(endDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedEndDate))
                {
                    endDateOnly = parsedEndDate;
                }
                else if (DateOnly.TryParseExact(endDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedEndDate2))
                {
                    endDateOnly = parsedEndDate2;
                }
                else
                {
                    return Json(new { success = false, message = "Định dạng ngày kết thúc không hợp lệ (dd/MM/yyyy hoặc yyyy-MM-dd)" });
                }
            }

            if (!startDateOnly.HasValue && !endDateOnly.HasValue)
            {
                if (string.IsNullOrEmpty(danhSo))
                {
                    return Json(new { success = false, message = "At least one of start date, end date, or DanhSo must be provided." });
                }

                var resultByDanhSo = await _documentService.GetDocument_BHByDanhSo(danhSo);
                if (resultByDanhSo == null || !resultByDanhSo.Any())
                {
                    return Json(new { success = false, message = "No documents found for the specified DanhSo." });
                }

                var documentViewModels = new List<Document_BH_ViewModel>();
                foreach (var item in resultByDanhSo)
                {
                    // Thay thế truy vấn lấy nhân viên:
                    var nhanvienInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(item.DanhSo);
                    // Thay thế truy vấn lấy đơn vị:
                    var donViList = await _khamSucKhoeService.GetAllDonVi();

                    var doc = new Document_BH_ViewModel
                    {
                        Id = item.Id,
                        DanhSo = item.DanhSo,
                        TenNhanVien = nhanvienInfo?.HoTen ?? "Chưa xác định",
                        TenDonVi = donViList.FirstOrDefault(d => d.ID == nhanvienInfo?.ID_DonVi)?.TenToChuc ?? "Chưa xác định",
                        TenBenhVien = item.TenBenhVien,
                        DateUpload = item.DateUpload,
                        ThongTin = item.ThongTin,
                        LoaiHinhDieuTri = item.LoaiHinhDieuTri,
                        NgayBatDauKham = item.NgayBatDauKham,
                        NgayKetThucKham = item.NgayKetThucKham,
                        FilesAttachments = item.FilesAttachments,
                        HinhThucKCB = item.HinhThucKCB,
                        NoteByDoctor = item.NoteByDoctor,
                        Status = item.Status
                    };
                    documentViewModels.Add(doc);
                }
                return Ok(documentViewModels);
            }

            // If only one date is provided, treat it as a single-day search
            if (startDateOnly.HasValue && !endDateOnly.HasValue)
            {
                endDateOnly = startDateOnly;
            }
            else if (!startDateOnly.HasValue && endDateOnly.HasValue)
            {
                startDateOnly = endDateOnly;
            }

            // Validate date range
            if (startDateOnly > endDateOnly)
            {
                return Json(new { success = false, message = "Start date cannot be later than end date." });
            }

            // Search by date range
            var resultByDate = await _documentService.GetDocument_BHByDateRange(startDateOnly.Value, endDateOnly.Value);
            if (resultByDate == null || !resultByDate.Any())
            {
                return Json(new { success = false, message = "No documents found for the specified date range." });
            }

            var documentViewModelsByDate = new List<Document_BH_ViewModel>();
            foreach (var item in resultByDate)
            {
                // Thay thế truy vấn lấy nhân viên:
                var nhanvienInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(item.DanhSo);
                // Thay thế truy vấn lấy đơn vị:
                var donViList = await _khamSucKhoeService.GetAllDonVi();

                var doc = new Document_BH_ViewModel
                {
                    Id = item.Id,
                    DanhSo = item.DanhSo,
                    TenNhanVien = nhanvienInfo?.HoTen ?? "Chưa xác định",
                    TenDonVi = donViList.FirstOrDefault(d => d.ID == nhanvienInfo?.ID_DonVi)?.TenToChuc ?? "Chưa xác định",
                    TenBenhVien = item.TenBenhVien,
                    DateUpload = item.DateUpload,
                    ThongTin = item.ThongTin,
                    LoaiHinhDieuTri = item.LoaiHinhDieuTri,
                    NgayBatDauKham = item.NgayBatDauKham,
                    NgayKetThucKham = item.NgayKetThucKham,
                    FilesAttachments = item.FilesAttachments,
                    HinhThucKCB = item.HinhThucKCB,
                    NoteByDoctor = item.NoteByDoctor,
                    Status = item.Status
                };
                documentViewModelsByDate.Add(doc);
            }

            return Ok(documentViewModelsByDate);
        }
        [HttpGet]
        [Authorize (Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminIndex()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var startDate = today.AddDays(-7);
            var doccument = await _documentService.GetDocument_BHByDateRange(startDate, today);
            List<Document_BH_ViewModel> doccumentViewModel = new List<Document_BH_ViewModel>();
            foreach (var item in doccument)
            {
                // Lấy thông tin nhân viên
                var nhanvienInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(item.DanhSo);
                // Lấy danh sách đơn vị
                var donViList = await _khamSucKhoeService.GetAllDonVi();
                // Mapping đơn vị
                var tenDonVi = donViList?.FirstOrDefault(d => d.ID == nhanvienInfo?.ID_DonVi)?.TenToChuc ?? "Chưa xác định";

                Document_BH_ViewModel doc = new Document_BH_ViewModel
                {
                    Id= item.Id,
                    DanhSo = item.DanhSo,
                    TenNhanVien = nhanvienInfo.HoTen ?? "Chưa xác định",
                    TenDonVi = tenDonVi,
                    TenBenhVien = item.TenBenhVien,
                    DateUpload = item.DateUpload,
                    ThongTin = item.ThongTin,
                    LoaiHinhDieuTri=item.LoaiHinhDieuTri,
                    NgayBatDauKham = item.NgayBatDauKham,
                    NgayKetThucKham = item.NgayKetThucKham,
                    FilesAttachments = item.FilesAttachments,
                    HinhThucKCB = item.HinhThucKCB,
                    NoteByDoctor = item.NoteByDoctor,
                    Status = item.Status,
                };
                doccumentViewModel.Add(doc);
            }          
            return View(doccumentViewModel);
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> AdminUpdateDocument_BH(int id)
        {
            string? danhSo = User.FindFirstValue("DanhSo");
            if (string.IsNullOrEmpty(danhSo))
            {
                // Handle missing DanhSo, e.g., redirect or show error
                return RedirectToAction("Login", "Account");
            }
            Document_BH model = await _documentService.GetDocument_BHById(id);
            Document_BH_EditModel editModel = new Document_BH_EditModel
            {
                Id = model.Id,
                DanhSo = model.DanhSo,
                DateUpload = model.DateUpload,
                TenBenhVien = model.TenBenhVien,
                ThongTin = model.ThongTin,
                NgayBatDauKham = model.NgayBatDauKham,
                NgayKetThucKham = model.NgayKetThucKham,
                LoaiHinhDieuTri = model.LoaiHinhDieuTri,
                DongYSuDung = model.DongYSuDung.ToString(),
                NoteByDoctor = model.NoteByDoctor,
                Status = model.Status,
                FilesAttachments = model.FilesAttachments.ToList()
            };
            //phải check language của người dùng để hiển thị status đúng ngôn ngữ
            var language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;          
            List<string> status;
            status = new List<string>()
                {
                    "TTYT đang kiểm tra hồ sơ",
                    "Bổ sung hồ sơ để tiếp tục xem xét",
                    "Hoàn thành",
                    "TTYT проверяет документы",
                    "Дополнить документы для дальнейшего рассмотрения",
                    "Завершено"
                };
            ViewBag.Status = status.Select(f => new SelectListItem() { Value = f, Text = f });
            return View(editModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Medic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminUpdateDocument_BH(Document_BH_EditModel model)
        {
            if (!Document_BH_EditModelValidator.ValidateTextList(model.FileNames))
            {
                ModelState.AddModelError("FileNames", "Tên file chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!Document_BH_EditModelValidator.ValidateTextList(model.Descriptions))
            {
                ModelState.AddModelError("Descriptions", "Mô tả chỉ được nhập chữ, số, khoảng trắng, dấu . , ; ( )");
            }
            if (!ModelState.IsValid)
            {
                // Collect all error messages from the model state
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, message = "Dữ liệu không hợp lệ.", errors = errorMessages });
            }
            model.DateUpload = DateTime.Now;
            // Đã xóa HtmlEncoder.Default.Encode khi lưu dữ liệu user
            if (model.Descriptions != null)
            {
                for (int i = 0; i < model.Descriptions.Count; i++)
                {
                    model.Descriptions[i] = HtmlEncoder.Default.Encode(model.Descriptions[i] ?? string.Empty);
                }
            }
            var result = await _documentService.AdminUpdateDocument_BH(model);

            if (result)
            {
                return Json(new { success = true, message = "Cập nhật hồ sơ thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Cập nhật hồ sơ thất bại." });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> DocstoExcel(string startDate, string endDate)
        {
            try
            {
                DateOnly? startDateOnly = null;
                DateOnly? endDateOnly = null;
                
                // Parse date strings from frontend (accept both dd/MM/yyyy and yyyy-MM-dd formats)
                if (!string.IsNullOrEmpty(startDate))
                {
                    if (DateOnly.TryParseExact(startDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedStartDate))
                    {
                        startDateOnly = parsedStartDate;
                    }
                    else if (DateOnly.TryParseExact(startDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedStartDate2))
                    {
                        startDateOnly = parsedStartDate2;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Định dạng ngày bắt đầu không hợp lệ (dd/MM/yyyy hoặc yyyy-MM-dd)" });
                    }
                }
                
                if (!string.IsNullOrEmpty(endDate))
                {
                    if (DateOnly.TryParseExact(endDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedEndDate))
                    {
                        endDateOnly = parsedEndDate;
                    }
                    else if (DateOnly.TryParseExact(endDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedEndDate2))
                    {
                        endDateOnly = parsedEndDate2;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Định dạng ngày kết thúc không hợp lệ (dd/MM/yyyy hoặc yyyy-MM-dd)" });
                    }
                }

                // If only one date is provided, treat it as a single-day search
                if (startDateOnly.HasValue && !endDateOnly.HasValue)
                {
                    endDateOnly = startDateOnly;
                }
                else if (!startDateOnly.HasValue && endDateOnly.HasValue)
                {
                    startDateOnly = endDateOnly;
                }
                else if (!startDateOnly.HasValue && !endDateOnly.HasValue)
                {
                    // If no dates provided, use current year
                    startDateOnly = new DateOnly(DateTime.Now.Year, 1, 1);
                    endDateOnly = new DateOnly(DateTime.Now.Year, 12, 31);
                }

                // Validate date range
                if (startDateOnly > endDateOnly)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu không thể sau ngày kết thúc." });
                }

                // Validate date range không quá lớn (tối đa 1 năm)
                var dateRange = endDateOnly.Value.DayNumber - startDateOnly.Value.DayNumber;
                if (dateRange > 365)
                {
                    return Json(new { success = false, message = "Khoảng thời gian không được vượt quá 1 năm." });
                }

                MemoryStream memoryStream = await _documentService.GetDocExcel(startDateOnly.Value, endDateOnly.Value);
                
                return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "docs.xlsx");
            }
            catch (FileNotFoundException ex)
            {
                return Json(new { success = false, message = "Không tìm thấy file template Excel." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xuất Excel: {ex.Message}" });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Medic")]
        public async Task<IActionResult> DocstoExcelforOneUser(int id)
        {
            var language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;
            var document = await _documentService.GetDocument_BHById(id);
            MemoryStream memoryStream = await _documentService.GetDocExcelforOneUser(document);

            //return File(memoryStream, "application/vnd.ms-excel","docs.xls");
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "docs.xls");
        }
    }
}
