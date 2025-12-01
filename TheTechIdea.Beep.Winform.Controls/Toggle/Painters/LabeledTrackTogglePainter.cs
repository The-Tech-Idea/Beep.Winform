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

            // Use ToggleFontHelpers for consistent font management
            using (var offFont = GetLabelFont(false))
            using (var onFont = GetLabelFont(true))
            {
                // Get track color for contrast checking
                Color trackColor = GetTrackColor(state);
                
                // Calculate text colors with accessibility in mind
                Color offTextColor = Owner.IsOn ? 
                    Color.FromArgb(200, Owner.OnColor) : 
                    Color.White;
                Color onTextColor = Owner.IsOn ? 
                    Color.White : 
                    Color.FromArgb(200, Owner.OffColor);

                // Apply high contrast adjustments if enabled
                if (ToggleAccessibilityHelpers.IsHighContrastMode())
                {
                    var (onColor, offColor, thumbColor, textColor) = ToggleAccessibilityHelpers.GetHighContrastColors();
                    offTextColor = textColor;
                    onTextColor = textColor;
                }
                else
                {
                    // Ensure WCAG contrast compliance
                    Color trackColorForContrast = Owner.IsOn ? Owner.OnColor : Owner.OffColor;
                    offTextColor = ToggleAccessibilityHelpers.EnsureContrastRatio(offTextColor, trackColorForContrast, 4.5)
                        ? offTextColor
                        : ToggleAccessibilityHelpers.AdjustForContrast(offTextColor, trackColorForContrast, 4.5);
                    onTextColor = ToggleAccessibilityHelpers.EnsureContrastRatio(onTextColor, trackColorForContrast, 4.5)
                        ? onTextColor
                        : ToggleAccessibilityHelpers.AdjustForContrast(onTextColor, trackColorForContrast, 4.5);
                }

                // Draw OFF label
                if (!OffLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OffText, offFont, 
                        Owner.IsOn ? offTextColor : onTextColor, 
                        OffLabelRegion);
                }

                // Draw ON label
                if (!OnLabelRegion.IsEmpty)
                {
                    DrawCenteredText(g, Owner.OnText, onFont, 
                        Owner.IsOn ? onTextColor : offTextColor, 
                        OnLabelRegion);
                }
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
