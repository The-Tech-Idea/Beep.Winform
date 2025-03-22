using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public class BeepCellConfig
    {
        public BeepCellConfig()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int ColumnIndex { get; set; }
        public int DisplayIndex { get; set; }

        private int _rowIdx; // used for to store the display header  of row
        public int RowIndex
        {
            get { return _rowIdx; }
            set { _rowIdx = value; }
        }
        public int X { get; set; } // Updated dynamically
        public int Y { get; set; } // Updated dynamically
        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height
        public Rectangle Rect{ get; set; } // Updated dynamically
        public bool IsSelected { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsVisible { get; set; }
        public bool IsAggregation { get; set; } = false;
        public object CellData { get; set; }
        = new object();
        public object CellValue { get; set; }
        public object ParentCellValue { get; set; }
        public SimpleItem ParentItem { get; set; }
        public string CellType { get; set; }
        public string CellFormat { get; set; }
        public string CellAlignment { get; set; }
        public string CellBackColor { get; set; }
        public string CellForeColor { get; set; }
        public string CellFont { get; set; }
        public string CellToolTip { get; set; }
        public object OldValue { get; set; } = new object();
        public SimpleItem Item { get; set; }
        public List<SimpleItem> FilterdList { get; set; } = new List<SimpleItem>();

        public event EventHandler<BeepCellEventArgs> OnCellSelected;
        public event EventHandler<BeepCellEventArgs> OnCellValidate;
        public event EventHandler<BeepCellEventArgs> OnCellValueChanged;

        public IBeepUIComponent UIComponent { get; set; }
        // Method to trigger OnCellSelected event
        public void SelectCell()
        {
            OnCellSelected?.Invoke(this, new BeepCellEventArgs(this));
        }

        // Method to trigger OnCellValidate event
        public void ValidateCell()
        {
            OnCellValidate?.Invoke(this, new BeepCellEventArgs(this));
        }
        public void ApplyTheme(BeepTheme theme)
        {
            if (UIComponent != null)
            {
                UIComponent.ApplyTheme(theme);
            }
        }
    }

    // Custom event args for row events
    public class BeepCellSelectedEventArgs
    {
        public BeepCellSelectedEventArgs(int row, int col, BeepCellConfig cell)
        {
            Row = row;
            Col = col;
            Cell = cell;
        }
        public BeepCellSelectedEventArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public int Row { get; }
        public int Col { get; }
        public BeepCellConfig Cell { get; set; }
    }

    // Custom event args for cell events
    public class BeepCellEventArgs : EventArgs
    {
        public BeepCellConfig Cell { get; }
        public bool Cancel { get; set; }= false;
        public Graphics Graphics { get; set; }
        public BeepCellEventArgs(BeepCellConfig cell) => Cell = cell;
    }
}
