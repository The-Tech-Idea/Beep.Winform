using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Telerik/Kendo UI inspired professional grid navigation.
    /// Uses theme colors directly — no ControlPaint.Dark/Light.
    /// </summary>
    public class TelerikNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Telerik;
        public override int RecommendedHeight => 44;
        public override int RecommendedMinWidth => 600;

        private const int BUTTON_HEIGHT = 30;
        private const int BUTTON_WIDTH = 32;
        private const int SPACING = 4;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Professional background using theme color directly
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Border using theme border color
            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Navigation buttons group (left)
            PaintTelerikButton(g, grid, layout.FirstButtonRect, "⏮", () => grid.Render.GoToFirstPage(grid), theme, false);
            PaintTelerikButton(g, grid, layout.PreviousButtonRect, "◀", () => grid.Render.GoToPreviousPage(grid), theme, false);
            PaintTelerikButton(g, grid, layout.NextButtonRect, "▶", () => grid.Render.GoToNextPage(grid), theme, false);
            PaintTelerikButton(g, grid, layout.LastButtonRect, "⏭", () => grid.Render.GoToLastPage(grid), theme, false);

            // Page info
            PaintTelerikPageInfo(g, layout.PositionIndicatorRect, grid, theme);

            // CRUD buttons group (right)
            PaintTelerikButton(g, grid, layout.AddNewButtonRect, "+", () => grid.InsertNew(), theme, true);
            PaintTelerikButton(g, grid, layout.DeleteButtonRect, "✕", () => grid.DeleteCurrent(), theme, false);
            PaintTelerikButton(g, grid, layout.SaveButtonRect, "✓", () => grid.Save(), theme, true);
        }

        private void PaintTelerikButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string icon, Action action, IBeepTheme theme, bool isPrimary)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(icon, bounds, null, action);

            // Button background using theme colors directly
            Color bgColor = isPrimary ? theme.AccentColor : theme.GridHeaderBackColor;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Border using theme colors
            Color borderColor = isPrimary ? theme.AccentColor : theme.GridHeaderBorderColor;
            using (var pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Subtle inner highlight (using semi-transparent white — not ControlPaint)
            Rectangle highlightRect = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height / 2);
            using (var brush = new LinearGradientBrush(highlightRect, 
                Color.FromArgb(40, Color.White), Color.FromArgb(0, Color.White), 90f))
            {
                g.FillRectangle(brush, highlightRect);
            }

            // Icon using theme colors
            Color iconColor = isPrimary ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            using (var font = new Font("Segoe UI", 11, FontStyle.Regular))
            {
                DrawCenteredText(g, icon, font, iconColor, bounds);
            }
        }

        private void PaintTelerikPageInfo(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            if (bounds.IsEmpty || grid?.Data?.Rows == null) return;

            // Professional info display using theme color directly
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Use grid's pagination methods
            int currentPage = grid.Render.GetCurrentPage(grid);
            int totalPages = grid.Render.GetTotalPages(grid);
            int current = grid.Selection.RowIndex + 1;
            int total = grid.Data.Rows.Count;
            
            // Show both page and record info in one line
            string text = $"Page {currentPage} of {totalPages}  •  {current} / {total} records";
            using (var font = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                DrawCenteredText(g, text, font, theme.GridHeaderForeColor, bounds);
            }
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintTelerikButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintTelerikPageInfo
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = true,
                ButtonSize = new Size(BUTTON_WIDTH, BUTTON_HEIGHT),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_HEIGHT) / 2;

            // Navigation group (left)
            int x = availableBounds.X + 12;
            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING;

            layout.NextButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);

            // Page info (center)
            int infoWidth = 180;
            int centerX = availableBounds.X + (availableBounds.Width - infoWidth) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX, y, infoWidth, BUTTON_HEIGHT);

            // CRUD group (right)
            x = availableBounds.Right - 12 - BUTTON_WIDTH;
            layout.SaveButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            
            x -= BUTTON_WIDTH + SPACING;
            layout.DeleteButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            
            x -= BUTTON_WIDTH + SPACING + 8;
            layout.AddNewButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
