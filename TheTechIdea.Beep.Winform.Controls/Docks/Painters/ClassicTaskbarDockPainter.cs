using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Classic Windows taskbar-style dock painter (Windows 7/10 style)
    /// Features:
    /// - Flat design with sharp edges
    /// - System accent color integration
    /// - Rectangular items with minimal padding
    /// - Vertical line indicators for running apps
    /// - No rounded corners (or minimal)
    /// </summary>
    public class ClassicTaskbarDockPainter : DockPainterBase
    {
        private const int IndicatorWidth = 3;
        private const int ItemPadding = 4;

        /// <summary>
        /// Paints the dock background
        /// </summary>
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Windows 10 style - minimal or no rounding
            int cornerRadius = Math.Min(config.CornerRadius, 2);

            using (var path = CreateRoundedPath(bounds, cornerRadius))
            {
                // Flat background color
                var bgColor = ResolveBackground(
                    config,
                    theme,
                    Color.FromArgb(45, 45, 48) // Dark taskbar default
                );

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Subtle top border for definition
                if (config.ShowBorder)
                {
                    var borderColor = ResolveBorder(
                        config,
                        theme,
                        Color.FromArgb(60, 60, 65)
                    );

                    using (var pen = new Pen(borderColor, 1f))
                    {
                        g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    }
                }
            }
        }

        /// <summary>
        /// Paints a single dock item
        /// </summary>
        protected override void PaintDockItemCore(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var bounds = itemState.Bounds;
            var interactionState = GetInteractionState(itemState);

            // Flat rectangular highlight, at an intensity that distinguishes the states rather than
            // treating four of them as one. This branch used to be a four-way OR that painted the
            // same fill for hovered, selected, focused and pressed - which is why Cyberpunk, Dracula
            // and Terminal reported those four states as identical pixels.
            float intensity = interactionState switch
            {
                DockInteractionState.Pressed => 0.45f,
                DockInteractionState.Selected => 0.30f,
                DockInteractionState.Hovered => 0.20f,
                DockInteractionState.Focused => 0.12f,
                _ => 0f
            };

            if (intensity > 0f)
            {
                PaintItemBackground(g, bounds, itemState, config, theme, intensity);
            }

            // Paint icon
            PaintItemIcon(g, itemState, config, theme, itemState.CurrentOpacity);
        }

        private void PaintItemBackground(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme, float intensity = 0.3f)
        {
            var bgBounds = bounds;
            bgBounds.Inflate(ItemPadding, ItemPadding);

            Color bgColor;
            if (itemState.IsSelected)
            {
                // Selected - use accent color
                bgColor = GetColor(
                    config.SelectedColor,
                    ResolveAccentColor(config, theme),
                    intensity
                );
            }
            else
            {
                // Hovered
                bgColor = GetColor(
                    config.HoverColor,
                    theme?.BackgroundColor ?? Color.FromArgb(55, 55, 60),
                    0.5f
                );
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bgBounds);
            }
        }

        /// <summary>
        /// Paints the running indicator for dock items
        /// </summary>
        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsRunning && !itemState.IsSelected)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Vertical line indicator at bottom (Windows 10 style)
            var indicatorColor = itemState.IsSelected
                ? (theme?.AccentColor ?? Color.FromArgb(0, 120, 215))
                : Color.FromArgb(150, 150, 150);

            int lineHeight = itemState.IsSelected ? 3 : 2;
            int lineWidth = bounds.Width - 8;
            int lineX = bounds.X + 4;
            int lineY = bounds.Bottom + 2;

            using (var brush = new SolidBrush(indicatorColor))
            {
                g.FillRectangle(brush, lineX, lineY, lineWidth, lineHeight);
            }
        }
    }
}
