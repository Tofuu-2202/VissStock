using Newtonsoft.Json;

namespace VisssStock.Application.Models
{
    #region CellData
    public class CellData
    {
        [JsonProperty("r")]
        public int Row { get; set; }

        [JsonProperty("c")]
        public int Column { get; set; }

        [JsonProperty("v")]
        public CellValue Value { get; set; }

        public int CreateDate { get; set; }

        public CellData Clone()
        {
            return new CellData
            {
                Row = this.Row,
                Column = this.Column,
                Value = this.Value?.Clone() // Sao chép sâu đối tượng Value
            };
        }
    }

    public class CellValue
    {
        [JsonProperty("ct")]
        public CellType CellType { get; set; }

        [JsonProperty("m")]
        public string? DisplayValue { get; set; }

        [JsonProperty("v")]
        public object? ActualValue { get; set; }

        [JsonProperty("bl")]
        public int? Bold { get; set; }

        [JsonProperty("it")]
        public int? Italic { get; set; }

        [JsonProperty("ff")]
        public int? FontFamily { get; set; }

        [JsonProperty("fs")]
        public int? FontSize { get; set; }

        [JsonProperty("ht")]
        public int? HorizontalAlignment { get; set; }

        [JsonProperty("vt")]
        public int? VerticalAlignment { get; set; }

        [JsonProperty("f")]
        public string? Formula { get; set; }

        public CellValue Clone()
        {
            return new CellValue
            {
                CellType = this.CellType,
                DisplayValue = this.DisplayValue,
                ActualValue = this.ActualValue,
                Bold = this.Bold,
                Italic = this.Italic,
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                HorizontalAlignment = this.HorizontalAlignment,
                VerticalAlignment = this.VerticalAlignment,
                Formula = this.Formula
            };
        }
    }

    public class CellType
    {
        [JsonProperty("fa")]
        public string? Format { get; set; }

        [JsonProperty("t")]
        public string? Type { get; set; }

        [JsonProperty("g")]
        public string? g { get; set; }
    }
    #endregion
    #region border style
    public class BorderInfo
    {
        public string RangeType { get; set; }
        public Value Value { get; set; }
    }

    public class Value
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public BorderStyle L { get; set; }
        public BorderStyle R { get; set; }
        public BorderStyle T { get; set; }
        public BorderStyle B { get; set; }
    }

    public class BorderStyle
    {
        public int Style { get; set; }
        public string Color { get; set; }
    }

    public class RangeInfo
    {
        public string RangeType { get; set; }
        public string BorderType { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public List<Range> Range { get; set; }
    }

    public class Range
    {
        public List<int> Row { get; set; }
        public List<int> Column { get; set; }
    }
    #endregion
    public class Config
    {
        public Dictionary<string, object> merge { get; set; }
        public Dictionary<string, object> rowlen { get; set; }
        public Dictionary<string, object> columnlen { get; set; }
        public Dictionary<string, object> rowhidden { get; set; }
        public Dictionary<string, object> colhidden { get; set; }
        public List<BorderInfo> borderInfo { get; set; }
        public Dictionary<string, object> authority { get; set; }
    }

    public class range
    {
        public int? row_focus { get; set; }
        public int? colum_forcus { get; set; }
    }

    public class frozen
    {
        public string type { get; set; }
        public range range { get; set; }
    }

    public class Worksheet
    {
        public string? name { get; set; }
        public string? color { get; set; }
        public string? id { get; set; }
        public int? status { get; set; }
        public int? order { get; set; }
        public int? hide { get; set; }
        public int? row { get; set; }
        public int? column { get; set; }
        public int? defaultRowHeight { get; set; }
        public int? defaultColWidth { get; set; }
        public List<CellData> celldata { get; set; }
        public Config? config { get; set; }
        public int? scrollLeft { get; set; }
        public int? scrollTop { get; set; }
        public List<object> luckysheet_select_save { get; set; }
        public List<object> calcChain { get; set; }
        public bool? isPivotTable { get; set; }
        public Dictionary<string, object> pivotTable { get; set; }
        public Dictionary<string, object> filter_select { get; set; }
        public object? filter { get; set; }
        public List<object> luckysheet_alternateformat_save { get; set; }
        public List<object> luckysheet_alternateformat_save_modelCustom { get; set; }
        public Dictionary<string, object> luckysheet_conditionformat_save { get; set; }
        public frozen frozen { get; set; }
        public List<object> chart { get; set; }
        public double? zoomRatio { get; set; }
        public List<object> image { get; set; }
        public int? showGridLines { get; set; }

        public Worksheet Clone()
        {
            return new Worksheet
            {
                name = this.name,
                color = this.color,
                id = this.id,
                status = this.status,
                order = this.order,
                hide = this.hide,
                row = this.row,
                column = this.column,
                defaultRowHeight = this.defaultRowHeight,
                defaultColWidth = this.defaultColWidth,
                celldata = this.celldata.Select(x => x.Clone()).ToList(),
                config = this.config,
                scrollLeft = this.scrollLeft,
                scrollTop = this.scrollTop,
                luckysheet_select_save = this.luckysheet_select_save,
                calcChain = this.calcChain,
                isPivotTable = this.isPivotTable,
                pivotTable = this.pivotTable,
                filter_select = this.filter_select,
                filter = this.filter,
                luckysheet_alternateformat_save = this.luckysheet_alternateformat_save,
                luckysheet_alternateformat_save_modelCustom = this.luckysheet_alternateformat_save_modelCustom,
                luckysheet_conditionformat_save = this.luckysheet_conditionformat_save,
                frozen = this.frozen,
                chart = this.chart,
                zoomRatio = this.zoomRatio,
                image = this.image,
                showGridLines = this.showGridLines
            };
        }
    }
}
