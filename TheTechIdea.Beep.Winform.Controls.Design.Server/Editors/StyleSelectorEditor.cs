using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Design-time editor for BeepControlStyle property
    /// Launches StyleSelectorDialog to browse all 56+ available styles
    /// </summary>
    public class StyleSelectorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (provider == null) return value;

            // Get the current style
            BeepControlStyle currentStyle = value is BeepControlStyle style ? style : BeepControlStyle.Material3;

            // Launch the style selector dialog
            using (var dialog = new StyleSelectorDialog(currentStyle))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedStyle;
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
            if (e.Value is BeepControlStyle style)
            {
                try
                {
                    // Draw a simple visual indicator of the style
                    var rect = e.Bounds;
                    var color = GetStyleColor(style);

                    // Draw simple rectangle (rounded drawing requires GraphicsExtensions which is in the Controls project)
                    using (var brush = new SolidBrush(color))
                    using (var pen = new Pen(Color.Gray, 1))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(pen, rect);
                    }
                }
                catch
                {
                    // Fallback to simple colored rectangle
                    using (var brush = new SolidBrush(Color.LightBlue))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }
            }
        }

        private Color GetStyleColor(BeepControlStyle style)
        {
            // Return a representative color for each style
            return style switch
            {
                BeepControlStyle.Material3 => Color.FromArgb(103, 80, 164),
                BeepControlStyle.iOS15 => Color.FromArgb(0, 122, 255),
                BeepControlStyle.Fluent2 => Color.FromArgb(0, 120, 212),
                BeepControlStyle.AntDesign => Color.FromArgb(24, 144, 255),
                BeepControlStyle.Minimal => Color.FromArgb(240, 240, 240),
                BeepControlStyle.Brutalist => Color.Black,
                BeepControlStyle.Neumorphism => Color.FromArgb(225, 225, 235),
                BeepControlStyle.Glassmorphism => Color.FromArgb(180, 255, 255, 255),
                BeepControlStyle.Nord => Color.FromArgb(136, 192, 208),
                BeepControlStyle.Dracula => Color.FromArgb(189, 147, 249),
                BeepControlStyle.Tokyo => Color.FromArgb(122, 162, 247),
                BeepControlStyle.Cyberpunk => Color.FromArgb(255, 0, 255),
                BeepControlStyle.Neon => Color.FromArgb(0, 255, 255),
                _ => Color.FromArgb(100, 150, 200)
            };
        }
    }
}

