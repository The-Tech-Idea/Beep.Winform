using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Category list with chips for multi-select
    /// Shows selected items as chips at the top (from image 1)
    /// </summary>
    internal class CategoryChipsPainter : OutlinedListBoxPainter
    {
        public override bool SupportsCheckboxes() => true;
        
        protected override int DrawSearchArea(Graphics g, Rectangle drawingRect, int yOffset)
        {
            // Draw chips area for selected items
            var selectedItems = _owner.SelectedItems;
            if (selectedItems != null && selectedItems.Count > 0)
            {
                int chipY = yOffset + 8;
                int chipX = drawingRect.X + 8;
                int maxWidth = drawingRect.Width - 16;
                
                foreach (var item in selectedItems.Take(5)) // Show max 5 chips
                {
                    var chipSize = DrawChip(g, item.Text, chipX, chipY);
                    chipX += chipSize.Width + 8;
                    
                    if (chipX > maxWidth)
                    {
                        chipX = drawingRect.X + 8;
                        chipY += chipSize.Height + 8;
                    }
                }
                
                yOffset = chipY + 32;
                
                // Draw separator line
                using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
                {
                    g.DrawLine(pen, drawingRect.Left, yOffset, drawingRect.Right, yOffset);
                }
                
                yOffset += 8;
            }
            
            // Draw search area if enabled
            if (_owner.ShowSearch)
            {
                return base.DrawSearchArea(g, drawingRect, yOffset);
            }
            
            return yOffset;
        }
        
        private Size DrawChip(Graphics g, string text, int x, int y)
        {
            var textSize = System.Windows.Forms.TextRenderer.MeasureText(text, _owner.TextFont);
            int chipWidth = textSize.Width + 32;
            int chipHeight = 24;

            Rectangle chipRect = new Rectangle(x, y, chipWidth, chipHeight);

            // Improved shadow with gradient
            var shadowRect = Rectangle.Inflate(chipRect, 2, 2);
            using (var shadowBrush = new LinearGradientBrush(shadowRect, Color.FromArgb(50, Color.Black), Color.Transparent, 90f))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }

            // Enhanced chip background with gradient
            Color chipBg = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.PrimaryColor ?? Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.AccentColor ?? Color.FromArgb(33, 150, 243);
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(chipRect, 12))
            using (var brush = new LinearGradientBrush(chipRect, Color.FromArgb(230, chipBg.R, chipBg.G, chipBg.B), Color.FromArgb(200, chipBg.R, chipBg.G, chipBg.B), LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }

            // Draw text with better contrast
            var textRect = new Rectangle(x + 12, y, chipWidth - 24, chipHeight);
            Color chipText = _theme?.OnPrimaryColor ?? Color.White;
            System.Windows.Forms.TextRenderer.DrawText(g, text, _owner.TextFont, textRect, chipText,
                System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.VerticalCenter);

            // Enhanced hover effect for X button
            Rectangle xRect = new Rectangle(chipRect.Right - 20, y + 6, 12, 12);
            using (var pen = new Pen(Color.White, 1.5f))
            {
                g.DrawLine(pen, xRect.Left, xRect.Top, xRect.Right, xRect.Bottom);
                g.DrawLine(pen, xRect.Right, xRect.Top, xRect.Left, xRect.Bottom);
            }

            if (xRect.Contains(_owner.PointToClient(Control.MousePosition)))
            {
                using (var hoverBrush = new SolidBrush(Color.FromArgb(100, Color.White)))
                {
                    g.FillEllipse(hoverBrush, xRect);
                }
            }

            return new Size(chipWidth, chipHeight);
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
}
