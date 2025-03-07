namespace VisssStock.Application.Models
{
    public class UploadResult
    {
        public string? FileName { get; set; }
        public string? FileNameNoExt { get; set; }
        public long? Size { get; set; }
        public string? ContentType { get; set; }
        public string? ErrorCode { get; set; }
    }
}