using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Microsoft Fluent Design navigation with modern aesthetics.
    /// Uses theme colors directly — no ControlPaint.Dark/Light.
    /// </summary>
    public class FluentNavigationPainter : BaseNavigationPainter
    {
        public override navigationStyle Style => navigationStyle.Fluent;
        public override int RecommendedHeight => 52;
        public override int RecommendedMinWidth => 450;

        private const int BUTTON_SIZE = 36;
        private const int SPACING = 8;
        private const int BORDER_RADIUS = 4;

        public override void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            grid.ClearHitList();

            // Draw flat background using theme color directly
            using (var bgBrush = new SolidBrush(theme.GridHeaderBackColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // Subtle top and bottom borders using theme border color
            using (var borderPen = new Pen(theme.GridHeaderBorderColor, 1))
            {
                g.DrawLine(borderPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                g.DrawLine(borderPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
            }

            var layout = CalculateLayout(bounds, grid.Data.Rows.Count, true);

            // Paint navigation buttons (left) with hit areas
            grid.AddHitArea("First", layout.FirstButtonRect, null, () => grid.MoveFirst());
            PaintButton(g, layout.FirstButtonRect, NavigationButtonType.First, 
                NavigationButtonState.Normal, null, theme);
            
            grid.AddHitArea("Prev", layout.PreviousButtonRect, null, () => grid.MovePrevious());
            PaintButton(g, layout.PreviousButtonRect, NavigationButtonType.Previous, 
                NavigationButtonState.Normal, null, theme);
            
            grid.AddHitArea("Next", layout.NextButtonRect, null, () => grid.MoveNext());
            PaintButton(g, layout.NextButtonRect, NavigationButtonType.Next, 
                NavigationButtonState.Normal, null, theme);
            
            grid.AddHitArea("Last", layout.LastButtonRect, null, () => grid.MoveLast());
            PaintButton(g, layout.LastButtonRect, NavigationButtonType.Last, 
                NavigationButtonState.Normal, null, theme);

            // Paint position indicator (center)
            PaintPositionIndicator(g, layout.PositionIndicatorRect, 
                grid.Selection.RowIndex + 1, grid.Data.Rows.Count, theme);

            // Paint CRUD buttons (right) with Fluent accent and hit areas
            grid.AddHitArea("Insert", layout.AddNewButtonRect, null, () => grid.InsertNew());
            PaintFluentButton(g, layout.AddNewButtonRect, NavigationButtonType.AddNew, 
                NavigationButtonState.Normal, true, theme);
            
            grid.AddHitArea("Delete", layout.DeleteButtonRect, null, () => grid.DeleteCurrent());
            PaintButton(g, layout.DeleteButtonRect, NavigationButtonType.Delete, 
                NavigationButtonState.Normal, null, theme);
            
            grid.AddHitArea("Save", layout.SaveButtonRect, null, () => grid.Save());
            PaintFluentButton(g, layout.SaveButtonRect, NavigationButtonType.Save, 
                NavigationButtonState.Normal, false, theme);
        }

        public override void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            // Use theme colors directly for each state
            Color backColor = theme.GridHeaderBackColor;
            Color borderColor = theme.GridHeaderBorderColor;
            Color iconColor = theme.GridHeaderForeColor;

            if (state == NavigationButtonState.Hovered)
            {
                backColor = theme.ButtonHoverBackColor;
                borderColor = theme.GridHeaderHoverBorderColor;
            }
            else if (state == NavigationButtonState.Pressed)
            {
                backColor = theme.ButtonSelectedBackColor;
                borderColor = theme.GridHeaderSelectedBorderColor;
            }
            else if (state == NavigationButtonState.Disabled)
            {
                iconColor = Color.FromArgb(128, theme.GridHeaderForeColor);
            }

            // Draw button with subtle shadow
            var shadowBounds = new Rectangle(bounds.X, bounds.Y + 1, bounds.Width, bounds.Height);
            using (var shadowPath = CreateRoundedRectangle(shadowBounds, BORDER_RADIUS))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Draw button
            using (var path = CreateRoundedRectangle(bounds, BORDER_RADIUS))
            using (var brush = new SolidBrush(backColor))
            using (var pen = new Pen(borderColor, 1))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // Draw icon
            string icon = GetFluentIcon(buttonType);
            using (var font = new Font("Segoe UI", 11, FontStyle.Regular))
            {
                DrawCenteredText(g, icon, font, iconColor, bounds);
            }
        }

        private void PaintFluentButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, bool isPrimary, IBeepTheme theme)
        {
            if (bounds.IsEmpty) return;

            Color backColor = isPrimary 
                ? theme.AccentColor
                : theme.ButtonBackColor;
            Color iconColor = isPrimary ? theme.GridHeaderBackColor : theme.ButtonForeColor;

            if (state == NavigationButtonState.Hovered)
            {
                backColor = isPrimary 
                    ? theme.ButtonHoverBackColor
                    : theme.ButtonHoverBackColor;
            }
            else if (state == NavigationButtonState.Pressed)
            {
                backColor = isPrimary 
                    ? theme.ButtonSelectedBackColor
                    : theme.ButtonSelectedBackColor;
            }

            // Draw button with shadow
            var shadowBounds = new Rectangle(bounds.X, bounds.Y + 2, bounds.Width, bounds.Height);
            using (var shadowPath = CreateRoundedRectangle(shadowBounds, BORDER_RADIUS))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            using (var path = CreateRoundedRectangle(bounds, BORDER_RADIUS))
            using (var brush = new SolidBrush(backColor))
            {
                g.FillPath(brush, path);
            }

            // Draw icon
            string icon = GetFluentIcon(buttonType);
            using (var font = new Font("Segoe UI", 11, FontStyle.Regular))
            {
                DrawCenteredText(g, icon, font, iconColor, bounds);
            }
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
                IconOnly = true,
                ButtonSize = new Size(BUTTON_SIZE, BUTTON_SIZE),
                ButtonSpacing = SPACING,
                TotalHeight = RecommendedHeight
            };

            int x = availableBounds.X + 16;
            int y = availableBounds.Y + (availableBounds.Height - BUTTON_SIZE) / 2;

            // Navigation buttons (left)
            layout.FirstButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.PreviousButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING + 12;

            layout.NextButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            x += BUTTON_SIZE + SPACING;

            layout.LastButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            // Position indicator (center)
            int indicatorWidth = 140;
            int centerX = availableBounds.X + (availableBounds.Width - indicatorWidth) / 2;
            layout.PositionIndicatorRect = new Rectangle(centerX, y, indicatorWidth, BUTTON_SIZE);

            // CRUD buttons (right)
            x = availableBounds.Right - 16 - BUTTON_SIZE;
            layout.SaveButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING;
            layout.DeleteButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);
            
            x -= BUTTON_SIZE + SPACING + 8;
            layout.AddNewButtonRect = new Rectangle(x, y, BUTTON_SIZE, BUTTON_SIZE);

            layout.TotalWidth = availableBounds.Width;

            return layout;
        }

        private string GetFluentIcon(NavigationButtonType buttonType)
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
