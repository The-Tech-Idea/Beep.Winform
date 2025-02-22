using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel; // Ensure correct namespace for BeepImage

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepImageColumn : DataGridViewColumn
    {
        public BeepImageColumn() : base(new BeepImageCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepImageCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepImageEditingControl); // Use BeepImage for editing
        public override Type ValueType => typeof(string); // Store image path as string
        public override object DefaultNewRowValue => string.Empty; // Default empty path

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepImageEditingControl control)
            {
                control.ImagePath = initialFormattedValue?.ToString();
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepImageEditingControl : BeepImage, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepImageEditingControl()
        {
            this.Size = new Size(100, 100);
            this.ScaleMode = ImageScaleMode.KeepAspectRatio;

            // Handle click event to open image selector
            this.Click += BeepImage_Click;
        }

        private void BeepImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog { Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.ImagePath = dlg.FileName;
                    valueChanged = true;
                    dataGridView?.NotifyCurrentCellDirty(true);
                }
            }
        }

        public object EditingControlFormattedValue
        {
            get => this.ImagePath;
            set => this.ImagePath = value?.ToString();
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.ImagePath;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => true;

        public void PrepareEditingControlForEdit(bool selectAll) { }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }
    }
}
