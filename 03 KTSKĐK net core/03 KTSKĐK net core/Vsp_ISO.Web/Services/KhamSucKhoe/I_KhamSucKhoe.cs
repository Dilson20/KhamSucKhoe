using Microsoft.EntityFrameworkCore;
using VSP_HealthExam.Web.EditModels;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models;
using VSP_HealthExam.Web.Models.KhamSucKhoe;
using Microsoft.Extensions.Localization;
using VSP_HealthExam.Web;
using Spire.Xls;
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
            // Đường dẫn template
            string templatePath = Path.Combine("wwwroot", "Documents", "DanhSachDangKy.xlsx");
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

            // Đọc template
            using (var workbook = new Workbook())
            {
                workbook.LoadFromFile(templatePath);
                var sheet = workbook.Worksheets[0];
                // Ghi tên nhóm vào ô A5
                sheet.Range["A5"].Value = tenNhom;

                int row = 7;
                int stt = 1;
                foreach (var item in list)
                {
                    View_DanhBa_NhanVien nv = null;
                    nhanVienDict.TryGetValue(item.DanhSo, out nv);
                    string hoTen = nv?.HoTen ?? "";
                    string chucDanh = nv?.ChucDanh ?? "";
                                            string donVi = donViDict.ContainsKey(item.DonViId) ? (donViDict[item.DonViId].TenTat ?? donViDict[item.DonViId].TenToChuc) : "";
                    string gioiTinh = nv?.GioiTinh ?? "";
                    // Lấy năm sinh từ NgaySinh
                    string namSinh = "";
                    if (nv?.NgaySinh.HasValue == true)
                    {
                        namSinh = nv.NgaySinh.Value.Year.ToString();
                    }
                    string dienThoai = !string.IsNullOrEmpty(nv?.Mobile) ? nv.Mobile : (nv?.Phone_CQ ?? "");
                    string thoiGian = item.LichKhamSucKhoe?.ThoiGianBatDau.ToString("dd/MM/yyyy HH:mm") ?? item.CreateDateTime.ToString("dd/MM/yyyy HH:mm");
                    string yeuCauDacBiet = item.YeuCauDacBiet ?? "";

                    sheet.Range[row, 1].Value = stt.ToString(); // No
                    sheet.Range[row, 2].Value = item.DanhSo; // Danh số
                    sheet.Range[row, 3].Value = hoTen; // Họ và tên
                    sheet.Range[row, 4].Value = chucDanh; // Chức danh
                    sheet.Range[row, 5].Value = donVi; // Đơn vị
                    sheet.Range[row, 6].Value = namSinh; // Năm sinh
                    sheet.Range[row, 7].Value = gioiTinh; // Giới tính
                    sheet.Range[row, 8].Value = dienThoai; // Điện thoại
                    sheet.Range[row, 9].Value = thoiGian; // Thời gian
                    sheet.Range[row, 10].Value = yeuCauDacBiet; // Note
                    row++;
                    stt++;
                }
                using (var ms = new MemoryStream())
                {
                    workbook.SaveToStream(ms);
                    return ms.ToArray();
                }
            }
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

            // Đọc template
            using (var workbook = new Workbook())
            {
                workbook.LoadFromFile(templatePath);
                var sheet = workbook.Worksheets[0];

                // Điền đơn vị vào template (giả sử ô B3)
                sheet.Range["B3"].Value = tenDonVi;
                
                // Điền ngày hiện tại vào ô F3
                sheet.Range["F3"].Value = DateTime.Now.ToString("dd/MM/yyyy");

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

                    sheet.Range[row, 1].Value = stt.ToString(); // STT
                    sheet.Range[row, 2].Value = nv.DanhSo ?? ""; // Danh số
                    sheet.Range[row, 3].Value = nv.HoTen ?? ""; // Họ và tên
                    sheet.Range[row, 4].Value = nv.GioiTinh ?? ""; // Giới tính
                    sheet.Range[row, 5].Value = namSinh; // Năm sinh
                    sheet.Range[row, 6].Value = nv.ChucDanh ?? ""; // Chức danh
                    
                    row++;
                    stt++;
                }

                using (var ms = new MemoryStream())
                {
                    workbook.SaveToStream(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
