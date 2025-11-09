using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Labeled track style - toggle with ON/OFF text on the track
    /// </summary>
    internal class LabeledTrackTogglePainter : BeepTogglePainterBase
    {
        public LabeledTrackTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Similar to pill but with text inside track
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

            // Label regions if labels are shown
            if (Owner.ShowLabels)
            {
                int labelWidth = TrackRegion.Width / 2 - 4;
                int labelHeight = TrackRegion.Height - 4;
                
                OffLabelRegion = new Rectangle(
                    TrackRegion.X + 2,
                    TrackRegion.Y + 2,
                    labelWidth,
                    labelHeight
                );

                OnLabelRegion = new Rectangle(
                    TrackRegion.X + TrackRegion.Width / 2 + 2,
                    TrackRegion.Y + 2,
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
            int radius = trackRect.Height / 2;

            using (var path = GetRoundedRectPath(trackRect, radius))
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);
            }
        }

        protected override void PaintLabels(Graphics g, ControlState state)
        {
            if (!Owner.ShowLabels)
                return;

            Font font = new Font(Owner.Font.FontFamily, Owner.Font.Size - 2, FontStyle.Bold);
            Color textColor = Owner.IsOn ? 
                Color.FromArgb(200, Owner.OnColor) : 
                Color.FromArgb(200, Owner.OffColor);

            try
            {
                // Draw OFF label
                if (!OffLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OffText, font, 
                        Owner.IsOn ? textColor : Color.White, 
                        OffLabelRegion);
                }

                // Draw ON label
                if (!OnLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OnText, font, 
                        Owner.IsOn ? Color.White : textColor, 
                        OnLabelRegion);
                }
            }
            finally
            {
                font?.Dispose();
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            if (thumbRect.IsEmpty)
                return;

            Color thumbColor = GetThumbColor(state);

            using (var brush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(brush, thumbRect);

                // Border
                using (var pen = new Pen(Color.FromArgb(40, Color.Black), 1.5f))
                {
                    g.DrawEllipse(pen, thumbRect);
                }
            }
        }

        #endregion
    }
}
