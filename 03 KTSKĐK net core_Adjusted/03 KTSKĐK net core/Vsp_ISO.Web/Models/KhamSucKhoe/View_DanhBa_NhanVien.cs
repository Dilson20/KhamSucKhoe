using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_HealthExam.Web.Models.KhamSucKhoe
{
    public class View_DanhBa_NhanVien
    {
        public int BoPhan_id { get; set; }
        public int? NhanVien_id { get; set; }
        public string? DanhSo { get; set; }
        public string? HoTen { get; set; }
        public string? HoTen_ru {  get; set; }
        public string? Phone_CQ { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? ChucDanh { get; set; }
        public string? ChucDanh_ru { get; set; }
        public int? Loai_CBCNV { get; set; }
        public string? UserPassword { get; set; }
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }

    }
}
