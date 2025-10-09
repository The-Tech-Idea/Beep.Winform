using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Glass-effect form painter with transparency and gradient overlays.
    /// Features semi-transparent background with subtle gradient and glass caption bar.
    /// </summary>
    internal sealed class GlassFormPainter : IFormPainter
    {
        /// <summary>
        /// Paints a semi-transparent glass effect background with gradient overlay.
        /// Uses 240 alpha for background and adds white gradient for glass effect.
        /// </summary>
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
            var surface = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetSurface(style);
            
            // Semi-transparent glass effect background (240 alpha)
            using var brush = new SolidBrush(Color.FromArgb(240, bg.R, bg.G, bg.B));
            g.FillRectangle(brush, owner.ClientRectangle);
            
            // Subtle white gradient overlay for glass effect (30 alpha to 5 alpha)
            using var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle, 
                Color.FromArgb(30, 255, 255, 255), 
                Color.FromArgb(5, 255, 255, 255), 
                90f);
            g.FillRectangle(gradBrush, owner.ClientRectangle);
        }

        /// <summary>
        /// Paints the caption bar with glass effect gradient and highlight.
        /// Features gradient from 160 to 120 alpha, top highlight line, and bottom border.
        /// </summary>
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var style = BeepStyling.GetControlStyle();
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            var surface = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetSurface(style);
            
            //// Glass caption bar with vertical gradient (160 to 120 alpha)
            //using var gradBrush = new LinearGradientBrush(
            //    captionRect,
            //    Color.FromArgb(160, surface.R, surface.G, surface.B),
            //    Color.FromArgb(120, surface.R, surface.G, surface.B),
            //    90f);
          //  g.FillRectangle(gradBrush, captionRect);
            
            // Top highlight line for glass effect (100 alpha white)
            using var highlight = new Pen(Color.FromArgb(100, 255, 255, 255), 1f);
            g.DrawLine(highlight, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);
            
            // Bottom border line with primary color
            using var border = new Pen(primary, 1f);
            g.DrawLine(border, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
            
            // Note: Title text is now handled by title region, not painter
        }

        /// <summary>
        /// Paints double borders with glass effect - outer primary border and inner highlight.
        /// Features 2px primary border and 1px semi-transparent white inner border.
        /// </summary>
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            
            // Outer border with primary color (2px)
            using var pen = new Pen(primary, 2f);
            var r = owner.ClientRectangle; 
            r.Width -= 2; 
            r.Height -= 2;
            r.Inflate(1, 1);
            g.DrawRectangle(pen, r);
            
            // Inner glass highlight (80 alpha white, 1px)
            using var innerPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1f);
            var innerRect = owner.ClientRectangle;
            innerRect.Inflate(-2, -2);
            g.DrawRectangle(innerPen, innerRect);
        }
    }
}
