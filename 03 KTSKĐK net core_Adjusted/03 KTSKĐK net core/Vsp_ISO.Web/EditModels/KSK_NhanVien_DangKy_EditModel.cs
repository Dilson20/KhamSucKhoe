using System.ComponentModel.DataAnnotations;

namespace VSP_HealthExam.Web.EditModels
{
    public class KSK_NhanVien_DangKy_EditModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Đơn vị ID là bắt buộc")]
        public int DonViId { get; set; }
        
        public string DanhSo { get; set; }
        
        [Required(ErrorMessage = "Lịch khám sức khỏe ID là bắt buộc")]
        public int LichKhamSucKhoeId { get; set; }
        
        public string? YeuCauDacBiet { get; set; }
        
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        
        public DateTime? UpdateDateTime { get; set; }
        
        public string? UpdatePerson { get; set; }
    }
} 