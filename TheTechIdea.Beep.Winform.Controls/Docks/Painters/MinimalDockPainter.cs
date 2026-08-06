using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Ultra-minimal dock painter with clean aesthetics
    /// Features:
    /// - Near-invisible background
    /// - Icon-focused design
    /// - Simple line indicator for selection
    /// - Subtle opacity changes on hover
    /// - No shadows or borders
    /// </summary>
    public class MinimalDockPainter : DockPainterBase
    {
        private const float InactiveOpacity = 0.7f;
        private const float ActiveOpacity = 1.0f;

        /// <summary>
        /// Near-invisible by design. Declared as the style default rather than a private constant so
        /// that the value the painter uses and the value the config reports are the same thing - the
        /// constant used to shadow <see cref="DockConfig.BackgroundOpacity"/> by name, so the call
        /// site below read as though it honoured the config and did not.
        /// </summary>
        protected override float? StyleBackgroundOpacity => 0.05f;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            // Minimal style - extremely subtle or no visible background
            if (config.ShowBackground)
            {
                using (var path = CreateRoundedPath(bounds, config.CornerRadius))
                {
                    var bgColor = ResolveBackground(
                        config,
                        theme,
                        Color.White
                    );

                    using (var brush = new SolidBrush(bgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // No shadows, no borders - pure minimalism
        }

        protected override void PaintDockItemCore(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = itemState.Bounds;

            // Calculate opacity based on state
            float opacity = itemState.CurrentOpacity;
            if (!itemState.IsHovered && !itemState.IsSelected)
            {
                opacity *= InactiveOpacity;
            }

            // Minimal's hover was an opacity change on the icon alone, which the measurement could
            // not tell apart from Normal - and neither could a user glancing at it. A hairline
            // underline in the theme's hover colour is the smallest mark that still reads as
            // feedback, which is what this style is for. ArcDock inherits it.
            if (itemState.IsHovered && !itemState.IsSelected)
            {
                var hover = ResolveHoverColor(config, theme);
                int thickness = Math.Max(1, bounds.Height / 24);
                var underline = new Rectangle(
                    bounds.Left + bounds.Width / 4,
                    bounds.Bottom + thickness,
                    bounds.Width / 2,
                    thickness);

                using var brush = new SolidBrush(Color.FromArgb(220, hover));
                g.FillRectangle(brush, underline);
            }

            // Paint icon with state-based opacity
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, opacity);
            }

            // Subtle hover effect - slightly larger
            // (handled by layout calculation in base class)
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (!itemState.IsSelected && !itemState.IsRunning)
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Simple line indicator below icon
            int lineWidth = bounds.Width - 20;
            int lineX = bounds.X + 10;
            int lineY = bounds.Bottom + 6;

            using (var pen = new Pen(theme?.AccentColor ?? Color.Black, itemState.IsSelected ? 2f : 1f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, lineX, lineY, lineX + lineWidth, lineY);
            }
        }
    }
}
