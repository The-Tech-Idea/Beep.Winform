using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;


namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Tree Navigation - Hierarchical navigation with expandable/collapsible nodes
    /// </summary>
    internal sealed class TreeNavigationPainter : WidgetPainterBase, IDisposable
    {
        private WidgetContext? _lastCtx;
        private bool _wheelHooked;

        private Font? _nodeName;

        private const int ItemHeightDp = 24;
        private const int IndentDp     = 20;
        private const int IconSizeDp   = 16;
        private const int ExpandSizeDp = 12;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            if (!_wheelHooked && Owner != null) { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
        }

        protected override void RebuildFonts()
        {
            _nodeName?.Dispose();
            _nodeName = BeepThemesManager.ToFont(Theme?.LabelSmall ?? new TypographyStyle { FontSize = 9f }, true);
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (_lastCtx == null) return;
            int maxY = Math.Max(0, _lastCtx.TotalContentHeight - _lastCtx.ContentRect.Height);
            _lastCtx.ScrollOffsetY = Math.Max(0, Math.Min(_lastCtx.ScrollOffsetY - e.Delta / 120 * Dp(ItemHeightDp) * 3, maxY));
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.X + Dp(8), ctx.DrawingRect.Y + Dp(8),
                ctx.DrawingRect.Width - Dp(16), ctx.DrawingRect.Height - Dp(16));

            var treeItems = ctx.TreeItems?.Cast<TreeNodeItem>().ToList() ?? new List<TreeNodeItem>();
            ctx.TotalContentHeight = treeItems.Count * Dp(ItemHeightDp);
            ClampScrollOffset(ctx);
            _lastCtx = ctx;
            return ctx;
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            var bgBrush = PaintersFactory.GetSolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            var treeItems = ctx.TreeItems?.Cast<TreeNodeItem>().ToList() ?? CreateSampleTreeStructure();
            if (!treeItems.Any()) return;
            DrawTreeStructure(g, ctx, treeItems);
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

        private void DrawTreeStructure(Graphics g, WidgetContext ctx, List<TreeNodeItem> items)
        {
            int stride    = Dp(ItemHeightDp);
            int indent    = Dp(IndentDp);
            int iconSz    = Dp(IconSizeDp);
            int expandSz  = Dp(ExpandSizeDp);

            var primaryColor  = Theme?.PrimaryColor ?? Color.Blue;
            var textBrush     = PaintersFactory.GetSolidBrush(Theme?.ForeColor ?? Color.Black);
            var dimTextBrush  = PaintersFactory.GetSolidBrush(Color.FromArgb(140, Theme?.ForeColor ?? Color.Black));
            var linePen       = PaintersFactory.GetPen(Color.FromArgb(100, Color.Gray), 1);
            var nameFormat    = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

            var savedClip = g.Clip;
            g.SetClip(ctx.ContentRect);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = ctx.ContentRect.Y + i * stride - ctx.ScrollOffsetY;
                if (y + stride < ctx.ContentRect.Y) continue;
                if (y > ctx.ContentRect.Bottom) break;

                int x = ctx.ContentRect.X + item.Level * indent;
                var itemRect = new Rectangle(x, y, ctx.ContentRect.Width - item.Level * indent, stride);

                // Tree connector lines
                if (item.Level > 0)
                {
                    int lineX = ctx.ContentRect.X + (item.Level - 1) * indent + Dp(8);
                    g.DrawLine(linePen, lineX, y, lineX, y + stride / 2);
                    g.DrawLine(linePen, lineX, y + stride / 2, lineX + Dp(12), y + stride / 2);
                }

                // Expand/collapse chevron
                if (item.HasChildren)
                {
                    var expandRect = new Rectangle(x, y + (stride - expandSz) / 2, expandSz, expandSz);
                    string chevronSvg = item.IsExpanded ? SvgsUI.ChevronDown : SvgsUI.ChevronRight;
                    using var expandPath = CreateRoundedPath(expandRect, 0);
                    StyledImagePainter.PaintWithTint(g, expandPath, chevronSvg, primaryColor, 0.8f);
                }

                // Node icon
                int iconOffX = item.HasChildren ? expandSz + Dp(4) : Dp(4);
                var iconRect = new Rectangle(x + iconOffX, y + (stride - iconSz) / 2, iconSz, iconSz);
                string nodeIcon = GetTreeNodeIconSvg(item);
                Color iconColor = item.HasChildren ? primaryColor : Color.FromArgb(140, Theme?.ForeColor ?? Color.Black);
                using var iconPath = CreateRoundedPath(iconRect, 0);
                StyledImagePainter.PaintWithTint(g, iconPath, nodeIcon, iconColor, 0.8f);

                // Node text
                var textRect = new Rectangle(iconRect.Right + Dp(4), y, itemRect.Width - (iconRect.Right + Dp(4) - x), stride);
                if (_nodeName != null)
                    g.DrawString(item.Text, _nodeName, item.HasChildren ? textBrush : dimTextBrush, textRect, nameFormat);
            }

            g.Clip = savedClip;
        }

        private string GetTreeNodeIconSvg(TreeNodeItem item)
        {
            if (item.HasChildren && item.IsExpanded) return SvgsUI.FolderOpen;
            if (item.HasChildren) return SvgsUI.Folder;

            var text = item.Text?.ToLower() ?? "";
            if (text.Contains(".pdf") || text.Contains(".doc")) return SvgsUI.FileText;
            if (text.Contains(".jpg") || text.Contains(".png") || text.Contains("image")) return SvgsUI.Photo;
            if (text.Contains("setting")) return SvgsUI.Settings;
            return SvgsUI.FileText;
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, IsAreaHovered("TreeNav_Scroll"));
        }

        public void Dispose()
        {
            if (_wheelHooked && Owner != null) { Owner.MouseWheel -= OnMouseWheel; _wheelHooked = false; }
            _nodeName?.Dispose();
        }
    }

}
