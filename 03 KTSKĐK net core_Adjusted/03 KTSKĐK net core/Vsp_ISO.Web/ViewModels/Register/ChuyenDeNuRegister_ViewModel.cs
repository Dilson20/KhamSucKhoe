namespace VSP_HealthExam.Web.ViewModels.Register
{
    public class ChuyenDeNuRegister_ViewModel
    {
        public int Id { get; set; }
        public string DanhSoCu { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public int DonViId { get; set; }
        public string DonViName { get; set; }
        public int TrangThaiPheDuyet { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string? LyDoTuChoi { get; set; }
        public string FullName { get; set; }
    }
} 