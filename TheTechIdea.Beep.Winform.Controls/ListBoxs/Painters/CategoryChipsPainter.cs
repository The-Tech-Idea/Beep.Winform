using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
                int gap = Scale(8);
                int chipY = yOffset + gap;
                int chipX = drawingRect.X + gap;
                int maxWidth = drawingRect.Width - Scale(16);
                
                foreach (var item in selectedItems.Take(5)) // Show max 5 chips
                {
                    var chipSize = DrawChip(g, item.Text, chipX, chipY);
                    chipX += chipSize.Width + gap;
                    
                    if (chipX > maxWidth)
                    {
                        chipX = drawingRect.X + gap;
                        chipY += chipSize.Height + gap;
                    }
                }
                
                yOffset = chipY + Scale(32);
                
                // Draw separator line
                using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
                {
                    g.DrawLine(pen, drawingRect.Left, yOffset, drawingRect.Right, yOffset);
                }
                
                yOffset += gap;
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
            SizeF textSizeF = TextUtils.MeasureText(g, text, _owner.TextFont);
            var textSize = new Size((int)textSizeF.Width, (int)textSizeF.Height);
            int chipPadH = Scale(16);
            int chipWidth = textSize.Width + chipPadH * 2;
            int chipHeight = Scale(24);
            int chipRadius = Scale(12);

            Rectangle chipRect = new Rectangle(x, y, chipWidth, chipHeight);

            // Improved shadow with gradient
            var shadowRect = Rectangle.Inflate(chipRect, Scale(2), Scale(2));
            using (var shadowBrush = new LinearGradientBrush(shadowRect, Color.FromArgb(50, Color.Black), Color.Transparent, 90f))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }

            // Enhanced chip background with gradient
            Color chipBg = _theme?.PrimaryColor ?? _theme?.AccentColor ?? Color.Empty;
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(chipRect, chipRadius))
            using (var brush = new LinearGradientBrush(chipRect, Color.FromArgb(230, chipBg.R, chipBg.G, chipBg.B), Color.FromArgb(200, chipBg.R, chipBg.G, chipBg.B), LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }

            // Draw text with better contrast
            int textPad = Scale(12);
            var textRect = new Rectangle(x + textPad, y, chipWidth - textPad * 2, chipHeight);
            Color chipText = _theme?.OnPrimaryColor ?? Color.White;
            System.Windows.Forms.TextRenderer.DrawText(g, text, _owner.TextFont, textRect, chipText,
                System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.VerticalCenter);

            // Enhanced hover effect for X button
            int xSize = Scale(12);
            int xInset = Scale(20);
            int xYOff = Scale(6);
            Rectangle xRect = new Rectangle(chipRect.Right - xInset, y + xYOff, xSize, xSize);
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
    }
}
