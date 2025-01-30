using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns.CustomDataGridViewColumns;

namespace TheTechIdea.Beep.Winform.Controls
{
    [Serializable]
    public class BeepGridColumnConfig
    {
        public BeepGridColumnConfig()
        {
            GuidID = Guid.NewGuid().ToString();
            Width = 100;
            Visible = true;
            ColumnType = "Text";
            ColumnCaption = "Column";
        }

        public string ColumnType { get; set; }
        public BeepGridColumnType CellEditor { get; set; }
        public string Name { get; set; }
        [Required]
        public string ColumnCaption { get; set; }
        public int Width { get; set; }
        public string Format { get; set; }
        public string Alignment { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public string Font { get; set; }
        public string ToolTip { get; set; }
        public bool Visible { get; set; }
        public int Index { get; set; }
        public int VisibleIndex { get; set; }
        public bool ReadOnly { get; set; }
        public bool TotalOn { get; set; }
        public bool HasTotal { get; set; }
        public string GuidID { get; set; }
        public bool IsReadOnly { get; set; } = false;
        public bool IsFilteOn { get; set; } = false;
        public bool IsTotalOn { get; set; } = false;
        public bool IsFiltered { get; set; } = false;
        public bool IsSorted { get; set; } = false;
        public List<string> AutoCompleteSource { get; set; }
        public decimal Total { get; set; }
        public string Filter { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public string DataSourceName { get; set; }
        public string DataSourceType { get; set; }
        public string Query { get; set; }
        public string ColumnName { get; set; }
        public string ParentColumn { get; set; }
        public string ChildColumn { get; set; }
        public DataSourceMode DataSourceMode { get; set; }
        public DataGridViewColumnSortMode SortMode { get; set; }
        public DataGridViewTriState Resizable { get; set; }
        public DataGridViewAutoSizeColumnMode AutoSizeMode { get; set; }
        public int DisplayIndex { get; set; }
        public int DividerWidth { get; set; }
        public string DisplayMember { get; set; }
        public string ValueMember { get; set; }
        public string FilterMember { get; set; }
        public string SortMember { get; set; }
        public Color ProgressBarColor { get; set; }
        public int ProgressBarMaxValue { get; set; }
        public int ProgressBarMinValue { get; set; }
        public int ProgressBarStep { get; set; }
        public int ProgressBarValue { get; set; }
        public string ProgressBarStyle { get; set; }
        public string ProgressBarText { get; set; }
        public Color FilledStarColor { get; set; }
        public Color EmptyStarColor { get; set; }
        public int MaxStars { get; set; }
        public bool Frozen { get; set; }
        public int MinimumWidth { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public string ColumnFormat { get; set; }
        public string ColumnAlignment { get; set; }
        public string ColumnBackColor { get; set; }
        public string ColumnForeColor { get; set; }
        public string ColumnFont { get; set; }
        public string ColumnToolTip { get; set; }
        public string ColumnVisible { get; set; }
        public string ColumnVisibleIndex { get; set; }
       public  ColumnHeaderStyle HeaderStyle { get; set; } = new ColumnHeaderStyle();

    }
    public class ColumnHeaderStyle
    { // Defines column header properties like width, font, etc.
        public SizeF Width { get; set; }
        public string Typography { get; set; }
        public string FontSize { get; set; }
        public string ForeColor { get; set; }
        public string BackColor { get; set; }
    }
    [Serializable]
    public class BeepGridColumnConfigCollection : BindingList<BeepGridColumnConfig>
    {
        public BeepGridColumnConfigCollection() : base() { }
        public BeepGridColumnConfigCollection(BindingList<BeepGridColumnConfigCollection> list) { }
    }
    public enum MenuItemType
    {
        Main,
        Child
    }
    public enum BeepGridColumnType
    {
        Text,
        CheckBox,
        ComboBox,
        DateTime,
        Image,
        ProgressBar,
        Rating,
        StarRating,
        Button,
        Link,
        Custom
    }
}
