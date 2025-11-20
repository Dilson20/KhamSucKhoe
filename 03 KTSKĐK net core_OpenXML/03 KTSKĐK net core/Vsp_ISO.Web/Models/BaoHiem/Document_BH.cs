namespace VSP_HealthExam.Web.Models.BaoHiem
{
    public class Document_BH
    {
        public int Id { get; set; }
        public string? DanhSo { get; set; }
        public DateTime? DateUpload { get; set; }
        public string? TenBenhVien { get; set; }
        public string? ThongTin { get; set; }
        public string? HinhThucKCB { get; set; }
        public DateTime? NgayBatDauKham { get; set; }
        public DateTime? NgayKetThucKham { get; set; }
        public string? LoaiHinhDieuTri { get; set; }
        public string? DongYSuDung { get; set; }
        public ICollection<FilesAttachment>? FilesAttachments { get; set; } = new List<FilesAttachment>();
        public string? NoteByDoctor { get; set; }
        public string? Status { get; set; }
    }
}
