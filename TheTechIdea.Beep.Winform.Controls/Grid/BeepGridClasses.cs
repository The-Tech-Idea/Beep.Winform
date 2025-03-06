
using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    #region "Beep Grid Data Classes"
    // Represents a single row in the grid
    public class BeepGridRow
    {
        public BeepGridRow()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        public int OldDisplayIndex { get; set; }
        public bool IsDataLoaded { get; set; } = false;
        public BindingList<BeepGridCell> Cells { get; set; } = new BindingList<BeepGridCell>();
        public object RowData { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
       
        public int UpperX { get; set; } // Updated dynamically
        public int UpperY { get; set; } // Updated dynamically
        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height

        // Row Events
        public event EventHandler<BeepGridRowEventArgs> OnRowSelected;
        public event EventHandler<BeepGridRowEventArgs> OnRowValidate;
        public event EventHandler<BeepGridRowEventArgs> OnRowDelete;
        public event EventHandler<BeepGridRowEventArgs> OnRowAdded;
        public event EventHandler<BeepGridRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;
        //public override bool Equals(object obj)
        //{
        //    if (obj is BeepGridCell other)
        //    {
        //        return this.Id == other.Id;
        //    }
        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}


        public void ApplyTheme(BeepTheme theme)
        {
            foreach (var cell in Cells)
            {
                cell.ApplyTheme(theme);
            }
        }
    }

    // Represents a single cell in the grid
    public class BeepGridCell
    {
        public BeepGridCell()
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
        public bool IsSelected { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsVisible { get; set; }
        public object CellData { get; set; }
        = new object();
        public object CellValue { get; set; }
        public string CellType { get; set; }
        public string CellFormat { get; set; }
        public string CellAlignment { get; set; }
        public string CellBackColor { get; set; }
        public string CellForeColor { get; set; }
        public string CellFont { get; set; }
        public string CellToolTip { get; set; }
        public object OldValue { get; set; } = new object();

        public event EventHandler<BeepGridCellEventArgs> OnCellSelected;
        public event EventHandler<BeepGridCellEventArgs> OnCellValidate;

        public IBeepUIComponent UIComponent { get; set; }
        // Method to trigger OnCellSelected event
        public void SelectCell()
        {
            OnCellSelected?.Invoke(this, new BeepGridCellEventArgs(this));
        }

        // Method to trigger OnCellValidate event
        public void ValidateCell()
        {
            OnCellValidate?.Invoke(this, new BeepGridCellEventArgs(this));
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
    public class BeepGridRowEventArgs : EventArgs
    {
        public BeepGridRow Row { get; }
        public BeepGridRowEventArgs(BeepGridRow row) => Row = row;
    }

    // Custom event args for cell events
    public class BeepGridCellEventArgs : EventArgs
    {
        public BeepGridCell Cell { get; }

        public BeepGridCellEventArgs(BeepGridCell cell) => Cell = cell;
    }
    #endregion "Beep Grid Data Classes"
}
