using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// jQuery DataTables inspired pagination Style
    /// </summary>
    public class DataTablesNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.DataTables;
        public override int RecommendedHeight => 50;
        public override int RecommendedMinWidth => 650;

        private const int BUTTON_SIZE = 32;
        private const int SPACING = 0; // DataTables uses connected buttons

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Background using theme
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Left info text
            PaintDataTablesInfo(g, layout.PositionIndicatorRect, grid.Selection.RowIndex + 1, 
                grid.Data.Rows.Count, theme);

            // Center pagination
            PaintDataTablesPagination(g, grid, layout, theme);

            // Right CRUD buttons
            PaintDataTablesButton(g, grid, layout.AddNewButtonRect, "New", () => grid.InsertNew(), theme, false, true);
            PaintDataTablesButton(g, grid, layout.DeleteButtonRect, "Delete", () => grid.DeleteCurrent(), theme, false, false);
            PaintDataTablesButton(g, grid, layout.SaveButtonRect, "Save", () => grid.Save(), theme, false, false);
        }

        private void PaintDataTablesPagination(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            int totalRecords = grid.Data.Rows.Count;
            int pageSize = grid.VisibleRowCapacity; // Dynamic page size
            int currentPage = grid.Render.GetCurrentPage(grid);
            int totalPages = grid.Render.GetTotalPages(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 5);

            // Calculate center position for pagination group
            int paginationWidth = BUTTON_SIZE * 7; // Previous + 5 pages + Next
            int centerX = layout.FirstButtonRect.X + (layout.FirstButtonRect.Width - paginationWidth) / 2;
            int y = layout.FirstButtonRect.Y;

            int x = centerX;

            // Previous button (rounded left)
            Rectangle prevRect = new Rectangle(x, y, BUTTON_SIZE + 20, BUTTON_SIZE);
            PaintDataTablesButton(g, grid, prevRect, "Previous", () => grid.Render.GoToPreviousPage(grid), theme, false, true);
            x += BUTTON_SIZE + 20;

            // Page numbers (5 max)
            for (int page = startPage; page <= endPage; page++)
            {
                Rectangle pageRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
                bool isActive = page == currentPage;

                int targetPage = page;
                grid.AddHitArea($"Page{page}", pageRect, null, () => grid.Render.GoToPage(grid, targetPage));

                // Background
                Color bgColor = isActive ? theme.AccentColor : theme.GridHeaderBackColor;
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillRectangle(brush, pageRect);
                }

                // Border
                using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
                {
                    g.DrawRectangle(pen, pageRect);
                }

                // Page number
                Color textColor = isActive ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                {
                    DrawCenteredText(g, page.ToString(), font, textColor, pageRect);
                }

                x += BUTTON_SIZE;
            }

            // Next button (rounded right)
            Rectangle nextRect = new Rectangle(x, y, BUTTON_SIZE + 20, BUTTON_SIZE);
            PaintDataTablesButton(g, grid, nextRect, "Next", () => grid.Render.GoToNextPage(grid), theme, false, false);
        }

        private void PaintDataTablesButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string text, Action action, IBeepTheme theme, bool isActive, bool roundLeft)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(text, bounds, null, action);

            // Background
            Color bgColor = isActive ? theme.AccentColor : theme.GridHeaderBackColor;
            using (var brush = new SolidBrush(bgColor))
            {
                if (roundLeft)
                {
                    using (var path = CreateRoundedRectangle(bounds, 4, true, false, true, false))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Border
            using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
            {
                if (roundLeft)
                {
                    using (var path = CreateRoundedRectangle(bounds, 4, true, false, true, false))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, bounds);
                }
            }

            // Text
            Color textColor = isActive ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                DrawCenteredText(g, text, font, textColor, bounds);
            }
        }

        private void PaintDataTablesInfo(Graphics g, Rectangle bounds, int current, int total, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            int start = current;
            int end = Math.Min(current + 9, total);

            string text = $"Showing {start} to {end} of {total} entries";
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var brush = new SolidBrush(theme.GridHeaderForeColor))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, brush, bounds, format);
            }
        }

        private GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius, 
            bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            GraphicsPath path = new GraphicsPath();
            
            if (topLeft)
                path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            else
                path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);

            if (topRight)
                path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            else
                path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);

            if (bottomRight)
                path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            else
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);

            if (bottomLeft)
                path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            else
                path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);

            path.CloseFigure();
            return path;
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintDataTablesButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintDataTablesInfo
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = false,
                ButtonSize = new Size(BUTTON_SIZE, BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Left info
            layout.PositionIndicatorRect = new Rectangle(availableBounds.X + 16, y, 250, BUTTON_SIZE);

            // Center pagination (calculated in PaintDataTablesPagination)
            int paginationWidth = (BUTTON_SIZE + 20) * 2 + BUTTON_SIZE * 5; // Prev + 5 pages + Next
            int centerX = availableBounds.X + (availableBounds.Width - paginationWidth) / 2;
            layout.FirstButtonRect = new Rectangle(centerX, y, paginationWidth, BUTTON_SIZE);

            // Right CRUD
            int x = availableBounds.Right - 16 - 60;
            layout.SaveButtonRect = new Rectangle(x, y, 60, BUTTON_SIZE);
            
            x -= 60 + 8;
            layout.DeleteButtonRect = new Rectangle(x, y, 60, BUTTON_SIZE);
            
            x -= 60 + 8;
            layout.AddNewButtonRect = new Rectangle(x, y, 60, BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
