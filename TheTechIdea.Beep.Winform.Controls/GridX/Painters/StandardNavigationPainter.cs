using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Classic Windows Forms navigation Style with traditional button appearance
    /// </summary>
    public class StandardNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Standard;
        public override int RecommendedHeight => 40;
        public override int RecommendedMinWidth => 500;

        private const int BUTTON_WIDTH = 75;
        private const int BUTTON_HEIGHT = 28;
        private const int SPACING = 6;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Draw classic Windows background using theme
            using (var brush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw border
            ControlPaint.DrawBorder3D(g, bounds, Border3DStyle.Sunken);

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Paint navigation buttons (left)
            PaintStandardButton(g, grid, layout.FirstButtonRect, "First", () => grid.MoveFirst(), theme);
            PaintStandardButton(g, grid, layout.PreviousButtonRect, "Previous", () => grid.MovePrevious(), theme);
            PaintStandardButton(g, grid, layout.NextButtonRect, "Next", () => grid.MoveNext(), theme);
            PaintStandardButton(g, grid, layout.LastButtonRect, "Last", () => grid.MoveLast(), theme);

            // Paint position indicator (center)
            PaintPositionIndicator(g, layout.PositionIndicatorRect, 
                grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);

            // Paint CRUD buttons (right)
            PaintStandardButton(g, grid, layout.AddNewButtonRect, "New", () => grid.InsertNew(), theme);
            PaintStandardButton(g, grid, layout.DeleteButtonRect, "Delete", () => grid.DeleteCurrent(), theme);
            PaintStandardButton(g, grid, layout.SaveButtonRect, "Save", () => grid.Save(), theme);
        }

        private void PaintStandardButton(Graphics g, BeepGridPro grid, Rectangle bounds, 
            string text, Action action, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            grid.AddHitArea(text, bounds, null, action);

            // Draw classic 3D button
            ButtonState state = ButtonState.Normal;
            ControlPaint.DrawButton(g, bounds, state);

            // Draw text
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
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            ButtonState btnState = state switch
            {
                NavigationButtonState.Pressed => ButtonState.Pushed,
                NavigationButtonState.Disabled => ButtonState.Inactive,
                _ => ButtonState.Normal
            };

            ControlPaint.DrawButton(g, bounds, btnState);
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            string text = $"Record {currentPosition} of {totalRecords}";
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

            int x = availableBounds.X + 12;
            int y = availableBounds.Y + (availableBounds.Height - BUTTON_HEIGHT) / 2;

            // Navigation buttons (left)
            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING + 12;

            layout.NextButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            x += BUTTON_WIDTH + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);

            // Position indicator (center)
            int indicatorWidth = 150;
            int centerX = availableBounds.X + (availableBounds.Width - indicatorWidth) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX, y, indicatorWidth, BUTTON_HEIGHT);

            // CRUD buttons (right)
            x = availableBounds.Right - 12 - BUTTON_WIDTH;
            layout.SaveButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            
            x -= BUTTON_WIDTH + SPACING;
            layout.DeleteButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            
            x -= BUTTON_WIDTH + SPACING;
            layout.AddNewButtonRect = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }
    }
}
