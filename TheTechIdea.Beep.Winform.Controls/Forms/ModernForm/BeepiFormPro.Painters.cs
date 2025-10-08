using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public interface IFormPainter
    {
        void PaintBackground(Graphics g, BeepiFormPro owner);
        void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);
        void PaintBorders(Graphics g, BeepiFormPro owner);
    }

    internal sealed class MinimalFormPainter : IFormPainter
    {
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, owner.ClientRectangle);
        }
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var style = BeepStyling.GetControlStyle();
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            using var line = new Pen(primary, 2f);
            g.DrawLine(line, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
            var textRect = captionRect;
            textRect.Inflate(-8, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var border = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBorder(style);
            using var pen = new Pen(border, 1f);
            var r = owner.ClientRectangle; r.Width -= 1; r.Height -= 1;
            g.DrawRectangle(pen, r);
        }
    }

    internal sealed class MaterialFormPainter : IFormPainter
    {
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepControlStyle.Material3;
            var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
            using var brush = new SolidBrush(bg);
            g.FillRectangle(brush, owner.ClientRectangle);
        }
        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var style = BeepControlStyle.Material3;
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            using var brush = new SolidBrush(primary);
            var bar = new Rectangle(captionRect.Left, captionRect.Top, 4, captionRect.Height);
            g.FillRectangle(brush, bar);
            var textRect = captionRect; textRect.Inflate(-12, 0);
            TextRenderer.DrawText(g, owner.Text ?? string.Empty, owner.Font, textRect, fg,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }
        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var style = BeepControlStyle.Material3;
            var border = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBorder(style);
            using var pen = new Pen(border, 1f);
            var r = owner.ClientRectangle; r.Width -= 1; r.Height -= 1;
            g.DrawRectangle(pen, r);
        }
    }

    internal sealed class GlassFormPainter : IFormPainter
    {
        public void PaintBackground(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var bg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetBackground(style);
            var surface = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetSurface(style);
            
            // Semi-transparent glass effect background
            using var brush = new SolidBrush(Color.FromArgb(240, bg.R, bg.G, bg.B));
            g.FillRectangle(brush, owner.ClientRectangle);
            
            // Subtle gradient overlay for glass effect
            using var gradBrush = new LinearGradientBrush(
                owner.ClientRectangle, 
                Color.FromArgb(30, 255, 255, 255), 
                Color.FromArgb(5, 255, 255, 255), 
                90f);
            g.FillRectangle(gradBrush, owner.ClientRectangle);
        }

        public void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect)
        {
            var style = BeepStyling.GetControlStyle();
            var fg = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetForeground(style);
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            var surface = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetSurface(style);
            
            // Glass caption bar with gradient
            using var gradBrush = new LinearGradientBrush(
                captionRect,
                Color.FromArgb(160, surface.R, surface.G, surface.B),
                Color.FromArgb(120, surface.R, surface.G, surface.B),
                90f);
            g.FillRectangle(gradBrush, captionRect);
            
            // Top highlight line for glass effect
            using var highlight = new Pen(Color.FromArgb(100, 255, 255, 255), 1f);
            g.DrawLine(highlight, captionRect.Left, captionRect.Top, captionRect.Right, captionRect.Top);
            
            // Bottom border line
            using var border = new Pen(primary, 1f);
            g.DrawLine(border, captionRect.Left, captionRect.Bottom - 1, captionRect.Right, captionRect.Bottom - 1);
            
            // Title text is now handled by title region, not painter
        }

        public void PaintBorders(Graphics g, BeepiFormPro owner)
        {
            var style = BeepStyling.GetControlStyle();
            var primary = TheTechIdea.Beep.Winform.Controls.Styling.Colors.StyleColors.GetPrimary(style);
            
            // Outer border with primary color
            using var pen = new Pen(primary, 2f);
            var r = owner.ClientRectangle; 
            r.Width -= 2; 
            r.Height -= 2;
            r.Inflate(1, 1);
            g.DrawRectangle(pen, r);
            
            // Inner glass highlight
            using var innerPen = new Pen(Color.FromArgb(80, 255, 255, 255), 1f);
            var innerRect = owner.ClientRectangle;
            innerRect.Inflate(-2, -2);
            g.DrawRectangle(innerPen, innerRect);
        }
    }
}