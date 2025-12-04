using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.Painters
{
    /// <summary>
    /// Minimal/Brutalist switch painter
    /// Features: Simple line track, small thumb, minimal decoration
    /// </summary>
    public class MinimalSwitchPainter : ISwitchPainter
    {
        private BeepSwitch _owner;

        public MinimalSwitchPainter(BeepSwitch owner)
        {
            _owner = owner;
        }

        public void CalculateLayout(BeepSwitch owner, SwitchMetrics metrics)
        {
            int padding = metrics.Padding;
            int trackHeight = Math.Min(20, owner.Height - (padding * 2));  // Thin track
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
                
                metrics.ThumbOffRect = new Rectangle(trackX, thumbY, metrics.ThumbSize, metrics.ThumbSize);
                metrics.ThumbOnRect = new Rectangle(metrics.TrackRect.Right - metrics.ThumbSize, thumbY, metrics.ThumbSize, metrics.ThumbSize);
                
                metrics.OffLabelRect = new Rectangle(padding, 0, offSize.Width, owner.Height);
                metrics.OnLabelRect = new Rectangle(metrics.TrackRect.Right + metrics.LabelPadding, 0, onSize.Width, owner.Height);
            }
            else
            {
                var onSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OnLabel, owner.Font);
                var offSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OffLabel, owner.Font);
                
                int vTrackWidth = Math.Min(20, owner.Width - (padding * 2));
                int vTrackHeight = (int)(vTrackWidth * GetTrackSizeRatio());
                int trackX = (owner.Width - vTrackWidth) / 2;
                int trackY = padding + onSize.Height + metrics.LabelPadding;
                
                metrics.TrackRect = new Rectangle(trackX, trackY, vTrackWidth, vTrackHeight);
                metrics.TrackWidth = vTrackWidth;
                metrics.TrackHeight = vTrackHeight;
                metrics.ThumbSize = vTrackWidth;  // Full width for minimal
                int thumbX = trackX;
                
                metrics.ThumbOffRect = new Rectangle(thumbX, trackY, metrics.ThumbSize, metrics.ThumbSize);
                metrics.ThumbOnRect = new Rectangle(thumbX, metrics.TrackRect.Bottom - metrics.ThumbSize, metrics.ThumbSize, metrics.ThumbSize);
                
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
            var theme = owner._currentTheme;  // BaseControl's protected field
            var controlState = ConvertToControlState(state);
            
            // Minimal: Just border, no fill
            var borderPainter = BorderPainterFactory.CreatePainter(owner.ControlStyle);
            if (borderPainter != null)
            {
                borderPainter.Paint(g, trackPath, false, owner.ControlStyle, theme, owner.UseThemeColors, controlState);
            }
        }

        public void PaintThumb(Graphics g, BeepSwitch owner, Rectangle thumbRect, SwitchState state)
        {
            using (var thumbPath = new GraphicsPath())
            {
                thumbPath.AddEllipse(thumbRect);
                
                // Minimal: Fill based on state
                Color thumbColor = owner.Checked 
                    ? (owner._currentTheme?.PrimaryColor ?? Color.Black)
                    : (owner._currentTheme?.SecondaryColor ?? Color.Gray);
                
                using (var brush = new SolidBrush(thumbColor))
                {
                    g.FillPath(brush, thumbPath);
                }
            }
        }

        public void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect)
        {
            var theme = owner._currentTheme;
            Color labelColor = theme?.ForeColor ?? Color.Black;
            
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OnLabel, owner.Font, onLabelRect, labelColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
                
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OffLabel, owner.Font, offLabelRect, labelColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public int GetAnimationDuration() => 150;  // Fast for minimal
        public float GetTrackSizeRatio() => 2.5f;  // Wider minimal track
        public float GetThumbSizeRatio() => 0.95f;  // Almost full height

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

