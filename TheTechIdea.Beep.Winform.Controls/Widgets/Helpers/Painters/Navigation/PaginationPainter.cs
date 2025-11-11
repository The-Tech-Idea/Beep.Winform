using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using BaseImage = TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Pagination - Page navigation with modern styling (interactive)
    /// + Keyboard: Left/Right prev/next, Home/End first/last, PageUp/Down +/- 5 pages
    /// </summary>
    internal sealed class PaginationPainter : WidgetPainterBase, IDisposable
    {
        private BaseImage.ImagePainter _imagePainter;
        private readonly List<(Rectangle rect, string id, string text, bool enabled, bool active, string icon)> _buttons = new();
        private int _currentPage;
        private int _totalPages;
        private bool _keysHooked;
        private WidgetContext _lastCtx;

        public PaginationPainter()
        {
            _imagePainter = new BaseImage.ImagePainter();
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            HookKeyboard();
        }

        private void HookKeyboard()
        {
            if (_keysHooked || Owner == null) return;
            try
            {
                Owner._input.LeftArrowKeyPressed += OnLeft;
                Owner._input.RightArrowKeyPressed += OnRight;
                Owner._input.HomeKeyPressed += OnHome;
                Owner._input.EndKeyPressed += OnEnd;
                Owner._input.PageUpKeyPressed += OnPageUp;
                Owner._input.PageDownKeyPressed += OnPageDown;
                _keysHooked = true;
            }
            catch { }
        }

        private void OnLeft(object? s, EventArgs e) => SetPage(_currentPage - 1);
        private void OnRight(object? s, EventArgs e) => SetPage(_currentPage + 1);
        private void OnHome(object? s, EventArgs e) => SetPage(1);
        private void OnEnd(object? s, EventArgs e) => SetPage(_totalPages);
        private void OnPageUp(object? s, EventArgs e) => SetPage(_currentPage - 5);
        private void OnPageDown(object? s, EventArgs e) => SetPage(_currentPage + 5);

        private void SetPage(int page)
        {
            if (_lastCtx == null) return;
            int total = _lastCtx.TotalPages > 0 ? _lastCtx.TotalPages : Math.Max(1, _totalPages);
            int clamped = Math.Clamp(page, 1, total);
            _lastCtx.CurrentPage = clamped;
            _currentPage = clamped;
            Owner?.Invalidate();
        }

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 8;
            ctx.DrawingRect = Rectangle.Inflate(drawingRect, -4, -4);
            
            ctx.ContentRect = new Rectangle(
                ctx.DrawingRect.Left + pad,
                ctx.DrawingRect.Top + pad,
                ctx.DrawingRect.Width - pad * 2,
                ctx.DrawingRect.Height - pad * 2
            );

            // Capture state
            _currentPage = ctx.CurrentPage > 0 ? ctx.CurrentPage : 1;
            _totalPages = ctx.TotalPages > 0 ? Math.Max(1, ctx.TotalPages) : 10;
            _currentPage = Math.Clamp(_currentPage, 1, _totalPages);

            BuildButtons(ctx);
            _lastCtx = ctx;
            return ctx;
        }

        private void BuildButtons(WidgetContext ctx)
        {
            _buttons.Clear();
            int buttonSize = 32;
            int spacing = 4;
            int maxVisiblePages = 7;

            int startPage = Math.Max(1, _currentPage - 3);
            int endPage = Math.Min(_totalPages, startPage + maxVisiblePages - 1);
            if (endPage - startPage + 1 < maxVisiblePages)
                startPage = Math.Max(1, endPage - maxVisiblePages + 1);

            int x = ctx.ContentRect.X;
            int y = ctx.ContentRect.Y + (ctx.ContentRect.Height - buttonSize) / 2;

            // First
            var firstRect = new Rectangle(x, y, buttonSize, buttonSize);
            _buttons.Add((firstRect, "Pagination_First", string.Empty, _currentPage > 1, false, "chevrons-left"));
            x += buttonSize + spacing;
            // Prev
            var prevRect = new Rectangle(x, y, buttonSize, buttonSize);
            _buttons.Add((prevRect, "Pagination_Prev", string.Empty, _currentPage > 1, false, "chevron-left"));
            x += buttonSize + spacing;

            // First page + ellipsis
            if (startPage > 1)
            {
                var r = new Rectangle(x, y, buttonSize, buttonSize);
                _buttons.Add((r, "Pagination_Page_1", "1", true, _currentPage == 1, null));
                x += buttonSize + spacing + 24; // reserve for ellipsis drawing
            }

            // Visible page numbers
            for (int page = startPage; page <= endPage; page++)
            {
                var r = new Rectangle(x, y, buttonSize, buttonSize);
                _buttons.Add((r, $"Pagination_Page_{page}", page.ToString(), true, page == _currentPage, null));
                x += buttonSize + spacing;
            }

            // Last page with ellipsis
            if (endPage < _totalPages)
            {
                x += 24; // space for ellipsis
                var r = new Rectangle(x, y, buttonSize, buttonSize);
                _buttons.Add((r, $"Pagination_Page_{_totalPages}", _totalPages.ToString(), true, _currentPage == _totalPages, null));
                x += buttonSize + spacing;
            }

            // Next
            var nextRect = new Rectangle(x, y, buttonSize, buttonSize);
            _buttons.Add((nextRect, "Pagination_Next", string.Empty, _currentPage < _totalPages, false, "chevron-right"));
            x += buttonSize + spacing;
            // Last
            var lastRect = new Rectangle(x, y, buttonSize, buttonSize);
            _buttons.Add((lastRect, "Pagination_Last", string.Empty, _currentPage < _totalPages, false, "chevrons-right"));
        }

        public override void DrawBackground(Graphics g, WidgetContext ctx)
        {
            using var bgBrush = new SolidBrush(Theme?.BackColor ?? Color.White);
            g.FillRectangle(bgBrush, ctx.DrawingRect);
        }

        public override void DrawContent(Graphics g, WidgetContext ctx)
        {
            _imagePainter.CurrentTheme = Theme;
            _imagePainter.ApplyThemeOnImage = true;

            // Ensure buttons reflect any runtime changes
            _currentPage = ctx.CurrentPage > 0 ? ctx.CurrentPage : _currentPage;
            _totalPages = ctx.TotalPages > 0 ? Math.Max(1, ctx.TotalPages) : _totalPages;
            _currentPage = Math.Clamp(_currentPage, 1, _totalPages);
            BuildButtons(ctx);
            _lastCtx = ctx;

            DrawModernPagination(g, ctx);
        }

        private void DrawModernPagination(Graphics g, WidgetContext ctx)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            int spacing = 4;
            
            using var pageFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);

            foreach (var (rect, id, text, enabled, active, icon) in _buttons)
            {
                bool hovered = IsAreaHovered(id);
                DrawPageButton(g, rect, text, enabled, active, iconName: icon, hovered: hovered);

                // Draw ellipsis where we reserved space
                if (id == "Pagination_Page_1" && _buttons.Any(b => b.id.StartsWith("Pagination_Page_") && int.Parse(b.id.Substring("Pagination_Page_".Length)) > 2))
                {
                    using var ellipsisBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.DrawString("...", pageFont, ellipsisBrush, new Point(rect.Right + spacing, rect.Y + rect.Height / 2 - 6));
                }
                if (id == $"Pagination_Page_{_totalPages}" && _buttons.Any(b => b.id.StartsWith("Pagination_Page_") && int.Parse(b.id.Substring("Pagination_Page_".Length)) < _totalPages - 1))
                {
                    using var ellipsisBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    g.DrawString("...", pageFont, ellipsisBrush, new Point(rect.X - 24, rect.Y + rect.Height / 2 - 6));
                }
            }
        }

        private void DrawPageButton(Graphics g, Rectangle rect, string text, bool enabled, bool isActive, string iconName = null, bool hovered = false)
        {
            var primaryColor = Theme?.PrimaryColor ?? Color.FromArgb(33, 150, 243);
            Color bgColor = isActive ? primaryColor : enabled ? (hovered ? Color.FromArgb(245, 247, 249) : Color.White) : Color.FromArgb(248, 249, 250);
            Color borderColor = isActive ? primaryColor : (hovered ? Color.FromArgb(200, 215, 230) : Color.FromArgb(220, 220, 220));
            using var bgBrush = new SolidBrush(bgColor);
            using var borderPen = new Pen(borderColor, 1);
            using var buttonPath = CreateRoundedPath(rect, 4);
            g.FillPath(bgBrush, buttonPath);
            g.DrawPath(borderPen, buttonPath);

            if (!string.IsNullOrEmpty(iconName))
            {
                var iconRect = new Rectangle(rect.X + (rect.Width - 16) / 2, rect.Y + (rect.Height - 16) / 2, 16, 16);
                Color iconColor = enabled ? (isActive ? Color.White : primaryColor) : Color.FromArgb(150, Color.Gray);
                _imagePainter.DrawSvg(g, iconName, iconRect, iconColor, 0.8f);
            }
            else if (!string.IsNullOrEmpty(text))
            {
                using var textFont = new Font(Owner.Font.FontFamily, 9f, FontStyle.Regular);
                Color textColor = isActive ? Color.White : enabled ? Color.FromArgb(60, 60, 60) : Color.FromArgb(150, Color.Gray);
                using var textBrush = new SolidBrush(textColor);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(text, textFont, textBrush, rect, format);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            foreach (var (rect, id, _, enabled, active, _) in _buttons)
            {
                if (IsAreaHovered(id))
                {
                    using var pen = new Pen(Theme?.AccentColor ?? Color.SteelBlue, 1.2f);
                    g.DrawRoundedRectangle(pen, rect, 4);
                }
            }

            if (ctx.ShowPageInfo)
            {
                using var infoFont = new Font(Owner.Font.FontFamily, 8f, FontStyle.Regular);
                using var infoBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                var infoText = $"Page {_currentPage} of {_totalPages}";
                var infoRect = new Rectangle(ctx.ContentRect.Right - 100, ctx.ContentRect.Y, 100, ctx.ContentRect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };
                g.DrawString(infoText, infoFont, infoBrush, infoRect, format);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            foreach (var (rect, id, text, enabled, active, icon) in _buttons)
            {
                if (!enabled && string.IsNullOrEmpty(text)) continue; // skip disabled nav buttons
                string captureId = id;
                owner.AddHitArea(id, rect, null, () =>
                {
                    int current = ctx.CurrentPage > 0 ? ctx.CurrentPage : _currentPage;
                    int total = ctx.TotalPages > 0 ? Math.Max(1, ctx.TotalPages) : _totalPages;
                    switch (captureId)
                    {
                        case "Pagination_First":
                            current = 1; break;
                        case "Pagination_Prev":
                            current = Math.Max(1, current - 1); break;
                        case "Pagination_Next":
                            current = Math.Min(total, current + 1); break;
                        case "Pagination_Last":
                            current = total; break;
                        default:
                            if (captureId.StartsWith("Pagination_Page_") && int.TryParse(captureId["Pagination_Page_".Length..], out int p))
                                current = Math.Clamp(p, 1, total);
                            break;
                    }
                    ctx.CurrentPage = current;
                    _currentPage = current;
                    notifyAreaHit?.Invoke(captureId, rect);
                    Owner?.Invalidate();
                });
            }
        }

        public void Dispose()
        {
            if (_keysHooked && Owner != null)
            {
                try
                {
                    Owner._input.LeftArrowKeyPressed -= OnLeft;
                    Owner._input.RightArrowKeyPressed -= OnRight;
                    Owner._input.HomeKeyPressed -= OnHome;
                    Owner._input.EndKeyPressed -= OnEnd;
                    Owner._input.PageUpKeyPressed -= OnPageUp;
                    Owner._input.PageDownKeyPressed -= OnPageDown;
                }
                catch { }
            }
            _imagePainter?.Dispose();
        }
    }
}