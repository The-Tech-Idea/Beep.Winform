using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Tailwind CSS inspired flat minimal design
    /// </summary>
    public class TailwindNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Tailwind;
        public override int RecommendedHeight => 46;
        public override int RecommendedMinWidth => 600;

        private const int BUTTON_HEIGHT = 36;
        private const int BUTTON_WIDTH = 36;
        private const int SPACING = 4;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Flat light background
            using (var brush = new SolidBrush(ControlPaint.Light(theme.GridHeaderBackColor, 0.05f)))
            {
                g.FillRectangle(brush, bounds);
            }

            // Very subtle top border
            using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
            {
                g.DrawLine(pen, bounds.X, bounds.Y, bounds.Right, bounds.Y);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Left section - CRUD with labels
            PaintTailwindButton(g, grid, layout.AddNewButtonRect, "Add new", () => grid.InsertNew(), theme, true, true);
            PaintTailwindButton(g, grid, layout.DeleteButtonRect, "Delete", () => grid.DeleteCurrent(), theme, false, true);
            PaintTailwindButton(g, grid, layout.SaveButtonRect, "Save", () => grid.Save(), theme, true, true);

            // Right section - Simple pagination
            PaintTailwindSimplePagination(g, grid, layout, theme);
        }

        private void PaintTailwindSimplePagination(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            int totalRecords = grid.Data.Rows.Count;
            int pageSize = grid.VisibleRowCapacity; // Dynamic page size
            int currentPage = grid.Render.GetCurrentPage(grid);
            int totalPages = grid.Render.GetTotalPages(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 3);

            int y = layout.FirstButtonRect.Y;
            int x = layout.FirstButtonRect.X;

            // "Prev" button
            Rectangle prevRect = new Rectangle(x, y, 60, BUTTON_HEIGHT);
            PaintTailwindButton(g, grid, prevRect, "Prev", () => grid.Render.GoToPreviousPage(grid), theme, false, false);
            x += 60 + SPACING;

            // Page numbers (simple 3-button layout)
            for (int page = startPage; page <= endPage; page++)
            {
                Rectangle pageRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                bool isActive = page == currentPage;

                int targetPage = page;
                grid.AddHitArea($"Page{page}", pageRect, null, () => grid.Render.GoToPage(grid, targetPage));

                // Flat design - filled for active, outline for inactive
                if (isActive)
                {
                    using (var brush = new SolidBrush(theme.AccentColor))
                    using (var path = CreateRoundedRectangle(pageRect, 6))
                    {
                        g.FillPath(brush, path);
                    }
                    using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                    {
                        DrawCenteredText(g, page.ToString(), font, theme.GridHeaderBackColor, pageRect);
                    }
                }
                else
                {
                    // Transparent with border
                    using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
                    using (var path = CreateRoundedRectangle(pageRect, 6))
                    {
                        g.DrawPath(pen, path);
                    }
                    using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                    {
                        DrawCenteredText(g, page.ToString(), font, theme.GridHeaderForeColor, pageRect);
                    }
                }

                x += BUTTON_WIDTH + SPACING;
            }

            // "Next" button
            Rectangle nextRect = new Rectangle(x, y, 60, BUTTON_HEIGHT);
            PaintTailwindButton(g, grid, nextRect, "Next", () => grid.Render.GoToNextPage(grid), theme, false, false);
        }

        private void PaintTailwindButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string text, Action action, IBeepTheme theme, bool isPrimary, bool hasLabel)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(text, bounds, null, action);

            // Flat Tailwind style
            if (isPrimary)
            {
                // Filled button
                using (var brush = new SolidBrush(theme.AccentColor))
                using (var path = CreateRoundedRectangle(bounds, 6))
                {
                    g.FillPath(brush, path);
                }

                using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
                {
                    DrawCenteredText(g, text, font, theme.GridHeaderBackColor, bounds);
                }
            }
            else
            {
                // Outline button
                using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
                using (var path = CreateRoundedRectangle(bounds, 6))
                {
                    g.DrawPath(pen, path);
                }

                using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
                {
                    DrawCenteredText(g, text, font, theme.GridHeaderForeColor, bounds);
                }
            }
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintTailwindButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Not used in Tailwind style (pagination handles display)
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = false,
                ButtonSize = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_HEIGHT) / 2;

            // Left section - CRUD buttons with labels
            int x = availableBounds.X + 16;
            layout.AddNewButtonRect = new Rectangle(x, y, 80, BUTTON_HEIGHT);
            x += 80 + SPACING + 4;

            layout.DeleteButtonRect = new Rectangle(x, y, 70, BUTTON_HEIGHT);
            x += 70 + SPACING;

            layout.SaveButtonRect = new Rectangle(x, y, 60, BUTTON_HEIGHT);

            // Right section - Simple pagination (Prev + pages + Next)
            int paginationWidth = 60 + SPACING + BUTTON_WIDTH * 3 + SPACING * 2 + 60; // Prev + 3 pages + Next
            x = availableBounds.Right - 16 - paginationWidth;
            layout.FirstButtonRect = new Rectangle(x, y, paginationWidth, BUTTON_HEIGHT);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
