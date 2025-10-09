using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Minimal form painter with clean lines and subtle accent borders.
    /// Features a simple underline on the caption bar with primary color.
    /// </summary>
    internal sealed class MinimalFormPainter : IFormPainter
    {
        /// <summary>
        /// Paints a solid background using the default control style background color.
        /// </summary>
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
            
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, owner.ClientRectangle);
        }

        /// <summary>
        /// Paints the caption bar with a simple primary color underline.
        /// Displays the form title text with standard formatting.
        /// </summary>
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var style = BeepStyling.GetControlStyle();
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            
            // Draw primary color underline at bottom of caption
            using var line = new Pen(primary, 2f);
            g.DrawLine(line, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
            
            // Draw title text with 8px padding
            var textRect = captionRect;
            textRect.Inflate(-8, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        /// <summary>
        /// Paints a simple 1px border around the form using the border color.
        /// </summary>
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var border = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBorder(style);
            
            using var pen = new Pen(border, 1f);
            var r = owner.ClientRectangle; 
            r.Width -= 1; 
            r.Height -= 1;
            g.DrawRectangle(pen, r);
        }
    }
}
