using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VSP_HealthExam.Web.EditModels.Register
{
    public class ChuyenDeNuRegister_EditModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Danh số cũ là bắt buộc")]
        [Display(Name = "Danh số cũ")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string DanhSoCu { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Đơn vị là bắt buộc")]
        [Display(Name = "Đơn vị")]
        public int DonViId { get; set; }

        [Display(Name = "Trạng thái phê duyệt")]
        public int TrangThaiPheDuyet { get; set; } = 0;

        [Display(Name = "Lý do từ chối")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? LyDoTuChoi { get; set; }

        [Display(Name = "Họ và tên")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string FullName { get; set; }
    }
} 