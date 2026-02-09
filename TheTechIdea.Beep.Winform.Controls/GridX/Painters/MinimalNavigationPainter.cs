using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Minimal compact navigation with simple arrows and text
    /// Very space-efficient design like "< 1 2 3 >"
    /// </summary>
    public class MinimalNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Minimal;
        public override int RecommendedHeight => 32;
        public override int RecommendedMinWidth => 250;

        private const int BUTTON_SIZE = 24;
        private const int SPACING = 2;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Minimal background using theme color directly
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, false);

            // Calculate current page using dynamic page size
            int currentPage = grid.Render.GetCurrentPage(grid);
            int totalPages = grid.Render.GetTotalPages(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 5);

            // Paint Previous with hit area
            grid.AddHitArea("Prev", layout.PreviousButtonRect, null, () => grid.Render.GoToPreviousPage(grid));
            PaintButton(g, layout.PreviousButtonRect, NavigationButtonType.Previous, 
                NavigationButtonState.Normal, null, theme);

            // Paint page numbers (max 5) with hit areas
            int pageIndex = 0;
            for (int page = startPage; page <= endPage && pageIndex < layout.PageNumberRects.Length; page++, pageIndex++)
            {
                bool isActive = page == currentPage;
                int targetPage = page;
                
                grid.AddHitArea($"Page{page}", layout.PageNumberRects[pageIndex], null, () => {
                    grid.Render.GoToPage(grid, targetPage);
                });
                
                PaintPageNumber(g, layout.PageNumberRects[pageIndex], page, isActive, theme);
            }

            // Paint Next with hit area
            grid.AddHitArea("Next", layout.NextButtonRect, null, () => grid.Render.GoToNextPage(grid));
            PaintButton(g, layout.NextButtonRect, NavigationButtonType.Next, 
                NavigationButtonState.Normal, null, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            Color textColor = theme.GridHeaderForeColor;

            if (state == NavigationButtonState.Hovered)
            {
                textColor = theme.AccentColor;
            }
            else if (state == NavigationButtonState.Disabled)
            {
                textColor = Color.FromArgb(128, theme.GridHeaderForeColor);
            }

            // Simple text, no background
            string content = buttonType == NavigationButtonType.Previous ? "<" : ">";
            using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                DrawCenteredText(g, content, font, textColor, bounds);
            }
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Not used in minimal Style
        }

        private void PaintPageNumber(Graphics g, Rectangle bounds, int pageNumber, 
            bool isActive, IBeepTheme theme)
        {
            Color textColor = isActive ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            Color backColor = isActive ? theme.AccentColor : Color.Transparent;

            if (backColor != Color.Transparent)
            {
                // Small circle for active page
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillEllipse(brush, bounds);
                }
            }

            using (var font = new Font("Segoe UI", 9, isActive ? FontStyle.Bold : FontStyle.Regular))
            {
                DrawCenteredText(g, pageNumber.ToString(), font, textColor, bounds);
            }
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = true,
                IconOnly = true,
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight,
                VisiblePageButtons = 5
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Center everything
            int totalWidth = BUTTON_SIZE + SPACING + (layout.VisiblePageButtons * (BUTTON_SIZE + SPACING)) + BUTTON_SIZE;
            int centerX = availableBounds.X + (availableBounds.Width - totalWidth) / 2;

            // Previous
            layout.PreviousButtonRect = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);
            centerX += BUTTON_SIZE + SPACING;

            // Page numbers
            layout.PageNumberRects = new Rectangle[layout.VisiblePageButtons];
            for (int i = 0; i < layout.VisiblePageButtons; i++)
            {
                layout.PageNumberRects[i] = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);
                centerX += BUTTON_SIZE + SPACING;
            }

            // Next
            layout.NextButtonRect = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);

            layout.TotalWidth = totalWidth;

            return layout;
        }
    }
}
