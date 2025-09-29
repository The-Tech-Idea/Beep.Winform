using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Tree Navigation - Hierarchical navigation with expandable/collapsible nodes
    /// </summary>
    internal sealed class TreeNavigationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;

        public TreeNavigationPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + 8, ctx.DrawingRect.Y + 8,
                ctx.DrawingRect.Width - 16, ctx.DrawingRect.Height - 16);
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.UseThemeColors = true;

            var treeItems = ctx.CustomData.ContainsKey("TreeItems") ?
                (List<TreeNodeItem>)ctx.CustomData["TreeItems"] : CreateSampleTreeStructure();

            if (!treeItems.Any()) return;

            DrawTreeStructure(g, ctx, treeItems, 0, 0);
        }

        private List<TreeNodeItem> CreateSampleTreeStructure()
        {
            return new List<TreeNodeItem>
            {
                new TreeNodeItem { Text = "Root", Level = 0, IsExpanded = true, HasChildren = true },
                new TreeNodeItem { Text = "Documents", Level = 1, IsExpanded = true, HasChildren = true },
                new TreeNodeItem { Text = "Report.pdf", Level = 2, IsExpanded = false, HasChildren = false },
                new TreeNodeItem { Text = "Images", Level = 1, IsExpanded = false, HasChildren = true },
                new TreeNodeItem { Text = "Settings", Level = 1, IsExpanded = false, HasChildren = false }
            };
        }

        private int DrawTreeStructure(Graphics g, WidgetContext ctx, List<TreeNodeItem> items, int startY, int currentIndex)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int itemHeight = 24;
            int indentSize = 20;
            
            using var treeFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
            using var textBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            using var linePen = new Pen(Color.FromArgb(100, Color.Gray), 1);

            int currentY = startY;

            foreach (var item in items)
            {
                if (currentY + itemHeight > ctx.ContentRect.Bottom) break;

                int x = ctx.ContentRect.X + item.Level * indentSize;
                var itemRect = new Rectangle(x, ctx.ContentRect.Y + currentY, 
                    ctx.ContentRect.Width - item.Level * indentSize, itemHeight);

                // Tree line connections
                if (item.Level > 0)
                {
                    int lineX = ctx.ContentRect.X + (item.Level - 1) * indentSize + 8;
                    g.DrawLine(linePen, lineX, itemRect.Y, lineX + 12, itemRect.Y + itemHeight / 2);
                    g.DrawLine(linePen, lineX, itemRect.Y + itemHeight / 2, lineX + 12, itemRect.Y + itemHeight / 2);
                }

                // Expand/collapse icon
                if (item.HasChildren)
                {
                    var expandRect = new Rectangle(x, itemRect.Y + (itemHeight - 12) / 2, 12, 12);
                    string expandIcon = item.IsExpanded ? "chevron-down" : "chevron-right";
                    _imagePainter.DrawSvg(g, expandIcon, expandRect, primaryColor, 0.8f);
                }

                // Node icon
                var iconRect = new Rectangle(x + (item.HasChildren ? 16 : 4), itemRect.Y + (itemHeight - 16) / 2, 16, 16);
                string nodeIcon = GetTreeNodeIcon(item);
                _imagePainter.DrawSvg(g, nodeIcon, iconRect, 
                    item.HasChildren ? primaryColor : Color.FromArgb(140, Theme?.ForeColor ?? Color.Black), 0.8f);

                // Node text
                var textRect = new Rectangle(iconRect.Right + 4, itemRect.Y, 
                    itemRect.Width - (iconRect.Right + 4 - itemRect.X), itemHeight);
                var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                g.DrawString(item.Text, treeFont, textBrush, textRect, format);

                currentY += itemHeight;
            }

            return currentY;
        }

        private string GetTreeNodeIcon(TreeNodeItem item)
        {
            if (item.HasChildren && item.IsExpanded) return "folder-open";
            if (item.HasChildren) return "folder";
            
            var text = item.Text?.ToLower() ?? "";
            if (text.Contains(".pdf")) return "file-text";
            if (text.Contains(".jpg") || text.Contains(".png") || text.Contains("image")) return "image";
            if (text.Contains(".doc")) return "file-text";
            if (text.Contains("setting")) return "settings";
            
            return "file";
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx) { }

        public void Dispose()
        {
            _imagePainter?.Dispose();
        }
    }

}