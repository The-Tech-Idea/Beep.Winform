using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Dropdown with smooth, gradual border styling
    /// </summary>
    internal class SmoothBorderPainter : BaseComboBoxPainter
    {
        private const int BorderRadius = 8;
        
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.BorderColor ?? Color.Empty, 200);
            var basePen = PaintersFactory.GetPen(borderColor, 1.5f);
            // need to modify LineJoin, clone the cached pen
            var pen = (System.Drawing.Pen)basePen.Clone();
            try
            {
                pen.LineJoin = LineJoin.Round;
                using (var path = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.DrawPath(pen, path);
                }
            }
            finally
            {
                pen.Dispose();
            }
        }
        
        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter - 1, rect.Bottom - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
    
    /// <summary>
    /// Dark themed dropdown with prominent borders
    /// </summary>
    internal class DarkBorderPainter : BaseComboBoxPainter
    {
      
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = Color.FromArgb(80, 80, 80);
            var pen = PaintersFactory.GetPen(borderColor, 2f);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
        }
        
        protected override void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            // Use muted color for placeholder text
            Color textColor;
            if (_helper.IsShowingPlaceholder())
            {
                textColor = Color.FromArgb(128, 200, 200, 200);
            }
            else
            {
                textColor = Color.White;
            }
            System.Windows.Forms.TextRenderer.DrawText(g, displayText, _owner.TextFont, 
                textAreaRect, textColor, 
                System.Windows.Forms.TextFormatFlags.Left | 
                System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }
    }
    
    /// <summary>
    /// Pill-shaped dropdown with full rounded corners
    /// </summary>
    internal class PillCornersComboBoxPainter : BaseComboBoxPainter
    {
     
        
        protected override void DrawBorder(Graphics g, Rectangle rect)
        {
            Color borderColor = _owner.Focused
                ? _theme?.PrimaryColor ?? Color.Empty
                : (_theme?.BorderColor ?? Color.Empty);
            
            var pen = PaintersFactory.GetPen(borderColor, _owner.Focused ? 2f : 1f);
            using (var path = GetPillPath(rect))
            {
                g.DrawPath(pen, path);
            }
        }
        
        // No separator for pill shape - cleaner look
        protected override bool ShowButtonSeparator => false;
        
        private GraphicsPath GetPillPath(Rectangle rect)
        {
            var path = new GraphicsPath();
            int radius = rect.Height / 2;
            int diameter = radius * 2;
            
            path.AddArc(rect.X, rect.Y, diameter, diameter, 90, 180);
            path.AddLine(rect.X + radius, rect.Y, rect.Right - radius - 1, rect.Y);
            path.AddArc(rect.Right - diameter - 1, rect.Y, diameter, diameter, 270, 180);
            path.AddLine(rect.Right - radius - 1, rect.Bottom - 1, rect.X + radius, rect.Bottom - 1);
            path.CloseFigure();
            
            return path;
        }
        
        public override System.Windows.Forms.Padding GetPreferredPadding()
        {
            return new System.Windows.Forms.Padding(20, 8, 16, 8); // Extra padding for pill shape
        }
    }
}
