using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.Painters
{
    /// <summary>
    /// Microsoft Fluent 2 switch painter
    /// Features: Acrylic background, reveal effects, rounded rectangle track
    /// </summary>
    public class Fluent2SwitchPainter : ISwitchPainter
    {
        private BeepSwitch _owner;

        public Fluent2SwitchPainter(BeepSwitch owner)
        {
            _owner = owner;
        }

        public void CalculateLayout(BeepSwitch owner, SwitchMetrics metrics)
        {
            // Fluent: 40x20px switch (wider ratio)
            int padding = metrics.Padding;
            int trackHeight = owner.Height - (padding * 2);
            int trackWidth = (int)(trackHeight * GetTrackSizeRatio());
            
            if (owner.Orientation == SwitchOrientation.Horizontal)
            {
                var offSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OffLabel, owner.Font);
                var onSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OnLabel, owner.Font);
                
                int trackX = padding + offSize.Width + metrics.LabelPadding;
                int trackY = (owner.Height - trackHeight) / 2;
                
                metrics.TrackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);
                metrics.TrackWidth = trackWidth;
                metrics.TrackHeight = trackHeight;
                
                metrics.ThumbSize = (int)(trackHeight * GetThumbSizeRatio());
                int thumbY = trackY + (trackHeight - metrics.ThumbSize) / 2;
                
                metrics.ThumbOffRect = new Rectangle(trackX + metrics.TrackPadding, thumbY, metrics.ThumbSize, metrics.ThumbSize);
                metrics.ThumbOnRect = new Rectangle(metrics.TrackRect.Right - metrics.ThumbSize - metrics.TrackPadding, thumbY, metrics.ThumbSize, metrics.ThumbSize);
                
                metrics.OffLabelRect = new Rectangle(padding, 0, offSize.Width, owner.Height);
                metrics.OnLabelRect = new Rectangle(metrics.TrackRect.Right + metrics.LabelPadding, 0, onSize.Width, owner.Height);
            }
            else
            {
                var onSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OnLabel, owner.Font);
                var offSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OffLabel, owner.Font);
                
                int vTrackWidth = owner.Width - (padding * 2);
                int vTrackHeight = (int)(vTrackWidth / GetTrackSizeRatio());
                int trackX = (owner.Width - vTrackWidth) / 2;
                int trackY = padding + onSize.Height + metrics.LabelPadding;
                
                metrics.TrackRect = new Rectangle(trackX, trackY, vTrackWidth, vTrackHeight);
                metrics.TrackWidth = vTrackWidth;
                metrics.TrackHeight = vTrackHeight;
                metrics.ThumbSize = (int)(vTrackWidth * GetThumbSizeRatio());
                int thumbX = trackX + (vTrackWidth - metrics.ThumbSize) / 2;
                
                metrics.ThumbOffRect = new Rectangle(thumbX, trackY + metrics.TrackPadding, metrics.ThumbSize, metrics.ThumbSize);
                metrics.ThumbOnRect = new Rectangle(thumbX, metrics.TrackRect.Bottom - metrics.ThumbSize - metrics.TrackPadding, metrics.ThumbSize, metrics.ThumbSize);
                
                metrics.OnLabelRect = new Rectangle(0, padding, owner.Width, onSize.Height);
                metrics.OffLabelRect = new Rectangle(0, metrics.TrackRect.Bottom + metrics.LabelPadding, owner.Width, offSize.Height);
            }
            
            UpdateThumbPosition(metrics);
        }

        private void UpdateThumbPosition(SwitchMetrics metrics)
        {
            int startX = metrics.ThumbOffRect.X;
            int startY = metrics.ThumbOffRect.Y;
            int endX = metrics.ThumbOnRect.X;
            int endY = metrics.ThumbOnRect.Y;
            
            int currentX = (int)(startX + (endX - startX) * metrics.AnimationProgress);
            int currentY = (int)(startY + (endY - startY) * metrics.AnimationProgress);
            
            metrics.ThumbCurrentRect = new Rectangle(currentX, currentY, metrics.ThumbSize, metrics.ThumbSize);
        }

        public void PaintTrack(Graphics g, BeepSwitch owner, GraphicsPath trackPath, SwitchState state)
        {
            var theme = owner._currentTheme;
            var controlState = ConvertToControlState(state);
            
            var backgroundPainter = BackgroundPainterFactory.CreatePainter(owner.ControlStyle);
            backgroundPainter?.Paint(g, trackPath, owner.ControlStyle, theme, owner.UseThemeColors, controlState);
            
            if (owner.Checked && !string.IsNullOrEmpty(owner.OnImagePath))
            {
                StyledImagePainter.PaintWithTint(g, trackPath, owner.OnImagePath, theme?.PrimaryColor ?? Color.Blue, opacity: 0.2f);
            }
            
            var borderPainter = BorderPainterFactory.CreatePainter(owner.ControlStyle);
            borderPainter?.Paint(g, trackPath, false, owner.ControlStyle, theme, owner.UseThemeColors, controlState);
        }

        public void PaintThumb(Graphics g, BeepSwitch owner, Rectangle thumbRect, SwitchState state)
        {
            using (var thumbPath = new GraphicsPath())
            {
                thumbPath.AddEllipse(thumbRect);
                
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, thumbPath);
                }
                
                using (var pen = new Pen(Color.FromArgb(25, 0, 0, 0), 1.5f))
                {
                    g.DrawPath(pen, thumbPath);
                }
            }
        }

        public void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect)
        {
            var theme = owner._currentTheme;
            Color onColor = owner.Checked ? (theme?.PrimaryColor ?? Color.Blue) : (theme?.DisabledForeColor ?? Color.Gray);
            Color offColor = !owner.Checked ? (theme?.ForeColor ?? Color.Black) : (theme?.DisabledForeColor ?? Color.Gray);
            
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OnLabel, owner.Font, onLabelRect, onColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
                
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OffLabel, owner.Font, offLabelRect, offColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public int GetAnimationDuration() => 200;
        public float GetTrackSizeRatio() => 40f / 20f;  // 2.0
        public float GetThumbSizeRatio() => 0.85f;

        private ControlState ConvertToControlState(SwitchState state)
        {
            return state switch
            {
                SwitchState.Off_Hover or SwitchState.On_Hover => ControlState.Hovered,
                SwitchState.Off_Pressed or SwitchState.On_Pressed => ControlState.Pressed,
                SwitchState.Off_Disabled or SwitchState.On_Disabled => ControlState.Disabled,
                SwitchState.Off_Focused or SwitchState.On_Focused => ControlState.Focused,
                _ => ControlState.Normal
            };
        }
    }
}

