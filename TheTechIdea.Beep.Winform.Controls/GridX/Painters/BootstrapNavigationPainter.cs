using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Bootstrap-inspired navigation painter with numbered pagination
    /// Similar to the blue "Previous 1 2 3 4 ... 20 Next" style
    /// </summary>
    public class BootstrapNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Bootstrap;
        public override int RecommendedHeight => 48;
        public override int RecommendedMinWidth => 500;

        private const int BUTTON_HEIGHT = 32;
        private const int BUTTON_MIN_WIDTH = 38;
        private const int SPACING = 4;
        private const int BORDER_RADIUS = 4;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Draw background using theme
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            var layout = CalculateLayoutWithPagination(bounds, grid.Data.Rows.Count, 
                grid.Selection.RowIndex + 1);

            // Get pagination info dynamically
            int currentPage = grid.Render.GetCurrentPage(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 5);

            // Paint Previous button with hit area
            grid.AddHitArea("Prev", layout.PreviousButtonRect, null, () => grid.Render.GoToPreviousPage(grid));
            PaintButton(g, layout.PreviousButtonRect, NavigationButtonType.Previous, 
                NavigationButtonState.Normal, null, theme);

            // Paint page numbers with hit areas
            int pageIndex = 0;
            for (int pageNum = startPage; pageNum <= endPage && pageIndex < layout.PageNumberRects.Length; pageNum++, pageIndex++)
            {
                var rect = layout.PageNumberRects[pageIndex];
                bool isActive = pageNum == currentPage;
                int targetPage = pageNum;
                
                // Register hit area for page number
                grid.AddHitArea($"Page{pageNum}", rect, null, () => grid.Render.GoToPage(grid, targetPage));
                
                PaintPageNumberButton(g, rect, pageNum, isActive, theme);
            }

            // Paint Next button with hit area
            grid.AddHitArea("Next", layout.NextButtonRect, null, () => grid.Render.GoToNextPage(grid));
            PaintButton(g, layout.NextButtonRect, NavigationButtonType.Next, 
                NavigationButtonState.Normal, null, theme);

            // Paint record info (right side)
            PaintRecordInfo(g, layout.RecordCountRect, grid.Selection.RowIndex + 1, 
                grid.Data.Rows.Count, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            Color backColor = theme.GridHeaderBackColor;
            Color borderColor = ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f);
            Color textColor = theme.AccentColor;

            if (state == NavigationButtonState.Hovered)
            {
                backColor = theme.ButtonHoverBackColor;
            }
            else if (state == NavigationButtonState.Pressed)
            {
                backColor = theme.ButtonSelectedBackColor;
            }
            else if (state == NavigationButtonState.Disabled)
            {
                textColor = ControlPaint.Dark(theme.GridHeaderForeColor, 0.5f);
            }

            // Draw background with border
            using (var path = CreateRoundedRectangle(bounds, BORDER_RADIUS))
            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(borderColor, 1))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Draw text
            string text = buttonType == NavigationButtonType.Previous ? "Previous" : "Next";
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var brush = new SolidBrush(textColor))
            {
                DrawCenteredText(g, text, font, textColor, bounds);
            }
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Not used in Bootstrap style - uses PaintRecordInfo instead
        }

        private void PaintPageNumberButton(Graphics g, Rectangle bounds, int pageNumber, 
            bool isActive, IBeepTheme theme)
        {
            Color backColor = isActive ? theme.AccentColor : theme.GridHeaderBackColor;
            Color borderColor = ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f);
            Color textColor = isActive ? theme.GridHeaderBackColor : theme.AccentColor;

            // Draw background with border
            using (var path = CreateRoundedRectangle(bounds, BORDER_RADIUS))
            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(borderColor, 1))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Draw page number
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                DrawCenteredText(g, pageNumber.ToString(), font, textColor, bounds);
            }
        }

        private void PaintRecordInfo(Graphics g, Rectangle bounds, int current, int total, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            string text = $"Showing {current} to {Math.Min(current + 9, total)} of {total}";
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var brush = new SolidBrush(ControlPaint.Dark(theme.GridHeaderForeColor, 0.3f)))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, brush, bounds, format);
            }
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, int totalRecords, 
            bool showCrudButtons)
        {
            return CalculateLayoutWithPagination(availableBounds, totalRecords, 1);
        }

        private NavigationLayout CalculateLayoutWithPagination(Rectangle availableBounds, 
            int totalRecords, int currentRecord)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = false,
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            // Calculate pagination - values will be recalculated dynamically when painting
            layout.TotalPages = 1; // Placeholder
            layout.CurrentPage = 1; // Placeholder
            layout.VisiblePageButtons = 5; // Max visible buttons

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_HEIGHT) / 2;
            int x = availableBounds.X + 16;

            // Previous button
            layout.PreviousButtonRect = new Rectangle(x, y, 80, BUTTON_HEIGHT);
            x += 80 + SPACING;

            // Page number buttons
            layout.PageNumberRects = new Rectangle[layout.VisiblePageButtons];
            for (int i = 0; i < layout.VisiblePageButtons; i++)
            {
                layout.PageNumberRects[i] = new Rectangle(x, y, BUTTON_MIN_WIDTH, BUTTON_HEIGHT);
                x += BUTTON_MIN_WIDTH + SPACING;
            }

            // Next button
            layout.NextButtonRect = new Rectangle(x, y, 80, BUTTON_HEIGHT);

            // Record info (right side)
            int infoWidth = 200;
            layout.RecordCountRect = new Rectangle(
                availableBounds.Right - infoWidth - 16, 
                y, 
                infoWidth, 
                BUTTON_HEIGHT);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
