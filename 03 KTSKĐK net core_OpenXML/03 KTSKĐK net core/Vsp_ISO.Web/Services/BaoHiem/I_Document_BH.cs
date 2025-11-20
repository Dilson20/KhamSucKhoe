using Microsoft.EntityFrameworkCore;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.BaoHiem;
using VSP_HealthExam.Web.ViewModels;
using VSP_HealthExam.Web.Services.KhamSucKhoe;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

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
                
                // Copy template to memory stream
                using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    await templateStream.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0;

                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(memoryStream, true))
                {
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

                    // Set date in cell I3
                    SetCellValue(workbookPart, worksheetPart, "I3", "Ngày " + DateTime.Now.ToString("dd/MM/yyyy"));

                    if (doccumentViewModel.Count > 0)
                    {
                        foreach (Document_BH_ViewModel docItem in doccumentViewModel)
                        {
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 1), stt.ToString());
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 2), docItem.TenDonVi);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 3), docItem.DanhSo);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 4), docItem.TenNhanVien);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 5), docItem.TenBenhVien);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 6), docItem.ThongTin); // chẩn đoán
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 7), docItem.NgayBatDauKham?.ToString("dd/MM/yyyy"));
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 8), docItem.NgayKetThucKham?.ToString("dd/MM/yyyy"));
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 9), docItem.LoaiHinhDieuTri);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 10), docItem.HinhThucKCB);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 11), docItem.NoteByDoctor);
                            SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 12), docItem.Status);
                            row++;
                            stt++;
                        }
                    }
                    else
                    {
                        // Set message when no data
                        SetCellValue(workbookPart, worksheetPart, "A7", "Không có dữ liệu trong khoảng thời gian này");
                    }

                    workbookPart.Workbook.Save();
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

            // Load template
            string templatePath = Path.Combine(tempFile, "giayxacnhankhaibao Temp.xlsx");
            using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                await templateStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            using (SpreadsheetDocument spreadsheetDoc = SpreadsheetDocument.Open(memoryStream, true))
            {
                WorkbookPart workbookPart = spreadsheetDoc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

                SetCellValue(workbookPart, worksheetPart, "B10", doc.TenNhanVien);
                SetCellValue(workbookPart, worksheetPart, "B12", nhanvienInfo?.GioiTinh ?? "");
                SetCellValue(workbookPart, worksheetPart, "B16", doc.DanhSo);
                SetCellValue(workbookPart, worksheetPart, "B18", doc.TenDonVi);
                SetCellValue(workbookPart, worksheetPart, "B22", doc.TenBenhVien);
                SetCellValue(workbookPart, worksheetPart, "B24", doc.NgayBatDauKham?.ToString("dd/MM/yyyy") + " : " + doc.NgayKetThucKham?.ToString("dd/MM/yyyy"));
                SetCellValue(workbookPart, worksheetPart, "B26", doc.HinhThucKCB);
                SetCellValue(workbookPart, worksheetPart, "B28", doc.ThongTin);
                SetCellValue(workbookPart, worksheetPart, "B30", doc.LoaiHinhDieuTri);
                SetCellValue(workbookPart, worksheetPart, "B46", doc.TenNhanVien);

                workbookPart.Workbook.Save();
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        // Helper methods for OpenXML
        private void SetCellValue(WorkbookPart workbookPart, WorksheetPart worksheetPart, string cellAddress, string value)
        {
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
            Cell cell = GetCell(sheetData, cellAddress);
            
            // Remove old cell value
            cell.CellValue?.Remove();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(value ?? "");
        }

        private Cell GetCell(SheetData sheetData, string cellAddress)
        {
            UInt32 row = GetRowIndex(cellAddress);
            string col = GetColumnName(cellAddress);

            Row targetRow = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == row);
            if (targetRow == null)
            {
                targetRow = new Row { RowIndex = row };
                sheetData.AppendChild(targetRow);
            }

            Cell cell = targetRow.Elements<Cell>().FirstOrDefault(c => c.CellReference == cellAddress);
            if (cell == null)
            {
                cell = new Cell { CellReference = cellAddress };
                targetRow.AppendChild(cell);
            }

            return cell;
        }

        private UInt32 GetRowIndex(string cellAddress)
        {
            return UInt32.Parse(System.Text.RegularExpressions.Regex.Replace(cellAddress, "[^0-9]", ""));
        }

        private string GetColumnName(string cellAddress)
        {
            return System.Text.RegularExpressions.Regex.Replace(cellAddress, "[^A-Z]", "");
        }

        private string GetCellAddress(int row, int column)
        {
            string colName = "";
            int col = column;
            while (col > 0)
            {
                col--;
                colName = Convert.ToChar(col % 26 + 65) + colName;
                col /= 26;
            }
            return colName + row;
        }

    }
}
