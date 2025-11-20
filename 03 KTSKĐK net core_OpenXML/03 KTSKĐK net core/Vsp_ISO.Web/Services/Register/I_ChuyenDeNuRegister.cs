using VSP_HealthExam.Web.Models.Register;
using VSP_HealthExam.Web.EditModels.Register;
using VSP_HealthExam.Web.ViewModels.Register;

namespace VSP_HealthExam.Web.Services.Register
{
    public interface I_ChuyenDeNuRegister
    {
        Task<List<ChuyenDeNuRegister_ViewModel>> GetAllAsync();
        Task<ChuyenDeNuRegister_ViewModel> GetByIdAsync(int id);
        Task<ChuyenDeNuRegister> GetByDanhSoAsync(string danhSo);
        Task<bool> CreateAsync(ChuyenDeNuRegister_EditModel model);
        Task<bool> UpdateAsync(ChuyenDeNuRegister_EditModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApproveAsync(int id);
        Task<bool> RejectAsync(int id, string? lyDoTuChoi = null);
        Task<bool> CheckDanhSoExistsAsync(string danhSo);
        Task<bool> IsDanhSoValidForRegistrationAsync(string danhSo);
    }
} 