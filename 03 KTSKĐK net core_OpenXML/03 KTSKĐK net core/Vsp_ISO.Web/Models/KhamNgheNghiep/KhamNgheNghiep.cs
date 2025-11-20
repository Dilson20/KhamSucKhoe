using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSP_HealthExam.Web.Models.KhamNgheNghiep
{
    [Microsoft.EntityFrameworkCore.Index(nameof(DanhSo), IsUnique = true)]
    [Microsoft.EntityFrameworkCore.Index(nameof(CCCD), IsUnique = true)]
    public class KhamNgheNghiep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [RegularExpression(@"^\d{5}$", ErrorMessage = "Sai định dạng danh số")]
        [DisplayName("Danh số")]
        [Required(ErrorMessage = "Danh số là bắt buộc")]
        public string DanhSo { get; set; }

        [RegularExpression(@"^\d{12}$", ErrorMessage = "Sai định dạng CCCD")]
        [DisplayName("CCCD")]
        [Required(ErrorMessage = "CCCD là bắt buộc")]
        public string CCCD { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được vượt quá 200 ký tự.")]
        [DisplayName("Ghi chú")]
        public string? Notes { get; set; }
    }
}
