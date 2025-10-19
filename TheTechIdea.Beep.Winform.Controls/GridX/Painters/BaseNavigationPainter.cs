using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Base class for navigation painters with common functionality
    /// </summary>
    public abstract class BaseNavigationPainter : INavigationPainter
    {
        public abstract navigationStyle Style { get; }
        public abstract int RecommendedHeight { get; }
        public abstract int RecommendedMinWidth { get; }

        public abstract void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid,IBeepTheme theme);
        public abstract void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme);
        public abstract void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme);
        public abstract NavigationLayout CalculateLayout(Rectangle availableBounds, int totalRecords, 
            bool showCrudButtons);

        #region Common Helper Methods

        /// <summary>
        /// Get standard button content (icons/text)
        /// </summary>
        public virtual string GetButtonContent(NavigationButtonType buttonType)
        {
            return buttonType switch
            {
                NavigationButtonType.First => "⏮",      // First
                NavigationButtonType.Previous => "◀",   // Previous
                NavigationButtonType.Next => "▶",       // Next
                NavigationButtonType.Last => "⏭",       // Last
                NavigationButtonType.AddNew => "+",     // Add
                NavigationButtonType.Delete => "✕",     // Delete
                NavigationButtonType.Save => "✓",       // Save
                NavigationButtonType.Cancel => "✖",     // Cancel
                _ => "?"
            };
        }

        /// <summary>
        /// Get standard tooltips
        /// </summary>
        public virtual string GetButtonTooltip(NavigationButtonType buttonType)
        {
            return buttonType switch
            {
                NavigationButtonType.First => "Go to first record",
                NavigationButtonType.Previous => "Go to previous record",
                NavigationButtonType.Next => "Go to next record",
                NavigationButtonType.Last => "Go to last record",
                NavigationButtonType.AddNew => "Add new record",
                NavigationButtonType.Delete => "Delete current record",
                NavigationButtonType.Save => "Save changes",
                NavigationButtonType.Cancel => "Cancel changes",
                _ => ""
            };
        }

        /// <summary>
        /// Create a rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0 || bounds.Width < radius * 2 || bounds.Height < radius * 2)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Create a rounded rectangle path with selective corners
        /// </summary>
        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius,
            bool topLeft, bool topRight, bool bottomLeft, bool bottomRight)
        {
            var path = new GraphicsPath();

            if (radius <= 0 || bounds.Width < radius * 2 || bounds.Height < radius * 2)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top-left corner
            if (topLeft)
                path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            else
                path.AddLine(bounds.X, bounds.Y, bounds.X, bounds.Y);

            // Top-right corner
            if (topRight)
                path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            else
                path.AddLine(bounds.Right, bounds.Y, bounds.Right, bounds.Y);

            // Bottom-right corner
            if (bottomRight)
                path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            else
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Right, bounds.Bottom);

            // Bottom-left corner
            if (bottomLeft)
                path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            else
                path.AddLine(bounds.X, bounds.Bottom, bounds.X, bounds.Bottom);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Get button color based on state
        /// </summary>
        protected Color GetButtonColor(NavigationButtonState state, IBeepTheme theme)
        {
            return state switch
            {
                NavigationButtonState.Pressed => ControlPaint.Dark(theme.ButtonBackColor, 0.2f),
                NavigationButtonState.Hovered => ControlPaint.Light(theme.ButtonBackColor, 0.1f),
                NavigationButtonState.Disabled => Color.FromArgb(230, 230, 230),
                _ => theme.ButtonBackColor
            };
        }

        /// <summary>
        /// Get text color based on state
        /// </summary>
        protected Color GetTextColor(NavigationButtonState state, IBeepTheme theme)
        {
            return state switch
            {
                NavigationButtonState.Disabled => Color.FromArgb(150, 150, 150),
                _ => theme.ButtonForeColor
            };
        }

        /// <summary>
        /// Draw centered text in a rectangle
        /// </summary>
        protected void DrawCenteredText(Graphics g, string text, Font font, Color color, Rectangle bounds)
        {
            // Validate parameters
            if (g == null || string.IsNullOrEmpty(text) || font == null || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            try
            {
                using (var brush = new SolidBrush(color))
                using (var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    g.DrawString(text, font, brush, bounds, format);
                }
            }
            catch (ArgumentException)
            {
                // Font or graphics parameter invalid - silently ignore
                // This can happen if font is disposed or graphics state is invalid
            }
        }

        /// <summary>
        /// Draw button shadow for depth effect
        /// </summary>
        protected void DrawButtonShadow(Graphics g, Rectangle bounds, Color shadowColor)
        {
            var shadowBounds = new Rectangle(bounds.X + 1, bounds.Y + 2, bounds.Width, bounds.Height);
            using (var brush = new SolidBrush(Color.FromArgb(30, shadowColor)))
            {
                g.FillRectangle(brush, shadowBounds);
            }
        }

        /// <summary>
        /// Calculate button size based on available height
        /// </summary>
        protected Size CalculateButtonSize(int availableHeight, bool isSquare = true)
        {
            int buttonHeight = Math.Min(32, availableHeight - 8);
            int buttonWidth = isSquare ? buttonHeight : buttonHeight + 20;
            return new Size(buttonWidth, buttonHeight);
        }

        #endregion
    }
}
