using Microsoft.EntityFrameworkCore;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.BaoHiem;
using VSP_HealthExam.Web.ViewModels;
using VSP_HealthExam.Web.Services.KhamSucKhoe;

namespace VSP_HealthExam.Web.Services.BaoHiem
{
    public interface I_Document_BH
    {
        Task<List<Document_BH>?> GetDocument_BHAll();
        Task<Document_BH>? GetDocument_BHById(int id);
        Task<List<Document_BH>?> GetDocument_BHByDanhSo(string danhso);
        Task<bool> CreateDocument_BHWithAttachment(Document_BH_EditModel model, string? culture = null);
        Task<bool> UpdateDocument_BH(Document_BH_EditModel model);
        Task<bool> AdminUpdateDocument_BH(Document_BH_EditModel model);
        Task<List<Document_BH>?> GetDocument_BHByDateRange(DateOnly startDate, DateOnly endDate);
        Task<MemoryStream> GetDocExcel(DateOnly startDate, DateOnly endDate);
        Task<MemoryStream> GetDocExcelforOneUser(Document_BH docToExcel);
    }
    public class Document_BH_Service : I_Document_BH
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly I_KhamSucKhoe _khamSucKhoeService;
        string directoryStorage = "";
        string outputDirectoryStorage = "";
        string tempFile = "";
        // Security configuration
        private readonly string[] _allowedFileExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".zip", ".rar", ".7z" };
        private readonly long _maxFileSize = 20 * 1024 * 1024; // 10MB
        private readonly string[] _allowedMimeTypes = {
            "application/pdf", 
            "application/msword", 
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "image/jpeg", 
            "image/png", 
            "image/gif", 
            "image/bmp", 
            "image/tiff",
            "application/zip",
            "application/x-zip-compressed",
            "application/x-rar-compressed",
            "application/x-7z-compressed",
            "application/octet-stream"
        };
        public Document_BH_Service(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, I_KhamSucKhoe khamSucKhoeService)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _khamSucKhoeService = khamSucKhoeService;
            directoryStorage = _configuration.GetValue<string>("directoryStorage");
            outputDirectoryStorage = _configuration.GetValue<string>("outputDirectoryStorage");
            tempFile = _configuration.GetValue<string>("tempFile");

            if (directoryStorage == null)
            {
                directoryStorage = Path.Combine(_webHostEnvironment.WebRootPath, "Documents","UploadFile");
                outputDirectoryStorage = Path.Combine(_webHostEnvironment.WebRootPath, "Documents", "TempFile");
                tempFile = Path.Combine(_webHostEnvironment.WebRootPath, "Documents");
                if (!Directory.Exists(directoryStorage))
                {
                    Directory.CreateDirectory(directoryStorage);
                    Directory.CreateDirectory(Path.Combine(directoryStorage, "UploadFile"));
                }
            }
        }
        public async Task<List<Document_BH>?> GetDocument_BHAll()
        {
            var result = _context.Document_BHs.ToList();
            return result;
        }
        public async Task<Document_BH>? GetDocument_BHById(int id)
        {
            var result = await _context.Document_BHs.Where(x => x.Id == id)
                 .Include(s => s.FilesAttachments)
                 .FirstOrDefaultAsync();
            return result;
        }
        public async Task<List<Document_BH>?> GetDocument_BHByDateRange(DateOnly startDate, DateOnly endDate)
        {
            // Convert DateOnly to DateTime for comparison
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue); // Start of startDate
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);   // End of endDate

            var result = await _context.Document_BHs
                .Where(x => x.DateUpload.HasValue &&
                            x.DateUpload.Value >= startDateTime &&
                            x.DateUpload.Value <= endDateTime)
                .Include(s => s.FilesAttachments)
                .ToListAsync();

            return result;
        }

        public async Task<List<Document_BH>?> GetDocument_BHByDanhSo(string danhso)
        {
            var result = await _context.Document_BHs
                .Where(x => x.DanhSo == danhso && x.DateUpload.HasValue && x.DateUpload.Value.Year == DateTime.Now.Year)
                .Include(s => s.FilesAttachments)
                .ToListAsync();
            return result;
        }
        public async Task<bool> CreateDocument_BHWithAttachment(Document_BH_EditModel model, string? culture = null)
        {
            if (model == null)
                return false;

            // Handle file upload first
            List<FilesAttachment> fileAttachments = new();
            if (model.Files != null && model.Files.Any())
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    var file = model.Files[i];
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!_allowedFileExtensions.Contains(ext))
                            throw new Exception($"File extension {ext} không được phép.");
                        if (file.Length > _maxFileSize)
                            throw new Exception($"File vượt quá dung lượng cho phép (10MB).");
                        if (!_allowedMimeTypes.Contains(file.ContentType))
                            throw new Exception($"Mime type {file.ContentType} không hợp lệ.");
                        // Generate unique file name
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName((file.FileName))}";
                        var filePath = Path.Combine(directoryStorage, fileName);
                        // Save file to disk
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await (file.CopyToAsync(stream));
                        }
                        // Create FilesAttachment object
                        var fileAttachment = new FilesAttachment
                        {
                            DanhSo = model.DanhSo,
                            CreatedDate = DateTime.Now,
                            FileName = fileName,
                            Description = model.Descriptions[i]
                        };
                        fileAttachments.Add(fileAttachment);
                    }
                }
            }

            // Determine status based on culture
            string status;
            if (culture == "ru")
            {
                status = "Сотрудник отправил документы";
            }
            else
            {
                status = "Nhân viên đã gửi hồ sơ";
            }

            Document_BH newmodel = new()
            {
                DanhSo = model.DanhSo,
                DateUpload = model.DateUpload,
                TenBenhVien = model.TenBenhVien,
                ThongTin = model.ThongTin,
                NgayBatDauKham = model.NgayBatDauKham,
                NgayKetThucKham = model.NgayKetThucKham,
                LoaiHinhDieuTri = model.LoaiHinhDieuTri,
                DongYSuDung = model.DongYSuDung,
                FilesAttachments = fileAttachments,
                HinhThucKCB = model.HinhThucKCB,
                Status = status
            };

            _context.Document_BHs.Add(newmodel);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateDocument_BH(Document_BH_EditModel model)
        {
            if (model == null)
                return false;
            var existingDocument = await _context.Document_BHs.FindAsync(model.Id);
            if (existingDocument == null)
                return false;
            // Update properties
            //existingDocument.DanhSo = model.DanhSo; no update
            //existingDocument.DateUpload = model.DateUpload;
            existingDocument.TenBenhVien = model.TenBenhVien;
            existingDocument.ThongTin = model.ThongTin;
            existingDocument.NgayBatDauKham = model.NgayBatDauKham;
            existingDocument.NgayKetThucKham = model.NgayKetThucKham;
            existingDocument.LoaiHinhDieuTri = model.LoaiHinhDieuTri;
            existingDocument.HinhThucKCB = model.HinhThucKCB;
            existingDocument.DongYSuDung = model.DongYSuDung;

            // Handle file attachments
            if (model.Files != null && model.Files.Any())
            {
                // Delete old files from disk if they exist
                if (existingDocument.FilesAttachments != null && existingDocument.FilesAttachments.Any())
                {
                    foreach (var oldAttachment in existingDocument.FilesAttachments)
                    {
                        if (!string.IsNullOrEmpty(oldAttachment.FileName))
                        {
                            var oldFilePath = Path.Combine(directoryStorage, oldAttachment.FileName);
                            if (File.Exists(oldFilePath))
                            {
                                File.Delete(oldFilePath);
                            }
                        }
                    }
                }

                List<FilesAttachment> fileAttachments = new();
                if (model.Files != null && model.Files.Any())
                {
                    for (int i = 0; i < model.Files.Count; i++)
                    {
                        var file = model.Files[i];
                        if (file.Length > 0)
                        {
                            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                            if (!_allowedFileExtensions.Contains(ext))
                                throw new Exception($"File extension {ext} không được phép.");
                            if (file.Length > _maxFileSize)
                                throw new Exception($"File vượt quá dung lượng cho phép (10MB).");
                            if (!_allowedMimeTypes.Contains(file.ContentType))
                                throw new Exception($"Mime type {file.ContentType} không hợp lệ.");
                            // Generate unique file name
                            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName((file.FileName))}";
                            var filePath = Path.Combine(directoryStorage, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await (file.CopyToAsync(stream));
                            }
                            var fileAttachment = new FilesAttachment
                            {
                                DanhSo = model.DanhSo,
                                CreatedDate = DateTime.Now,
                                FileName = fileName,
                                Description = model.Descriptions[i]
                            };
                            fileAttachments.Add(fileAttachment);
                        }
                    }
                }
                existingDocument.FilesAttachments = fileAttachments;
            }
            _context.Document_BHs.Update(existingDocument);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AdminUpdateDocument_BH(Document_BH_EditModel model)
        {
            if (model == null)
                return false;
            var existingDocument = await _context.Document_BHs.FindAsync(model.Id);
            if (existingDocument == null)
                return false;
            // Admin Update properties
            existingDocument.NoteByDoctor = model.NoteByDoctor;
            existingDocument.Status = model.Status;
            List<FilesAttachment> fileAttachments = new();
            if (model.Files != null && model.Files.Any())
            {
                for (int i = 0; i < model.Files.Count; i++)
                {
                    var file = model.Files[i];
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!_allowedFileExtensions.Contains(ext))
                            throw new Exception($"File extension {ext} không được phép.");
                        if (file.Length > _maxFileSize)
                            throw new Exception($"File vượt quá dung lượng cho phép (20MB).");
                        if (!_allowedMimeTypes.Contains(file.ContentType))
                            throw new Exception($"Mime type {file.ContentType} không hợp lệ.");
                        // Generate unique file name
                        var fileName = $"'Hoàn thành'_{Guid.NewGuid()}_{Path.GetFileName((file.FileName))}";
                        var filePath = Path.Combine(directoryStorage, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await (file.CopyToAsync(stream));
                        }
                        var fileAttachment = new FilesAttachment
                        {
                            DanhSo = model.DanhSo,
                            CreatedDate = DateTime.Now,
                            FileName = fileName,
                            Description = "File trả kết quả cho nhân viên"
                        };
                        fileAttachments.Add(fileAttachment);
                    }
                }
            }
            existingDocument.FilesAttachments = fileAttachments;
            _context.Document_BHs.Update(existingDocument);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<MemoryStream> GetDocExcel(DateOnly startDate, DateOnly endDate)
        {
            MemoryStream memoryStream = new MemoryStream();
            
            try
            {
                using (Spire.Xls.Workbook excelPackage = new Spire.Xls.Workbook())
                {
                    int row = 7;
                    int stt = 1;
                    // Convert DateOnly to DateTime for comparison
                    var startDateTime = startDate.ToDateTime(TimeOnly.MinValue); // Start of startDate
                    var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);   // End of endDate

                    var result = await _context.Document_BHs
                        .Where(x => x.DateUpload.HasValue &&
                                    x.DateUpload.Value >= startDateTime &&
                                    x.DateUpload.Value <= endDateTime)
                        .Include(s => s.FilesAttachments)
                        .ToListAsync();

                    List<Document_BH_ViewModel> doccumentViewModel = new List<Document_BH_ViewModel>();
                    foreach (var item in result)
                    {
                        // Lấy thông tin nhân viên
                        var nhanvienInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(item.DanhSo);
                        // Lấy lại danh sách đơn vị ở block xuất Excel
                        var donViList = await _khamSucKhoeService.GetAllDonVi();
                        // Mapping đơn vị, ép kiểu an toàn
                        int nhanvienDonViId = 0;
                        if (nhanvienInfo?.ID_DonVi != null)
                        {
                            int.TryParse(nhanvienInfo.ID_DonVi.ToString(), out nhanvienDonViId);
                        }
                        var tenDonVi = donViList?.FirstOrDefault(d => d.ID == nhanvienDonViId)?.TenToChuc ?? "Chưa xác định";

                        // Validate và escape dữ liệu trước khi xuất Excel
                        Document_BH_ViewModel doc = new Document_BH_ViewModel
                        {
                            Id = item.Id,
                            DanhSo = ValidateAndEscapeText(item.DanhSo),
                            TenNhanVien = ValidateAndEscapeText(nhanvienInfo?.HoTen ?? "Chưa xác định"),
                            TenDonVi = ValidateAndEscapeText(tenDonVi),
                            TenBenhVien = ValidateAndEscapeText(item.TenBenhVien),
                            DateUpload = item.DateUpload,
                            ThongTin = ValidateAndEscapeText(item.ThongTin),
                            LoaiHinhDieuTri = ValidateAndEscapeText(item.LoaiHinhDieuTri),
                            NgayBatDauKham = item.NgayBatDauKham,
                            NgayKetThucKham = item.NgayKetThucKham,
                            FilesAttachments = item.FilesAttachments,
                            HinhThucKCB = ValidateAndEscapeText(item.HinhThucKCB),
                            NoteByDoctor = ValidateAndEscapeText(item.NoteByDoctor),
                            Status = ValidateAndEscapeText(item.Status),
                        };
                        doccumentViewModel.Add(doc);
                    }
                    
                    // Kiểm tra file template có tồn tại không
                    string templatePath = Path.Combine(tempFile, "Template VN.xlsx");
                    if (!File.Exists(templatePath))
                    {
                        throw new FileNotFoundException($"Không tìm thấy file template: {templatePath}");
                    }
                    
                    if (doccumentViewModel.Count > 0)
                    {
                        excelPackage.LoadFromFile(templatePath);
                        Spire.Xls.Worksheet workSheet = excelPackage.Worksheets[0];

                        workSheet.Range["I3"].Value = "Ngày " + DateTime.Now.ToString("dd/MM/yyyy");

                        foreach (Document_BH_ViewModel doc in doccumentViewModel)
                        {
                            workSheet.Range[row, 1].Value = stt.ToString();
                            workSheet.Range[row, 2].Value = doc.TenDonVi;
                            workSheet.Range[row, 3].Value = doc.DanhSo;
                            workSheet.Range[row, 4].Value = doc.TenNhanVien;
                            workSheet.Range[row, 5].Value = doc.TenBenhVien;
                            workSheet.Range[row, 6].Value = doc.ThongTin;// chẩn đoán
                            workSheet.Range[row, 7].Value = doc.NgayBatDauKham?.ToString("dd/MM/yyyy");
                            workSheet.Range[row, 8].Value = doc.NgayKetThucKham?.ToString("dd/MM/yyyy");
                            workSheet.Range[row, 9].Value = doc.LoaiHinhDieuTri;
                            workSheet.Range[row, 10].Value = doc.HinhThucKCB;
                            workSheet.Range[row, 11].Value = doc.NoteByDoctor;
                            workSheet.Range[row, 12].Value = doc.Status;
                            row++;
                            stt++;
                        }
                    }
                    else
                    {
                        // Tạo file Excel trống với thông báo
                        excelPackage.LoadFromFile(templatePath);
                        Spire.Xls.Worksheet workSheet = excelPackage.Worksheets[0];
                        workSheet.Range["A7"].Value = "Không có dữ liệu trong khoảng thời gian này";
                    }
                    
                    excelPackage.SaveToStream(memoryStream);
                }
                
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo file Excel: {ex.Message}", ex);
            }
        }

        // Helper method để validate và escape text
        private string ValidateAndEscapeText(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // Kiểm tra có chứa ký tự HTML/script không
            if (text.Contains('<') || text.Contains('>') || text.Contains('"') || 
                text.Contains('\'') || text.Contains('&') || text.Contains('/'))
            {
                // Nếu có ký tự nguy hiểm, thay thế bằng dấu cách
                text = text.Replace("<", " ")
                          .Replace(">", " ")
                          .Replace("\"", " ")
                          .Replace("'", " ")
                          .Replace("&", " ")
                          .Replace("/", " ");
            }

            // Loại bỏ khoảng trắng thừa
            return text.Trim();
        }
        public async Task<MemoryStream> GetDocExcelforOneUser(Document_BH docToExcel)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (Spire.Xls.Workbook excelPackage = new Spire.Xls.Workbook())
            {
                // Ensure docToExcel and danhSo are not null
                if (docToExcel == null || string.IsNullOrEmpty(docToExcel.DanhSo))
                {
                    return memoryStream;
                }

                // Get employee info, handle possible nulls
                var nhanvienInfo = await _khamSucKhoeService.GetInfoNhanVienNOPassword(docToExcel.DanhSo);
                // Lấy lại danh sách đơn vị ở block xuất Excel
                var donViList = await _khamSucKhoeService.GetAllDonVi();
                // Mapping đơn vị, đảm bảo so sánh đúng kiểu
                int nhanvienDonViId = 0;
                if (nhanvienInfo?.ID_DonVi != null)
                {
                    int.TryParse(nhanvienInfo.ID_DonVi.ToString(), out nhanvienDonViId);
                }
                var tenDonVi = donViList?.FirstOrDefault(d => d.ID == nhanvienDonViId)?.TenToChuc ?? "Chưa xác định";

                var doc = new Document_BH_ViewModel
                {
                    Id = docToExcel.Id,
                    DanhSo = docToExcel.DanhSo,
                    TenNhanVien = nhanvienInfo?.HoTen ?? "Chưa xác định",
                    TenDonVi = tenDonVi,
                    TenBenhVien = docToExcel.TenBenhVien,
                    DateUpload = docToExcel.DateUpload,
                    ThongTin = docToExcel.ThongTin,
                    LoaiHinhDieuTri = docToExcel.LoaiHinhDieuTri,
                    NgayBatDauKham = docToExcel.NgayBatDauKham,
                    NgayKetThucKham = docToExcel.NgayKetThucKham,
                    FilesAttachments = docToExcel.FilesAttachments,
                    HinhThucKCB = docToExcel.HinhThucKCB,
                    NoteByDoctor = docToExcel.NoteByDoctor,
                    Status = docToExcel.Status,
                };

                // Only export if doc is valid
                excelPackage.LoadFromFile(Path.Combine(tempFile, "giayxacnhankhaibao Temp.xlsx"));
                Spire.Xls.Worksheet workSheet = excelPackage.Worksheets[0];

                workSheet.Range["B10"].Value = doc.TenNhanVien;
                workSheet.Range["B12"].Value = nhanvienInfo?.GioiTinh ?? "";
                // workSheet.Range["B14"].Value = nhanvienInfo?.NgaySinh?.ToString("dd/MM/yyyy"); // Đã xóa property NgaySinh
                workSheet.Range["B16"].Value = doc.DanhSo;
                workSheet.Range["B18"].Value = doc.TenDonVi;
                workSheet.Range["B22"].Value = doc.TenBenhVien;
                workSheet.Range["B24"].Value = doc.NgayBatDauKham?.ToString("dd/MM/yyyy")+" : "+ doc.NgayKetThucKham?.ToString("dd/MM/yyyy");
                workSheet.Range["B26"].Value = doc.HinhThucKCB;
                workSheet.Range["B28"].Value = doc.ThongTin;
                workSheet.Range["B30"].Value = doc.LoaiHinhDieuTri;
                workSheet.Range["B46"].Value = doc.TenNhanVien;

                excelPackage.SaveToStream(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

    }
}
