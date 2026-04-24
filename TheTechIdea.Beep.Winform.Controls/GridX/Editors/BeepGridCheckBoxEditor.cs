using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// CheckBox editor for grid cells. Supports bool, char, and string variants.
    /// </summary>
    public sealed class BeepGridCheckBoxEditor : IGridEditor
    {
        private readonly BeepColumnType _type;

        public BeepGridCheckBoxEditor(BeepColumnType type)
        {
            _type = type;
        }

        public Control CreateControl()
        {
            IBeepUIComponent editor = _type switch
            {
                BeepColumnType.CheckBoxBool => new BeepCheckBoxBool { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxChar => new BeepCheckBoxChar { IsChild = true, GridMode = true },
                BeepColumnType.CheckBoxString => new BeepCheckBoxString { IsChild = true, GridMode = true },
                _ => new BeepCheckBoxBool { IsChild = true, GridMode = true }
            };
            return (Control)editor;
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            if (control is not IBeepUIComponent) return;
            var t = theme as IBeepTheme ?? BeepThemesManager.CurrentTheme;
            control.BackColor = t?.BackColor != Color.Empty ? t.BackColor : Color.White;
            control.ForeColor = t?.ForeColor != Color.Empty ? t.ForeColor : Color.Black;

            if (control is BeepCheckBoxBool cbBool)
            {
                cbBool.GridMode = true;
                cbBool.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }
            else if (control is BeepCheckBoxChar cbChar)
            {
                cbChar.GridMode = true;
                cbChar.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }
            else if (control is BeepCheckBoxString cbString)
            {
                cbString.GridMode = true;
                cbString.CheckBoxSize = Math.Min(cell.Rect.Width - 4, cell.Rect.Height - 4);
            }

            // Theme is applied by GridEditHelper.BeginEdit() for all BaseControl editors
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
