using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Editors
{
    /// <summary>
    /// Generic fallback editor for less-common column types:
    /// Image, Button, ProgressBar, Radio, ListBox, ListOfValue.
    /// </summary>
    public sealed class BeepGridGenericEditor : IGridEditor
    {
        private readonly BeepColumnType _type;

        public BeepGridGenericEditor(BeepColumnType type)
        {
            _type = type;
        }

        public Control CreateControl()
        {
            IBeepUIComponent editor = _type switch
            {
                BeepColumnType.Image => new BeepImage { IsChild = true },
                BeepColumnType.Button => new BeepButton { IsChild = true },
                BeepColumnType.ProgressBar => new BeepProgressBar { IsChild = true },
                BeepColumnType.Radio => new BeepRadioGroup { IsChild = true, GridMode = true },
                BeepColumnType.ListBox => new BeepListBox { IsChild = true, GridMode = false },
                BeepColumnType.ListOfValue => new BeepListofValuesBox { IsChild = true, GridMode = false },
                _ => new BeepTextBox { IsChild = true, GridMode = true }
            };
            return (Control)editor;
        }

        public void Setup(Control control, BeepColumnConfig column, BeepCellConfig cell, object theme)
        {
            control.BackColor = Color.White;
            control.ForeColor = Color.Black;

            if (control is BeepListBox listBox)
            {
                listBox.ListItems.Clear();
                var items = column?.Items ?? new System.Collections.Generic.List<SimpleItem>();
                foreach (var item in items) listBox.ListItems.Add(item);
                listBox.GridMode = false;
                listBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is BeepRadioGroup radio)
            {
                radio.GridMode = true;
            }
            else if (control is BeepProgressBar pb)
            {
                // No special setup needed
            }
            else if (control is BeepImage img)
            {
                // No special setup needed
            }
            else if (control is BeepButton btn)
            {
                // No special setup needed
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
