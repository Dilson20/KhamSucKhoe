using Microsoft.EntityFrameworkCore;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.KhamSucKhoe;
using Microsoft.Extensions.Localization;
using VSP_HealthExam.Web;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;

namespace VSP_HealthExam.Web.Services.KhamSucKhoe
{
    public interface I_KhamSucKhoe
    {
        Task<View_NhanVien_All?> GetInfoNhanVien(string danhSo, string password);
        Task<View_NhanVien_All?> GetInfoNhanVienNOPassword(string danhSo);
        Task<List<DepartmentTree>?> GetAllDonVi();
        Task<List<KSK_LoaiNhom>> GetAllLoaiNhom();
        Task<LichKhamSucKhoe> CreateLichKhamSucKhoe(Lich_KSK_EditModel model);
        Task<LichKhamSucKhoe> UpdateLichKhamSucKhoe(Lich_KSK_EditModel model);
        Task<bool> DeleteLichKhamSucKhoe(int id);
        Task<List<LichKhamSucKhoe>> GetAllLichKhamSucKhoe();
        Task<LichKhamSucKhoe?> GetLichKhamSucKhoeById(int id);
        Task<List<LichKhamSucKhoe>> GetLichKhamSucKhoeByDate(DateTime date);
        Task<List<object>> GetLichKhamByDonVi(int donViId);
        Task<KSK_NhanVien_DangKy> CreateNhanVienDangKy(KSK_NhanVien_DangKy_EditModel model);
        Task<List<object>> GetLichDaDangKy(string danhSo, IStringLocalizer<SharedResource> localizer);
        Task<bool> KiemTraDaDangKyLoaiNhom(string danhSo, int loaiNhomId);
        Task<List<int>> GetLoaiNhomDaDangKy(string danhSo);
        Task<KSK_LoaiNhom?> GetLoaiNhomById(int id);
        Task<int> GetSoLuongDaDangKyByLichId(int lichId);
        Task<byte[]> ExportKSKToExcel(DateTime? fromDate, DateTime? toDate, int? donviId, int? nhomId);
        Task<byte[]> ExportChuaDangKyKSKToExcel(int? donviId);
        Task<List<View_NhanVien_All>> ViewNhanVienAll(int? donViId = null, string? searchTerm = null);
    }
    public class KhamSucKhoe_Service : I_KhamSucKhoe
    {
        private readonly SubDbContext _db;
        private readonly ApplicationDbContext _appDb;
        public KhamSucKhoe_Service(SubDbContext db, ApplicationDbContext appDb)
        {
            _db = db;
            _appDb = appDb;
        }
        public async Task<List<DepartmentTree>> GetAllDepartmentTree()
        {
            return await _db.DepartmentTree.ToListAsync();
        }
        public async Task<List<View_DanhBa_NhanVien>> GetAllNhanVien()
        {
            return await _db.View_DanhBa_NhanVien.ToListAsync();
        }
        public async Task<View_NhanVien_All?> GetInfoNhanVien(string danhSo, string password)
        {
            var nhanVien = await _db.View_DanhBa_NhanVien.Where(x => x.DanhSo == danhSo).FirstOrDefaultAsync();
            if (nhanVien != null && nhanVien.UserPassword == password)
            {
                var checkDonVi = await _db.DepartmentTree.Where(x => x.ID == nhanVien.BoPhan_id).FirstOrDefaultAsync();
                if (checkDonVi != null && checkDonVi.ID_DonVi.HasValue)
                {
                    var donVi = await _db.DepartmentTree.Where(x => x.ID == checkDonVi.ID_DonVi.Value).FirstOrDefaultAsync();

                    return new View_NhanVien_All
                    {
                        BoPhan_id = nhanVien.BoPhan_id,
                        NhanVien_id = nhanVien.NhanVien_id,
                        DanhSo = nhanVien.DanhSo,
                        HoTen = nhanVien.HoTen,
                        HoTen_ru = nhanVien.HoTen_ru,
                        Phone_CQ = nhanVien.Phone_CQ,
                        Mobile = nhanVien.Mobile,
                        Email = nhanVien.Email,
                        ChucDanh = nhanVien.ChucDanh,
                        ChucDanh_ru = nhanVien.ChucDanh_ru,
                        Loai_CBCNV = nhanVien.Loai_CBCNV,
                        DonVi = donVi.TenTat ?? donVi.TenToChuc,
                        GioiTinh = nhanVien.GioiTinh,
                        DonVi_ru = donVi.TenToChuc_ru,
                        ID_DonVi = donVi.ID_DonVi
                    };
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
        public async Task<View_NhanVien_All?> GetInfoNhanVienNOPassword(string danhSo)
        {
            var nhanVien = await _db.View_DanhBa_NhanVien.Where(x => x.DanhSo == danhSo).FirstOrDefaultAsync();
            if (nhanVien != null)
            {
                var checkDonVi = await _db.DepartmentTree.Where(x => x.ID == nhanVien.BoPhan_id).FirstOrDefaultAsync();
                if (checkDonVi != null && checkDonVi.ID_DonVi.HasValue)
                {
                    var donVi = await _db.DepartmentTree.Where(x => x.ID == checkDonVi.ID_DonVi.Value).FirstOrDefaultAsync();

                    return new View_NhanVien_All
                    {
                        BoPhan_id = nhanVien.BoPhan_id,
                        NhanVien_id = nhanVien.NhanVien_id,
                        DanhSo = nhanVien.DanhSo,
                        HoTen = nhanVien.HoTen,
                        HoTen_ru = nhanVien.HoTen_ru,
                        Phone_CQ = nhanVien.Phone_CQ,
                        Mobile = nhanVien.Mobile,
                        Email = nhanVien.Email,
                        ChucDanh = nhanVien.ChucDanh,
                        ChucDanh_ru = nhanVien.ChucDanh_ru,
                        Loai_CBCNV = nhanVien.Loai_CBCNV,
                        DonVi = donVi.TenTat ?? donVi.TenToChuc,
                        GioiTinh = nhanVien.GioiTinh,
                        DonVi_ru = donVi.TenToChuc_ru,
                        ID_DonVi = donVi.ID_DonVi
                    };
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
        public async Task<List<DepartmentTree>?> GetAllDonVi()
        { 
            var listDonVi = await _db.DepartmentTree.Where(x => x.Id_Cha == null && x.TamNgung == false).ToListAsync();
            if (listDonVi != null && listDonVi.Count > 0)
            {
                return listDonVi;
            }
            else return null;
        }
        public async Task<List<KSK_LoaiNhom>> GetAllLoaiNhom()
        {
            return await _appDb.KSK_LoaiNhom.ToListAsync();
        }
        public async Task<LichKhamSucKhoe> CreateLichKhamSucKhoe(Lich_KSK_EditModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model), "LichKhamSucKhoe model cannot be null");
            }

            // Kiểm tra thời gian kết thúc phải sau thời gian bắt đầu
            if (model.ThoiGianKetThuc <= model.ThoiGianBatDau)
            {
                throw new InvalidOperationException("Thời gian kết thúc phải sau thời gian bắt đầu.");
            }
            
            // Kiểm tra số lượng phải lớn hơn 0
            if (model.SoLuong <= 0)
            {
                throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
            }

            var lichKham = new LichKhamSucKhoe
            {
                DonViId = model.DonViId,
                ThoiGianBatDau = model.ThoiGianBatDau,
                ThoiGianKetThuc = model.ThoiGianKetThuc,
                SoLuong = model.SoLuong,
                GhiChu = model.GhiChu,
                LoaiNhom = model.LoaiNhom,
                Creator = model.Creator
            };
            _appDb.LichKhamSucKhoe.Add(lichKham);
            await _appDb.SaveChangesAsync();
            return lichKham;
        }
        public async Task<LichKhamSucKhoe> UpdateLichKhamSucKhoe(Lich_KSK_EditModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model), "LichKhamSucKhoe model cannot be null");
            }
            var lichKham = await _appDb.LichKhamSucKhoe.FindAsync(model.Id);
            if (lichKham == null)
            {
                throw new KeyNotFoundException($"LichKhamSucKhoe with Id {model.Id} not found.");
            }
            
            // Kiểm tra số lượng đã đăng ký - không cho phép giảm xuống dưới số đã đăng ký
            var soLuongDaDangKy = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.LichKhamSucKhoeId == model.Id)
                .CountAsync();
            
