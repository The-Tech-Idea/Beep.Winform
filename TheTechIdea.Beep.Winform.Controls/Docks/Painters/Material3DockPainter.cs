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

            // Material elevation shadow
            if (config.ShowShadow)
            {
                PaintMaterialShadow(g, bounds, config);
            }

            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
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
                        1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private void PaintMaterialShadow(Graphics g, Rectangle bounds, DockConfig config)
        {
            // Material elevation shadow (simulated blur)
            var shadowBounds = bounds;
            shadowBounds.Inflate(ElevationShadowBlur, ElevationShadowBlur);
            shadowBounds.Offset(0, 2);

            using (var shadowPath = CreateRoundedPath(shadowBounds, config.CornerRadius))
            {
                // Umbra (darkest shadow)
                for (int i = ElevationShadowBlur; i > 0; i--)
                {
                    int alpha = (int)(40 * (i / (float)ElevationShadowBlur));
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
            var bounds = itemState.Bounds;

            // Ripple effect for hover/selection
            if (itemState.IsHovered || itemState.IsSelected)
            {
                PaintRippleEffect(g, bounds, itemState, config, theme);
            }

            // Paint icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }
        }

        private void PaintRippleEffect(Graphics g, Rectangle bounds, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            var rippleBounds = bounds;
            rippleBounds.Inflate(6, 6);

            using (var path = CreateRoundedPath(rippleBounds, ItemRippleRadius))
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
