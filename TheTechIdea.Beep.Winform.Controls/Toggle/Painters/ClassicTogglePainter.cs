using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Classic/iOS style toggle painter - rounded pill with sliding circle thumb
    /// </summary>
    internal class ClassicTogglePainter : BeepTogglePainterBase
    {
        public ClassicTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Classic pill layout - rounded track with sliding circle thumb
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

            // No labels for classic style
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

            Color trackColor = GetTrackColor(state);
            int radius = trackRect.Height / 2;

            using (var path = GetRoundedRectPath(trackRect, radius))
            using (var brush = new SolidBrush(trackColor))
            {
                g.FillPath(brush, path);

                // Add subtle inner shadow for depth
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

            using (var brush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(brush, thumbRect);

                // Add shadow for 3D effect
                if (state != ControlState.Disabled)
                {
                    var shadowRect = new Rectangle(
                        thumbRect.X + 1,
                        thumbRect.Y + 2,
                        thumbRect.Width,
                        thumbRect.Height
                    );

                    using (var shadowPath = new GraphicsPath())
                    {
                        shadowPath.AddEllipse(shadowRect);
                        using (var shadowBrush = new PathGradientBrush(shadowPath))
                        {
                            shadowBrush.CenterColor = Color.FromArgb(40, Color.Black);
                            shadowBrush.SurroundColors = new[] { Color.Transparent };
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }

                    // Highlight on top
                    var highlightRect = new Rectangle(
                        thumbRect.X + 2,
                        thumbRect.Y + 2,
                        thumbRect.Width - 4,
                        thumbRect.Height / 2
                    );
                    using (var highlightBrush = new LinearGradientBrush(
                        highlightRect,
                        Color.FromArgb(60, Color.White),
                        Color.Transparent,
                        LinearGradientMode.Vertical))
                    {
                        g.FillEllipse(highlightBrush, highlightRect);
                    }
                }

                // Border
                using (var pen = new Pen(Color.FromArgb(80, Color.Black), 1))
                {
                    g.DrawEllipse(pen, thumbRect);
                }
            }
        }

        #endregion
    }
}
