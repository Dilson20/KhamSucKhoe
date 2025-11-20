namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    public class View_NhanVien_All
    {
        public int BoPhan_id { get; set; }
        public int? NhanVien_id { get; set; }
        public string? DanhSo { get; set; }
        public string? HoTen { get; set; }
        public string? HoTen_ru { get; set; }
        public string? Phone_CQ { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ChucDanh { get; set; }
        public string? ChucDanh_ru { get; set; }
        public int? Loai_CBCNV { get; set; }
        public int? ID_DonVi { get; set; } 
        public string? DonVi { get; set; }
        public string? DonVi_ru { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public List<int>? NhomNhanVien_id { get; set; } = new List<int>();
        public bool DaDangKyKSK { get; set; } = false;
        public DateTime? NgayKhamGanNhat { get; set; }
        public int? SoLanDangKy { get; set; }
    }
}
