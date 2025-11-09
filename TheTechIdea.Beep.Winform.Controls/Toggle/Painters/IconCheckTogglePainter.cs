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
    /// Check circle icon toggle - shows check-circle (ON/enabled) and x-circle (OFF/disabled) icons
    /// </summary>
    internal class IconCheckTogglePainter : BeepTogglePainterBase
    {
        public IconCheckTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            int trackHeight = Math.Min(bounds.Height, bounds.Height * 2 / 3);
            int trackWidth = Math.Max(bounds.Width, trackHeight * 2);
            
            TrackRegion = new Rectangle(bounds.X, bounds.Y + (bounds.Height - trackHeight) / 2, trackWidth, trackHeight);

            int thumbSize = (int)(trackHeight * 0.85f);
            int padding = (trackHeight - thumbSize) / 2;
            int thumbX = (int)(TrackRegion.X + padding + (TrackRegion.Width - thumbSize - padding * 2) * Owner.ThumbPosition);
            
            ThumbRegion = new Rectangle(thumbX, TrackRegion.Y + padding, thumbSize, thumbSize);

            int iconSize = (int)(thumbSize * 0.6f);
            IconRegion = new Rectangle(
                ThumbRegion.X + (ThumbRegion.Width - iconSize) / 2,
                ThumbRegion.Y + (ThumbRegion.Height - iconSize) / 2,
                iconSize, iconSize
            );

            OnLabelRegion = Rectangle.Empty;
            OffLabelRegion = Rectangle.Empty;
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            if (trackRect.IsEmpty) return;
            Color trackColor = GetTrackColor(state);
            int radius = trackRect.Height / 2;

            using (var path = GetRoundedRectPath(trackRect, radius))
            using (var brush = new LinearGradientBrush(trackRect, trackColor, ControlPaint.Dark(trackColor, 0.1f), LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty) return;
            Color thumbColor = GetThumbColor(state);

            if (state != ControlState.Disabled)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                    g.FillEllipse(shadowBrush, new Rectangle(thumbRect.X + 1, thumbRect.Y + 2, thumbRect.Width, thumbRect.Height));
            }

            using (var brush = new LinearGradientBrush(thumbRect, thumbColor, ControlPaint.Dark(thumbColor, 0.05f), LinearGradientMode.Vertical))
                g.FillEllipse(brush, thumbRect);

            using (var pen = new Pen(Color.FromArgb(60, Color.Black), 1.5f))
                g.DrawEllipse(pen, thumbRect);
        }

        protected override void PaintIcons(Graphics g, ControlState state)
        {
            if (IconRegion.IsEmpty) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color iconColor = state == ControlState.Disabled 
                ? Color.FromArgb(100, Color.Gray) 
                : Owner.IsOn ? Owner.OnColor : Owner.OffColor;

            // Use CheckCircle (ON/enabled) and XCircle (OFF/disabled)
            string iconPath = Owner.IsOn ? SvgsUI.CheckCircle : SvgsUI.XCircle;
            StyledImagePainter.PaintWithTint(g, IconRegion, iconPath, iconColor, 1f, 0);
        }

        #endregion
    }
}
