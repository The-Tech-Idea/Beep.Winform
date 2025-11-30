using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Painters
{
    /// <summary>
    /// Timeline style painter - items displayed as timeline entries with connectors
    /// Ideal for activity logs, history, and chronological data
    /// </summary>
    internal class TimelineListBoxPainter : BaseListBoxPainter
    {
        private readonly int _nodeSize = 16;
        private readonly int _lineWidth = 2;
        private readonly int _contentOffset = 40;
        private readonly int _cornerRadius = 6;

        public override int GetPreferredItemHeight()
        {
            return Math.Max(_owner.Font.Height + 32, 60);
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            int itemIndex = _owner.ListItems.IndexOf(item);
            bool isFirst = itemIndex == 0;
            bool isLast = itemIndex == _owner.ListItems.Count - 1;

            // Draw timeline connector
            DrawTimelineConnector(g, itemRect, isFirst, isLast, isSelected);

            // Draw timeline node
            var nodeRect = new Rectangle(
                itemRect.X + 12,
                itemRect.Y + (itemRect.Height - _nodeSize) / 2,
                _nodeSize, _nodeSize);
            DrawTimelineNode(g, nodeRect, item, isHovered, isSelected);

            // Draw content card
            var contentRect = new Rectangle(
                itemRect.X + _contentOffset,
                itemRect.Y + 4,
                itemRect.Width - _contentOffset - 8,
                itemRect.Height - 8);
            DrawContentCard(g, contentRect, item, isHovered, isSelected);
        }

        private void DrawTimelineConnector(Graphics g, Rectangle itemRect, bool isFirst, bool isLast, bool isSelected)
        {
            int centerX = itemRect.X + 12 + _nodeSize / 2;
            Color lineColor = isSelected 
                ? (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215))
                : (_theme?.BorderColor ?? Color.FromArgb(200, 200, 200));

            using (var pen = new Pen(lineColor, _lineWidth))
            {
                // Draw line above node (unless first item)
                if (!isFirst)
                {
                    g.DrawLine(pen, centerX, itemRect.Top, centerX, itemRect.Y + (itemRect.Height - _nodeSize) / 2);
                }

                // Draw line below node (unless last item)
                if (!isLast)
                {
                    int nodeBottom = itemRect.Y + (itemRect.Height + _nodeSize) / 2;
                    g.DrawLine(pen, centerX, nodeBottom, centerX, itemRect.Bottom);
                }
            }
        }

        private void DrawTimelineNode(Graphics g, Rectangle nodeRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            Color nodeColor;
            Color borderColor;

            if (isSelected)
            {
                nodeColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                borderColor = Color.White;
            }
            else if (isHovered)
            {
                nodeColor = Color.FromArgb(60, _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215));
                borderColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
            }
            else
            {
                // Use badge color if available, otherwise default
                nodeColor = item.BadgeBackColor != Color.Empty 
                    ? item.BadgeBackColor 
                    : (_theme?.BackgroundColor ?? Color.White);
                borderColor = _theme?.BorderColor ?? Color.FromArgb(180, 180, 180);
            }

            // Draw node circle
            using (var brush = new SolidBrush(nodeColor))
            {
                g.FillEllipse(brush, nodeRect);
            }

            using (var pen = new Pen(borderColor, 2f))
            {
                g.DrawEllipse(pen, nodeRect);
            }

            // Draw icon inside node if available
            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                var iconRect = Rectangle.Inflate(nodeRect, -3, -3);
                DrawItemImage(g, iconRect, item.ImagePath);
            }
            else if (isSelected)
            {
                // Draw checkmark for selected items
                using (var pen = new Pen(Color.White, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    Point[] checkPoints = new Point[]
                    {
                        new Point(nodeRect.Left + 4, nodeRect.Top + nodeRect.Height / 2),
                        new Point(nodeRect.Left + nodeRect.Width / 2 - 1, nodeRect.Bottom - 4),
                        new Point(nodeRect.Right - 4, nodeRect.Top + 4)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }

        private void DrawContentCard(Graphics g, Rectangle contentRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(contentRect, _cornerRadius))
            {
                // Background
                Color bgColor;
                if (isSelected)
                {
                    bgColor = _theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215);
                }
                else if (isHovered)
                {
                    bgColor = _theme?.ListItemHoverBackColor ?? Color.FromArgb(248, 248, 248);
                }
                else
                {
                    bgColor = _theme?.BackgroundColor ?? Color.White;
                }

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Border
                if (!isSelected)
                {
                    using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Arrow pointing to timeline
                DrawCardArrow(g, contentRect, isSelected);
            }

            // Draw content
            int textX = contentRect.X + 12;
            int textWidth = contentRect.Width - 24;

            // Timestamp (SubText2 or top-right)
            if (!string.IsNullOrEmpty(item.SubText2))
            {
                Color timeColor = isSelected 
                    ? Color.FromArgb(200, 255, 255, 255) 
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(140, 140, 140));
                
                using (var smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 2, FontStyle.Regular))
                {
                    var timeRect = new Rectangle(contentRect.Right - 80, contentRect.Y + 6, 70, 16);
                    using (var sf = new StringFormat { Alignment = StringAlignment.Far })
                    {
                        g.DrawString(item.SubText2, smallFont, new SolidBrush(timeColor), timeRect, sf);
                    }
                }
                textWidth -= 80;
            }

            // Title
            var titleRect = new Rectangle(textX, contentRect.Y + 8, textWidth, 20);
            Color titleColor = isSelected 
                ? Color.White 
                : (_theme?.ListItemForeColor ?? Color.FromArgb(30, 30, 30));
            
            using (var boldFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size, FontStyle.Bold))
            {
                DrawItemText(g, titleRect, item.Text ?? item.DisplayField, titleColor, boldFont);
            }

            // Description (SubText)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var descRect = new Rectangle(textX, contentRect.Y + 28, contentRect.Width - 24, 20);
                Color descColor = isSelected 
                    ? Color.FromArgb(220, 255, 255, 255) 
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(100, 100, 100));
                
                using (var smallFont = new Font(_owner.TextFont.FontFamily, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, descRect, item.SubText, descColor, smallFont);
                }
            }
        }

        private void DrawCardArrow(Graphics g, Rectangle contentRect, bool isSelected)
        {
            int arrowSize = 8;
            int arrowY = contentRect.Y + contentRect.Height / 2;
            
            Point[] arrowPoints = new Point[]
            {
                new Point(contentRect.X, arrowY),
                new Point(contentRect.X - arrowSize, arrowY - arrowSize / 2),
                new Point(contentRect.X - arrowSize, arrowY + arrowSize / 2)
            };

            Color arrowColor = isSelected 
                ? (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215))
                : (_theme?.BackgroundColor ?? Color.White);

            using (var brush = new SolidBrush(arrowColor))
            {
                g.FillPolygon(brush, arrowPoints);
            }

            // Arrow border (only if not selected)
            if (!isSelected)
            {
                using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(220, 220, 220), 1f))
                {
                    g.DrawLine(pen, arrowPoints[1], arrowPoints[0]);
                    g.DrawLine(pen, arrowPoints[0], arrowPoints[2]);
                }
            }
        }

        protected override void DrawItemBackground(Graphics g, Rectangle itemRect, bool isHovered, bool isSelected)
        {
            // Background is handled in DrawContentCard
        }
    }
}

