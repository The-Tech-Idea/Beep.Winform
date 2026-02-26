using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Minimal flat style - simple flat toggle with sliding indicator
    /// </summary>
    internal class MinimalTogglePainter : BeepTogglePainterBase
    {
        public MinimalTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Very flat track - thin line
            int trackHeight = Math.Max(4, bounds.Height / 4);
            int trackWidth = bounds.Width;
            
            TrackRegion = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - trackHeight) / 2,
                trackWidth,
                trackHeight
            );

            // Small thumb circle
            int thumbSize = Math.Min(bounds.Height, (int)(trackHeight * 3.5f));
            
            // Calculate thumb position based on animation
            int thumbX = (int)(TrackRegion.X + 
                (TrackRegion.Width - thumbSize) * Owner.ThumbPosition);
            
            ThumbRegion = new Rectangle(
                thumbX,
                bounds.Y + (bounds.Height - thumbSize) / 2,
                thumbSize,
                thumbSize
            );

            OnLabelRegion = Rectangle.Empty;
            OffLabelRegion = Rectangle.Empty;
            IconRegion = Rectangle.Empty;
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            if (trackRect.IsEmpty)
                return;

            // Flat track with subtle gradient
            Color baseColor = Owner.IsOn ? Owner.OnColor : Owner.OffColor;
            if (state == ControlState.Disabled)
                baseColor = Color.FromArgb(150, baseColor);

            Color lightColor = LightenColor(baseColor, 0.3f);
            Color darkColor = DarkenColor(baseColor, 0.1f);

            int radius = trackRect.Height / 2;

            using (var path = GetRoundedRectPath(trackRect, radius))
            using (var brush = new LinearGradientBrush(
                trackRect,
                lightColor,
                darkColor,
                LinearGradientMode.Vertical))
            {
                g.FillPath(brush, path);
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty)
                return;

            Color thumbColor = GetThumbColor(state);

            // Subtle shadow
            if (state != ControlState.Disabled)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(30, Color.Black)))
                {
                    var shadowRect = new Rectangle(
                        thumbRect.X + 1,
                        thumbRect.Y + 1,
                        thumbRect.Width,
                        thumbRect.Height
                    );
                    g.FillEllipse(shadowBrush, shadowRect);
                }
            }

            // Thumb with gradient
            using (var brush = new LinearGradientBrush(
                thumbRect,
                LightenColor(thumbColor, 0.1f),
                thumbColor,
                LinearGradientMode.Vertical))
            {
                g.FillEllipse(brush, thumbRect);
            }

            // Thin border
            Color borderColor = state == ControlState.Disabled 
                ? Color.FromArgb(100, Color.Gray)
                : Color.FromArgb(120, Color.Black);

            using (var pen = new Pen(borderColor, 1f))
            {
                g.DrawEllipse(pen, thumbRect);
            }

            // Highlight on hover/press
            if (state == ControlState.Hovered || state == ControlState.Pressed)
            {
                int glowAlpha = state == ControlState.Pressed ? 60 : 30;
                using (var glowBrush = new SolidBrush(Color.FromArgb(glowAlpha, Color.White)))
                {
                    var glowRect = new Rectangle(
                        thumbRect.X,
                        thumbRect.Y,
                        thumbRect.Width,
                        thumbRect.Height / 2
                    );
                    g.FillEllipse(glowBrush, glowRect);
                }
            }
        }

        #endregion
    }
}
