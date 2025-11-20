using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using VSP_HealthExam.Web.Entity;
using VSP_HealthExam.Web.Models.Register;
using VSP_HealthExam.Web.EditModels.Register;
using VSP_HealthExam.Web.ViewModels.Register;
using VSP_HealthExam.Web.Models.KhamSucKhoe;

namespace VSP_HealthExam.Web.Services.Register
{
    public class ChuyenDeNuRegister_Service : I_ChuyenDeNuRegister
    {
        private readonly ApplicationDbContext _context;
        private readonly SubDbContext _subContext;
        private readonly UserManager<AppUser> _userManager;

        public ChuyenDeNuRegister_Service(ApplicationDbContext context, SubDbContext subContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _subContext = subContext;
            _userManager = userManager;
        }

        public async Task<List<ChuyenDeNuRegister_ViewModel>> GetAllAsync()
        {
            // Lấy danh sách đăng ký từ context chính
            var registers = await _context.ChuyenDeNuRegister.ToListAsync();
            
            // Lấy danh sách đơn vị từ sub context
            var donViIds = registers.Select(r => r.DonViId).Distinct().ToList();
            var donViList = await _subContext.DepartmentTree
                .Where(d => d.ID_DonVi.HasValue && donViIds.Contains(d.ID_DonVi.Value))
                .ToDictionaryAsync(d => d.ID_DonVi.Value, d => d.TenTat ?? d.TenToChuc ?? "Không xác định");

            // Map dữ liệu
            var result = registers.Select(register => new ChuyenDeNuRegister_ViewModel
            {
                Id = register.Id,
                DanhSoCu = register.DanhSoCu,
                Password = register.Password,
                Email = register.Email,
                SoDienThoai = register.SoDienThoai,
                DonViId = register.DonViId,
                DonViName = donViList.ContainsKey(register.DonViId) ? donViList[register.DonViId] : "Không xác định",
                TrangThaiPheDuyet = register.TrangThaiPheDuyet,
                NgayTao = register.NgayTao,
                NgayCapNhat = register.NgayCapNhat,
                LyDoTuChoi = register.LyDoTuChoi,
                FullName = register.FullName
            }).ToList();

            return result;
        }

        public async Task<ChuyenDeNuRegister_ViewModel> GetByIdAsync(int id)
        {
            // Lấy đăng ký từ context chính
            var register = await _context.ChuyenDeNuRegister.FirstOrDefaultAsync(r => r.Id == id);
            if (register == null) return null;

            // Lấy thông tin đơn vị từ sub context
            var donVi = await _subContext.DepartmentTree.FirstOrDefaultAsync(d => d.ID_DonVi.HasValue && d.ID_DonVi.Value == register.DonViId);

            return new ChuyenDeNuRegister_ViewModel
            {
                Id = register.Id,
                DanhSoCu = register.DanhSoCu,
                Password = register.Password,
                Email = register.Email,
                SoDienThoai = register.SoDienThoai,
                DonViId = register.DonViId,
                DonViName = donVi?.TenTat ?? donVi?.TenToChuc ?? "Không xác định",
                TrangThaiPheDuyet = register.TrangThaiPheDuyet,
                NgayTao = register.NgayTao,
                NgayCapNhat = register.NgayCapNhat,
                LyDoTuChoi = register.LyDoTuChoi,
                FullName = register.FullName
            };
        }

        public async Task<bool> CreateAsync(ChuyenDeNuRegister_EditModel model)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreateAsync started for DanhSo: {model.DanhSoCu}");
                
                // Kiểm tra danh số có hợp lệ không
                var danhSoValid = await IsDanhSoValidForRegistrationAsync(model.DanhSoCu);
                System.Diagnostics.Debug.WriteLine($"DanhSo validation result: {danhSoValid}");
                if (!danhSoValid)
                {
                    System.Diagnostics.Debug.WriteLine("DanhSo validation failed");
                    return false; // DanhSo không hợp lệ để đăng ký
                }



