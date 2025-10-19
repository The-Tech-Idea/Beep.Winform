using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Card-based modern navigation with visual sections
    /// </summary>
    public class CardNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Card;
        public override int RecommendedHeight => 60;
        public override int RecommendedMinWidth => 700;

        private const int CARD_MARGIN = 8;
        private const int BUTTON_SIZE = 36;
        private const int SPACING = 8;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Light gray background
            using (var brush = new SolidBrush(ControlPaint.Light(theme.GridHeaderBackColor, 0.05f)))
            {
                g.FillRectangle(brush, bounds);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Draw 3 card sections
            PaintNavigationCard(g, grid, layout, theme);
            PaintInfoCard(g, grid, layout, theme);
            PaintActionsCard(g, grid, layout, theme);
        }

        private void PaintNavigationCard(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            Rectangle cardBounds = new Rectangle(
                layout.FirstButtonRect.X - 12,
                layout.FirstButtonRect.Y - 8,
                (BUTTON_SIZE + SPACING) * 4 + 16,
                BUTTON_SIZE + 16
            );

            PaintCard(g, cardBounds, theme);

            // Navigation buttons
            PaintCardButton(g, grid, layout.FirstButtonRect, "⏮", () => grid.MoveFirst(), theme, false);
            PaintCardButton(g, grid, layout.PreviousButtonRect, "◀", () => grid.MovePrevious(), theme, false);
            PaintCardButton(g, grid, layout.NextButtonRect, "▶", () => grid.MoveNext(), theme, false);
            PaintCardButton(g, grid, layout.LastButtonRect, "⏭", () => grid.MoveLast(), theme, false);
        }

        private void PaintInfoCard(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            Rectangle cardBounds = new Rectangle(
                layout.PositionIndicatorRect.X - 12,
                layout.PositionIndicatorRect.Y - 8,
                layout.PositionIndicatorRect.Width + 24,
                BUTTON_SIZE + 16
            );

            PaintCard(g, cardBounds, theme);

            // Info display
            int current = grid.Selection.RowIndex + 1;
            int total = grid.Data.Rows.Count;

            // Large current page number
            Rectangle currentRect = new Rectangle(
                layout.PositionIndicatorRect.X,
                layout.PositionIndicatorRect.Y,
                layout.PositionIndicatorRect.Width / 2,
                BUTTON_SIZE
            );
            using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
            {
                DrawCenteredText(g, current.ToString(), font, theme.AccentColor, currentRect);
            }

            // Small "of total"
            Rectangle totalRect = new Rectangle(
                layout.PositionIndicatorRect.X + layout.PositionIndicatorRect.Width / 2,
                layout.PositionIndicatorRect.Y,
                layout.PositionIndicatorRect.Width / 2,
                BUTTON_SIZE
            );
            using (var font = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                DrawCenteredText(g, $"of {total}", font, ControlPaint.Dark(theme.GridHeaderForeColor, 0.2f), totalRect);
            }
        }

        private void PaintActionsCard(Graphics g, BeepGridPro grid, NavigationLayout layout, IBeepTheme theme)
        {
            Rectangle cardBounds = new Rectangle(
                layout.AddNewButtonRect.X - 12,
                layout.AddNewButtonRect.Y - 8,
                (BUTTON_SIZE + SPACING) * 3 + 16,
                BUTTON_SIZE + 16
            );

            PaintCard(g, cardBounds, theme);

            // CRUD buttons
            PaintCardButton(g, grid, layout.AddNewButtonRect, "+", () => grid.InsertNew(), theme, true);
            PaintCardButton(g, grid, layout.DeleteButtonRect, "✕", () => grid.DeleteCurrent(), theme, false);
            PaintCardButton(g, grid, layout.SaveButtonRect, "✓", () => grid.Save(), theme, true);
        }

        private void PaintCard(Graphics g, Rectangle bounds, IBeepTheme theme)
        {
            // Shadow
            Rectangle shadowRect = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width, bounds.Height);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            using (var path = CreateRoundedRectangle(shadowRect, 8))
            {
                g.FillPath(shadowBrush, path);
            }

            // Card background
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            using (var path = CreateRoundedRectangle(bounds, 8))
            {
                g.FillPath(brush, path);
            }

            // Subtle border
            using (var pen = new Pen(ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f), 1))
            using (var path = CreateRoundedRectangle(bounds, 8))
            {
                g.DrawPath(pen, path);
            }
        }

        private void PaintCardButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string icon, Action action, IBeepTheme theme, bool isAccent)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(icon, bounds, null, action);

            // Circular button
            Rectangle circleBounds = new Rectangle(bounds.X, bounds.Y, BUTTON_SIZE, BUTTON_SIZE);
            
            Color bgColor = isAccent ? theme.AccentColor : ControlPaint.Light(theme.GridHeaderBackColor, 0.05f);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillEllipse(brush, circleBounds);
            }

            // Icon
            Color iconColor = isAccent ? theme.GridHeaderBackColor : theme.GridHeaderForeColor;
            using (var font = new Font("Segoe UI", 12, FontStyle.Regular))
            {
                DrawCenteredText(g, icon, font, iconColor, bounds);
            }
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            // Handled in PaintCardButton
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            // Handled in PaintInfoCard
        }

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, 
            int totalRecords, bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = true,
                ButtonSize = new Size(BUTTON_SIZE, BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Left card - Navigation
            int x = availableBounds.X + 20;
            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.NextButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            // Center card - Info
            int infoWidth = 140;
            int centerX = availableBounds.X + (availableBounds.Width - infoWidth) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX, y, infoWidth, BUTTON_SIZE);

            // Right card - Actions
            x = availableBounds.Right - 20 - BUTTON_SIZE;
            layout.SaveButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING;
            layout.DeleteButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING;
            layout.AddNewButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
