using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Svg;
using TheTechIdea.Beep.Winform.Controls; // Ensure correct namespace for BeepImage

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
   [ToolboxItem(false)]
    public class BeepDataGridViewSvgColumn : DataGridViewColumn
    {
        public BeepDataGridViewSvgColumn() : base(new BeepDataGridViewSvgCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepDataGridViewSvgCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepDataGridViewSvgEditingControl); // Use BeepImage for editing
        public override Type ValueType => typeof(string); // Store image path as a string
        public override object DefaultNewRowValue => string.Empty; // Default to empty path

        protected override object GetFormattedValue(
            object value,
            int rowIndex,
            ref DataGridViewCellStyle cellStyle,
            TypeConverter valueTypeConverter,
            TypeConverter formattedValueTypeConverter,
            DataGridViewDataErrorContexts context)
        {
            try
            {
                if (value is string svgPath && !string.IsNullOrEmpty(svgPath))
                {
                    // Load SVG and convert to Bitmap
                    SvgDocument svgDocument = SvgDocument.Open(svgPath);
                    Bitmap bitmap = svgDocument.Draw();

                    // Return the rendered bitmap
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, log them
                Console.WriteLine($"Error rendering SVG: {ex.Message}");
            }

            // Fallback to an empty image if an error occurs
            return new Bitmap(1, 1);
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepDataGridViewSvgEditingControl control)
            {
                control.ImagePath = initialFormattedValue?.ToString() ?? string.Empty;
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepDataGridViewSvgEditingControl : BeepImage, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepDataGridViewSvgEditingControl()
        {
            this.Size = new Size(100, 100); // Default size
            this.BackColor = Color.White;
        }

        public object EditingControlFormattedValue
        {
            get => this.ImagePath;
            set => this.ImagePath = value?.ToString() ?? string.Empty;
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
