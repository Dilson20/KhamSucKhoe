namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    public class KSK_NhanVien_DangKy
    {
        public int Id { get; set; }
        public int DonViId { get; set; }
        public string DanhSo { get; set; }
        public int LichKhamSucKhoeId { get; set; }
        public string? YeuCauDacBiet { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string? UpdatePerson { get; set; }
        public virtual LichKhamSucKhoe? LichKhamSucKhoe { get; set; }
    }
}
