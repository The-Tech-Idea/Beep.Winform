using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Dates;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// DateTime drop-down editor for grid cells.
    /// Opens the calendar popup immediately on begin-edit.
    /// </summary>
    public sealed class BeepGridDateDropDownEditor : IGridEditor
    {
        public Control CreateControl()
        {
            return new BeepDateDropDown { IsChild = true, GridMode = true };
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            if (control is not BeepDateDropDown ddd) return;
            ddd.GridMode = true;
            ddd.BackColor = Color.White;
            ddd.ForeColor = Color.Black;
            ddd.BorderStyle = BorderStyle.FixedSingle;
        }

        public void SetValue(Control control, object value)
        {
            if (control is not BeepDateDropDown ddd) return;

            if (value is DateTime dt)
                ddd.SelectedDateTime = dt;
            else if (value is string s && DateTime.TryParse(s, out var parsed))
                ddd.SelectedDateTime = parsed;
            else
                ddd.SelectedDateTime = DateTime.Now;

            if (control is IBeepUIComponent ic) ic.SetValue(value);
        }

        public object GetValue(Control control)
        {
            return (control as IBeepUIComponent)?.GetValue();
        }

        public void AttachEvents(Control control, IGridEditorEvents events)
        {
            if (control is BeepDateDropDown ddd && events != null)
            {
                ddd.DropDownClosed += (s, e) => events.RequestEndEdit(true);
            }
        }

        public void DetachEvents(Control control, IGridEditorEvents events) { }

        public void OnBeginEdit(Control control)
        {
            if (control is BeepDateDropDown ddd)
            {
                try { ddd.ShowPopup(); } catch { }
            }
        }

        public bool IsPopupOpen(Control control)
        {
            return control is BeepDateDropDown ddd && ddd._isPopupOpen;
        }
    }
}
