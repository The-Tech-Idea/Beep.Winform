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
    /// Icon thumb style - toggle with SVG icons (checkmark ON, X OFF) in thumb
    /// </summary>
    internal class IconThumbTogglePainter : BeepTogglePainterBase
    {
        public IconThumbTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
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
            int iconSize = (int)(thumbSize * 0.5f);
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
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);

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

            // Thumb circle
            using (var brush = new SolidBrush(thumbColor))
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

            // Use ToggleIconHelpers for consistent icon management
            // Get theme from Owner (BaseControl)
            var theme = Owner._currentTheme;
            var useTheme = Owner.UseThemeColors && theme != null;

            PaintIcon(g, IconRegion, Owner.IsOn, state, theme, useTheme);
        }

        private void DrawIconFallback(Graphics g, ControlState state)
        {
            Color iconColor = state == ControlState.Disabled 
                ? Color.FromArgb(100, Color.Gray) 
                : Color.FromArgb(220, Owner.IsOn ? Owner.OnColor : Owner.OffColor);

            using (var pen = new Pen(iconColor, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                if (Owner.IsOn)
                    DrawCheckmark(g, pen, IconRegion);
                else
                    DrawX(g, pen, IconRegion);
            }
        }

        #endregion

        #region Helper Methods

        private void DrawCheckmark(Graphics g, Pen pen, Rectangle rect)
        {
            // Checkmark path
            PointF[] points = new PointF[]
            {
                new PointF(rect.Left + rect.Width * 0.2f, rect.Top + rect.Height * 0.5f),
                new PointF(rect.Left + rect.Width * 0.4f, rect.Top + rect.Height * 0.7f),
                new PointF(rect.Right - rect.Width * 0.15f, rect.Top + rect.Height * 0.25f)
            };
            g.DrawLines(pen, points);
        }

        private void DrawX(Graphics g, Pen pen, Rectangle rect)
        {
            // X (cross) - two diagonal lines
            float padding = rect.Width * 0.25f;
            g.DrawLine(pen, 
                rect.Left + padding, rect.Top + padding,
                rect.Right - padding, rect.Bottom - padding);
            g.DrawLine(pen,
                rect.Right - padding, rect.Top + padding,
                rect.Left + padding, rect.Bottom - padding);
        }

        #endregion
    }
}
