                                                                             using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    
    public class BeepDataGridViewColorPickerColumn : DataGridViewColumn
    {
        public BeepDataGridViewColorPickerColumn() : base(new DataGridViewColorPickerCell())
        {
        }

        public override object Clone()
        {
            BeepDataGridViewColorPickerColumn clone = (BeepDataGridViewColorPickerColumn)base.Clone();
            return clone;
        }
    }

    public class DataGridViewColorPickerCell : DataGridViewTextBoxCell
    {
        public DataGridViewColorPickerCell()
            : base()
        {
            this.Style.Format = "Color";
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            DataGridViewColorPickerEditingControl colorControl = DataGridView.EditingControl as DataGridViewColorPickerEditingControl;

            // Use the current cell value as the initial value for the color picker
            if (this.Value == null || this.Value == DBNull.Value)
            {
                colorControl.SelectedColor = Color.Black;
            }
            else
            {
                colorControl.SelectedColor = (Color)this.Value;
            }
        }

        public override Type EditType
        {
            get
            {
                return typeof(DataGridViewColorPickerEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(Color);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return Color.Black;
            }
        }
    }
    [ToolboxItem(false)]
    public class DataGridViewColorPickerEditingControl : UserControl, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        private Button colorButton;
        private ColorDialog colorDialog;

        public DataGridViewColorPickerEditingControl()
        {
            // Setup control elements
            colorButton = new Button
            {
                Text = "Choose Color",
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            colorDialog = new ColorDialog();

            colorButton.Click += ColorButton_Click;
            this.Controls.Add(colorButton);
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.SelectedColor = colorDialog.Color;
            }
        }

        public Color SelectedColor
        {
            get
            {
                return colorButton.BackColor;
            }
            set
            {
                colorButton.BackColor = value;
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue property
        public object EditingControlFormattedValue
        {
            get { return SelectedColor; }
            set
            {
                if (value is Color colorValue)
                {
                    this.SelectedColor = colorValue;
                }
            }
        }

        // Implements the IDataGridViewEditingControl.GetEditingControlFormattedValue method
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the IDataGridViewEditingControl.ApplyCellStyleToEditingControl method
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex property
        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey method
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the control handle input keys.
            return true;
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit method
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation is needed
        }

        // Implements the IDataGridViewEditingControl.RepositionEditingControlOnValueChange property
        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlDataGridView property
        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlValueChanged property
        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        // Implements the IDataGridViewEditingControl.EditingPanelCursor property
        public Cursor EditingPanelCursor
        {
            get { return Cursors.Default; }
        }

        // Implements the IDataGridViewEditingControl.EditingControlCursor property
        public Cursor EditingControlCursor
        {
            get { return base.Cursor; }
        }
    }

}