                var register = new ChuyenDeNuRegister
                {
                    DanhSoCu = model.DanhSoCu,
                    Password = model.Password,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    DonViId = model.DonViId,
                    TrangThaiPheDuyet = 0, // Mặc định là chờ phê duyệt
                    FullName = model.FullName, // Lấy tên từ form
                    NgayTao = DateTime.Now
                };

                System.Diagnostics.Debug.WriteLine("Adding register to context");
                _context.ChuyenDeNuRegister.Add(register);
                
                System.Diagnostics.Debug.WriteLine("Saving changes to database");
                await _context.SaveChangesAsync();
                
                System.Diagnostics.Debug.WriteLine("CreateAsync completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                // Log exception để debug
                System.Diagnostics.Debug.WriteLine($"CreateAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"CreateAsync stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(ChuyenDeNuRegister_EditModel model)
        {
            try
            {
                var register = await _context.ChuyenDeNuRegister.FindAsync(model.Id);
                if (register == null) return false;

                register.DanhSoCu = model.DanhSoCu;
                register.Password = model.Password;
                register.Email = model.Email;
                register.SoDienThoai = model.SoDienThoai;
                register.DonViId = model.DonViId;
                register.TrangThaiPheDuyet = model.TrangThaiPheDuyet;
                register.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var register = await _context.ChuyenDeNuRegister.FindAsync(id);
                if (register == null) return false;

                _context.ChuyenDeNuRegister.Remove(register);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ApproveAsync(int id)
        {
            try
            {
                var register = await _context.ChuyenDeNuRegister.FindAsync(id);
                if (register == null) return false;

                // Kiểm tra trong View_DanhBa_NhanVien để đảm bảo thông tin hợp lệ
                var nhanVienInfo = await _subContext.View_DanhBa_NhanVien
                    .FirstOrDefaultAsync(x => x.DanhSo == register.DanhSoCu);
                
                if (nhanVienInfo == null)
                {
                    return false; // Không tìm thấy nhân viên trong hệ thống
                }

                // Cập nhật FullName từ thông tin nhân viên
                register.FullName = nhanVienInfo.HoTen;

                // Cập nhật trạng thái phê duyệt
                register.TrangThaiPheDuyet = 1;
                register.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RejectAsync(int id, string? lyDoTuChoi = null)
        {
            try
            {
                var register = await _context.ChuyenDeNuRegister.FindAsync(id);
                if (register == null) return false;

                register.TrangThaiPheDuyet = 2;
                register.LyDoTuChoi = lyDoTuChoi;
                register.NgayCapNhat = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ChuyenDeNuRegister> GetByDanhSoAsync(string danhSo)
        {
            return await _context.ChuyenDeNuRegister
                .FirstOrDefaultAsync(x => x.DanhSoCu == danhSo);
        }

        public async Task<bool> CheckDanhSoExistsAsync(string danhSo)
        {
            // Kiểm tra trong bảng AppUser (nếu đã có user thì không cho đăng ký)
            var existingUser = await _userManager.FindByNameAsync(danhSo);
            if (existingUser != null)
            {
                return true; // DanhSo đã tồn tại trong hệ thống user
            }

            // Kiểm tra trong bảng ChuyenDeNuRegister (nếu đã đăng ký thì không cho đăng ký lại)
            var existingRegister = await _context.ChuyenDeNuRegister
                .FirstOrDefaultAsync(x => x.DanhSoCu == danhSo);
            if (existingRegister != null)
            {
                return true; // Đã có đăng ký với DanhSo này
            }

            // Kiểm tra trong View_DanhBa_NhanVien (phải có trong hệ thống nhân viên mới được đăng ký)
            var nhanVienInfo = await _subContext.View_DanhBa_NhanVien
                .FirstOrDefaultAsync(x => x.DanhSo == danhSo);
            
            return nhanVienInfo == null; // Trả về true nếu KHÔNG tìm thấy trong hệ thống nhân viên
        }

        public async Task<bool> IsDanhSoValidForRegistrationAsync(string danhSo)
        {
            // Tạm thời bypass để test
            return true;
        }
    }
} 