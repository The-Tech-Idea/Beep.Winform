using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [Serializable]
    public class ColumnConfig
    {
        public ColumnConfig()
        {
           // Column = column;
          //  GuidID = Guid.NewGuid().ToString();
        }
        public string ColumnName { get; set; }
        public string ColumnCaption { get; set; }
        public string ColumnType { get; set; }
        public int Index { get; set; }
        public string GuidID { get; set; }
        public bool IsReadOnly { get; set; }=false;
        public bool IsFilteOn { get; set; } =false;
        public bool IsTotalOn { get; set; }=false;
        public bool HasTotal { get; set; } = false;
        public bool IsFiltered { get; set; } = false;
        public bool IsSorted { get; set; } = false;
        public List<string> AutoCompleteSource { get; set; }
        public decimal Total { get; set; }
        public string Filter { get; set; }
        public decimal OldValue  {get;set;}
        public decimal NewValue { get; set; }
        public int ColumnWidth { get; set; }
        public string ColumnFormat { get; set; }
        public string ColumnAlignment { get; set; }
        public string ColumnBackColor { get; set; }
        public string ColumnForeColor { get; set; }
        public string ColumnFont { get; set; }
        public string ColumnToolTip { get; set; }
        public string ColumnVisible { get; set; }
        public string ColumnVisibleIndex { get; set; }
    }
}
