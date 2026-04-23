using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// Numeric up-down editor for grid cells.
    /// </summary>
    public sealed class BeepGridNumericEditor : IGridEditor
    {
        public Control CreateControl()
        {
            return new BeepNumericUpDown { IsChild = true, GridMode = true };
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            if (control is not BeepNumericUpDown num) return;
            num.GridMode = true;
            num.BackColor = Color.White;
            num.ForeColor = Color.Black;
        }

        public void SetValue(Control control, object value)
        {
            if (control is IBeepUIComponent ic) ic.SetValue(value);
        }

        public object GetValue(Control control)
        {
            return (control as IBeepUIComponent)?.GetValue();
        }

        public void AttachEvents(Control control, IGridEditorEvents events) { }
        public void DetachEvents(Control control, IGridEditorEvents events) { }
        public void OnBeginEdit(Control control) { }
        public bool IsPopupOpen(Control control) => false;
    }
}
