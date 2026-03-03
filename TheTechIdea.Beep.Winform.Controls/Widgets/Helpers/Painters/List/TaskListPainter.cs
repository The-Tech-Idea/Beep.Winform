using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// TaskList - Checklist/todo Style with checkbox and item hit areas
    /// </summary>
    internal sealed class TaskListPainter : WidgetPainterBase
    {
        private readonly List<Rectangle> _itemRects    = new();
        private readonly List<Rectangle> _checkboxRects = new();

        // Cached fonts — never created inside draw path
        private Font? _titleFont;
        private Font? _taskFont;
        private Font? _taskDoneFont;

        // Mouse-wheel scroll
        private bool _wheelHooked;

        // Layout constants (logical dp)
        private const int ItemHeightDp   = 32;
        private const int CheckSizeDp    = 12;
        private const int CheckOffsetXDp = 8;
        private const int TextOffsetXDp  = 28;
        private const int PadDp          = 16;
        private const int HeaderHeightDp = 24;

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookMouseWheel();
        }

        private void HookMouseWheel()
        {
            if (_wheelHooked || Owner == null) return;
            try { Owner.MouseWheel += OnMouseWheel; _wheelHooked = true; }
            catch { }
        }

        private void OnMouseWheel(object? s, System.Windows.Forms.MouseEventArgs e)
        {
            if (LastCtx == null) return;
            int lineH = Dp(ItemHeightDp);
            LastCtx.ScrollOffsetY = Math.Max(0,
                Math.Min(LastCtx.ScrollOffsetY - e.Delta / 120 * lineH * 3,
                         Math.Max(0, LastCtx.TotalContentHeight - LastCtx.ContentRect.Height)));
            Owner?.Invalidate();
        }

        protected override void RebuildFonts()
        {
            _titleFont?.Dispose();
            _taskFont?.Dispose();
            _taskDoneFont?.Dispose();
            var titleStyle    = Theme?.TaskCardTitleFont    ?? new TypographyStyle { FontSize = 11f, FontWeight = FontWeight.Bold };
            var taskStyle     = Theme?.LabelSmall           ?? new TypographyStyle { FontSize = 9f };
            var taskDoneStyle = Theme?.LabelSmall           ?? new TypographyStyle { FontSize = 9f };
            if (taskDoneStyle == taskStyle) taskDoneStyle = new TypographyStyle { FontSize = taskStyle.FontSize, IsStrikeout = true };
            _titleFont   = BeepThemesManager.ToFont(titleStyle,    applyDpiScaling: true);
            _taskFont    = BeepThemesManager.ToFont(taskStyle,     applyDpiScaling: true);
            _taskDoneFont = BeepThemesManager.ToFont(taskDoneStyle, applyDpiScaling: true);
        }

        private WidgetContext? LastCtx;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            LastCtx = ctx;
            int pad = Dp(PadDp);
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);

            int headerH = Dp(HeaderHeightDp);
            ctx.HeaderRect  = new Rectangle(ctx.DrawingRect.Left + pad, ctx.DrawingRect.Top + pad, ctx.DrawingRect.Width - pad * 2, headerH);
            ctx.ContentRect = new Rectangle(ctx.DrawingRect.Left + pad, ctx.HeaderRect.Bottom + Dp(8), ctx.DrawingRect.Width - pad * 2,
                                            ctx.DrawingRect.Height - headerH - pad * 3);

            // Pre-compute item and checkbox rects (virtual / non-scrolled positions)
            _itemRects.Clear();
            _checkboxRects.Clear();
            int itemH = Dp(ItemHeightDp);
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                ctx.TotalContentHeight = items.Count * itemH;
                for (int i = 0; i < items.Count; i++)
                {
                    int y       = ctx.ContentRect.Y + i * itemH;  // unscrolled
                    var iRect   = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width, itemH);
                    var chkRect = new Rectangle(iRect.X + Dp(CheckOffsetXDp), y + (itemH - Dp(CheckSizeDp)) / 2, Dp(CheckSizeDp), Dp(CheckSizeDp));
                    _itemRects.Add(iRect);
                    _checkboxRects.Add(chkRect);
                }
            }
            else
            {
                ctx.TotalContentHeight = 0;
            }
            ctx.TotalContentWidth = 0;
            ClampScrollOffset(ctx);
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
            if (ctx.ShowHeader && !string.IsNullOrEmpty(ctx.Title) && _titleFont != null)
            {
                using var titleBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
                g.DrawString(ctx.Title, _titleFont, titleBrush, ctx.HeaderRect);
            }

            // Draw task items
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                var savedClip = g.Clip;
                g.SetClip(ctx.ContentRect);
                DrawTaskItems(g, ctx, items);
                g.Clip = savedClip;
            }
        }

        private void DrawTaskItems(Graphics g, WidgetContext ctx, List<ListItem> items)
        {
            if (!items.Any()) return;

            int itemH    = Dp(ItemHeightDp);
            int scroll   = ctx.ScrollOffsetY;
            var primary  = Theme?.PrimaryColor ?? Color.Blue;

            using var taskFmt = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int y = (i < _itemRects.Count ? _itemRects[i].Y : ctx.ContentRect.Y + i * itemH) - scroll;
                if (y + itemH < ctx.ContentRect.Y) continue;
                if (y > ctx.ContentRect.Bottom) break;

                var itemRect    = new Rectangle(ctx.ContentRect.X, y, ctx.ContentRect.Width - (NeedsVerticalScroll(ctx) ? Dp(10) : 0), itemH);
                var checkboxRect = new Rectangle(itemRect.X + Dp(CheckOffsetXDp), y + (itemH - Dp(CheckSizeDp)) / 2, Dp(CheckSizeDp), Dp(CheckSizeDp));

                // Hover background
                if (IsAreaHovered($"TaskList_Item_{i}"))
                {
                    using var hover = new SolidBrush(Color.FromArgb(6, primary));
                    g.FillRectangle(hover, itemRect);
                }
                
                // Checkbox
                // Check if it's a TaskItem and if it's completed
                bool isCompleted = item is TaskItem taskItem && taskItem.IsCompleted;
                using var checkboxPen = new Pen(Color.FromArgb(IsAreaHovered($"TaskList_Check_{i}") ? 200 : 150, Color.Gray), 1);
                g.DrawRectangle(checkboxPen, checkboxRect);

                if (isCompleted)
                {
                    using var checkBrush = new SolidBrush(Theme?.PrimaryColor ?? Color.Green);
                    g.FillRectangle(checkBrush, Rectangle.Inflate(checkboxRect, -Dp(2), -Dp(2)));
                    using var checkPen = new Pen(Color.White, 1.5f);
                    int d = Dp(3);
                    g.DrawLines(checkPen, new Point[]
                    {
                        new Point(checkboxRect.X + d,     checkboxRect.Y + checkboxRect.Height / 2),
                        new Point(checkboxRect.X + checkboxRect.Width / 2, checkboxRect.Bottom - d),
                        new Point(checkboxRect.Right - d, checkboxRect.Y + d)
                    });
                }
                
                // Task text
                var taskRect = new Rectangle(itemRect.X + Dp(TextOffsetXDp), y, itemRect.Width - Dp(TextOffsetXDp + 4), itemH);
                if (!string.IsNullOrEmpty(item.Title))
                {
                    Color textColor = isCompleted ? Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray) : (Theme?.ForeColor ?? Color.Black);
                    var font        = isCompleted ? _taskDoneFont : _taskFont;
                    var taskBrush   = PaintersFactory.GetSolidBrush(textColor);
                    if (font != null)
                        g.DrawString(item.Title, font, taskBrush, taskRect, taskFmt);
                }
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            bool scrollHov = IsAreaHovered("TaskList_Scroll");
            DrawVerticalScrollbar(g, ctx.ContentRect, ctx, scrollHov);
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            for (int i = 0; i < _itemRects.Count; i++)
            {
                int idx = i;
                var itemRect = _itemRects[i];
                var checkRect = _checkboxRects[i];

                owner.AddHitArea($"TaskList_Item_{idx}", itemRect, null, () =>
                {
                    ctx.SelectedTaskIndex = idx;
                    notifyAreaHit?.Invoke($"TaskList_Item_{idx}", itemRect);
                    Owner?.Invalidate();
                });

                owner.AddHitArea($"TaskList_Check_{idx}", checkRect, null, () =>
                {
                    ctx.ToggleTaskIndex = idx;
                    notifyAreaHit?.Invoke($"TaskList_Check_{idx}", checkRect);
                    Owner?.Invalidate();
                });
            }
        }
    }
}