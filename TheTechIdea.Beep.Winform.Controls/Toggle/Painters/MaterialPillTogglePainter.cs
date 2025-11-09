using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Material Design pill style with ripple effect and elevation
    /// </summary>
    internal class MaterialPillTogglePainter : BeepTogglePainterBase
    {
        private List<RippleEffect> _ripples = new List<RippleEffect>();
        
        public MaterialPillTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Material pill layout
            int trackHeight = Math.Min(bounds.Height, bounds.Height * 2 / 3);
            int trackWidth = Math.Max(bounds.Width, trackHeight * 2);
            
            TrackRegion = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - trackHeight) / 2,
                trackWidth,
                trackHeight
            );

            // Larger thumb for Material Design
            int thumbSize = (int)(trackHeight * 1.2f);
            int yOffset = (bounds.Height - thumbSize) / 2;
            
            // Calculate thumb position
            int thumbX = (int)(TrackRegion.X + 
                (TrackRegion.Width - thumbSize) * Owner.ThumbPosition);
            
            ThumbRegion = new Rectangle(
                thumbX,
                bounds.Y + yOffset,
                thumbSize,
                thumbSize
            );

            OnLabelRegion = Rectangle.Empty;
            OffLabelRegion = Rectangle.Empty;
            IconRegion = Rectangle.Empty;
        }

        #endregion

        #region Ripple Effect

        public void AddRipple(Point location)
        {
            _ripples.Add(new RippleEffect
            {
                Center = location,
                StartTime = DateTime.Now,
                MaxRadius = 40
            });
        }

        private void UpdateRipples()
        {
            _ripples.RemoveAll(r => (DateTime.Now - r.StartTime).TotalMilliseconds > 600);
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            if (trackRect.IsEmpty)
                return;

            Color trackColor = GetTrackColor(state);
            int radius = trackRect.Height / 2;

            // Material elevation shadow
            if (state != ControlState.Disabled)
            {
                PaintElevationShadow(g, trackRect, radius, 1);
            }

            using (var path = GetRoundedRectPath(trackRect, radius))
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty)
                return;

            Color thumbColor = GetThumbColor(state);

            // Material elevation shadow (higher elevation)
            if (state != ControlState.Disabled)
            {
                int elevation = state == ControlState.Pressed ? 8 : 4;
                PaintCircleElevationShadow(g, thumbRect, elevation);
            }

            // Thumb circle
            using (var brush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(brush, thumbRect);
            }

            // Ripple effects
            UpdateRipples();
            foreach (var ripple in _ripples)
            {
                PaintRipple(g, ripple, thumbRect);
            }

            // State overlay
            if (state == ControlState.Hover)
            {
                using (var hoverBrush = new SolidBrush(Color.FromArgb(20, Color.Black)))
                {
                    g.FillEllipse(hoverBrush, thumbRect);
                }
            }
            else if (state == ControlState.Pressed)
            {
                using (var pressBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                {
                    g.FillEllipse(pressBrush, thumbRect);
                }
            }
        }

        #endregion

        #region Helper Methods

        private void PaintElevationShadow(Graphics g, Rectangle rect, int cornerRadius, int elevation)
        {
            int shadowLayers = elevation * 2;
            int maxOffset = elevation;
            int maxBlur = elevation * 2;

            for (int i = 0; i < shadowLayers; i++)
            {
                int alpha = 15 - (i * 15 / shadowLayers);
                int offset = (i * maxOffset) / shadowLayers;
                int blur = (i * maxBlur) / shadowLayers;

                var shadowRect = new Rectangle(
                    rect.X + offset,
                    rect.Y + offset + blur / 2,
                    rect.Width,
                    rect.Height
                );

                using (var path = GetRoundedRectPath(shadowRect, cornerRadius))
                using (var brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private void PaintCircleElevationShadow(Graphics g, Rectangle thumbRect, int elevation)
        {
            int shadowLayers = elevation * 2;
            int maxOffset = elevation;
            int maxBlur = elevation * 2;

            for (int i = 0; i < shadowLayers; i++)
            {
                int alpha = 20 - (i * 20 / shadowLayers);
                int offset = (i * maxOffset) / shadowLayers;
                int blur = (i * maxBlur) / shadowLayers;

                var shadowRect = new Rectangle(
                    thumbRect.X + offset,
                    thumbRect.Y + offset + blur / 2,
                    thumbRect.Width + blur,
                    thumbRect.Height + blur
                );

                using (var brush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillEllipse(brush, shadowRect);
                }
            }
        }

        private void PaintRipple(Graphics g, RippleEffect ripple, Rectangle bounds)
        {
            double elapsed = (DateTime.Now - ripple.StartTime).TotalMilliseconds;
            double duration = 600.0;
            float progress = (float)(elapsed / duration);

            if (progress > 1f) return;

            // Ease out
            float easedProgress = 1f - (float)Math.Pow(1f - progress, 3);
            
            float currentRadius = ripple.MaxRadius * easedProgress;
            int alpha = (int)(60 * (1f - progress));

            using (var brush = new SolidBrush(Color.FromArgb(alpha, Color.White)))
            {
                float x = ripple.Center.X - currentRadius;
                float y = ripple.Center.Y - currentRadius;
                float diameter = currentRadius * 2;

                g.FillEllipse(brush, x, y, diameter, diameter);
            }
        }

        #endregion

        #region Nested Class

        private class RippleEffect
        {
            public Point Center { get; set; }
            public DateTime StartTime { get; set; }
            public float MaxRadius { get; set; }
        }

        #endregion
    }
}
