using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Material Design inspired navigation painter with flat design and accent colors
    /// </summary>
    public class MaterialNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Material;
        public override int RecommendedHeight => 56;
        public override int RecommendedMinWidth => 400;

        private const int BUTTON_SIZE = 40;
        private const int ICON_BUTTON_SIZE = 36;
        private const int SPACING = 8;
        private const int RIPPLE_RADIUS = 20;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Clear existing navigator hit tests
            grid.ClearHitList();

            // Draw background using theme color
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Draw bottom border with theme-aware color
            Color borderColor = ControlPaint.Dark(theme.GridHeaderBackColor, 0.1f);
            using (var borderPen = new Pen(borderColor, 1))
            {
                g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Paint navigation buttons (left side) with hit areas
            PaintButtonWithHitArea(g, grid, layout.FirstButtonRect, NavigationButtonType.First, 
                "First", () => grid.MoveFirst(), theme);
            PaintButtonWithHitArea(g, grid, layout.PreviousButtonRect, NavigationButtonType.Previous, 
                "Prev", () => grid.MovePrevious(), theme);
            PaintButtonWithHitArea(g, grid, layout.NextButtonRect, NavigationButtonType.Next, 
                "Next", () => grid.MoveNext(), theme);
            PaintButtonWithHitArea(g, grid, layout.LastButtonRect, NavigationButtonType.Last, 
                "Last", () => grid.MoveLast(), theme);

            // Paint position indicator (center)
            PaintPositionIndicator(g, layout.PositionIndicatorRect, 
                grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);

            // Paint CRUD buttons (right side) with hit areas
            PaintButtonWithHitArea(g, grid, layout.AddNewButtonRect, NavigationButtonType.AddNew, 
                "Insert", () => grid.InsertNew(), theme);
            PaintButtonWithHitArea(g, grid, layout.DeleteButtonRect, NavigationButtonType.Delete, 
                "Delete", () => grid.DeleteCurrent(), theme);
            PaintButtonWithHitArea(g, grid, layout.SaveButtonRect, NavigationButtonType.Save, 
                "Save", () => grid.Save(), theme);
        }

        private void PaintButtonWithHitArea(Graphics g, BeepGridPro grid, Rectangle bounds, 
            NavigationButtonType buttonType, string hitAreaName, Action action, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            // Register hit area for click handling
            grid.AddHitArea(hitAreaName, bounds, null, action);

            // Paint the button
            PaintButton(g, bounds, buttonType, NavigationButtonState.Normal, null, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            // Use theme-aware colors
            Color iconColor = state == NavigationButtonState.Disabled 
                ? ControlPaint.Dark(theme.GridHeaderForeColor, 0.5f)
                : theme.GridHeaderForeColor;

            if (state == NavigationButtonState.Hovered)
            {
                // Draw circular hover effect
                Color hoverColor = ControlPaint.Light(theme.ButtonHoverBackColor, 0.3f);
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, hoverColor)))
                {
                    g.FillEllipse(hoverBrush, bounds);
                }
            }
            else if (state == NavigationButtonState.Pressed)
            {
                // Draw circular pressed effect
                Color pressColor = theme.ButtonSelectedBackColor;
                using (var pressBrush = new SolidBrush(Color.FromArgb(50, pressColor)))
                {
                    g.FillEllipse(pressBrush, bounds);
                }
            }

            // Draw icon
            string icon = GetMaterialIcon(buttonType);
            using (var font = new Font("Segoe UI", 16, FontStyle.Regular))
            using (var brush = new SolidBrush(iconColor))
            using (var format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(icon, font, brush, bounds, format);
            }
        }

        public override void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            string text = $"{currentPosition} of {totalRecords}";
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

        public override NavigationLayout CalculateLayout(Rectangle availableBounds, int totalRecords, 
            bool showCrudButtons)
        {
            var layout = new NavigationLayout
            {
                IsCompact = false,
                IconOnly = true,
                ButtonSize = new Size(ICON_BUTTON_SIZE, ICON_BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int x = availableBounds.X + 16;
            int y = availableBounds.Y + (availableBounds.Height - ICON_BUTTON_SIZE) / 2;

            // Navigation buttons (left)
            layout.FirstButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);
            x += ICON_BUTTON_SIZE + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);
            x += ICON_BUTTON_SIZE + SPACING + 8;

            layout.NextButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);
            x += ICON_BUTTON_SIZE + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);

            // Position indicator (center)
            int indicatorWidth = 120;
            int centerX = availableBounds.X + (availableBounds.Width - indicatorWidth) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX, y, indicatorWidth, ICON_BUTTON_SIZE);

            // CRUD buttons (right)
            x = availableBounds.Right - 16 - ICON_BUTTON_SIZE;
            layout.SaveButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);
            
            x -= ICON_BUTTON_SIZE + SPACING;
            layout.DeleteButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);
            
            x -= ICON_BUTTON_SIZE + SPACING;
            layout.AddNewButtonRect = new Rectangle(x, y, ICON_BUTTON_SIZE, ICON_BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }

        private string GetMaterialIcon(NavigationButtonType buttonType)
        {
            return buttonType switch
            {
                NavigationButtonType.First => "⏮",
                NavigationButtonType.Previous => "◀",
                NavigationButtonType.Next => "▶",
                NavigationButtonType.Last => "⏭",
                NavigationButtonType.AddNew => "+",
                NavigationButtonType.Delete => "✕",
                NavigationButtonType.Save => "✓",
                NavigationButtonType.Cancel => "✖",
                _ => "?"
            };
        }
    }
}
