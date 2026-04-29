using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;

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
            return Math.Max(_owner.TextFont.Height + Scale(32), Scale(60));
        }

        protected override void DrawItem(Graphics g, Rectangle itemRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            if (g == null || itemRect.IsEmpty || item == null) return;

            DrawItemBackgroundEx(g, itemRect, item, isHovered, isSelected);

            int itemIndex = _owner.ListItems.IndexOf(item);
            bool isFirst = itemIndex == 0;
            bool isLast = itemIndex == _owner.ListItems.Count - 1;

            // Draw timeline connector
            DrawTimelineConnector(g, itemRect, isFirst, isLast, isSelected);

            // Draw timeline node
            var nodeRect = new Rectangle(
                itemRect.X + Scale(12),
                itemRect.Y + (itemRect.Height - Scale(_nodeSize)) / 2,
                Scale(_nodeSize), Scale(_nodeSize));
            DrawTimelineNode(g, nodeRect, item, isHovered, isSelected);

            // Draw content card
            var contentRect = new Rectangle(
                itemRect.X + Scale(_contentOffset),
                itemRect.Y + Scale(4),
                itemRect.Width - Scale(_contentOffset) - Scale(8),
                itemRect.Height - Scale(8));
            DrawContentCard(g, contentRect, item, isHovered, isSelected);
        }

        private void DrawTimelineConnector(Graphics g, Rectangle itemRect, bool isFirst, bool isLast, bool isSelected)
        {
            int centerX = itemRect.X + Scale(12) + Scale(_nodeSize) / 2;
            Color lineColor = isSelected 
                ? (_theme?.PrimaryColor ?? Color.FromArgb(0, 120, 215))
                : (_theme?.BorderColor ?? Color.FromArgb(200, 200, 200));

            using (var pen = new Pen(lineColor, _lineWidth))
            {
                // Draw line above node (unless first item)
                if (!isFirst)
                {
                    g.DrawLine(pen, centerX, itemRect.Top, centerX, itemRect.Y + (itemRect.Height - Scale(_nodeSize)) / 2);
                }

                // Draw line below node (unless last item)
                if (!isLast)
                {
                    int nodeBottom = itemRect.Y + (itemRect.Height + Scale(_nodeSize)) / 2;
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
                var iconRect = Rectangle.Inflate(nodeRect, -Scale(3), -Scale(3));
                DrawItemImage(g, iconRect, item.ImagePath);
            }
            else if (isSelected)
            {
                // Draw checkmark for selected items
                using (var pen = new Pen(Color.White, 2f))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    int cp = Scale(4);
                    Point[] checkPoints = new Point[]
                    {
                        new Point(nodeRect.Left + cp, nodeRect.Top + nodeRect.Height / 2),
                        new Point(nodeRect.Left + nodeRect.Width / 2 - Scale(1), nodeRect.Bottom - cp),
                        new Point(nodeRect.Right - cp, nodeRect.Top + cp)
                    };
                    g.DrawLines(pen, checkPoints);
                }
            }
        }

        private void DrawContentCard(Graphics g, Rectangle contentRect, SimpleItem item, bool isHovered, bool isSelected)
        {
            using (var path = GraphicsExtensions.CreateRoundedRectanglePath(contentRect, new CornerRadius(Scale(_cornerRadius))))
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
            int textX = contentRect.X + Scale(12);
            int textWidth = contentRect.Width - Scale(24);

            // Timestamp (SubText2 or top-right)
            if (!string.IsNullOrEmpty(item.SubText2))
            {
                Color timeColor = isSelected 
                    ? Color.FromArgb(200, 255, 255, 255) 
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(140, 140, 140));
                
                using (var smallFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 2, FontStyle.Regular))
                {
                    var timeRect = new Rectangle(contentRect.Right - Scale(80), contentRect.Y + Scale(6), Scale(70), Scale(16));
                    using (var sf = new StringFormat { Alignment = StringAlignment.Far })
                    {
                        g.DrawString(item.SubText2, smallFont, new SolidBrush(timeColor), timeRect, sf);
                    }
                }
                textWidth -= Scale(80);
            }

            // Title
            var titleRect = new Rectangle(textX, contentRect.Y + Scale(8), textWidth, Scale(20));
            Color titleColor = isSelected 
                ? Color.White 
                : (_theme?.ListItemForeColor ?? Color.FromArgb(30, 30, 30));
            
            using (var boldFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size, FontStyle.Bold))
            {
                DrawItemText(g, titleRect, item.Text ?? item.DisplayField, titleColor, boldFont);
            }

            // Description (SubText)
            if (!string.IsNullOrEmpty(item.SubText))
            {
                var descRect = new Rectangle(textX, contentRect.Y + Scale(28), contentRect.Width - Scale(24), Scale(20));
                Color descColor = isSelected 
                    ? Color.FromArgb(220, 255, 255, 255) 
                    : (_theme?.DisabledForeColor ?? Color.FromArgb(100, 100, 100));
                
                using (var smallFont = BeepFontManager.GetFont(_owner.TextFont.Name, _owner.TextFont.Size - 1, FontStyle.Regular))
                {
                    DrawItemText(g, descRect, item.SubText, descColor, smallFont);
                }
            }
        }

        private void DrawCardArrow(Graphics g, Rectangle contentRect, bool isSelected)
        {
            int arrowSize = Scale(8);
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

