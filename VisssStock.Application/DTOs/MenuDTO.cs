namespace VisssStock.Application.DTOs
{
    public class MenuDTO
    {
        public string? Subheader { get; set; }
        public List<Items> Items { get; set; }
    }

    public class Items
    {
        public string Id { get; set; }
        public int? ParentID { get; set; }
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public List<ChildrenDTO> Children { get; set; } = new List<ChildrenDTO>();
    }

    public class ChildrenDTO
    {
        public string? title { get; set; }
        public string? path { get; set; }

        public List<ChildrenDTO> Children { get; set; } = new List<ChildrenDTO>();


        public ChildrenDTO(string title, string path)
        {
            this.title = title;
            this.path = path;
        }

    }

}
