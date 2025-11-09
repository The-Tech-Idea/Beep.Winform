using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Rectangular style - rectangular toggle with ON/OFF text and colored thumb
    /// </summary>
    internal class RectangularTogglePainter : BeepTogglePainterBase
    {
        public RectangularTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Rectangular track with slight rounding
            int trackHeight = Math.Min(bounds.Height, bounds.Height * 3 / 4);
            int trackWidth = Math.Max(bounds.Width, trackHeight * 2);
            
            TrackRegion = new Rectangle(
                bounds.X,
                bounds.Y + (bounds.Height - trackHeight) / 2,
                trackWidth,
                trackHeight
            );

            // Thumb is a rounded rectangle
            int thumbWidth = (int)(TrackRegion.Width * 0.45f);
            int thumbHeight = (int)(trackHeight * 0.85f);
            int padding = (trackHeight - thumbHeight) / 2;
            
            // Calculate thumb position based on animation
            int thumbX = (int)(TrackRegion.X + padding + 
                (TrackRegion.Width - thumbWidth - padding * 2) * Owner.ThumbPosition);
            
            ThumbRegion = new Rectangle(
                thumbX,
                TrackRegion.Y + padding,
                thumbWidth,
                thumbHeight
            );

            // Label regions for ON/OFF text
            if (Owner.ShowLabels)
            {
                int labelWidth = TrackRegion.Width / 2 - padding * 2;
                int labelHeight = trackHeight - padding * 2;
                
                OffLabelRegion = new Rectangle(
                    TrackRegion.X + padding * 2,
                    TrackRegion.Y + padding,
                    labelWidth,
                    labelHeight
                );

                OnLabelRegion = new Rectangle(
                    TrackRegion.X + TrackRegion.Width / 2 + padding,
                    TrackRegion.Y + padding,
                    labelWidth,
                    labelHeight
                );
            }
            else
            {
                OffLabelRegion = Rectangle.Empty;
                OnLabelRegion = Rectangle.Empty;
            }

            IconRegion = Rectangle.Empty;
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            if (trackRect.IsEmpty)
                return;

            Color trackColor = GetTrackColor(state);
            int cornerRadius = Math.Min(6, trackRect.Height / 6);

            using (var path = GetRoundedRectPath(trackRect, cornerRadius))
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);

                // Border
                using (var borderPen = new Pen(DarkenColor(trackColor, 0.2f), 1.5f))
                {
                    g.DrawPath(borderPen, path);
                }
            }
        }

        protected override void PaintLabels(Graphics g, ControlState state)
        {
            if (!Owner.ShowLabels)
                return;

            using (var font = new Font(Owner.Font.FontFamily, Owner.Font.Size - 1, FontStyle.Bold))
            {
                Color activeColor = Color.White;
                Color inactiveColor = Color.FromArgb(150, Color.Gray);

                // Draw OFF label
                if (!OffLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OffText, font, 
                        Owner.IsOn ? inactiveColor : activeColor, 
                        OffLabelRegion);
                }

                // Draw ON label
                if (!OnLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OnText, font, 
                        Owner.IsOn ? activeColor : inactiveColor, 
                        OnLabelRegion);
                }
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty)
                return;

            Color thumbColor = GetThumbColor(state);
            int cornerRadius = Math.Min(4, thumbRect.Height / 4);

            // Shadow
            if (state != ControlState.Disabled)
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(50, Color.Black)))
                {
                    var shadowRect = new Rectangle(
                        thumbRect.X + 2,
                        thumbRect.Y + 2,
                        thumbRect.Width,
                        thumbRect.Height
                    );
                    using (var shadowPath = GetRoundedRectPath(shadowRect, cornerRadius))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Thumb
            using (var path = GetRoundedRectPath(thumbRect, cornerRadius))
            using (var brush = new SolidBrush(thumbColor))
            {
                g.FillPath(brush, path);

                // Gradient highlight
                if (state != ControlState.Disabled)
                {
                    using (var highlightBrush = new LinearGradientBrush(
                        thumbRect,
                        Color.FromArgb(60, Color.White),
                        Color.FromArgb(0, Color.White),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(highlightBrush, path);
                    }
                }

                // Border
                using (var pen = new Pen(Color.FromArgb(80, Color.Black), 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        #endregion
    }
}
