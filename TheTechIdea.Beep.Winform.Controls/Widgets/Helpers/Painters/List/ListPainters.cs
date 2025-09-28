using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// ActivityFeed - Timeline-style activities
    /// </summary>
    internal sealed class ActivityFeedPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Header with title
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                24
            );
            
            // Content area for activities
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 8,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                var format = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect, format);
            }
            
            // Draw activity items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                int maxItems = ctx.CustomData.ContainsKey("MaxVisibleItems") ? (int)ctx.CustomData["MaxVisibleItems"] : 10;
                
                DrawActivityItems(g, ctx.ContentRect, items.Take(maxItems).ToList());
            }
        }

        private void DrawActivityItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(40, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var timeFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
            using var timeBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Timeline dot
                var dotRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(Theme?.AccentColor ?? Color.Blue);
                g.FillEllipse(dotBrush, dotRect);
                
                // Timeline line (except for last item)
                if (i < items.Count - 1)
                {
                    using var linePen = new Pen(Color.FromArgb(50, Color.Gray), 1);
                    g.DrawLine(linePen, dotRect.X + 4, dotRect.Bottom, dotRect.X + 4, y + itemHeight);
                }
                
                // Content
                var contentRect = new Rectangle(rect.X + 24, y + 4, rect.Width - 32, itemHeight - 8);
                var nameRect = new Rectangle(contentRect.X, contentRect.Y, contentRect.Width * 2 / 3, contentRect.Height / 2);
                var timeRect = new Rectangle(contentRect.Right - contentRect.Width / 3, contentRect.Y, contentRect.Width / 3, contentRect.Height / 2);
                var descRect = new Rectangle(contentRect.X, contentRect.Y + contentRect.Height / 2, contentRect.Width, contentRect.Height / 2);
                
                // Draw name and time
                if (item.ContainsKey("Name"))
                {
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect);
                }
                
                if (item.ContainsKey("Time"))
                {
                    var timeFormat = new StringFormat { Alignment = StringAlignment.Far };
                    g.DrawString(item["Time"].ToString(), timeFont, timeBrush, timeRect, timeFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    g.DrawString(item["Value"].ToString(), timeFont, timeBrush, descRect);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw scroll indicators or load more button
        }
    }

    /// <summary>
    /// DataTable - Structured data table
    /// </summary>
    internal sealed class DataTablePainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            // Header row
            ctx.HeaderRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                28
            );
            
            // Data rows
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.HeaderRect.Bottom + 2,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3
            );
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw header
            if (ctx.ShowHeader && ctx.Labels?.Any() == true)
            {
                DrawTableHeader(g, ctx.HeaderRect, ctx.Labels);
            }
            
            // Draw data rows
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                int maxItems = ctx.CustomData.ContainsKey("MaxVisibleItems") ? (int)ctx.CustomData["MaxVisibleItems"] : 10;
                
                DrawTableRows(g, ctx.ContentRect, items.Take(maxItems).ToList(), ctx.Labels);
            }
        }

        private void DrawTableHeader(Graphics g, Rectangle rect, List<string> columns)
        {
            // Header background
            using var headerBrush = new SolidBrush(Color.FromArgb(20, Color.Gray));
            g.FillRectangle(headerBrush, rect);
            
            // Column headers
            using var headerFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var headerTextBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            
            int colWidth = rect.Width / Math.Max(columns.Count, 1);
            for (int i = 0; i < columns.Count; i++)
            {
                var colRect = new Rectangle(rect.X + i * colWidth, rect.Y, colWidth - 1, rect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(columns[i], headerFont, headerTextBrush, colRect, format);
                
                // Column separator
                if (i < columns.Count - 1)
                {
                    using var separatorPen = new Pen(Color.FromArgb(50, Color.Gray), 1);
                    g.DrawLine(separatorPen, colRect.Right, rect.Top, colRect.Right, rect.Bottom);
                }
            }
        }

        private void DrawTableRows(Graphics g, Rectangle rect, List<Dictionary<string, object>> items, List<string> columns)
        {
            if (!items.Any() || !columns.Any()) return;
            
            int rowHeight = Math.Min(24, rect.Height / Math.Max(items.Count, 1));
            int colWidth = rect.Width / columns.Count;
            
            using var cellFont = new Font(Owner.Font.FontFamily, 8f);
            using var cellBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
            
            for (int row = 0; row < items.Count; row++)
            {
                var item = items[row];
                int y = rect.Y + row * rowHeight;
                
                // Alternate row background
                if (row % 2 == 1)
                {
                    using var altRowBrush = new SolidBrush(Color.FromArgb(10, Color.Gray));
                    g.FillRectangle(altRowBrush, rect.X, y, rect.Width, rowHeight);
                }
                
                // Cell data
                for (int col = 0; col < columns.Count; col++)
                {
                    var cellRect = new Rectangle(rect.X + col * colWidth + 4, y, colWidth - 8, rowHeight);
                    string cellValue = item.ContainsKey(columns[col]) ? item[columns[col]]?.ToString() ?? "" : "";
                    
                    var format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    g.DrawString(cellValue, cellFont, cellBrush, cellRect, format);
                }
                
                // Row separator
                using var rowPen = new Pen(Color.FromArgb(30, Color.Gray), 1);
                g.DrawLine(rowPen, rect.X, y + rowHeight - 1, rect.Right, y + rowHeight - 1);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw selection highlights or sort indicators
        }
    }

    /// <summary>
    /// RankingList - Ordered ranking list
    /// </summary>
    internal sealed class RankingListPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw ranking items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawRankingItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawRankingItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(32, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var rankFont = new Font(Owner.Font.FontFamily, 10f, FontStyle.Bold);
            using var valueFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Rank badge
                var rankRect = new Rectangle(rect.X, y + 4, 24, itemHeight - 8);
                Color rankColor = i < 3 ? GetRankColor(i) : Color.FromArgb(100, Color.Gray);
                
                using var rankBrush = new SolidBrush(rankColor);
                using var rankPath = CreateRoundedPath(rankRect, 4);
                g.FillPath(rankBrush, rankPath);
                
                using var rankTextBrush = new SolidBrush(Color.White);
                var rankFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString((i + 1).ToString(), rankFont, rankTextBrush, rankRect, rankFormat);
                
                // Item content
                var contentRect = new Rectangle(rect.X + 32, y, rect.Width - 100, itemHeight);
                var valueRect = new Rectangle(rect.Right - 60, y, 60, itemHeight);
                
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, contentRect, nameFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    using var valueBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    var valueFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Value"].ToString(), valueFont, valueBrush, valueRect, valueFormat);
                }
            }
        }

        private Color GetRankColor(int rank)
        {
            return rank switch
            {
                0 => Color.FromArgb(255, 193, 7),  // Gold
                1 => Color.FromArgb(158, 158, 158), // Silver
                2 => Color.FromArgb(205, 127, 50),  // Bronze
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw medals or special indicators for top ranks
        }
    }

    /// <summary>
    /// StatusList - Items with status indicators
    /// </summary>
    internal sealed class StatusListPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw status items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawStatusItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawStatusItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var statusFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Status indicator dot
                string status = item.ContainsKey("Status") ? item["Status"].ToString() : "Unknown";
                Color statusColor = GetStatusColor(status);
                
                var dotRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 4, 8, 8);
                using var dotBrush = new SolidBrush(statusColor);
                g.FillEllipse(dotBrush, dotRect);
                
                // Item name
                var nameRect = new Rectangle(rect.X + 24, y, rect.Width - 120, itemHeight);
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }
                
                // Status text
                var statusRect = new Rectangle(rect.Right - 90, y, 90, itemHeight);
                using var statusBrush = new SolidBrush(statusColor);
                var statusFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(status, statusFont, statusBrush, statusRect, statusFormat);
            }
        }

        private Color GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "active" or "online" or "running" => Color.Green,
                "inactive" or "offline" or "stopped" => Color.Red,
                "busy" or "warning" or "pending" => Color.Orange,
                "idle" or "paused" => Color.Gray,
                _ => Color.Gray
            };
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw status legend or summary counts
        }
    }

    /// <summary>
    /// ProfileList - User/profile listings
    /// </summary>
    internal sealed class ProfileListPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw profile items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawProfileItems(g, ctx.ContentRect, items, ctx.AccentColor);
            }
        }

        private void DrawProfileItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items, Color accentColor)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(48, rect.Height / Math.Max(items.Count, 1));
            using var nameFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Bold);
            using var roleFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Avatar placeholder
                var avatarRect = new Rectangle(rect.X + 8, y + 8, itemHeight - 16, itemHeight - 16);
                using var avatarBrush = new SolidBrush(Color.FromArgb(30, accentColor));
                g.FillEllipse(avatarBrush, avatarRect);
                
                // Avatar border
                using var avatarPen = new Pen(Color.FromArgb(100, accentColor), 1);
                g.DrawEllipse(avatarPen, avatarRect);
                
                // Profile info
                var nameRect = new Rectangle(rect.X + itemHeight + 8, y + 8, rect.Width - itemHeight - 16, (itemHeight - 16) / 2);
                var roleRect = new Rectangle(rect.X + itemHeight + 8, nameRect.Bottom, rect.Width - itemHeight - 16, (itemHeight - 16) / 2);
                
                if (item.ContainsKey("Name"))
                {
                    using var nameBrush = new SolidBrush(Color.FromArgb(180, Color.Black));
                    var nameFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), nameFont, nameBrush, nameRect, nameFormat);
                }
                
                if (item.ContainsKey("Value"))
                {
                    using var roleBrush = new SolidBrush(Color.FromArgb(120, Color.Gray));
                    var roleFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Value"].ToString(), roleFont, roleBrush, roleRect, roleFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw online status indicators or action buttons
        }
    }

    /// <summary>
    /// TaskList - Checklist/todo style  
    /// </summary>
    internal sealed class TaskListPainter : WidgetPainterBase
    {
        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -8, -8);
            
            ctx.HeaderRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, 24);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + 8, ctx.DrawingRect.Width - pad * 2, ctx.DrawingRect.Height - ctx.HeaderRect.Height - pad * 3);
            
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            DrawSoftShadow(g, ctx.DrawingRect, 12, layers: 4, offset: 2);
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            using var bgPath = CreateRoundedPath(ctx.DrawingRect, ctx.CornerRadius);
            g.FillPath(bgBrush, bgPath);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            // Draw title
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title))
            {
                using var titleFont = new Font(Owner.Font.FontFamily, 11f, FontStyle.Bold);
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.DrawString(ctx.Title, titleFont, titleBrush, ctx.HeaderRect);
            }
            
            // Draw task items
            if (ctx.CustomData.ContainsKey("Items"))
            {
                var items = (List<Dictionary<string, object>>)ctx.CustomData["Items"];
                DrawTaskItems(g, ctx.ContentRect, items);
            }
        }

        private void DrawTaskItems(Graphics g, Rectangle rect, List<Dictionary<string, object>> items)
        {
            if (!items.Any()) return;
            
            int itemHeight = Math.Min(28, rect.Height / Math.Max(items.Count, 1));
            using var taskFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = rect.Y + i * itemHeight;
                
                // Checkbox
                var checkboxRect = new Rectangle(rect.X + 8, y + itemHeight / 2 - 6, 12, 12);
                bool isCompleted = item.ContainsKey("Status") && item["Status"].ToString().ToLower() == "completed";
                
                using var checkboxPen = new Pen(Color.FromArgb(150, Color.Gray), 1);
                g.DrawRectangle(checkboxPen, checkboxRect);
                
                if (isCompleted)
                {
                    using var checkBrush = new SolidBrush(Color.Green);
                    g.FillRectangle(checkBrush, Rectangle.Inflate(checkboxRect, -2, -2));
                    
                    // Checkmark
                    using var checkPen = new Pen(Color.White, 2);
                    g.DrawLines(checkPen, new Point[]
                    {
                        new Point(checkboxRect.X + 3, checkboxRect.Y + 6),
                        new Point(checkboxRect.X + 6, checkboxRect.Y + 9),
                        new Point(checkboxRect.X + 10, checkboxRect.Y + 3)
                    });
                }
                
                // Task text
                var taskRect = new Rectangle(rect.X + 28, y, rect.Width - 36, itemHeight);
                if (item.ContainsKey("Name"))
                {
                    Color textColor = isCompleted ? Color.FromArgb(120, Color.Gray) : Color.FromArgb(180, Color.Black);
                    FontStyle fontStyle = isCompleted ? FontStyle.Strikeout : FontStyle.Regular;
                    
                    using var taskTextFont = new Font(Owner.Font.FontFamily, 9f, fontStyle);
                    using var taskBrush = new SolidBrush(textColor);
                    var taskFormat = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(item["Name"].ToString(), taskTextFont, taskBrush, taskRect, taskFormat);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Optional: Draw progress indicator or completion summary
        }
    }
}