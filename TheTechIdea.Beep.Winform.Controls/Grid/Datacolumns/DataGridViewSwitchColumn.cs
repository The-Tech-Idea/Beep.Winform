using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewSwitchColumn : DataGridViewColumn
    {
        public BeepDataGridViewSwitchColumn() : base(new DataGridViewSwitchCell())
        {
        }

        public override object Clone()
        {
            var clone = (BeepDataGridViewSwitchColumn)base.Clone();
            return clone;
        }
    }
    public class DataGridViewSwitchCell : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var control = DataGridView.EditingControl as SwitchEditingControl;

            if (control != null)
            {
                // Set the switch state based on the cell value
                control.IsOn = (bool)(this.Value ?? false);
            }
        }

        public override Type EditType => typeof(SwitchEditingControl);

        public override Type ValueType => typeof(bool);

        public override object DefaultNewRowValue => false;
    }
    public class SwitchEditingControl : Control, IDataGridViewEditingControl
    {
        private bool isOn;
        private DataGridView dataGridView;
        private bool valueChanged;
        private int rowIndex;

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    Invalidate(); // Redraw control when value changes
                }
            }
        }

        public SwitchEditingControl()
        {
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(50, 25);
            this.MouseClick += SwitchEditingControl_MouseClick;
        }

        // Toggles the switch state when clicked
        private void SwitchEditingControl_MouseClick(object sender, MouseEventArgs e)
        {
            IsOn = !IsOn;
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        // Draw the switch
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Background of the control
            g.FillRectangle(IsOn ? Brushes.Green : Brushes.Red, 0, 0, this.Width, this.Height);

            // Circle (button)
            g.FillEllipse(Brushes.White, IsOn ? this.Width - this.Height : 0, 0, this.Height, this.Height);

            // Draw border
            g.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property
        public object EditingControlFormattedValue
        {
            get => IsOn;
            set
            {
                if (value is bool boolValue)
                {
                    IsOn = boolValue;
                }
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => IsOn;

        // Implements the IDataGridViewEditingControl.ApplyCellStyleToEditingControl method
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.BackColor = dataGridViewCellStyle.BackColor;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property
        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the switch handle specific keys
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Space:
                case Keys.Enter:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit method
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation is needed for the switch control
        }

        // Implements the IDataGridViewEditingControl.RepositionEditingControlOnValueChange property
        public bool RepositionEditingControlOnValueChange => false;

        // Implements the IDataGridViewEditingControl.EditingControlDataGridView property
        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        // Implements the IDataGridViewEditingControl.EditingControlValueChanged property
        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        // Implements the IDataGridViewEditingControl.EditingPanelCursor property
        public Cursor EditingPanelCursor => Cursors.Default;
    }
}
