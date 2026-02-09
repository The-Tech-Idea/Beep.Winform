using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Material Design inspired navigation painter with flat design and accent colors.
    /// Uses theme colors directly — no ControlPaint.Dark/Light.
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

        // Cached fonts to avoid allocations in paint
        private static Font _iconFont;
        private static Font _textFont;
        private static Font IconFont => _iconFont ??= SafeCreateFont("Segoe UI Symbol", 14f, FontStyle.Regular);
        private static Font TextFont => _textFont ??= SafeCreateFont("Segoe UI", 9f, FontStyle.Regular);

        private static Font SafeCreateFont(string familyName, float size, FontStyle style)
        {
            try
            {
                var font = BeepFontManager.GetCachedFont(familyName, size, style);
                if (font != null) return font;
            }
            catch { /* ignore */ }

            try { return new Font(familyName, size, style); } catch { }
            try { return new Font("Segoe UI", size, style); } catch { }
            try { return new Font(FontFamily.GenericSansSerif, size, style); } catch { }
            try { return new Font("Arial", size, style); } catch { }
            return SystemFonts.DefaultFont;
        }

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Draw background using theme color directly
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Draw bottom border using theme border color
            using (var borderPen = new Pen(theme.GridHeaderBorderColor, 1))
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
            int rowIndex = grid.Selection?.RowIndex ?? 0;
            int rowCount = grid.Data?.Rows?.Count ?? 0;
            PaintPositionIndicator(g, layout.PositionIndicatorRect, rowIndex + 1, rowCount, theme);

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

            grid.AddHitArea(hitAreaName, bounds, null, action);
            PaintButton(g, bounds, buttonType, NavigationButtonState.Normal, null, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0) return;

            // Use theme colors directly for each state
            Color iconColor = theme.GridHeaderForeColor;

            if (state == NavigationButtonState.Disabled)
            {
                iconColor = Color.FromArgb(128, theme.GridHeaderForeColor);
            }
            else if (state == NavigationButtonState.Hovered)
            {
                // Draw circular hover effect using theme hover color
                using (var hoverBrush = new SolidBrush(Color.FromArgb(30, theme.GridHeaderHoverBackColor)))
                {
                    g.FillEllipse(hoverBrush, bounds);
                }
            }
            else if (state == NavigationButtonState.Pressed)
            {
                // Draw circular pressed effect using theme selected color
                using (var pressBrush = new SolidBrush(Color.FromArgb(50, theme.GridHeaderSelectedBackColor)))
                {
                    g.FillEllipse(pressBrush, bounds);
                }
            }

            // Draw icon using cached font
            string icon = GetMaterialIcon(buttonType);
            var font = IconFont;
            if (font == null) return;

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
            if (bounds.IsEmpty || bounds.Width <= 0 || bounds.Height <= 0) return;

            string text = $"{currentPosition} of {totalRecords}";
            var font = TextFont;
            if (font == null) return;

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
                NavigationButtonType.First => "|◀",
                NavigationButtonType.Previous => "◀",
                NavigationButtonType.Next => "▶",
                NavigationButtonType.Last => "▶|",
                NavigationButtonType.AddNew => "+",
                NavigationButtonType.Delete => "✕",
                NavigationButtonType.Save => "✓",
                NavigationButtonType.Cancel => "✖",
                _ => "?"
            };
        }
    }
}
