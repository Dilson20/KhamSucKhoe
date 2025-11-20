namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    public class KSK_LoaiNhom
    {
        public int Id { get; set; }
        public string TenNhom { get; set; }
        public string? Color { get; set; }
        public int? SoPhutThucHien { get; set; }
        // Navigation property for one-to-one relationship
        public virtual ICollection<LichKhamSucKhoe> LichKhamSucKhoes { get; set; }
    }
}
