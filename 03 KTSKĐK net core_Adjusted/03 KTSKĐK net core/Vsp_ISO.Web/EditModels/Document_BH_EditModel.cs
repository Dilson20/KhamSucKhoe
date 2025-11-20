using System.ComponentModel.DataAnnotations;
using VSP_HealthExam.Web.Models.BaoHiem;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace VSP_HealthExam.Web.EditModels
{
    public class Document_BH_EditModel
    {
        public int Id { get; set; }
        public string DanhSo { get; set; }
        public DateTime? DateUpload { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? TenBenhVien { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? ThongTin { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? HinhThucKCB { get; set; }
        public DateTime? NgayBatDauKham { get; set; } 
        public DateTime? NgayKetThucKham { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? LoaiHinhDieuTri { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? DongYSuDung { get; set; }
        public List<IFormFile>? Files { get; set; } = default!;    
        public List<string>? FileNames { get; set; } = default!;
        public List<string>? Descriptions { get; set; } = default!;
        public ICollection<FilesAttachment>? FilesAttachments { get; set; } = new List<FilesAttachment>();
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? NoteByDoctor { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", ErrorMessage = "Chỉ được nhập chữ, số, khoảng trắng, dấu . , ; : ( ). Không được chứa ký tự HTML/script")]
        public string? Status { get; set; }
    }

    public static class Document_BH_EditModelValidator
    {
        private static readonly Regex _textRegex = new Regex(@"^[a-zA-Z0-9\s.,;:()àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ-]*$", RegexOptions.Compiled);

        public static bool ValidateTextList(IEnumerable<string>? list)
        {
            if (list == null) return true;
            foreach (var item in list)
            {
                if (!_textRegex.IsMatch(item ?? ""))
                    return false;
            }
            return true;
        }
    }
}
