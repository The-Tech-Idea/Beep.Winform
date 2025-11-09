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
                    Color.FromArgb(255, ControlPaint.Dark(trackColor, 0.1f)),
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
                ControlPaint.Dark(thumbColor, 0.05f),
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

            // Get icon color based on state
            Color iconColor = state == ControlState.Disabled 
                ? Color.FromArgb(100, Color.Gray) 
                : Owner.IsOn ? Owner.OnColor : Owner.OffColor;

            // Use Tool icon for settings (gear/wrench icon)
            string iconPath = SvgsUI.Tool;

            // Save graphics state
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

        #endregion
    }
}
