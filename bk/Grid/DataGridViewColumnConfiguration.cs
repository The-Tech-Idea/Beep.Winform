using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [Serializable]
    public class DataGridViewColumnConfiguration
    {
        public string Name { get; set; }
        public int DisplayIndex { get; set; }
        public string DataPropertyName { get; set; }
        public DataGridViewAutoSizeColumnMode AutoSizeMode { get; set; }
        public DataGridViewCell CellTemplate { get; set; }
        public ContextMenuStrip ContextMenuStrip { get; set; }
        public DataGridViewCellStyle DefaultCellStyle { get; set; }
        public Type DefaultHeaderCellType { get; set; }
        public bool Displayed { get; set; }
        public int DividerWidth { get; set; }
        public float FillWeight { get; set; }
        public bool Frozen { get; set; }
        public bool HasDefaultCellStyle { get; set; }
        public DataGridViewColumnHeaderCell HeaderCell { get; set; }
        public string HeaderText { get; set; }
        public int InheritedAutoSizeMode { get; set; }
        public DataGridViewCellStyle InheritedStyle { get; set; }
        public bool IsDataBound { get; set; }
        public bool IsNameSet { get; set; }
        public bool IsValueTypeSet { get; set; }
        public int MinimumWidth { get; set; }
        public bool ReadOnly { get; set; }
        public DataGridViewTriState Resizable { get; set; }
     
        public DataGridViewColumnSortMode SortMode { get; set; }
        public int SortPropertyCore { get; set; }
        public DataGridViewElementStates State { get; set; }
        public string ToolTipText { get; set; }
        public Type ValueType { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }

        // Additional properties you may want to include

        // Constructors, methods, and additional logic as needed
    }




}
