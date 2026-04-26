using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Switchs.Models;
using TheTechIdea.Beep.Winform.Controls.Switchs.Helpers;
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
            var theme = owner._currentTheme;
            bool isHovered = state == SwitchState.Off_Hover || state == SwitchState.On_Hover;
            bool isDisabled = state == SwitchState.Off_Disabled || state == SwitchState.On_Disabled;

            using (var thumbPath = new GraphicsPath())
            {
                thumbPath.AddEllipse(thumbRect);

                Color thumbColor = SwitchThemeHelpers.GetThumbColor(theme, owner.UseThemeColors, owner.Checked, isHovered, isDisabled);

                if (SwitchStyleHelpers.ShouldShowThumbShadow(owner.ControlStyle))
                {
                    Color shadowColor = SwitchThemeHelpers.GetThumbShadowColor(theme, owner.UseThemeColors, 2);
                    Point offset = SwitchStyleHelpers.GetShadowOffset(owner.ControlStyle);
                    Rectangle shadowRect = new Rectangle(
                        thumbRect.X + offset.X,
                        thumbRect.Y + offset.Y,
                        thumbRect.Width,
                        thumbRect.Height);
                    using (var shadowPath = new GraphicsPath())
                    {
                        shadowPath.AddEllipse(shadowRect);
                        using (var shadowBrush = new SolidBrush(shadowColor))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }

                using (var brush = new SolidBrush(thumbColor))
                {
                    g.FillPath(brush, thumbPath);
                }

                using (var pen = new Pen(Color.FromArgb(40, 0, 0, 0), 1f))
                {
                    g.DrawPath(pen, thumbPath);
                }

                if (owner.Checked && !string.IsNullOrEmpty(owner.OnIconName))
                {
                    DrawThumbIcon(g, thumbRect, owner.OnIconName, theme?.SuccessColor ?? Color.Green);
                }
            }
        }

        private void DrawThumbIcon(Graphics g, Rectangle thumbRect, string iconName, Color iconColor)
        {
            if (string.IsNullOrEmpty(iconName)) return;
            
            // Use icon helpers for consistent icon rendering
            string iconPath = TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchIconHelpers.GetSwitchIconPath(
                iconName, 
                isOn: true);
            
            Rectangle iconRect = TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchIconHelpers.CalculateThumbIconBounds(
                thumbRect, 
                thumbRect.Width);
            
            TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchIconHelpers.PaintIcon(
                g,
                iconRect,
                iconPath,
                iconColor,
                _owner._currentTheme,
                _owner.UseThemeColors,
                _owner.ControlStyle,
                1.0f);
        }

        public void PaintLabels(Graphics g, BeepSwitch owner, Rectangle onLabelRect, Rectangle offLabelRect)
        {
            var theme = owner._currentTheme;
            bool isDisabled = !owner.Enabled;

            Color onColor = SwitchThemeHelpers.GetLabelTextColor(theme, owner.UseThemeColors, isOn: true, isActive: owner.Checked, isDisabled);
            Color offColor = SwitchThemeHelpers.GetLabelTextColor(theme, owner.UseThemeColors, isOn: false, isActive: !owner.Checked, isDisabled);

            System.Windows.Forms.TextRenderer.DrawText(g, owner.OnLabel, owner.Font, onLabelRect, onColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);

            System.Windows.Forms.TextRenderer.DrawText(g, owner.OffLabel, owner.Font, offLabelRect, offColor,
                System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter);
        }

        public int GetAnimationDuration() => TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchStyleHelpers.GetAnimationDuration(_owner.ControlStyle);
        public float GetTrackSizeRatio() => TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchStyleHelpers.GetTrackSizeRatio(_owner.ControlStyle);
        public float GetThumbSizeRatio() => TheTechIdea.Beep.Winform.Controls.Switchs.Helpers.SwitchStyleHelpers.GetThumbSizeRatio(_owner.ControlStyle);

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

