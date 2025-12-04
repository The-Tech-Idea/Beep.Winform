using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.Painters
{
    /// <summary>
    /// Material Design 3 switch painter
    /// Features: Tonal surfaces, state layers, elevated thumb, 48dp touch targets
    /// </summary>
    public class Material3SwitchPainter : ISwitchPainter
    {
        private BeepSwitch _owner;

        public Material3SwitchPainter(BeepSwitch owner)
        {
            _owner = owner;
        }

        public void CalculateLayout(BeepSwitch owner, SwitchMetrics metrics)
        {
            // Material 3: 52x32px switch
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
                
                metrics.ThumbOffRect = new Rectangle(
                    trackX + metrics.TrackPadding,
                    thumbY,
                    metrics.ThumbSize,
                    metrics.ThumbSize
                );
                metrics.ThumbOnRect = new Rectangle(
                    metrics.TrackRect.Right - metrics.ThumbSize - metrics.TrackPadding,
                    thumbY,
                    metrics.ThumbSize,
                    metrics.ThumbSize
                );
                
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
            
            // Use BackgroundPainterFactory!
            var backgroundPainter = BackgroundPainterFactory.CreatePainter(owner.ControlStyle);
            if (backgroundPainter != null)
            {
                backgroundPainter.Paint(g, trackPath, owner.ControlStyle, theme, owner.UseThemeColors, controlState);
            }
            
            // Track image (using StyledImagePainter!)
            if (owner.Checked && !string.IsNullOrEmpty(owner.OnImagePath))
            {
                StyledImagePainter.PaintWithTint(g, trackPath, owner.OnImagePath, theme?.SuccessColor ?? Color.Green, opacity: 0.15f);
            }
            else if (!owner.Checked && !string.IsNullOrEmpty(owner.OffImagePath))
            {
                StyledImagePainter.PaintWithTint(g, trackPath, owner.OffImagePath, theme?.SecondaryColor ?? Color.Gray, opacity: 0.2f);
            }
            
            // Border
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
                
                // Material 3: White thumb with subtle shadow
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, thumbPath);
                }
                
                using (var pen = new Pen(Color.FromArgb(40, 0, 0, 0), 1f))
                {
                    g.DrawPath(pen, thumbPath);
                }
                
                // Optional icon
                if (owner.Checked && !string.IsNullOrEmpty(owner.OnIconName))
                {
                    DrawThumbIcon(g, thumbRect, owner.OnIconName, owner._currentTheme?.SuccessColor ?? Color.Green);
                }
            }
        }

        private void DrawThumbIcon(Graphics g, Rectangle thumbRect, string iconName, Color iconColor)
        {
            if (string.IsNullOrEmpty(iconName)) return;
            
            int iconSize = (int)(thumbRect.Width * 0.55f);
            var iconRect = new Rectangle(
                thumbRect.X + (thumbRect.Width - iconSize) / 2,
                thumbRect.Y + (thumbRect.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
            
            var iconProperty = typeof(SvgsUI).GetProperty(iconName.Replace("-", "").Replace("_", ""), 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            string iconPath = iconProperty?.GetValue(null) as string;
            if (!string.IsNullOrEmpty(iconPath))
            {
                using (var iconPath2 = new GraphicsPath())
                {
                    iconPath2.AddEllipse(iconRect);
                    StyledImagePainter.PaintWithTint(g, iconPath2, iconPath, iconColor, opacity: 1.0f);
                }
            }
        }

        public void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect)
        {
            var theme = owner._currentTheme;
            Color onColor = owner.Checked 
                ? (theme?.SuccessColor ?? Color.FromArgb(76, 175, 80))
                : (theme?.DisabledForeColor ?? Color.FromArgb(180, 180, 180));
            
            Color offColor = !owner.Checked 
                ? (theme?.ForeColor ?? Color.Black)
                : (theme?.DisabledForeColor ?? Color.FromArgb(180, 180, 180));
            
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OnLabel, owner.Font, onLabelRect, onColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
                
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OffLabel, owner.Font, offLabelRect, offColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public int GetAnimationDuration() => 200;
        public float GetTrackSizeRatio() => 52f / 32f;  // 1.625
        public float GetThumbSizeRatio() => 0.88f;

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

