using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Dynamic;

namespace VisssStock.Application.Models
{
    public class DefaultData
    {
        public string name { get; set; }
        public Guid id { get; set; }
        public List<CellDataX> celldata { get; set; }
        public int order { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public Dictionary<string, object> Config { get; set; } = new Dictionary<string, object>();
        public object pivotTable { get; set; }
        public bool isPivotTable { get; set; }
        public int status { get; set; }
        public List<string> luckysheet_selection_range { get; set; }
        public List<Object> dataVerification { get; set; }
    }

    public class CellDataX
    {
        public int R { get; set; }
        public int C { get; set; }
        public object V { get; set; }
    }
}

