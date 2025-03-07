namespace VisssStock.Application.DTOs
{
    public class MenuBO
    {
        public int ParentId { get; set; }
        public string? Subheader { get; set; }
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public int? Orderno { get; set; }

    }
}
