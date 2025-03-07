namespace VisssStock.Application.Models
{
    public class ReportResponse
    {
//số lượng tài liệu
        public int totalOfDocs { get; set; }

//sô dung lượng đã sử dụng để lưu tài liệu
        public float totalOfDocsCapacity { get; set; }

        public TypeOfDoc typeOfDoc { get; set; }
    }


    public class TypeOfDoc
    {
        public int image { get; set; }
        public int docs { get; set; }
        public int videos { get; set; }
        public int sound { get; set; }
        public int others { get; set; }
    }
}