            if (model.SoLuong < soLuongDaDangKy)
            {
                throw new InvalidOperationException($"Không thể giảm số lượng xuống {model.SoLuong} vì đã có {soLuongDaDangKy} người đăng ký.");
            }
            
            // Kiểm tra thời gian kết thúc phải sau thời gian bắt đầu
            if (model.ThoiGianKetThuc <= model.ThoiGianBatDau)
            {
                throw new InvalidOperationException("Thời gian kết thúc phải sau thời gian bắt đầu.");
            }
            
            // Kiểm tra số lượng phải lớn hơn 0
            if (model.SoLuong <= 0)
            {
                throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
            }
            
            lichKham.ThoiGianBatDau = model.ThoiGianBatDau;
            lichKham.ThoiGianKetThuc = model.ThoiGianKetThuc;
            lichKham.SoLuong = model.SoLuong;
            lichKham.GhiChu = model.GhiChu;
            lichKham.LoaiNhom = model.LoaiNhom;
            lichKham.UpdatePerson = model.UpdatePerson;

            _appDb.LichKhamSucKhoe.Update(lichKham);
            await _appDb.SaveChangesAsync();
            
            return lichKham;
        }
        public async Task<bool> DeleteLichKhamSucKhoe(int id)
        {
            // Lấy lịch khám và các đăng ký liên quan
            var lichKham = await _appDb.LichKhamSucKhoe
                .Include(x => x.KSK_NhanVien_DangKys)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (lichKham == null)
            {
                return false; // Không tìm thấy
            }

            // Xóa các đăng ký nhân viên liên quan
            if (lichKham.KSK_NhanVien_DangKys != null && lichKham.KSK_NhanVien_DangKys.Any())
            {
                _appDb.KSK_NhanVien_DangKy.RemoveRange(lichKham.KSK_NhanVien_DangKys);
            }

            // Xóa lịch khám
            _appDb.LichKhamSucKhoe.Remove(lichKham);
            await _appDb.SaveChangesAsync();
            return true;
        }
        public async Task<List<LichKhamSucKhoe>> GetAllLichKhamSucKhoe()
        {
            return await _appDb.LichKhamSucKhoe.ToListAsync();
        }
        public async Task<LichKhamSucKhoe?> GetLichKhamSucKhoeById(int id)
        {
            return await _appDb.LichKhamSucKhoe.FindAsync(id);
        }
        public async Task<List<LichKhamSucKhoe>> GetLichKhamSucKhoeByDate(DateTime date)
        {
            return await _appDb.LichKhamSucKhoe
                .Where(x =>  x.ThoiGianBatDau.Date == date.Date )
                .ToListAsync();
        }

