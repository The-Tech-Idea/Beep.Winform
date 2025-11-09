using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// macOS-style dock painter with glass effect and magnification
    /// Features:
    /// - Frosted glass background with blur
    /// - Smooth magnification spring effect
    /// - Reflection effect below icons
    /// - Subtle dot indicators for running/active apps
    /// - Rounded pill shape
    /// </summary>
    public class AppleDockPainter : DockPainterBase
    {
        private const int ReflectionHeight = 20;
        private const int GlassBlurRadius = 15;

        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var metrics = GetMetrics(config, theme, false);

            // Paint shadow first
            if (metrics.ShowShadow)
            {
                PaintDockShadow(g, bounds, metrics);
            }

            // Create glass background
            using (var path = CreateRoundedPath(bounds, metrics.CornerRadius))
            {
                // Glass background with blur simulation
                PaintGlassBackground(g, path, bounds, metrics);

                // Border
                if (metrics.ShowBorder)
                {
                    PaintGlassBorder(g, path, metrics);
                }

                // Inner glow for depth
                PaintInnerGlow(g, path, bounds, metrics);
            }
        }

        private void PaintDockShadow(Graphics g, Rectangle bounds, DockPainterMetrics metrics)
        {
            var shadowBounds = bounds;
            shadowBounds.Inflate(5, 5);
            shadowBounds.Offset(0, 3);

            using (var shadowPath = CreateRoundedPath(shadowBounds, metrics.CornerRadius))
            {
                for (int i = 10; i > 0; i--)
                {
                    int alpha = (int)(50 * (i / 10f));
                    using (var pen = new Pen(Color.FromArgb(alpha, metrics.ShadowColor), i / 2f))
                    {
                        g.DrawPath(pen, shadowPath);
                    }
                }
            }
        }

        private void PaintGlassBackground(Graphics g, GraphicsPath path, Rectangle bounds, DockPainterMetrics metrics)
        {
            // Base glass color
            var glassColor = Color.FromArgb(
                (int)(metrics.BackgroundOpacity * 255),
                metrics.BackgroundColor
            );

            // Multi-layer glass effect
            using (var baseGradient = new LinearGradientBrush(
                bounds,
                Color.FromArgb((int)(255 * metrics.BackgroundOpacity * 0.9), glassColor),
                Color.FromArgb((int)(255 * metrics.BackgroundOpacity * 0.7), glassColor),
                90f))
            {
                g.FillPath(baseGradient, path);
            }

            // Highlight at top for glass shine
            var highlightRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 3);
            using (var highlightPath = CreateRoundedPath(highlightRect, metrics.CornerRadius))
            using (var highlightBrush = new LinearGradientBrush(
                highlightRect,
                Color.FromArgb(40, Color.White),
                Color.FromArgb(10, Color.White),
                90f))
            {
                g.FillPath(highlightBrush, highlightPath);
            }
        }

        private void PaintGlassBorder(Graphics g, GraphicsPath path, DockPainterMetrics metrics)
        {
            var borderColor = Color.FromArgb((int)(0.3f * 255), metrics.BorderColor);

            using (var pen = new Pen(borderColor, metrics.BorderWidth))
            {
                g.DrawPath(pen, path);
            }

            // Inner light border for depth
            var innerBounds = path.GetBounds();
            innerBounds.Inflate(-1, -1);
            using (var innerPath = CreateRoundedPath(Rectangle.Round(innerBounds), metrics.CornerRadius - 1))
            using (var innerPen = new Pen(Color.FromArgb(30, Color.White), 1))
            {
                g.DrawPath(innerPen, innerPath);
            }
        }

        private void PaintInnerGlow(Graphics g, GraphicsPath path, Rectangle bounds, DockPainterMetrics metrics)
        {
            // Subtle inner glow at bottom
            var glowRect = new Rectangle(bounds.X, bounds.Bottom - 20, bounds.Width, 20);
            using (var glowBrush = new LinearGradientBrush(
                glowRect,
                Color.FromArgb(0, Color.White),
                Color.FromArgb(10, Color.White),
                270f))
            {
                g.FillRectangle(glowBrush, glowRect);
            }
        }

        public override void PaintDockItem(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var metrics = GetMetrics(config, theme, false);

            var bounds = itemState.Bounds;

            // Item shadow
            if (metrics.ShowShadow && itemState.IsHovered)
            {
                PaintItemShadow(g, bounds, itemState.CurrentScale, metrics);
            }

            // Main icon
            if (!string.IsNullOrEmpty(itemState.Item.ImagePath))
            {
                PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, theme, itemState.CurrentOpacity);
            }

            // Reflection effect
            if (metrics.ShowReflection && (itemState.IsHovered || itemState.IsSelected))
            {
                PaintReflection(g, bounds, itemState, metrics);
            }

            // Badge (notification count)
            if (config.ShowBadges && itemState.BadgeCount > 0)
            {
                PaintBadge(g, bounds, itemState.BadgeCount, metrics);
            }

            // Glow effect for hover
            if (metrics.ShowGlow && itemState.IsHovered)
            {
                PaintGlow(g, bounds, metrics.GlowColor, 8);
            }
        }

        private void PaintItemShadow(Graphics g, Rectangle bounds, float scale, DockPainterMetrics metrics)
        {
            var shadowBounds = bounds;
            shadowBounds.Offset(0, (int)(4 * scale));

            int shadowSize = (int)(6 * scale);

            for (int i = shadowSize; i > 0; i--)
            {
                int alpha = (int)(40 * (i / (float)shadowSize));
                using (var brush = new SolidBrush(Color.FromArgb(alpha, metrics.ShadowColor)))
                {
                    var ellipseBounds = shadowBounds;
                    ellipseBounds.Inflate(i, i / 2);
                    g.FillEllipse(brush, ellipseBounds);
                }
            }
        }

        private void PaintReflection(Graphics g, Rectangle bounds, DockItemState itemState, DockPainterMetrics metrics)
        {
            if (string.IsNullOrEmpty(itemState.Item.ImagePath))
                return;

            // Create reflection below icon
            var reflectionRect = new Rectangle(
                bounds.X,
                bounds.Bottom + 2,
                bounds.Width,
                ReflectionHeight
            );

            // Flip and fade the icon for reflection
            using (var path = CreateRoundedPath(reflectionRect, metrics.CornerRadius / 2))
            {
                g.SetClip(path);

                var matrix = new Matrix();
                matrix.Translate(bounds.X + bounds.Width / 2f, bounds.Bottom + ReflectionHeight);
                matrix.Scale(1, -1);
                matrix.Translate(-(bounds.X + bounds.Width / 2f), -bounds.Y);

                g.Transform = matrix;

                // Draw faded icon
                using (var fadePath = CreateRoundedPath(bounds, metrics.CornerRadius / 2))
                {
                    // Paint icon with reduced opacity for reflection
                    var config = new DockConfig(); // Temporary for PaintItemIcon signature
                    PaintItemIcon(g, bounds, itemState.Item.ImagePath, config, null!, 0.3f);
                }

                g.ResetTransform();
                g.ResetClip();

                // Gradient fade on reflection
                using (var fadeBrush = new LinearGradientBrush(
                    reflectionRect,
                    Color.FromArgb(50, Color.White),
                    Color.FromArgb(0, Color.White),
                    90f))
                {
                    g.FillRectangle(fadeBrush, reflectionRect);
                }
            }
        }

        private void PaintBadge(Graphics g, Rectangle itemBounds, int count, DockPainterMetrics metrics)
        {
            string text = count > 99 ? "99+" : count.ToString();
            int badgeSize = 20;

            var badgeRect = new Rectangle(
                itemBounds.Right - badgeSize / 2,
                itemBounds.Top - badgeSize / 2,
                badgeSize,
                badgeSize
            );

            // Badge background
            using (var bgBrush = new SolidBrush(metrics.BadgeBackgroundColor))
            {
                g.FillEllipse(bgBrush, badgeRect);
            }

            // Badge border
            using (var borderPen = new Pen(Color.White, 2))
            {
                g.DrawEllipse(borderPen, badgeRect);
            }

            // Badge text
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var textBrush = new SolidBrush(metrics.BadgeForegroundColor))
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, textBrush, badgeRect, sf);
            }
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (config.IndicatorStyle == DockIndicatorStyle.None)
                return;

            var metrics = GetMetrics(config, theme, false);
            var bounds = itemState.Bounds;

            switch (config.IndicatorStyle)
            {
                case DockIndicatorStyle.Dot:
                    PaintDotIndicator(g, bounds, itemState, metrics, config);
                    break;

                case DockIndicatorStyle.Line:
                    PaintLineIndicator(g, bounds, itemState, metrics, config);
                    break;

                case DockIndicatorStyle.Glow:
                    if (itemState.IsSelected || itemState.IsRunning)
                    {
                        PaintGlow(g, bounds, metrics.IndicatorColor, 12);
                    }
                    break;
            }
        }

        private void PaintDotIndicator(Graphics g, Rectangle bounds, DockItemState itemState, DockPainterMetrics metrics, DockConfig config)
        {
            int dotSize = itemState.IsSelected ? 6 : 4;
            int dotY = config.Orientation == DockOrientation.Horizontal
                ? bounds.Bottom + 8
                : bounds.Right + 8;

            var dotRect = new Rectangle(
                bounds.X + bounds.Width / 2 - dotSize / 2,
                dotY,
                dotSize,
                dotSize
            );

            using (var brush = new SolidBrush(metrics.IndicatorColor))
            {
                g.FillEllipse(brush, dotRect);
            }

            // Subtle glow around dot
            if (itemState.IsSelected)
            {
                using (var glowBrush = new SolidBrush(Color.FromArgb(50, metrics.IndicatorColor)))
                {
                    var glowRect = dotRect;
                    glowRect.Inflate(2, 2);
                    g.FillEllipse(glowBrush, glowRect);
                }
            }
        }

        private void PaintLineIndicator(Graphics g, Rectangle bounds, DockItemState itemState, DockPainterMetrics metrics, DockConfig config)
        {
            int lineLength = bounds.Width - 10;
            int lineY = bounds.Bottom + 6;

            using (var pen = new Pen(metrics.IndicatorColor, itemState.IsSelected ? 3 : 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                g.DrawLine(pen,
                    bounds.X + 5,
                    lineY,
                    bounds.X + 5 + lineLength,
                    lineY);
            }
        }
    }
}
