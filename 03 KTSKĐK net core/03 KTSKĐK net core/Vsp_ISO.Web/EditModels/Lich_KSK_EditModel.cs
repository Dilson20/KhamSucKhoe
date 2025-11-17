using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VSP_HealthExam.Web.EditModels
{
    public class Lich_KSK_EditModel
    {
        [Required(ErrorMessage = "ID là bắt buộc")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Đơn vị ID là bắt buộc")]
        public int DonViId { get; set; }
        
        [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
        public DateTime ThoiGianBatDau { get; set; }
        
        [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
        public DateTime ThoiGianKetThuc { get; set; }
        
        // Custom validation để đảm bảo thời gian kết thúc sau thời gian bắt đầu
        public bool IsValidTimeRange()
        {
            return ThoiGianKetThuc > ThoiGianBatDau;
        }
        
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }
        
        [Required(ErrorMessage = "Loại nhóm là bắt buộc")]
        public int LoaiNhom { get; set; }
        
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? GhiChu { get; set; }
        
        public string? Creator { get; set; }
        
        public string? UpdatePerson { get; set; }
    }
}