        public async Task<List<object>> GetLichKhamByDonVi(int donViId)
        {
            var lichKhamList = await _appDb.LichKhamSucKhoe
                .Where(x => x.DonViId == donViId && x.ThoiGianBatDau > DateTime.Now)
                .Include(x => x.LoaiNhomNavigation)
                .OrderBy(x => x.ThoiGianBatDau)
                .ToListAsync();

            var result = new List<object>();
            
            foreach (var lich in lichKhamList)
            {
                // Đếm số lượng đã đăng ký cho lịch này
                var soLuongDaDangKy = await _appDb.KSK_NhanVien_DangKy
                    .Where(x => x.LichKhamSucKhoeId == lich.Id)
                    .CountAsync();

                // Tính số slot trống
                var soSlotTrong = lich.SoLuong - soLuongDaDangKy;

                result.Add(new
                {
                    id = lich.Id,
                    donViId = lich.DonViId,
                    thoiGianBatDau = lich.ThoiGianBatDau,
                    thoiGianKetThuc = lich.ThoiGianKetThuc,
                    soLuong = lich.SoLuong,
                    soLuongDaDangKy = soLuongDaDangKy,
                    soSlotTrong = soSlotTrong,
                    ghiChu = lich.GhiChu,
                    loaiNhom = lich.LoaiNhom,
                    tenLoaiNhom = lich.LoaiNhomNavigation?.TenNhom ?? "Không xác định"
                });
            }

            return result;
        }

