using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// ComboBox editor for grid cells with list-item population and popup handling.
    /// </summary>
    public sealed class BeepGridComboBoxEditor : IGridEditor
    {
        public Control CreateControl()
        {
            return new BeepComboBox { IsChild = true, GridMode = false };
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            if (control is not BeepComboBox combo) return;

            combo.ListItems.Clear();
            var items = GetFilteredItems(column, cell);
            foreach (var item in items) combo.ListItems.Add(item);

            combo.GridMode = false;
            combo.BackColor = Color.White;
            combo.ForeColor = Color.Black;
            combo.BorderStyle = BorderStyle.FixedSingle;
        }

        public void SetValue(Control control, object value)
        {
            if (control is IBeepUIComponent ic) ic.SetValue(value);
        }

        public object GetValue(Control control)
        {
            return (control as IBeepUIComponent)?.GetValue();
        }

        public void AttachEvents(Control control, IGridEditorEvents events)
        {
            if (control is BeepComboBox combo && events != null)
            {
                combo.PopupClosed += (s, e) => events.RequestEndEdit(true);
            }
        }

        public void DetachEvents(Control control, IGridEditorEvents events)
        {
            // Event handler is a closure; no specific removal needed for the closure pattern
            // because the control will be disposed.
        }

        public void OnBeginEdit(Control control)
        {
            // Popup is opened via user interaction or can be triggered here if desired.
        }

        public bool IsPopupOpen(Control control)
        {
            return control is BeepComboBox combo && combo.IsDropdownOpen;
        }

        private System.Collections.Generic.List<SimpleItem> GetFilteredItems(BeepColumnConfig column, BeepCellConfig cell)
        {
            var baseItems = column?.Items ?? new System.Collections.Generic.List<SimpleItem>();
            if (baseItems == null || baseItems.Count == 0)
                return new System.Collections.Generic.List<SimpleItem>();

            if (!string.IsNullOrEmpty(column.ParentColumnName))
            {
                object parentValue = cell?.ParentCellValue;
                if (cell?.FilterdList != null && cell.FilterdList.Count > 0)
                    return cell.FilterdList;
                if (parentValue != null)
                    return baseItems.Where(i => i.ParentValue?.ToString() == parentValue.ToString()).ToList();
            }
            return baseItems.ToList();
        }
    }
}
