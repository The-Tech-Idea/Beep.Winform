using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Design-time editor for icon properties
    /// Launches IconPickerDialog to browse the SvgsUI icon library
    /// </summary>
    public class IconPickerEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider == null) return value;

            // Get the current icon name
            string currentIcon = value as string ?? string.Empty;

            // Launch the icon picker dialog
            using (var dialog = new IconPickerDialog(currentIcon))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedIconPath;
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
            if (e.Value is string iconPath && !string.IsNullOrEmpty(iconPath))
            {
                try
                {
                    // Try to draw a mini preview of the icon
                    // This would require loading the SVG, which we'll skip for now
                    // Just draw a colored square to indicate an icon is selected
                    using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(100, 150, 200)))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                    
                    // Draw a small icon glyph
                    using (var font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold))
                    using (var textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                    {
                        var format = new System.Drawing.StringFormat
                        {
                            Alignment = System.Drawing.StringAlignment.Center,
                            LineAlignment = System.Drawing.StringAlignment.Center
                        };
                        e.Graphics.DrawString("⭐", font, textBrush, e.Bounds, format);
                    }
                }
                catch
                {
                    // If preview fails, just draw a colored indicator
                    using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }
            }
        }
    }
}

