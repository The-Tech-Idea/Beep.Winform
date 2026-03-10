using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// Material Design 3 dock painter with elevation shadows and ripple effects
    /// Features:
    /// - Elevated container with Material shadows
    /// - Ripple effects on hover/click
    /// - Bold color accents
    /// - Rounded corners with Material specifications
    /// - Dot indicators for running apps
    /// </summary>
    public class Material3DockPainter : DockPainterBase
    {
        private const int ElevationShadowBlur = 12;
        private const int ItemRippleRadius = 12;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var metrics = GetScaledMetrics(config, theme, g);

            // Material elevation shadow
            if (config.ShowShadow)
            {
                PaintMaterialShadow(g, bounds, metrics);
            }

            using (var path = CreateRoundedPath(bounds, metrics.CornerRadius))
            {
                // Surface background
                var bgColor = GetColor(
                    config.BackgroundColor,
                    theme?.SurfaceColor ?? Color.FromArgb(250, 250, 250),
                    config.BackgroundOpacity
                );

                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Border (optional)
                if (config.ShowBorder)
                {
                    using (var pen = new Pen(
                        GetColor(config.BorderColor, theme?.BorderColor ?? Color.FromArgb(230, 230, 230), 1f),
                        metrics.BorderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void PaintMaterialShadow(Graphics g, Rectangle bounds, DockPainterMetrics metrics)
        {
            // Material elevation shadow (simulated blur)
            var shadowBounds = bounds;
            shadowBounds.Inflate(metrics.ShadowBlur, metrics.ShadowBlur);
            shadowBounds.Offset(0, DpiScalingHelper.ScaleValue(2, DpiScalingHelper.GetDpiScaleFactor(g)));

            using (var shadowPath = CreateRoundedPath(shadowBounds, metrics.CornerRadius))
            {
                // Umbra (darkest shadow)
                for (int i = metrics.ShadowBlur; i > 0; i--)
                {
                    int alpha = (int)(40 * (i / (float)Math.Max(1, metrics.ShadowBlur)));
                    using (var pen = new Pen(Color.FromArgb(alpha, Color.Black), i / 3f))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var metrics = GetScaledMetrics(config, theme, g);
            var bounds = itemState.Bounds;

            // Ripple effect for hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintRippleEffect(g, bounds, itemState, config, theme, metrics);
            }

            // Paint icon
            PaintItemIcon(g, itemState, config, theme, itemState.CurrentOpacity);
        }

        private void PaintRippleEffect(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme, DockPainterMetrics metrics)
        {
            var rippleBounds = bounds;
            var rippleInflate = DpiScalingHelper.ScaleValue(6, DpiScalingHelper.GetDpiScaleFactor(g));
            rippleBounds.Inflate(rippleInflate, rippleInflate);

            using (var path = CreateRoundedPath(rippleBounds, metrics.ItemCornerRadius))
            {
                Color rippleColor;
                if (itemState.IsSelected)
                {
                    // Selected - use accent color
                    rippleColor = GetColor(
                        config.SelectedColor,
                        theme?.AccentColor ?? Color.FromArgb(103, 80, 164),
                        0.15f
                    );
                }
                else
                {
                    // Hovered - use hover color
                    rippleColor = GetColor(
                        config.HoverColor,
                        theme?.ButtonHoverBackColor ?? Color.FromArgb(245, 245, 245),
                        0.7f
                    );
                }

                using (var brush = new SolidBrush(rippleColor))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (config.IndicatorStyle == DockIndicatorStyle.None ||
                (!itemState.IsRunning && !itemState.IsSelected))
            {
                return;
            }

            var bounds = itemState.Bounds;

            // Material dot indicator
            int dotSize = itemState.IsSelected ? 6 : 4;
            var dotRect = new Rectangle(
                bounds.X + bounds.Width / 2 - dotSize / 2,
                bounds.Bottom + 8,
                dotSize,
                dotSize
            );

            using (var brush = new SolidBrush(theme?.AccentColor ?? Color.FromArgb(103, 80, 164)))
            {
                g.FillEllipse(brush, dotRect);
            }
        }
    }
}