        public async Task<KSK_NhanVien_DangKy> CreateNhanVienDangKy(KSK_NhanVien_DangKy_EditModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "KSK_NhanVien_DangKy model cannot be null");
            }

            // Kiểm tra xem nhân viên đã đăng ký lịch này chưa
            var existingRegistration = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.DanhSo == model.DanhSo && x.LichKhamSucKhoeId == model.LichKhamSucKhoeId)
                .FirstOrDefaultAsync();

            if (existingRegistration != null)
            {
                throw new InvalidOperationException("Nhân viên đã đăng ký lịch khám sức khỏe này rồi");
            }

            // Kiểm tra giới hạn số lượng đăng ký
            var lichKham = await _appDb.LichKhamSucKhoe
                .Where(x => x.Id == model.LichKhamSucKhoeId)
                .FirstOrDefaultAsync();

            if (lichKham == null)
            {
                throw new InvalidOperationException("Không tìm thấy lịch khám sức khỏe");
            }

            // Đếm số lượng đã đăng ký cho lịch này
            var soLuongDaDangKy = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.LichKhamSucKhoeId == model.LichKhamSucKhoeId)
                .CountAsync();

            // Kiểm tra xem có vượt quá giới hạn không
            if (soLuongDaDangKy >= lichKham.SoLuong)
            {
                throw new InvalidOperationException($"Lịch khám này đã đạt giới hạn tối đa ({lichKham.SoLuong} người). Không thể đăng ký thêm.");
            }

            var nhanVienDangKy = new KSK_NhanVien_DangKy
            {
                DonViId = model.DonViId,
                DanhSo = model.DanhSo,
                LichKhamSucKhoeId = model.LichKhamSucKhoeId,
                YeuCauDacBiet = model.YeuCauDacBiet,
                CreateDateTime = DateTime.Now,
                UpdateDateTime = null,
                UpdatePerson = null
            };

            try
            {
                _appDb.KSK_NhanVien_DangKy.Add(nhanVienDangKy);
                await _appDb.SaveChangesAsync();
                return nhanVienDangKy;
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                throw new InvalidOperationException($"Lỗi khi lưu đăng ký KSK: {ex.Message}", ex);
            }
        }

        public async Task<List<object>> GetLichDaDangKy(string danhSo, IStringLocalizer<SharedResource> localizer)
        {
            
            var danhSachDangKy = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.DanhSo == danhSo)
                .Include(x => x.LichKhamSucKhoe)
                .ThenInclude(x => x.LoaiNhomNavigation)
                .OrderBy(x => x.LichKhamSucKhoe.ThoiGianBatDau)
                .ToListAsync();

            
            // Lấy danh sách DonViId
            var donViIds = danhSachDangKy.Select(x => x.LichKhamSucKhoe.DonViId).Distinct().ToList();
            var donViDict = await _db.DepartmentTree.Where(x => donViIds.Contains(x.ID)).ToDictionaryAsync(x => x.ID, x => x.TenTat ?? x.TenToChuc);

            var result = new List<object>();
            
            foreach (var dangKy in danhSachDangKy)
            {
                var lichKham = dangKy.LichKhamSucKhoe;
                var loaiNhom = lichKham.LoaiNhomNavigation;
                
                var thoiGianBatDau = lichKham.ThoiGianBatDau;
                var thoiGianKetThuc = lichKham.ThoiGianKetThuc;
                var tenDonVi = donViDict.ContainsKey(lichKham.DonViId) ? donViDict[lichKham.DonViId] : localizer["KSK.Status_Unknown"];

                // Đa ngôn ngữ hóa tên loại nhóm
                string tenLoaiNhom = localizer["KSK.Status_Unknown"];
                if (loaiNhom != null)
                {
                    switch (loaiNhom.Id)
                    {
                        case 1:
                            tenLoaiNhom = localizer["KSK.ExaminationType_Periodic"];
                            break;
                        case 2:
                            tenLoaiNhom = localizer["KSK.ExaminationType_Leadership"];
                            break;
                        case 3:
                            tenLoaiNhom = localizer["KSK.ExaminationType_WomenSpecialized"];
                            break;
                        case 4:
                            tenLoaiNhom = localizer["KSK.ExaminationType_Cardiovascular"];
                            break;
                        case 5:
                            tenLoaiNhom = localizer["KSK.ExaminationType_Vaccine"];
                            break;
                        case 6:
                            tenLoaiNhom = localizer["KSK.ExaminationType_HealthCheck"];
                            break;
                        default:
                            tenLoaiNhom = loaiNhom.TenNhom ?? localizer["KSK.Status_Unknown"];
                            break;
                    }
                }

                result.Add(new
                {
                    id = dangKy.Id,
                    lichKhamId = lichKham.Id,
                    thoiGianBatDau = lichKham.ThoiGianBatDau,
                    thoiGianKetThuc = lichKham.ThoiGianKetThuc,
                    loaiNhom = lichKham.LoaiNhom,
                    tenLoaiNhom = tenLoaiNhom,
                    yeuCauDacBiet = dangKy.YeuCauDacBiet,
                    ghiChu = lichKham.GhiChu,
                    trangThai = localizer["KSK.Status_Registered"].ToString(),
                    tenDonVi = tenDonVi
                });
            }

            return result;
        }

        public async Task<bool> KiemTraDaDangKyLoaiNhom(string danhSo, int loaiNhomId)
        {
            
            // Kiểm tra xem user đã đăng ký loại nhóm này chưa
            var daDangKy = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.DanhSo == danhSo)
                .Include(x => x.LichKhamSucKhoe)
                .AnyAsync(x => x.LichKhamSucKhoe.LoaiNhom == loaiNhomId);

            return daDangKy;
        }

        public async Task<List<int>> GetLoaiNhomDaDangKy(string danhSo)
        {
            
            // Lấy danh sách loại nhóm đã đăng ký của user
            var loaiNhomDaDangKy = await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.DanhSo == danhSo)
                .Include(x => x.LichKhamSucKhoe)
                .Select(x => x.LichKhamSucKhoe.LoaiNhom)
                .Distinct()
                .ToListAsync();

            return loaiNhomDaDangKy;
        }

        public async Task<KSK_LoaiNhom?> GetLoaiNhomById(int id)
        {
            return await _appDb.KSK_LoaiNhom.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> GetSoLuongDaDangKyByLichId(int lichId)
        {
            return await _appDb.KSK_NhanVien_DangKy
                .Where(x => x.LichKhamSucKhoeId == lichId)
                .CountAsync();
        }
       
        public async Task<byte[]> ExportKSKToExcel(DateTime? fromDate, DateTime? toDate, int? donviId, int? nhomId)
        {
            System.Diagnostics.Debug.WriteLine("=== START ExportKSKToExcel ===");
            
            // Đường dẫn template
            string templatePath = Path.Combine("wwwroot", "Documents", "DanhSachDangKy.xlsx");
            System.Diagnostics.Debug.WriteLine($"Template path: {templatePath}");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file template: {templatePath}");
            }

            // Lấy tên nhóm
            string tenNhom = "";
            if (nhomId.HasValue)
            {
                var nhom = await _appDb.KSK_LoaiNhom.FirstOrDefaultAsync(x => x.Id == nhomId.Value);
                tenNhom = nhom?.TenNhom ?? "";
                System.Diagnostics.Debug.WriteLine($"Nhom: {tenNhom}");
            }

            // Lấy danh sách đăng ký, join với lịch, nhân viên, đơn vị
            var query = _appDb.KSK_NhanVien_DangKy
                .Include(x => x.LichKhamSucKhoe)
                .AsQueryable();
            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date == toDate.Value.Date)
            {
                var ngay = fromDate.Value.Date;
                query = query.Where(x => x.LichKhamSucKhoe.ThoiGianBatDau.Date == ngay);
            }
            else
            {
                if (fromDate.HasValue)
                    query = query.Where(x => x.LichKhamSucKhoe.ThoiGianBatDau >= fromDate.Value);
                if (toDate.HasValue)
                    query = query.Where(x => x.LichKhamSucKhoe.ThoiGianBatDau <= toDate.Value);
            }
            if (donviId.HasValue)
                query = query.Where(x => x.DonViId == donviId.Value);
            if (nhomId.HasValue)
                query = query.Where(x => x.LichKhamSucKhoe.LoaiNhom == nhomId.Value);

            var list = await query.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"Total records to export: {list.Count}");

            // Lấy thông tin nhân viên
            var danhSoList = list.Select(x => x.DanhSo).Distinct().ToList();
            var nhanVienDict = await _db.View_DanhBa_NhanVien
                .Where(x => danhSoList.Contains(x.DanhSo))
                .ToDictionaryAsync(x => x.DanhSo);

            // Lấy thông tin đơn vị
            var donViIds = list.Select(x => x.DonViId).Distinct().ToList();
            var donViDict = await _db.DepartmentTree
                .Where(x => donViIds.Contains(x.ID))
                .ToDictionaryAsync(x => x.ID);
            
            System.Diagnostics.Debug.WriteLine($"DEBUG: Department dictionary contains {donViDict.Count} departments:");
            foreach (var dv in donViDict)
            {
                System.Diagnostics.Debug.WriteLine($"  ID: {dv.Key}, TenTat: '{dv.Value.TenTat}', TenToChuc: '{dv.Value.TenToChuc}'");
            }

            // Copy template và sửa đổi
            var ms = new MemoryStream();
            using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await templateStream.CopyToAsync(ms);
            }
            ms.Position = 0;

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(ms, true))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                // Ghi tên nhóm vào ô A5
                SetCellValue(workbookPart, worksheetPart, "A5", tenNhom);

                int row = 7;
                int stt = 1;
                foreach (var item in list)
                {
                    System.Diagnostics.Debug.WriteLine($"\n--- Processing row {row}, STT {stt} ---");
                    System.Diagnostics.Debug.WriteLine($"DanhSo: {item.DanhSo}, DonViId: {item.DonViId}");
                    
                    View_DanhBa_NhanVien nv = null;
                    nhanVienDict.TryGetValue(item.DanhSo, out nv);
                    
                    // Get values - these will be cleaned by SetCellValue
                    string hoTen = nv?.HoTen ?? "";
                    string chucDanh = nv?.ChucDanh ?? "";
                    string donVi = donViDict.ContainsKey(item.DonViId) ? (donViDict[item.DonViId].TenTat ?? donViDict[item.DonViId].TenToChuc ?? "") : "";
                    
                    System.Diagnostics.Debug.WriteLine($"BEFORE ESCAPE - DonVi: '{donVi}'");
                    System.Diagnostics.Debug.WriteLine($"  Contains &: {donVi.Contains("&")}");
                    System.Diagnostics.Debug.WriteLine($"  Hex dump: {BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(donVi))}");
                    
                    string gioiTinh = nv?.GioiTinh ?? "";
                    string namSinh = "";
                    if (nv?.NgaySinh.HasValue == true)
                    {
                        namSinh = nv.NgaySinh.Value.Year.ToString();
                    }
                    string dienThoai = !string.IsNullOrEmpty(nv?.Mobile) ? nv.Mobile : (nv?.Phone_CQ ?? "");
                    string thoiGian = item.LichKhamSucKhoe?.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm") ?? item.CreateDateTime.ToString("dd/MM/yyyy HH:mm");
                    string yeuCauDacBiet = item.YeuCauDacBiet ?? "";

                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 1), stt.ToString()); // No
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 2), item.DanhSo); // Danh số
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 3), hoTen); // Họ và tên
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 4), chucDanh); // Chức danh
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 5), donVi); // Đơn vị - Fixed!
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 6), namSinh); // Năm sinh
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 7), gioiTinh); // Giới tính
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 8), dienThoai); // Điện thoại
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 9), thoiGian); // Thời gian
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 10), yeuCauDacBiet); // Note
                    row++;
                    stt++;
                }

                System.Diagnostics.Debug.WriteLine("\n=== Saving workbook ===");
                workbookPart.Workbook.Save();
            }

            System.Diagnostics.Debug.WriteLine($"=== END ExportKSKToExcel - Generated {ms.Length} bytes ===");
            return ms.ToArray();
        }

        public async Task<byte[]> ExportChuaDangKyKSKToExcel(int? donviId)
        {
            // Đường dẫn template
            string templatePath = Path.Combine("wwwroot", "Documents", "DanhSachChuaDangKy.xlsx");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file template: {templatePath}");
            }

            // Lấy tên đơn vị nếu có chọn
            string tenDonVi = "";
            if (donviId.HasValue)
            {
                var donVi = await _db.DepartmentTree.FirstOrDefaultAsync(x => x.ID == donviId.Value);
                tenDonVi = donVi?.TenTat ?? donVi?.TenToChuc ?? "";
            }

            // Lấy tất cả nhân viên từ View_DanhBa_NhanVien (giống ExportKSKToExcel)
            var allNhanVien = await _db.View_DanhBa_NhanVien.ToListAsync();

            // Lấy danh sách những người đã đăng ký khám sức khỏe trong năm nay
            var currentYear = DateTime.Now.Year;
            var startOfYear = new DateTime(currentYear, 1, 1);
            var endOfYear = new DateTime(currentYear, 12, 31);

            var daDangKyQuery = _appDb.KSK_NhanVien_DangKy
                .Include(x => x.LichKhamSucKhoe)
                .Where(x => x.LichKhamSucKhoe.ThoiGianBatDau >= startOfYear && 
                           x.LichKhamSucKhoe.ThoiGianBatDau <= endOfYear);

            if (donviId.HasValue)
            {
                daDangKyQuery = daDangKyQuery.Where(x => x.DonViId == donviId.Value);
            }

            var daDangKyList = await daDangKyQuery.ToListAsync();
            var danhSoDaDangKy = daDangKyList.Select(x => x.DanhSo).Distinct().ToList();

            // Lọc ra những người chưa đăng ký và sắp xếp theo tăng dần của danh số
            var chuaDangKyList = allNhanVien
                .Where(x => !danhSoDaDangKy.Contains(x.DanhSo))
                .OrderBy(x => x.DanhSo)
                .ToList();

            // Lọc theo đơn vị nếu có chọn
            if (donviId.HasValue)
            {
                var nhanVienWithDonVi = new List<View_DanhBa_NhanVien>();
                foreach (var nv in chuaDangKyList)
                {
                    var checkDonVi = await _db.DepartmentTree.Where(x => x.ID == nv.BoPhan_id).FirstOrDefaultAsync();
                    if (checkDonVi != null && checkDonVi.ID_DonVi.HasValue && checkDonVi.ID_DonVi.Value == donviId.Value)
                    {
                        nhanVienWithDonVi.Add(nv);
                    }
                }
                chuaDangKyList = nhanVienWithDonVi;
            }

            // Copy template và sửa đổi
            var ms = new MemoryStream();
            using (var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await templateStream.CopyToAsync(ms);
            }
            ms.Position = 0;

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(ms, true))
            {
                WorkbookPart workbookPart = doc.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

                // Điền đơn vị vào template (ô B3)
                SetCellValue(workbookPart, worksheetPart, "B3", tenDonVi);
                
                // Điền ngày hiện tại vào ô F3
                SetCellValue(workbookPart, worksheetPart, "F3", DateTime.Now.ToString("dd/MM/yyyy"));

                int row = 7; // Bắt đầu từ dòng 7
                int stt = 1;
                foreach (var nv in chuaDangKyList)
                {
                    // Lấy năm sinh từ ngày sinh
                    string namSinh = "";
                    if (nv.NgaySinh.HasValue)
                    {
                        namSinh = nv.NgaySinh.Value.Year.ToString();
                    }

                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 1), stt.ToString()); // STT
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 2), nv.DanhSo ?? ""); // Danh số
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 3), nv.HoTen ?? ""); // Họ và tên
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 4), nv.GioiTinh ?? ""); // Giới tính
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 5), namSinh); // Năm sinh
                    SetCellValue(workbookPart, worksheetPart, GetCellAddress(row, 6), nv.ChucDanh ?? ""); // Chức danh
                    
                    row++;
                    stt++;
                }

                workbookPart.Workbook.Save();
            }

            return ms.ToArray();
        }

        /// <summary>
        /// Lấy danh sách tất cả nhân viên với thông tin đơn vị và trạng thái khám sức khỏe
        /// </summary>
        /// <param name="donViId">ID Đơn vị để lọc (optional)</param>
        /// <param name="searchTerm">Tìm kiếm theo danh số hoặc tên (optional)</param>
        /// <returns>Danh sách nhân viên với thông tin chi tiết</returns>
        public async Task<List<View_NhanVien_All>> ViewNhanVienAll(int? donViId = null, string? searchTerm = null)
        {
            // Lấy tất cả nhân viên từ View_DanhBa_NhanVien
            var allNhanVien = await _db.View_DanhBa_NhanVien.ToListAsync();

            // Lọc theo đơn vị nếu có
            if (donViId.HasValue)
            {
                var filteredNhanVien = new List<View_DanhBa_NhanVien>();
                foreach (var nv in allNhanVien)
                {
                    var checkDonVi = await _db.DepartmentTree
                        .Where(x => x.ID == nv.BoPhan_id)
                        .FirstOrDefaultAsync();
                    
                    if (checkDonVi != null && checkDonVi.ID_DonVi.HasValue && checkDonVi.ID_DonVi.Value == donViId.Value)
                    {
                        filteredNhanVien.Add(nv);
                    }
                }
                allNhanVien = filteredNhanVien;
            }

            // Lọc theo tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                allNhanVien = allNhanVien
                    .Where(x => (x.DanhSo?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                               (x.HoTen?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Lấy danh sách những người đã đăng ký khám sức khỏe
            var currentYear = DateTime.Now.Year;
            var startOfYear = new DateTime(currentYear, 1, 1);
            var endOfYear = new DateTime(currentYear, 12, 31);

            var daDangKyQuery = _appDb.KSK_NhanVien_DangKy
                .Include(x => x.LichKhamSucKhoe)
                .Where(x => x.LichKhamSucKhoe.ThoiGianBatDau >= startOfYear && 
                           x.LichKhamSucKhoe.ThoiGianBatDau <= endOfYear);

            var daDangKyList = await daDangKyQuery.ToListAsync();

            // Map dữ liệu
            var result = new List<View_NhanVien_All>();
            foreach (var nv in allNhanVien)
            {
                var checkDonVi = await _db.DepartmentTree
                    .Where(x => x.ID == nv.BoPhan_id)
                    .FirstOrDefaultAsync();
                
                DepartmentTree? donVi = null;
                if (checkDonVi != null && checkDonVi.ID_DonVi.HasValue)
                {
                    donVi = await _db.DepartmentTree
                        .Where(x => x.ID == checkDonVi.ID_DonVi.Value)
                        .FirstOrDefaultAsync();
                }

                // Kiểm tra xem đã đăng ký khám sức khỏe chưa
                var dangKyNhanVien = daDangKyList
                    .Where(x => x.DanhSo == nv.DanhSo)
                    .ToList();

                var daDangKyKSK = dangKyNhanVien.Any();
                var ngayKhamGanNhat = dangKyNhanVien
                    .OrderByDescending(x => x.LichKhamSucKhoe.ThoiGianBatDau)
                    .FirstOrDefault()
                    ?.LichKhamSucKhoe.ThoiGianBatDau;

                result.Add(new View_NhanVien_All
                {
                    BoPhan_id = nv.BoPhan_id,
                    NhanVien_id = nv.NhanVien_id,
                    DanhSo = nv.DanhSo,
                    HoTen = nv.HoTen,
                    HoTen_ru = nv.HoTen_ru,
                    Phone_CQ = nv.Phone_CQ,
                    Mobile = nv.Mobile,
                    Email = nv.Email,
                    ChucDanh = nv.ChucDanh,
                    ChucDanh_ru = nv.ChucDanh_ru,
                    Loai_CBCNV = nv.Loai_CBCNV,
                    DonVi = donVi?.TenTat ?? donVi?.TenToChuc ?? "Không xác định",
                    DonVi_ru = donVi?.TenToChuc_ru,
                    GioiTinh = nv.GioiTinh,
                    NgaySinh = nv.NgaySinh,
                    ID_DonVi = donVi?.ID_DonVi,
                    NhomNhanVien_id = new List<int>()
                });
            }

            return result.OrderBy(x => x.DanhSo).ToList();
        }

        // ==================== HELPER METHODS FOR OPENXML ====================
        
        /// <summary>
        /// Set cell value using InlineString with proper Unicode handling
        /// This properly handles Vietnamese characters and & symbol
        /// </summary>
        private void SetCellValue(WorkbookPart workbookPart, WorksheetPart worksheetPart, string cellAddress, string value)
        {
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
            Cell cell = GetCell(sheetData, cellAddress);
            
            System.Diagnostics.Debug.WriteLine($"\nSetCellValue - Cell: {cellAddress}");
            System.Diagnostics.Debug.WriteLine($"  Original value: '{value}'");
            
            // Only remove control characters - keep Unicode and all printable chars
            string cleanValue = "";
            if (!string.IsNullOrEmpty(value))
            {
                cleanValue = new string(value.Where(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t').ToArray());
            }
            
            System.Diagnostics.Debug.WriteLine($"  After cleaning: '{cleanValue}'");
            
            // Remove old cell content
            cell.CellValue?.Remove();
            cell.InlineString?.Remove();
            
            // Use InlineString with Text that has xml:space="preserve"
            // This properly handles Unicode Vietnamese characters and special chars like &
            cell.DataType = CellValues.InlineString;
            
            Text text = new Text(cleanValue);
            text.Space = SpaceProcessingModeValues.Preserve; // Important for spaces and Unicode
            
            cell.InlineString = new InlineString(text);
            
            System.Diagnostics.Debug.WriteLine($"  Cell set with InlineString + Unicode support");
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