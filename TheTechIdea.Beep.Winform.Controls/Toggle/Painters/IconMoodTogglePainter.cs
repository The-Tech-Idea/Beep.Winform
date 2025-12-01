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
    /// Mood icon toggle - shows smile (ON/happy) and frown (OFF/sad) icons
    /// </summary>
    internal class IconMoodTogglePainter : BeepTogglePainterBase
    {
        public IconMoodTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
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

            // Get icon color based on state - use green for smile, red for frown (special case - semantic colors)
            Color iconColor;
            if (state == ControlState.Disabled)
            {
                iconColor = Color.FromArgb(100, Color.Gray);
            }
            else if (Owner.IsOn)
            {
                // Happy - use green
                iconColor = Color.FromArgb(255, 76, 175, 80); // Material Green
            }
            else
            {
                // Sad - use red
                iconColor = Color.FromArgb(255, 244, 67, 54); // Material Red
            }

            // Get icon path using helper
            string iconPath = GetIconPath(Owner.IsOn);
            if (string.IsNullOrEmpty(iconPath))
                iconPath = Owner.IsOn ? SvgsUI.Smile : SvgsUI.Frown;

            // Use StyledImagePainter with custom colors
            using (var iconPathShape = GetRoundedRectPath(IconRegion, 0))
            {
                StyledImagePainter.PaintWithTint(g, iconPathShape, iconPath, iconColor, 1f, 0);
            }
        }

        #endregion

        #region Helper Methods

        private void DrawMoodIconFallback(Graphics g, ControlState state)
        {
            Color iconColor;
            if (state == ControlState.Disabled)
            {
                iconColor = Color.FromArgb(100, Color.Gray);
            }
            else if (Owner.IsOn)
            {
                iconColor = Color.FromArgb(255, 76, 175, 80); // Material Green
            }
            else
            {
                iconColor = Color.FromArgb(255, 244, 67, 54); // Material Red
            }

            using (var pen = new Pen(iconColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                float cx = IconRegion.X + IconRegion.Width / 2f;
                float cy = IconRegion.Y + IconRegion.Height / 2f;
                float size = Math.Min(IconRegion.Width, IconRegion.Height) * 0.6f;

                // Face circle
                g.DrawEllipse(pen, cx - size / 2, cy - size / 2, size, size);

                // Eyes
                float eyeY = cy - size * 0.15f;
                float eyeSize = size * 0.1f;
                using (var eyeBrush = new SolidBrush(iconColor))
                {
                    g.FillEllipse(eyeBrush, cx - size * 0.2f - eyeSize / 2, eyeY - eyeSize / 2, eyeSize, eyeSize);
                    g.FillEllipse(eyeBrush, cx + size * 0.2f - eyeSize / 2, eyeY - eyeSize / 2, eyeSize, eyeSize);
                }

                // Mouth
                float mouthY = cy + size * 0.15f;
                float mouthWidth = size * 0.4f;
                float mouthHeight = size * 0.2f;

                if (Owner.IsOn)
                {
                    // Smile (arc up)
                    g.DrawArc(pen,
                        cx - mouthWidth / 2,
                        mouthY - mouthHeight,
                        mouthWidth,
                        mouthHeight * 2,
                        0, 180);
                }
                else
                {
                    // Frown (arc down)
                    g.DrawArc(pen,
                        cx - mouthWidth / 2,
                        mouthY - mouthHeight,
                        mouthWidth,
                        mouthHeight * 2,
                        180, 180);
                }
            }
        }

        #endregion
    }
}
