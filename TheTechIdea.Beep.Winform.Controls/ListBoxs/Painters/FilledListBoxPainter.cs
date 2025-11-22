using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Filled list box with elevation, shadow, and distinct filled background style
    /// </summary>
    internal class FilledListBoxPainter : BaseListBoxPainter
    {
        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            var rect = itemRect;
            rect.Inflate(-4, -2);
            
            DrawItemBackgroundEx(g, rect, item, isHovered, isSelected);

            // Use layout rects for content
            var info = _layout.GetCachedLayout().FirstOrDefault(i => i.Item == item);
            var iconRect = info?.IconRect ?? Rectangle.Empty;
            var textRect = info?.TextRect ?? rect;

            if (_owner.ShowImage && !string.IsNullOrEmpty(item.ImagePath) && !iconRect.IsEmpty)
            {
                DrawItemImage(g, iconRect, item.ImagePath);
            }

            Color textColor = isSelected ? Color.White : _helper.GetTextColor();
            DrawItemText(g, textRect, item.Text, textColor, _owner.TextFont);
        }
        
        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty) return;

            // Create rounded path
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(itemRect, 6))
            {
                if (isSelected)
                {
                    var selColor = _theme?.PrimaryColor ?? Color.LightBlue;
                    
                    // Draw shadow for elevation
                    var shadowRect = itemRect;
                    shadowRect.Offset(0, 2);
                    using (var shadowBrush = new LinearGradientBrush(shadowRect, 
                        Color.FromArgb(40, Color.Black), 
                        Color.Transparent, 
                        90f))
                    {
                        g.FillRectangle(shadowBrush, shadowRect);
                    }

                    // Filled background with primary color
                    using (var brush = new SolidBrush(selColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Selection border
                    using (var pen = new Pen(Color.FromArgb(200, selColor), 2f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else if (isHovered)
                {
                    // Hover state: lighter fill with shadow
                    var hoverColor = Color.FromArgb(240, 240, 240);
                    
                    using (var shadowBrush = new LinearGradientBrush(itemRect,
                        Color.FromArgb(20, Color.Black),
                        Color.Transparent,
                        90f))
                    {
                        g.FillRectangle(shadowBrush, itemRect);
                    }

                    using (var brush = new SolidBrush(hoverColor))
                    {
                        g.FillPath(brush, path);
                    }

                    // Hover border
                    using (var pen = new Pen(_theme?.AccentColor ?? Color.Gray, 1.5f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    // Normal filled style
                    using (var brush = new SolidBrush(Color.FromArgb(250, 250, 250)))
                    {
                        g.FillPath(brush, path);
                    }

                    using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
        
        public override int GetPreferredItemHeight()
        {
            return 44;
        }
    }
}
