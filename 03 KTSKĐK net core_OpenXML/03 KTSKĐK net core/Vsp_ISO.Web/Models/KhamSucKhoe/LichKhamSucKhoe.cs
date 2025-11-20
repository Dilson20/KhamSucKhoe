namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    public class LichKhamSucKhoe
    {
        public int Id { get; set; }
        public int DonViId { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public int SoLuong { get; set; }
        public int LoaiNhom { get; set; }
        public string? GhiChu { get; set; }
        public string? Creator { get; set; }
        public string? UpdatePerson { get; set; }
        public virtual KSK_LoaiNhom? LoaiNhomNavigation { get; set; }
        public virtual ICollection<KSK_NhanVien_DangKy>? KSK_NhanVien_DangKys { get; set; }
    }
}
