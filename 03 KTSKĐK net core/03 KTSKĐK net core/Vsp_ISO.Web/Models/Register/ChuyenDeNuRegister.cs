using System.ComponentModel.DataAnnotations;
using VSP_HealthExam.Web.Models.KhamSucKhoe;

namespace VSP_HealthExam.Web.Models.Register
{
    public class ChuyenDeNuRegister
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Danh số cũ là bắt buộc")]
        [Display(Name = "Danh số cũ")]
        public string DanhSoCu { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Đơn vị là bắt buộc")]
        [Display(Name = "Đơn vị")]
        public int DonViId { get; set; }

        [Display(Name = "Trạng thái phê duyệt")]
        public int TrangThaiPheDuyet { get; set; } = 0;

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Lý do từ chối")]
        public string? LyDoTuChoi { get; set; }

        public string? FullName { get; set; }
    }
} 