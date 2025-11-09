using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Toggle.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Painters
{
    /// <summary>
    /// Button style - toggle with two separate ON/OFF buttons
    /// </summary>
    internal class ButtonStyleTogglePainter : BeepTogglePainterBase
    {
        public ButtonStyleTogglePainter(BeepToggle owner, BeepToggleLayoutHelper layout)
            : base(owner, layout)
        {
        }

        #region Layout Calculation

        public override void CalculateLayout(Rectangle bounds)
        {
            // Two buttons side by side
            int gap = 4;
            int buttonWidth = (bounds.Width - gap) / 2;
            int buttonHeight = bounds.Height;

            // OFF button (left)
            OffLabelRegion = new Rectangle(
                bounds.X,
                bounds.Y,
                buttonWidth,
                buttonHeight
            );

            // ON button (right)
            OnLabelRegion = new Rectangle(
                bounds.X + buttonWidth + gap,
                bounds.Y,
                buttonWidth,
                buttonHeight
            );

            // Track region is the entire bounds
            TrackRegion = bounds;

            // Thumb region is the active button
            ThumbRegion = Owner.IsOn ? OnLabelRegion : OffLabelRegion;

            IconRegion = Rectangle.Empty;
        }

        #endregion

        #region Hit Testing Override

        public override ToggleHitRegion GetHitRegion(Point point)
        {
            if (OnLabelRegion.Contains(point))
                return ToggleHitRegion.OnLabel;
            
            if (OffLabelRegion.Contains(point))
                return ToggleHitRegion.OffLabel;
            
            return ToggleHitRegion.None;
        }

        #endregion

        #region Painting

        protected override void PaintTrack(Graphics g, Rectangle trackRect, ControlState state)
        {
            // Track is painted as part of the buttons
        }

        protected override void PaintThumb(Graphics g, Rectangle thumbRect, ControlState state)
        {
            // Thumb is painted as part of the buttons
        }

        protected override void PaintLabels(Graphics g, ControlState state)
        {
            PaintButton(g, OffLabelRegion, Owner.OffText, !Owner.IsOn, state);
            PaintButton(g, OnLabelRegion, Owner.OnText, Owner.IsOn, state);
        }

        #endregion

        #region Helper Methods

        private void PaintButton(Graphics g, Rectangle buttonRect, string text, bool isActive, ControlState state)
        {
            if (buttonRect.IsEmpty)
                return;

            int cornerRadius = Math.Min(6, buttonRect.Height / 6);

            // Determine colors
            Color bgColor;
            Color textColor;
            Color borderColor;

            if (state == ControlState.Disabled)
            {
                bgColor = Color.FromArgb(200, Color.LightGray);
                textColor = Color.FromArgb(120, Color.Gray);
                borderColor = Color.FromArgb(100, Color.Gray);
            }
            else if (isActive)
            {
                bgColor = Owner.IsOn ? Owner.OnColor : Owner.OffColor;
                textColor = Color.White;
                borderColor = DarkenColor(bgColor, 0.3f);
            }
            else
            {
                bgColor = Color.FromArgb(240, 240, 240);
                textColor = Color.FromArgb(120, Color.Gray);
                borderColor = Color.FromArgb(200, Color.LightGray);
            }

            // Apply hover/press effects
            if (state == ControlState.Hover && !isActive)
            {
                bgColor = Color.FromArgb(230, 230, 230);
            }
            else if (state == ControlState.Pressed && !isActive)
            {
                bgColor = Color.FromArgb(220, 220, 220);
            }

            using (var path = GetRoundedRectPath(buttonRect, cornerRadius))
            {
                // Shadow for active button
                if (isActive && state != ControlState.Disabled)
                {
                    var shadowRect = new Rectangle(
                        buttonRect.X + 1,
                        buttonRect.Y + 2,
                        buttonRect.Width,
                        buttonRect.Height
                    );
                    using (var shadowPath = GetRoundedRectPath(shadowRect, cornerRadius))
                    using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                    {
                        g.FillPath(shadowBrush, shadowPath);
                    }
                }

                // Button background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Gradient highlight for active button
                if (isActive && state != ControlState.Disabled)
                {
                    using (var highlightBrush = new LinearGradientBrush(
                        buttonRect,
                        Color.FromArgb(50, Color.White),
                        Color.FromArgb(0, Color.White),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(highlightBrush, path);
                    }
                }

                // Border
                using (var pen = new Pen(borderColor, isActive ? 2f : 1f))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            using (var font = new Font(Owner.Font.FontFamily, Owner.Font.Size, 
                isActive ? FontStyle.Bold : FontStyle.Regular))
            {
                DrawCenteredText(g, text, font, textColor, buttonRect);
            }
        }

        #endregion
    }
}
