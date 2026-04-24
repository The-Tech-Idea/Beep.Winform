using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Settings icon toggle - shows gear icon with rotation animation when toggling
    /// </summary>
    internal class IconSettingsTogglePainter : BeepTogglePainterBase
    {
        public IconSettingsTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Pill track
            int trackHeight = Math.Min(bounds.Height, bounds.Height * 2 / 3);
            int trackWidth = Math.Max(bounds.Width, trackHeight * 2);
            
            TrackRegion = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - trackHeight) / 2,
                trackWidth,
                trackHeight
            );

            // Thumb is a circle
            int thumbSize = (int)(trackHeight * 0.85f);
            int padding = (trackHeight - thumbSize) / 2;
            
            // Calculate thumb position based on animation
            int thumbX = (int)(TrackRegion.X + padding + 
                (TrackRegion.Width - thumbSize - padding * 2) * Owner.ThumbPosition);
            
            ThumbRegion = new Rectangle(
                thumbX,
                TrackRegion.Y + padding,
                thumbSize,
                thumbSize
            );

            // Icon region inside thumb
            int iconSize = (int)(thumbSize * 0.6f);
            IconRegion = new Rectangle(
                ThumbRegion.X + (ThumbRegion.Width - iconSize) / 2,
                ThumbRegion.Y + (ThumbRegion.Height - iconSize) / 2,
                iconSize,
                iconSize
            );

            OnLabelRegion = Rectangle.Empty;
            OffLabelRegion = Rectangle.Empty;
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            if (trackRect.IsEmpty)
                return;

            Color trackColor = GetTrackColor(state);
            int radius = trackRect.Height / 2;

            using (var path = GetRoundedRectPath(trackRect, radius))
            {
                // Gradient background
                using (var brush = new LinearGradientBrush(
                    trackRect,
                    Color.FromArgb(255, trackColor),
                    Color.FromArgb(255, ShiftLuminance(trackColor, -0.1f)),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }

                // Inner shadow
                if (state != ControlState.Disabled)
                {
                    using (var innerPath = GetRoundedRectPath(
                        new Rectangle(trackRect.X + 1, trackRect.Y + 1, 
                                    trackRect.Width - 2, trackRect.Height - 2), 
                        radius - 1))
                    using (var pen = new Pen(Color.FromArgb(30, Color.Black), 1))
                    {
                        g.DrawPath(pen, innerPath);
                    }
                }
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty)
                return;

            Color thumbColor = GetThumbColor(state);

            // Shadow
            if (state != ControlState.Disabled)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                {
                    var shadowRect = new Rectangle(
                        thumbRect.X + 1,
                        thumbRect.Y + 2,
                        thumbRect.Width,
                        thumbRect.Height
                    );
                    g.FillEllipse(shadowBrush, shadowRect);
                }
            }

            // Thumb circle with gradient
            using (var brush = new LinearGradientBrush(
                thumbRect,
                thumbColor,
                ShiftLuminance(thumbColor, -0.05f),
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(brush, thumbRect);
            }

            // Highlight
            if (state != ControlState.Disabled)
            {
                using (var highlightBrush = new LinearGradientBrush(
                    thumbRect,
                    Color.FromArgb(40, Color.White),
                    Color.FromArgb(0, Color.White),
                    LinearGradientMode.Vertical))
                {
                    var highlightRect = new Rectangle(
                        thumbRect.X,
                        thumbRect.Y,
                        thumbRect.Width,
                        thumbRect.Height / 2
                    );
                    g.FillEllipse(highlightBrush, highlightRect);
                }
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(60, Color.Black), 1.5f))
            {
                g.DrawEllipse(pen, thumbRect);
            }
        }

        protected override void PaintIcons(Graphics g, ControlState state)
        {
            if (IconRegion.IsEmpty)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Calculate rotation angle based on thumb position (rotate 180 degrees during toggle)
            float rotationAngle = Owner.ThumbPosition * 180f;

            // Get icon color using helper (supports theme)
            var theme = Owner._currentTheme;
            var useTheme = Owner.UseThemeColors && theme != null;
            Color iconColor = ToggleIconHelpers.GetIconColor(Owner, Owner.IsOn, state, theme, useTheme);

            // Get icon path using helper
            string iconPath = GetIconPath(Owner.IsOn);
            if (string.IsNullOrEmpty(iconPath))
                iconPath = SvgsUI.Settings ?? SvgsUI.Adjustments ?? SvgsUI.Settings;

            // Save graphics state for rotation
            var graphicsState = g.Save();
            try
            {
                // Move to center of icon region
                g.TranslateTransform(
                    IconRegion.X + IconRegion.Width / 2f,
                    IconRegion.Y + IconRegion.Height / 2f);
                
                // Rotate
                g.RotateTransform(rotationAngle);
                
                // Create rotated icon rectangle (centered)
                var rotatedRect = new Rectangle(
                    -IconRegion.Width / 2, 
                    -IconRegion.Height / 2,
                    IconRegion.Width,
                    IconRegion.Height);

                // Use StyledImagePainter to paint the SVG with tinting
                StyledImagePainter.PaintWithTint(g, rotatedRect, iconPath, iconColor, 1f, 0);
            }
            finally
            {
                // Restore graphics state
                g.Restore(graphicsState);
            }
        }

        #endregion

        #region Helper Methods

        private void DrawSettingsIconFallback(Graphics g, ControlState state, float rotationAngle)
        {
            Color iconColor = state == ControlState.Disabled 
                ? Color.FromArgb(100, Color.Gray) 
                : Owner.IsOn ? Owner.OnColor : Owner.OffColor;

            var state2 = g.Save();
            
            // Move to center of icon region
            g.TranslateTransform(
                IconRegion.X + IconRegion.Width / 2f,
                IconRegion.Y + IconRegion.Height / 2f);
            
            // Rotate
            g.RotateTransform(rotationAngle);

            using (var pen = new Pen(iconColor, 2f))
            using (var brush = new SolidBrush(iconColor))
            {
                float size = Math.Min(IconRegion.Width, IconRegion.Height) * 0.6f;
                
                // Draw gear (simplified)
                // Center circle
                var centerRect = new RectangleF(-size * 0.2f, -size * 0.2f, size * 0.4f, size * 0.4f);
                g.FillEllipse(brush, centerRect);

                // Teeth (8 rectangles around center)
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * 45f;
                    g.ResetTransform();
                    g.TranslateTransform(
                        IconRegion.X + IconRegion.Width / 2f,
                        IconRegion.Y + IconRegion.Height / 2f);
                    g.RotateTransform(rotationAngle + angle);
                    
                    var tooth = new RectangleF(
                        -size * 0.1f,
                        -size * 0.5f,
                        size * 0.2f,
                        size * 0.2f
                    );
                    g.FillRectangle(brush, tooth);
                }
            }
            
            g.Restore(state2);
        }

        private static Color ShiftLuminance(Color color, float amount)
        {
            float h, s, l;
            ColorToHsl(color, out h, out s, out l);
            l = Math.Max(0, Math.Min(1, l + amount));
            return ColorFromHsl(h, s, l);
        }

        private static void ColorToHsl(Color color, out float h, out float s, out float l)
        {
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;
            l = (max + min) / 2.0f;
            if (delta == 0) { h = 0; s = 0; }
            else
            {
                s = l < 0.5f ? delta / (max + min) : delta / (2.0f - max - min);
                if (r == max) h = (g - b) / delta;
                else if (g == max) h = 2.0f + (b - r) / delta;
                else h = 4.0f + (r - g) / delta;
                h /= 6.0f;
                if (h < 0) h += 1.0f;
            }
        }

        private static Color ColorFromHsl(float h, float s, float l)
        {
            float r, g, b;
            if (s == 0) { r = g = b = l; }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;
                r = HueToRgb(p, q, h + 1.0f / 3.0f);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0f / 3.0f);
            }
            return Color.FromArgb(255, Math.Max(0, Math.Min(255, (int)(r * 255))), Math.Max(0, Math.Min(255, (int)(g * 255))), Math.Max(0, Math.Min(255, (int)(b * 255))));
        }

        private static float HueToRgb(float p, float q, float t)
        {
            if (t < 0) t += 1.0f;
            if (t > 1) t -= 1.0f;
            if (t < 1.0f / 6.0f) return p + (q - p) * 6.0f * t;
            if (t < 1.0f / 2.0f) return q;
            if (t < 2.0f / 3.0f) return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            return p;
        }

        #endregion
    }
}

