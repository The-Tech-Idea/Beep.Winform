using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Checkbox style - square checkbox with checkmark when ON
    /// </summary>
    internal class CheckboxStyleTogglePainter : BeepTogglePainterBase
    {
        public CheckboxStyleTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Square checkbox
            int size = Math.Min(bounds.Width, bounds.Height);
            
            TrackRegion = new Rectangle(
                bounds.X + (bounds.Width - size) / 2,
                bounds.Y + (bounds.Height - size) / 2,
                size,
                size
            );

            ThumbRegion = TrackRegion; // Same as track for checkbox

            // Icon (checkmark) region inside the box
            int iconSize = (int)(size * 0.6f);
            IconRegion = new Rectangle(
                TrackRegion.X + (TrackRegion.Width - iconSize) / 2,
                TrackRegion.Y + (TrackRegion.Height - iconSize) / 2,
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

            int cornerRadius = Math.Min(4, trackRect.Height / 8);

            // Background color
            Color bgColor;
            if (state == ControlState.Disabled)
            {
                bgColor = Owner.IsOn 
                    ? Color.FromArgb(150, Owner.OnColor) 
                    : Color.FromArgb(200, Color.LightGray);
            }
            else if (Owner.IsOn)
            {
                bgColor = Owner.OnColor;
            }
            else
            {
                bgColor = Color.White;
            }

            using (var path = GetRoundedRectPath(trackRect, cornerRadius))
            {
                // Shadow
                if (state != ControlState.Disabled && !Owner.IsOn)
                {
                    var shadowRect = new Rectangle(
                        trackRect.X + 1,
                        trackRect.Y + 1,
                        trackRect.Width,
                        trackRect.Height
                    );
                    using (var shadowPath = GetRoundedRectPath(shadowRect, cornerRadius))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black)))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // Fill
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Gradient for ON state
                if (Owner.IsOn && state != ControlState.Disabled)
                {
                    using (var highlightBrush = new LinearGradientBrush(
                        trackRect,
                        Color.FromArgb(40, Color.White),
                        Color.FromArgb(0, Color.White),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(highlightBrush, path);
                    }
                }

                // Border
                Color borderColor;
                if (state == ControlState.Disabled)
                {
                    borderColor = Color.FromArgb(150, Color.Gray);
                }
                else if (Owner.IsOn)
                {
                    borderColor = DarkenColor(Owner.OnColor, 0.3f);
                }
                else
                {
                    borderColor = Color.FromArgb(180, Color.Gray);
                }

                float borderWidth = (state == ControlState.Pressed || Owner.IsOn) ? 2f : 1.5f;
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            // Thumb is same as track for checkbox style
        }

        protected override void PaintIcons(Graphics g, ControlState state)
        {
            if (!Owner.IsOn || IconRegion.IsEmpty)
                return;

            // Draw checkmark
            Color checkColor = state == ControlState.Disabled 
                ? Color.FromArgb(150, Color.White) 
                : Color.White;

            using (var pen = new Pen(checkColor, 2.5f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Checkmark animation based on thumb position
                float progress = Owner.ThumbPosition; // 0 to 1
                
                // Checkmark path
                PointF p1 = new PointF(
                    IconRegion.Left + IconRegion.Width * 0.2f,
                    IconRegion.Top + IconRegion.Height * 0.5f
                );
                PointF p2 = new PointF(
                    IconRegion.Left + IconRegion.Width * 0.4f,
                    IconRegion.Top + IconRegion.Height * 0.7f
                );
                PointF p3 = new PointF(
                    IconRegion.Right - IconRegion.Width * 0.15f,
                    IconRegion.Top + IconRegion.Height * 0.25f
                );

                // Animate checkmark drawing
                if (progress >= 0.5f)
                {
                    // First stroke (p1 to p2)
                    float stroke1Progress = Math.Min(1f, (progress - 0.5f) * 2f);
                    PointF p2Animated = new PointF(
                        p1.X + (p2.X - p1.X) * stroke1Progress,
                        p1.Y + (p2.Y - p1.Y) * stroke1Progress
                    );
                    g.DrawLine(pen, p1, p2Animated);

                    // Second stroke (p2 to p3) - only if first stroke is complete
                    if (stroke1Progress >= 1f)
                    {
                        float stroke2Progress = (progress - 0.75f) * 4f;
                        if (stroke2Progress > 0)
                        {
                            PointF p3Animated = new PointF(
                                p2.X + (p3.X - p2.X) * Math.Min(1f, stroke2Progress),
                                p2.Y + (p3.Y - p2.Y) * Math.Min(1f, stroke2Progress)
                            );
                            g.DrawLine(pen, p2, p3Animated);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
