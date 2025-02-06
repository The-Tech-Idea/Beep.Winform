using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewImageComboBoxColumn : DataGridViewColumn
    {
        public BeepDataGridViewImageComboBoxColumn() : base(new DataGridViewImageComboBoxCell())
        {
        }
        public override object Clone()
        {
            var clone = (BeepDataGridViewImageComboBoxColumn)base.Clone();
            return clone;
        }
    }
    public class DataGridViewImageComboBoxCell : DataGridViewTextBoxCell
    {
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var control = DataGridView.EditingControl as ImageComboBoxEditingControl;
            if (control != null)
            {
                control.Items.Clear(); // Clear existing rootnodeitems

                //// Example: adding some rootnodeitems with image and text
                //control.Buttons.Add(new ImageComboBoxItem("Option 1", Properties.Resources.Icon1));
                //control.Buttons.Add(new ImageComboBoxItem("Option 2", Properties.Resources.Icon2));
                //control.Buttons.Add(new ImageComboBoxItem("Option 3", Properties.Resources.Icon3));

                // Set the selected value based on the cell's value
                control.SelectedItem = this.Value;
            }
        }

        public override Type EditType => typeof(ImageComboBoxEditingControl);

        public override Type ValueType => typeof(string); // Assuming string as the value type

        public override object DefaultNewRowValue => "Option 1";
    }
    // Custom ComboBox control with images
    public class ImageComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged;
        private int rowIndex;

        public ImageComboBoxEditingControl()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DropDownStyle = ComboBoxStyle.DropDownList; // Ensures no user input
            this.DrawItem += ImageComboBoxEditingControl_DrawItem;
        }

        // Handles custom drawing of the combo box rootnodeitems (image + text)
        private void ImageComboBoxEditingControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                ImageComboBoxItem item = (ImageComboBoxItem)this.Items[e.Index];
                if (item.Image != null)
                {
                    e.Graphics.DrawImage(item.Image, new Rectangle(e.Bounds.Left, e.Bounds.Top, 20, 20)); // Draw image
                }
                e.Graphics.DrawString(item.Text, e.Font, Brushes.Black, e.Bounds.Left + 24, e.Bounds.Top); // Draw text
            }
            e.DrawFocusRectangle();
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedItem;
            set
            {
                if (value is ImageComboBoxItem item)
                {
                    this.SelectedItem = item;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        public Cursor EditingPanelCursor => Cursors.Default;

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public bool RepositionEditingControlOnValueChange => false;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }
    }

    // Helper class to represent an item in the combo box that has an image and text
    public class ImageComboBoxItem
    {
        public string Text { get; set; }
        public Image Image { get; set; }

        public ImageComboBoxItem(string text, Image image)
        {
            this.Text = text;
            this.Image = image;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
