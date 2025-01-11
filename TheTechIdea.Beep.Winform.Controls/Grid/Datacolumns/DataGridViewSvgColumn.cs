using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Svg;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    [ToolboxBitmap(typeof(BeepDataGridViewSvgColumn), "DataGridViewImageColumn.bmp")]
    [ToolboxItem(false)]
    public class BeepDataGridViewSvgColumn : DataGridViewImageColumn
    {
        public BeepDataGridViewSvgColumn()
        {
            this.CellTemplate = new DataGridViewSvgCell();
        }
    }

    public class DataGridViewSvgCell : DataGridViewImageCell
    {
        public DataGridViewSvgCell()
        {
            // This column contains images (in this case, SVGs rendered as bitmaps)
            this.ValueType = typeof(string); // Path to SVG file
        }

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
                if (value is string svgPath)
                {
                    // Try to load the SVG from the given path
                    SvgDocument svgDocument = SvgDocument.Open(svgPath);

                    // Render the SVG as a Bitmap
                    Bitmap bitmap = svgDocument.Draw();

                    // Return the rendered bitmap
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, and potentially log them
                Console.WriteLine($"Error rendering SVG: {ex.Message}");
            }

            // Fallback to the base implementation if not an SVG or an error occurred
            return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }

        public override Type ValueType => typeof(string);  // The path to the SVG file
        public override Type FormattedValueType => typeof(Bitmap);  // The rendered Bitmap
    }
}
