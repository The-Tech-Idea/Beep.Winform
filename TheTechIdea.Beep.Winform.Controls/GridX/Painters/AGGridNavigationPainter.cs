using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// AG Grid inspired modern navigation with dropdown and clean design
    /// Similar to "Show 10 | Prev 1 2 3 4 5 ... 90 Next | Per Page (10)"
    /// </summary>
    public class AGGridNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.AGGrid;
        public override int RecommendedHeight => 44;
        public override int RecommendedMinWidth => 600;

        private const int BUTTON_SIZE = 28;
        private const int SPACING = 6;
        private const int COMBO_WIDTH = 80;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Draw background using theme color directly
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Top border using theme border color
            using (var borderPen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawLine(borderPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, false);

            // Paint "Show X" text (left)
            PaintShowText(g, layout.LeftSectionRect, theme);

            // Paint navigation in center with hit areas
            PaintCenterNavigationWithHitAreas(g, grid, layout, grid.Selection.RowIndex + 1, 
                grid.Data.Rows.Count, theme);

            // Paint "Per Page" dropdown (right) with hit area
            grid.AddHitArea("PageSize", layout.PageSizeComboRect, null, null);
            PaintPageSizeCombo(g, layout.PageSizeComboRect, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            Color backColor = Color.Transparent;
            Color textColor = theme.GridHeaderForeColor;

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
                textColor = Color.FromArgb(128, theme.GridHeaderForeColor);
            }

            // Draw background (no border)
            if (backColor != Color.Transparent)
            {
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            // Draw icon/text
            string content = buttonType == NavigationButtonType.Previous ? "◀" : "▶";
            using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                DrawCenteredText(g, content, font, textColor, bounds);
            }
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintCenterNavigation
        }

        private void PaintShowText(Graphics g, Rectangle bounds, IBeepTheme theme)
        {
            string text = "Show 10";
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

        private void PaintCenterNavigationWithHitAreas(Graphics g, BeepGridPro grid, NavigationLayout layout, 
            int currentRecord, int totalRecords, IBeepTheme theme)
        {
            // Paint Prev button with hit area
            grid.AddHitArea("Prev", layout.PreviousButtonRect, null, () => grid.Render.GoToPreviousPage(grid));
            PaintButton(g, layout.PreviousButtonRect, NavigationButtonType.Previous, 
                NavigationButtonState.Normal, null, theme);

            // Get pagination info
            int currentPage = grid.Render.GetCurrentPage(grid);
            var (startPage, endPage) = grid.Render.GetVisiblePageRange(grid, 5);

            // Paint page numbers with hit areas
            int pageIndex = 0;
            for (int pageNum = startPage; pageNum <= endPage && pageIndex < layout.PageNumberRects.Length; pageNum++, pageIndex++)
            {
                var rect = layout.PageNumberRects[pageIndex];
                bool isActive = pageNum == currentPage;
                int targetPage = pageNum;
                
                grid.AddHitArea($"Page{pageNum}", rect, null, () => grid.Render.GoToPage(grid, targetPage));
                
                PaintPageNumber(g, rect, pageNum, isActive, theme);
            }

            // Paint Next button with hit area
            grid.AddHitArea("Next", layout.NextButtonRect, null, () => grid.Render.GoToNextPage(grid));
            PaintButton(g, layout.NextButtonRect, NavigationButtonType.Next, 
                NavigationButtonState.Normal, null, theme);
        }

        private void PaintPageNumber(Graphics g, Rectangle bounds, int pageNumber, 
            bool isActive, IBeepTheme theme)
        {
            Color textColor = isActive ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            Color backColor = isActive ? theme.AccentColor : Color.Transparent;

            if (backColor != Color.Transparent)
            {
                using (var brush = new SolidBrush(backColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }

            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                DrawCenteredText(g, pageNumber.ToString(), font, textColor, bounds);
            }
        }

        private void PaintPageSizeCombo(Graphics g, Rectangle bounds, IBeepTheme theme)
        {
            // Draw combo box background
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.FillRectangle(brush, bounds);
                g.DrawRectangle(pen, bounds);
            }

            // Draw text
            string text = "Per Page";
            using (var font = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var brush = new SolidBrush(theme.GridHeaderForeColor))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(text, font, brush, bounds, format);
            }

            // Draw dropdown arrow
            var arrowRect = new Rectangle(bounds.Right - 20, bounds.Y, 20, bounds.Height);
            using (var pen = new Pen(theme.GridHeaderForeColor, 2))
            {
                int cx = arrowRect.X + arrowRect.Width / 2;
                int cy = arrowRect.Y + arrowRect.Height / 2;
                g.DrawLine(pen, cx - 3, cy - 2, cx, cy + 2);
                g.DrawLine(pen, cx, cy + 2, cx + 3, cy - 2);
            }
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = true,
                IconOnly = false,
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight,
                VisiblePageButtons = 5
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Left section - "Show X"
            layout.LeftSectionRect = new Rectangle(
                availableBounds.X + 16, 
                y, 
                100, 
                BUTTON_SIZE);

            // Center section - pagination
            int centerX = availableBounds.X + (availableBounds.Width - (layout.VisiblePageButtons * BUTTON_SIZE + 2 * BUTTON_SIZE + SPACING * 6)) / 2;
            
            layout.PreviousButtonRect = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);
            centerX += BUTTON_SIZE + SPACING;

            layout.PageNumberRects = new Rectangle[layout.VisiblePageButtons];
            for (int i = 0; i < layout.VisiblePageButtons; i++)
            {
                layout.PageNumberRects[i] = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);
                centerX += BUTTON_SIZE + SPACING;
            }

            layout.NextButtonRect = new Rectangle(centerX, y, BUTTON_SIZE, BUTTON_SIZE);

            // Right section - page size combo
            layout.PageSizeComboRect = new Rectangle(
                availableBounds.Right - COMBO_WIDTH - 16, 
                y, 
                COMBO_WIDTH, 
                BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
