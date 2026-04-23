using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// Masked text editor for grid cells.
    /// Uses <see cref="BeepTextBox"/> with masking properties applied from the column config.
    /// </summary>
    public sealed class BeepGridMaskedEditor : IGridEditor
    {
        public Control CreateControl()
        {
            return new BeepTextBox { IsChild = true, GridMode = true };
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            if (control is not BeepTextBox st) return;
            st.GridMode = true;
            st.BackColor = Color.White;
            st.ForeColor = Color.Black;
            st.BorderStyle = BorderStyle.FixedSingle;

            // Apply mask configuration from column if available
            if (!string.IsNullOrEmpty(column.CustomMask))
            {
                st.MaskFormat = TextBoxMaskFormat.Custom;
                st.CustomMask = column.CustomMask;
            }
            else if (column.MaskFormat != TextBoxMaskFormat.None)
            {
                st.MaskFormat = column.MaskFormat;
            }
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
