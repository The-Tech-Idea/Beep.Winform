using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// DataTable - Structured data table
    /// Updated: Uses BaseControl.DrawingRect; adds header and row hit areas with hover effects
    /// + Per-column header hit areas; click to sort asc/desc; sort indicator glyphs; local sorting when drawing
    /// + Optional paging via PageIndex/PageSize; selection highlight; empty-state placeholder
    /// + Numeric cell right-alignment and simple pager buttons
    /// </summary>
    internal sealed class DataTablePainter : WidgetPainterBase
    {
        private Rectangle _headerRectCache;
        private List<Rectangle> _rowRects = new();
        private List<Rectangle> _headerColRects = new();
        private int _visibleRowCount;
        private Rectangle _pagerPrevRect;
        private Rectangle _pagerNextRect;

        public override WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx)
        {
            int pad = 16;
            var baseRect = Owner?.DrawingRect ?? drawingRect;
            ctx.DrawingRect = Rectangle.Inflate(baseRect, -8, -8);
            
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
            
            _headerRectCache = ctx.HeaderRect;

            // Build per-column header rects if labels are available
            _headerColRects.Clear();
            if (ctx.Labels?.Any() == true)
            {
                int colWidth = ctx.HeaderRect.Width / Math.Max(ctx.Labels.Count, 1);
                for (int i = 0; i < ctx.Labels.Count; i++)
                {
                    _headerColRects.Add(new Rectangle(ctx.HeaderRect.X + i * colWidth, ctx.HeaderRect.Y, colWidth - 1, ctx.HeaderRect.Height));
                }
            }

            // Build row rects (paged if PageIndex/PageSize present)
            _rowRects = CalculateRowRects(ctx);

            // Pager buttons (bottom-right inside content)
            _pagerPrevRect = Rectangle.Empty;
            _pagerNextRect = Rectangle.Empty;
            var items = ctx.ListItems;
            if (items != null && items.Count > 0)
            {
                int pageIndex = Math.Max(0, ctx.PageIndex);
                int pageSize = Math.Max(1, ctx.PageSize);
                bool hasPaging = pageSize < items.Count;
                if (hasPaging)
                {
                    int btnW = 22; int btnH = 18; int gap = 6;
                    int y = ctx.ContentRect.Bottom - btnH;
                    _pagerPrevRect = new Rectangle(ctx.ContentRect.Right - (btnW * 2 + gap), y, btnW, btnH);
                    _pagerNextRect = new Rectangle(ctx.ContentRect.Right - btnW, y, btnW, btnH);
                }
            }
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
            if (ctx.ShowHeader && (ctx.Labels?.Any() == true))
            {
                int sortedCol = ctx.SortColumnIndex;
                string sortDir = ctx.SortDirection ?? "";
                DrawTableHeader(g, ctx.HeaderRect, ctx.Labels, sortedCol, sortDir);
            }
            
            // Draw data rows (optionally sorted locally by current sort state and paged)
            var items = ctx.ListItems;
            if (items != null && items.Count > 0 && (ctx.Labels?.Any() == true))
            {
                int pageIndex = Math.Max(0, ctx.PageIndex);
                int pageSize = Math.Max(1, ctx.PageSize);

                var itemsToRender = items.ToList();
                int sortedCol = ctx.SortColumnIndex;
                string sortDir = ctx.SortDirection ?? "";
                if (sortedCol >= 0 && sortedCol < ctx.Labels.Count && itemsToRender.Count > 1)
                {
                    string key = ctx.Labels[sortedCol];
                    Func<ListItem, object?> sel = item => GetListItemProperty(item, key);
                    IOrderedEnumerable<ListItem> ordered = sortDir == "desc"
                        ? itemsToRender.OrderByDescending(x => sel(x))
                        : itemsToRender.OrderBy(x => sel(x));
                    itemsToRender = ordered.ToList();
                }

                // Apply paging after sorting
                int skip = pageIndex * pageSize;
                if (skip >= 0 && skip < itemsToRender.Count)
                    itemsToRender = itemsToRender.Skip(skip).Take(pageSize).ToList();
                else
                    itemsToRender = new List<ListItem>();

                _visibleRowCount = itemsToRender.Count;
                if (_visibleRowCount == 0)
                {
                    DrawEmptyState(g, ctx.ContentRect, ctx);
                }
                else
                {
                    DrawTableRows(g, ctx.ContentRect, itemsToRender, ctx.Labels);
                }
            }
            else
            {
                DrawEmptyState(g, ctx.ContentRect, ctx);
            }

            // Draw pager buttons
            if (!_pagerPrevRect.IsEmpty)
            {
                DrawPagerButton(g, _pagerPrevRect, "�", IsAreaHovered("DataTable_PrevPage"));
            }
            if (!_pagerNextRect.IsEmpty)
            {
                DrawPagerButton(g, _pagerNextRect, "�", IsAreaHovered("DataTable_NextPage"));
            }

            // Hover accents
            if (IsAreaHovered("DataTable_Header"))
            {
                using var h = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                g.FillRectangle(h, _headerRectCache);
            }
            for (int i = 0; i < _rowRects.Count; i++)
            {
                if (IsAreaHovered($"DataTable_Row_{i}"))
                {
                    using var h = new SolidBrush(Color.FromArgb(6, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(h, _rowRects[i]);
                }
            }
        }

        private void DrawPagerButton(Graphics g, Rectangle rect, string glyph, bool hovered)
        {
            using var bg = new SolidBrush(Color.FromArgb(hovered ? 24 : 12, Theme?.PrimaryColor ?? Color.Blue));
            using var border = new Pen(Color.FromArgb(60, Theme?.BorderColor ?? Color.Gray));
            using var f = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.FillRoundedRectangle(bg, rect, 3);
            g.DrawRoundedRectangle(border, rect, 3);
            using var fg = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            g.DrawString(glyph, f, fg, rect, fmt);
        }

        private void DrawEmptyState(Graphics g, Rectangle rect, WidgetContext ctx)
        {
            using var txt = new SolidBrush(Color.FromArgb(120, Theme?.ForeColor ?? Color.Gray));
            using var f = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Italic);
            var fmt = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(ctx.EmptyText ?? "No data to display", f, txt, rect, fmt);
        }

        private void DrawTableHeader(Graphics g, Rectangle rect, List<string> columns, int sortedCol, string sortDir)
        {
            // Header background
            using var headerBrush = new SolidBrush(Color.FromArgb(20, Theme?.BorderColor ?? Color.Gray));
            g.FillRectangle(headerBrush, rect);
            
            // Column headers
            using var headerFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 9f, FontStyle.Bold);
            using var headerTextBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
            using var sortedTextBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
            int colWidth = rect.Width / Math.Max(columns.Count, 1);
            for (int i = 0; i < columns.Count; i++)
            {
                var colRect = new Rectangle(rect.X + i * colWidth, rect.Y, colWidth - 1, rect.Height);
                var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                bool hovered = IsAreaHovered($"DataTable_HeaderCol_{i}");
                if (hovered)
                {
                    using var h = new SolidBrush(Color.FromArgb(8, Theme?.PrimaryColor ?? Color.Blue));
                    g.FillRectangle(h, colRect);
                }

                // Draw column name and optional sort glyph
                bool isSorted = i == sortedCol;
                var brush = isSorted ? sortedTextBrush : headerTextBrush;
                string title = columns[i];
                g.DrawString(title, headerFont, brush, colRect, format);

                if (isSorted)
                {
                    // Simple arrow indicator
                    int cx = colRect.Right - 12;
                    int cy = colRect.Y + colRect.Height / 2;
                    Point[] tri = sortDir == "desc"
                        ? new[] { new Point(cx - 5, cy - 3), new Point(cx + 5, cy - 3), new Point(cx, cy + 4) }
                        : new[] { new Point(cx - 5, cy + 3), new Point(cx + 5, cy + 3), new Point(cx, cy - 4) };
                    using var triBrush = new SolidBrush(Theme?.ForeColor ?? Color.Black);
                    g.FillPolygon(triBrush, tri);
                }
                
                // Column separator
                if (i < columns.Count - 1)
                {
                    using var separatorPen = new Pen(Color.FromArgb(50, Theme?.BorderColor ?? Color.Gray), 1);
                    g.DrawLine(separatorPen, colRect.Right, rect.Top, colRect.Right, rect.Bottom);
                }
            }
        }

        private object? GetListItemProperty(ListItem item, string propertyName)
        {
            return propertyName.ToLowerInvariant() switch
            {
                "id" => item.Id,
                "title" => item.Title,
                "subtitle" => item.Subtitle,
                "status" => item.Status,
                "timestamp" => item.Timestamp,
                "iconpath" => item.IconPath,
                _ => null
            };
        }

        private void DrawTableRows(Graphics g, Rectangle rect, List<ListItem> items, List<string> columns)
        {
            if (!items.Any() || !columns.Any()) return;
            
            int rowHeight = Math.Min(24, rect.Height / Math.Max(items.Count, 1));
            int colWidth = rect.Width / columns.Count;
            
            using var cellFont = new Font(Owner?.Font?.FontFamily ?? SystemFonts.DefaultFont.FontFamily, 8f);
            using var cellBrush = new SolidBrush(Color.FromArgb(150, Theme?.ForeColor ?? Color.Black));
            
            for (int row = 0; row < items.Count; row++)
            {
                var item = items[row];
                int y = rect.Y + row * rowHeight;
                
                // Alternate row background
                if (row % 2 == 1)
                {
                    using var altRowBrush = new SolidBrush(Color.FromArgb(10, Theme?.BorderColor ?? Color.Gray));
                    g.FillRectangle(altRowBrush, rect.X, y, rect.Width, rowHeight);
                }
                
                // Cell data
                for (int col = 0; col < columns.Count; col++)
                {
                    var cellRect = new Rectangle(rect.X + col * colWidth + 4, y, colWidth - 8, rowHeight);
                    string key = columns[col];
                    string cellValue = GetListItemProperty(item, key)?.ToString() ?? "";

                    // Align numbers to the right, others to the near
                    var fmt = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
                    if (decimal.TryParse(cellValue, out _))
                        fmt.Alignment = StringAlignment.Far;
                    
                    g.DrawString(cellValue, cellFont, cellBrush, cellRect, fmt);
                }
                
                // Row separator
                using var rowPen = new Pen(Color.FromArgb(30, Theme?.BorderColor ?? Color.Gray), 1);
                g.DrawLine(rowPen, rect.X, y + rowHeight - 1, rect.Right, y + rowHeight - 1);
            }
        }

        public override void DrawForegroundAccents(Graphics g, WidgetContext ctx)
        {
            // Selection highlight if present
            if (ctx.SelectedRowIndex >= 0 && ctx.SelectedRowIndex < _rowRects.Count)
            {
                var r = _rowRects[ctx.SelectedRowIndex];
                using var sel = new SolidBrush(Color.FromArgb(10, Theme?.PrimaryColor ?? Color.Blue));
                using var pen = new Pen(Color.FromArgb(140, Theme?.PrimaryColor ?? Color.Blue), 1f);
                g.FillRectangle(sel, r);
                g.DrawRectangle(pen, r);
            }
        }

        public override void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle>? notifyAreaHit)
        {
            if (owner == null) return;
            ClearOwnerHitAreas();

            if (!_headerRectCache.IsEmpty)
            {
                owner.AddHitArea("DataTable_Header", _headerRectCache, null, () =>
                {
                    ctx.HeaderClicked = true;
                    notifyAreaHit?.Invoke("DataTable_Header", _headerRectCache);
                });
            }

            // Per-column header clicking to sort
            for (int i = 0; i < _headerColRects.Count; i++)
            {
                int idx = i;
                var rect = _headerColRects[i];
                owner.AddHitArea($"DataTable_HeaderCol_{idx}", rect, null, () =>
                {
                    int current = ctx.SortColumnIndex;
                    string dir = ctx.SortDirection ?? "asc";
                    if (current == idx)
                    {
                        // Toggle
                        dir = dir == "asc" ? "desc" : "asc";
                    }
                    else
                    {
                        current = idx; dir = "asc";
                    }
                    ctx.SortColumnIndex = current;
                    ctx.SortDirection = dir;
                    notifyAreaHit?.Invoke($"DataTable_HeaderCol_{idx}", rect);
                    Owner?.Invalidate();
                });
            }

            for (int i = 0; i < _rowRects.Count; i++)
            {
                int idx = i;
                var rect = _rowRects[i];
                owner.AddHitArea($"DataTable_Row_{idx}", rect, null, () =>
                {
                    ctx.SelectedRowIndex = idx;
                    notifyAreaHit?.Invoke($"DataTable_Row_{idx}", rect);
                    Owner?.Invalidate();
                });
            }

            // Pager buttons
            if (!_pagerPrevRect.IsEmpty)
            {
                owner.AddHitArea("DataTable_PrevPage", _pagerPrevRect, null, () =>
                {
                    var items = ctx.ListItems;
                    if (items != null && items.Count > 0)
                    {
                        int pageIndex = Math.Max(0, ctx.PageIndex);
                        int pageSize = Math.Max(1, ctx.PageSize);
                        if (pageSize < items.Count && pageIndex > 0)
                        {
                            ctx.PageIndex = pageIndex - 1;
                            notifyAreaHit?.Invoke("DataTable_PrevPage", _pagerPrevRect);
                            Owner?.Invalidate();
                        }
                    }
                });
            }
            if (!_pagerNextRect.IsEmpty)
            {
                owner.AddHitArea("DataTable_NextPage", _pagerNextRect, null, () =>
                {
                    var items = ctx.ListItems;
                    if (items != null && items.Count > 0)
                    {
                        int pageIndex = Math.Max(0, ctx.PageIndex);
                        int pageSize = Math.Max(1, ctx.PageSize);
                        int maxIndex = Math.Max(0, (int)Math.Ceiling(items.Count / (double)pageSize) - 1);
                        if (pageSize < items.Count && pageIndex < maxIndex)
                        {
                            ctx.PageIndex = pageIndex + 1;
                            notifyAreaHit?.Invoke("DataTable_NextPage", _pagerNextRect);
                            Owner?.Invalidate();
                        }
                    }
                });
            }
        }

        private List<Rectangle> CalculateRowRects(WidgetContext ctx)
        {
            var result = new List<Rectangle>();
            var items = ctx.ListItems;
            if (items == null || items.Count == 0) return result;

            int pageIndex = Math.Max(0, ctx.PageIndex);
            int pageSize = Math.Max(1, ctx.PageSize);
            int start = pageIndex * pageSize;
            int visible = start < items.Count ? Math.Min(pageSize, items.Count - start) : 0;

            int rowHeight = visible > 0 ? Math.Min(24, ctx.ContentRect.Height / Math.Max(visible, 1)) : 24;
            for (int i = 0; i < visible; i++)
            {
                result.Add(new Rectangle(ctx.ContentRect.X, ctx.ContentRect.Y + i * rowHeight, ctx.ContentRect.Width, rowHeight));
            }
            return result;
        }
    }
}