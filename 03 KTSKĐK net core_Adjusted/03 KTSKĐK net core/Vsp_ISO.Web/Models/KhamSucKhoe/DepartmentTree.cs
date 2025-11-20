using System.ComponentModel.DataAnnotations.Schema;

namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    [Table("DM_ToChucDonVi")]
    public class DepartmentTree
    {
        public int ID { get; set; }
        public string? TenToChuc { get; set; }
        public string? TenToChuc_ru { get; set; }
        public int? Id_Cha { get; set; }
        public int? ID_DonVi { get; set; }
        public int? Cap { get; set; }
        public string? TenTat { get; set; }
        public string? TenTat_ru { get; set; }
        public string? Ma { get; set; }
        public string? Ma_DV { get; set; }
        public int? STT { get; set; }
        public bool? TamNgung { get; set; }
        public int? Khoi_id { get; set; }
        public int? Phong_id { get; set; }
        public int? Gian_id { get; set; }
        public int? To_id { get; set; }
        public int? Nhom_id { get; set; }
        public virtual ICollection<DepartmentTree> ChildDepartments { get; set; } = new List<DepartmentTree>();
        [NotMapped]
        public ICollection<View_NhanVien_All> ListNhanVien { get; set; } = new List<View_NhanVien_All>();
    }
}
