namespace VisssStock.Application.Models
{
    public class UploadExcelFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadExcelFileResponse
    {
        public bool IsSuccess { get; set; }
    }
    public class  ExcelBulkUploadUserParameter
    {
        public bool IsSuccess { get; set; }
    }

    public class ImportOrderRevenueExcelDto
    {
        public IFormFile File { get; set; }

        public int WorksheetIndex { get; set; }

        public int CompanyId { get; set; }
    }
}
