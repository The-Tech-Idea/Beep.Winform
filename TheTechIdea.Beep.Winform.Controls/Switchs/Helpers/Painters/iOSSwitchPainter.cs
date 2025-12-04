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
    /// iOS-style switch painter
    /// Perfect pill shape with smooth animation
    /// Colors: iOS green when on, light gray when off
    /// </summary>
    public class iOSSwitchPainter : ISwitchPainter
    {
        private BeepSwitch _owner;

        public iOSSwitchPainter(BeepSwitch owner)
        {
            _owner = owner;
        }

        public void CalculateLayout(BeepSwitch owner, SwitchMetrics metrics)
        {
            // iOS switch proportions: 51x31px
            int padding = metrics.Padding;
            
            // Calculate track size based on control size
            int trackHeight = owner.Height - (padding * 2);
            int trackWidth = (int)(trackHeight * GetTrackSizeRatio());
            
            // Position track based on orientation
            if (owner.Orientation == SwitchOrientation.Horizontal)
            {
                // Measure label sizes
                var offSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OffLabel, owner.Font);
                var onSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OnLabel, owner.Font);
                
                int trackX = padding + offSize.Width + metrics.LabelPadding;
                int trackY = (owner.Height - trackHeight) / 2;
                
                metrics.TrackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);
                metrics.TrackWidth = trackWidth;
                metrics.TrackHeight = trackHeight;
                
                // Thumb size (slightly smaller than track height)
                metrics.ThumbSize = (int)(trackHeight * GetThumbSizeRatio());
                int thumbY = trackY + (trackHeight - metrics.ThumbSize) / 2;
                
                // Thumb positions
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
                
                // Label positions
                metrics.OffLabelRect = new Rectangle(
                    padding,
                    0,
                    offSize.Width,
                    owner.Height
                );
                metrics.OnLabelRect = new Rectangle(
                    metrics.TrackRect.Right + metrics.LabelPadding,
                    0,
                    onSize.Width,
                    owner.Height
                );
            }
            else // Vertical
            {
                // Vertical orientation (similar calculation)
                var onSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OnLabel, owner.Font);
                var offSize = System.Windows.Forms.TextRenderer.MeasureText(owner.OffLabel, owner.Font);
                
                int vTrackWidth2 = owner.Width - (padding * 2);
                int vTrackHeight2 = (int)(vTrackWidth2 / GetTrackSizeRatio());
                int trackX = (owner.Width - vTrackWidth2) / 2;
                int trackY = padding + onSize.Height + metrics.LabelPadding;
                
                metrics.TrackRect = new Rectangle(trackX, trackY, vTrackWidth2, vTrackHeight2);
                metrics.TrackWidth = vTrackWidth2;
                metrics.TrackHeight = vTrackHeight2;
                
                metrics.ThumbSize = (int)(vTrackWidth2 * GetThumbSizeRatio());
                int thumbX = trackX + (vTrackWidth2 - metrics.ThumbSize) / 2;
                
                metrics.ThumbOffRect = new Rectangle(
                    thumbX,
                    trackY + metrics.TrackPadding,
                    metrics.ThumbSize,
                    metrics.ThumbSize
                );
                metrics.ThumbOnRect = new Rectangle(
                    thumbX,
                    metrics.TrackRect.Bottom - metrics.ThumbSize - metrics.TrackPadding,
                    metrics.ThumbSize,
                    metrics.ThumbSize
                );
                
                // Label positions
                metrics.OnLabelRect = new Rectangle(0, padding, owner.Width, onSize.Height);
                metrics.OffLabelRect = new Rectangle(0, metrics.TrackRect.Bottom + metrics.LabelPadding, owner.Width, offSize.Height);
            }
            
            // Set current thumb position based on animation progress
            UpdateThumbPosition(metrics);
        }

        private void UpdateThumbPosition(SwitchMetrics metrics)
        {
            // Interpolate between off and on positions
            int startX = metrics.ThumbOffRect.X;
            int startY = metrics.ThumbOffRect.Y;
            int endX = metrics.ThumbOnRect.X;
            int endY = metrics.ThumbOnRect.Y;
            
            int currentX = (int)(startX + (endX - startX) * metrics.AnimationProgress);
            int currentY = (int)(startY + (endY - startY) * metrics.AnimationProgress);
            
            metrics.ThumbCurrentRect = new Rectangle(
                currentX,
                currentY,
                metrics.ThumbSize,
                metrics.ThumbSize
            );
        }

        public void PaintTrack(Graphics g, BeepSwitch owner, GraphicsPath trackPath, SwitchState state)
        {
            // Use BackgroundPainterFactory for consistent styling!
            var theme = owner._currentTheme;  // BaseControl's protected field
            var controlState = ConvertToControlState(state);
            
            // Get background painter
            var backgroundPainter = BackgroundPainterFactory.CreatePainter(owner.ControlStyle);
            if (backgroundPainter != null)
            {
                backgroundPainter.Paint(g, trackPath, owner.ControlStyle, theme, 
                    owner.UseThemeColors, controlState);
            }
            
            // Add track image if specified (using StyledImagePainter!)
            if (owner.Checked && !string.IsNullOrEmpty(owner.OnImagePath))
            {
                StyledImagePainter.PaintWithTint(g, trackPath, 
                    owner.OnImagePath, 
                    theme?.SuccessColor ?? Color.Green, 
                    opacity: 0.2f);
            }
            else if (!owner.Checked && !string.IsNullOrEmpty(owner.OffImagePath))
            {
                StyledImagePainter.PaintWithTint(g, trackPath, 
                    owner.OffImagePath, 
                    theme?.SecondaryColor ?? Color.Gray, 
                    opacity: 0.3f);
            }
            
            // Border
            var borderPainter = BorderPainterFactory.CreatePainter(owner.ControlStyle);
            if (borderPainter != null)
            {
                borderPainter.Paint(g, trackPath, false, owner.ControlStyle, theme, 
                    owner.UseThemeColors, controlState);
            }
        }

        public void PaintThumb(Graphics g, BeepSwitch owner, Rectangle thumbRect, SwitchState state)
        {
            // iOS-style thumb: White circle with subtle shadow
            using (var thumbPath = new GraphicsPath())
            {
                thumbPath.AddEllipse(thumbRect);
                
                // White fill (always white for iOS style)
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, thumbPath);
                }
                
                // Subtle border
                using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0), 1f))
                {
                    g.DrawPath(pen, thumbPath);
                }
                
                // Optional: Draw icon in thumb (using icon library!)
                if (owner.Checked && !string.IsNullOrEmpty(owner.OnIconName))
                {
                    DrawThumbIcon(g, thumbRect, owner.OnIconName, owner._currentTheme?.SuccessColor ?? Color.Green);
                }
                else if (!owner.Checked && !string.IsNullOrEmpty(owner.OffIconName))
                {
                    DrawThumbIcon(g, thumbRect, owner.OffIconName, owner._currentTheme?.SecondaryColor ?? Color.Gray);
                }
            }
        }

        private void DrawThumbIcon(Graphics g, Rectangle thumbRect, string iconName, Color iconColor)
        {
            if (string.IsNullOrEmpty(iconName)) return;
            
            // Calculate icon rect (60% of thumb size, centered)
            int iconSize = (int)(thumbRect.Width * 0.6f);
            var iconRect = new Rectangle(
                thumbRect.X + (thumbRect.Width - iconSize) / 2,
                thumbRect.Y + (thumbRect.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
            
            // Get icon path from library (using reflection to find matching property)
            var iconProperty = typeof(SvgsUI).GetProperty(iconName.Replace("-", "").Replace("_", ""), 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);
            string iconPath = iconProperty?.GetValue(null) as string;
            if (string.IsNullOrEmpty(iconPath)) return;
            
            // Use StyledImagePainter with circle clipping!
            using (var iconPath2 = new GraphicsPath())
            {
                iconPath2.AddEllipse(iconRect);
                StyledImagePainter.PaintWithTint(g, iconPath2, iconPath, iconColor, opacity: 1.0f);
            }
        }

        public void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect)
        {
            // iOS style: Active label is bold/colored, inactive is dimmed
            var theme = owner._currentTheme;
            Color onColor = owner.Checked 
                ? (theme?.SuccessColor ?? Color.FromArgb(52, 199, 89))  // iOS green
                : (theme?.DisabledForeColor ?? Color.FromArgb(174, 174, 178));  // iOS disabled gray
            
            Color offColor = !owner.Checked 
                ? (theme?.ForeColor ?? Color.Black)
                : (theme?.DisabledForeColor ?? Color.FromArgb(174, 174, 178));
            
            // Draw labels
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OnLabel, owner.Font, onLabelRect, onColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
                
            System.Windows.Forms.TextRenderer.DrawText(g, owner.OffLabel, owner.Font, offLabelRect, offColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public int GetAnimationDuration()
        {
            return 300;  // iOS uses 300ms with spring easing
        }

        public float GetTrackSizeRatio()
        {
            return 51f / 31f;  // iOS switch ratio (1.645)
        }

        public float GetThumbSizeRatio()
        {
            return 0.87f;  // Thumb is 87% of track height
        }

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

