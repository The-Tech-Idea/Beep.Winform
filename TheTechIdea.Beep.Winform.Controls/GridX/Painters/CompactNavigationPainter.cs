using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Compact DevExpress-Style navigation with minimal height and icon buttons
    /// </summary>
    public class CompactNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Compact;
        public override int RecommendedHeight => 28;
        public override int RecommendedMinWidth => 350;

        private const int BUTTON_SIZE = 22;
        private const int SPACING = 3;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Compact background using theme color directly
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Thin border using theme border color
            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // All buttons with compact spacing
            PaintCompactButton(g, grid, layout.AddNewButtonRect, "+", () => grid.InsertNew(), theme);
            PaintCompactButton(g, grid, layout.DeleteButtonRect, "✕", () => grid.DeleteCurrent(), theme);
            
            PaintCompactButton(g, grid, layout.FirstButtonRect, "⏮", () => grid.MoveFirst(), theme);
            PaintCompactButton(g, grid, layout.PreviousButtonRect, "◀", () => grid.MovePrevious(), theme);
            
            // Compact counter
            PaintCompactCounter(g, layout.PositionIndicatorRect, 
                grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);
            
            PaintCompactButton(g, grid, layout.NextButtonRect, "▶", () => grid.MoveNext(), theme);
            PaintCompactButton(g, grid, layout.LastButtonRect, "⏭", () => grid.MoveLast(), theme);
            
            PaintCompactButton(g, grid, layout.SaveButtonRect, "✓", () => grid.Save(), theme);
        }

        private void PaintCompactButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string icon, Action action, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(icon, bounds, null, action);

            // Button background using theme color directly
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Thin border using theme border color
            using (var pen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawRectangle(pen, bounds);
            }

            // Draw icon
            using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                DrawCenteredText(g, icon, font, theme.GridHeaderForeColor, bounds);
            }
        }

        private void PaintCompactCounter(Graphics g, Rectangle bounds, int current, int total, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            string text = $"{current}/{total}";
            using (var font = new Font("Segoe UI", 8, FontStyle.Regular))
            using (var brush = new SolidBrush(theme.GridHeaderForeColor))
            {
                DrawCenteredText(g, text, font, theme.GridHeaderForeColor, bounds);
            }
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintCompactButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintCompactCounter
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = true,
                IconOnly = true,
                ButtonSize = new Size(BUTTON_SIZE, BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;
            
            // Center all controls
            int totalWidth = BUTTON_SIZE * 8 + SPACING * 7 + 50; // 8 buttons + counter
            int x = availableBounds.X + (availableBounds.Width - totalWidth) / 2;

            layout.AddNewButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.DeleteButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING + 4;

            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING + 2;

            layout.PositionIndicatorRect = new Rectangle(x, y, 50, BUTTON_SIZE);
            x += 50 + SPACING + 2;

            layout.NextButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING + 4;

            layout.SaveButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            layout.TotalWidth = totalWidth;

            return layout;
        }
    }
}
