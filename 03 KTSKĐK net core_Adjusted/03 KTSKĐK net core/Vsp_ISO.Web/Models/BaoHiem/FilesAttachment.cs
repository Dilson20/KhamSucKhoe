namespace VSP_HealthExam.Web.Models.BaoHiem
{
    public class FilesAttachment
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? DanhSo { get; set; }
        public string? Description { get; set; }
        public int Document_BHId { get; set; }
        public Document_BH? Document_BH { get; set; }   
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

