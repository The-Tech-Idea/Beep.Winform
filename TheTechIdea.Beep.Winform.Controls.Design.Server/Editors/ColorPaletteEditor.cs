using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Themes;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Design-time editor for selecting colors from theme palettes
    /// Provides access to theme colors and custom color selection
    /// </summary>
    public class ColorPaletteEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider == null)
                return value;

            Color currentColor = value is Color color ? color : Color.Empty;

            // Use standard color dialog for now
            // In a full implementation, this would show a custom dialog with:
            // - Theme color palette browser
            // - Custom color picker
            // - Color harmony suggestions
            // - Recent colors
            using (var colorDialog = new ColorDialog
            {
                Color = currentColor.IsEmpty ? Color.Black : currentColor,
                AllowFullOpen = true,
                FullOpen = true
            })
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    return colorDialog.Color;
                }
            }

            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is Color color && !color.IsEmpty)
            {
                using (var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // Draw border
                using (var pen = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                }
            }
        }
    }
}
