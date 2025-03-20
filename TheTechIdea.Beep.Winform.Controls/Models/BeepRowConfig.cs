using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Models
{
    public class BeepRowConfig
    {
        public BeepRowConfig()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        public int OldDisplayIndex { get; set; }
        public bool IsDataLoaded { get; set; } = false;
        public BindingList<BeepCellConfig> Cells { get; set; } = new BindingList<BeepCellConfig>();
        public object RowData { get; set; }
        public bool IsSelected { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsEditable { get; set; }
        public bool IsVisible { get; set; }
        public bool IsAggregation { get; set; } = false;
        public int UpperX { get; set; } // Updated dynamically
        public int UpperY { get; set; } // Updated dynamically
        public int Width { get; set; } = 100; // Default cell width
        public int Height { get; set; } = 30; // Default cell height

        // Row Events
        public event EventHandler<BeepRowEventArgs> OnRowSelected;
        public event EventHandler<BeepRowEventArgs> OnRowValidate;
        public event EventHandler<BeepRowEventArgs> OnRowDelete;
        public event EventHandler<BeepRowEventArgs> OnRowAdded;
        public event EventHandler<BeepRowEventArgs> OnRowUpdate;
        //Cell Events
        public event EventHandler<BeepCellEventArgs> OnCellSelected;
        public event EventHandler<BeepCellEventArgs> OnCellValidate;
        //public override bool Equals(object obj)
        //{
        //    if (obj is BeepCellConfig other)
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
    public class BeepRowEventArgs : EventArgs
    {
        public BeepRowConfig Row { get; }
        public BeepRowEventArgs(BeepRowConfig row) => Row = row;
    }
    public class BeepRowSelectedEventArgs
    {
        public BeepRowSelectedEventArgs(int row, BeepRowConfig rowdata)
        {
            RowIndex = row;
            Row = rowdata;
        }
        public BeepRowSelectedEventArgs(int row)
        {
            RowIndex = row;
        }
        public BeepRowConfig Row { get; }
        public int RowIndex { get; set; }

    }
}
