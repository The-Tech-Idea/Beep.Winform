using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Helper class for managing icons in toggle controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class ToggleIconHelpers
    {
        /// <summary>
        /// Gets the icon path for a specific toggle style and state
        /// Resolves icons from SvgsUI, custom paths, or fallback icons
        /// </summary>
        public static string GetIconPath(
            ToggleStyle toggleStyle,
            bool isOn,
            string customOnIconPath = null,
            string customOffIconPath = null)
        {
            // Priority 1: Custom icon paths (if provided)
            if (isOn && !string.IsNullOrEmpty(customOnIconPath))
                return customOnIconPath;
            if (!isOn && !string.IsNullOrEmpty(customOffIconPath))
                return customOffIconPath;

            // Priority 2: Style-specific icons from SvgsUI
            return toggleStyle switch
            {
                ToggleStyle.IconThumb => isOn ? SvgsUI.Check : SvgsUI.X,
                ToggleStyle.IconLock => isOn ? SvgsUI.Lock : SvgsUI.Unlock,
                ToggleStyle.IconSettings => SvgsUI.Settings ?? SvgsUI.Sliders ?? SvgsUI.Tool,
                ToggleStyle.IconMood => isOn ? SvgsUI.Smile ?? SvgsUI.Happy : SvgsUI.Frown ?? SvgsUI.Sad,
                ToggleStyle.IconCheck => isOn ? SvgsUI.CheckCircle : SvgsUI.XCircle,
                ToggleStyle.IconEye => isOn ? SvgsUI.Eye : SvgsUI.EyeOff,
                ToggleStyle.IconVolume => isOn ? SvgsUI.Volume : SvgsUI.VolumeX,
                ToggleStyle.IconMic => isOn ? SvgsUI.Mic : SvgsUI.MicOff,
                ToggleStyle.IconPower => isOn ? SvgsUI.Power : SvgsUI.PowerOff ?? SvgsUI.ZapOff,
                ToggleStyle.IconHeart => isOn ? SvgsUI.Heart : SvgsUI.HeartOff ?? SvgsUI.Heart,
                ToggleStyle.IconLike => isOn ? SvgsUI.ThumbsUp : SvgsUI.ThumbsDown,
                ToggleStyle.IconCustom => isOn 
                    ? (customOnIconPath ?? SvgsUI.Check)
                    : (customOffIconPath ?? SvgsUI.X),
                _ => isOn ? SvgsUI.Check : SvgsUI.X // Default fallback
            };
        }

        /// <summary>
        /// Gets the icon color based on toggle state and theme
        /// Integrates with ToggleThemeHelpers for theme-aware colors
        /// </summary>
        public static Color GetIconColor(
            BeepToggle toggle,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (toggle == null)
                return Color.Gray;

            // Disabled state always uses gray
            if (state == ControlState.Disabled)
                return Color.FromArgb(100, Color.Gray);

            // Use theme colors if available
            if (useThemeColors && theme != null)
            {
                return ToggleThemeHelpers.GetToggleTextColor(theme, useThemeColors, isOn);
            }

            // Use toggle's color properties
            return isOn ? toggle.OnColor : toggle.OffColor;
        }

        /// <summary>
        /// Calculates the appropriate icon size for a toggle
        /// Icons are sized as a percentage of thumb size
        /// </summary>
        public static Size GetIconSize(
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            Rectangle thumbBounds)
        {
            if (toggle == null || thumbBounds.IsEmpty)
                return new Size(16, 16);

            // Base icon size as percentage of thumb
            float iconRatio = toggleStyle switch
            {
                ToggleStyle.IconThumb => 0.5f,        // 50% of thumb
                ToggleStyle.IconCustom => 0.6f,       // 60% of thumb
                ToggleStyle.IconLock => 0.5f,
                ToggleStyle.IconSettings => 0.55f,
                ToggleStyle.IconMood => 0.6f,
                ToggleStyle.IconCheck => 0.5f,
                ToggleStyle.IconEye => 0.5f,
                ToggleStyle.IconVolume => 0.5f,
                ToggleStyle.IconMic => 0.5f,
                ToggleStyle.IconPower => 0.5f,
                ToggleStyle.IconHeart => 0.5f,
                ToggleStyle.IconLike => 0.5f,
                _ => 0.5f // Default 50%
            };

            int iconSize = (int)(Math.Min(thumbBounds.Width, thumbBounds.Height) * iconRatio);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 48));
            
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Paints an icon in a rectangle using StyledImagePainter
        /// Handles tinting, theme colors, and state-based styling
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle iconBounds,
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            if (iconBounds.IsEmpty || toggle == null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get icon path
            string iconPath = GetIconPath(
                toggleStyle,
                isOn,
                toggle.OnIconPath,
                toggle.OffIconPath);

            if (string.IsNullOrEmpty(iconPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(toggle, isOn, state, theme, useThemeColors);

            // Create GraphicsPath for icon bounds
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle))
            {
                // Paint icon with tinting using StyledImagePainter
                StyledImagePainter.PaintWithTint(
                    g,
                    iconPathShape,
                    iconPath,
                    iconColor,
                    1f,
                    0);
            }
        }

        /// <summary>
        /// Paints an icon in a circle using StyledImagePainter
        /// </summary>
        public static void PaintIconInCircle(
            Graphics g,
            float centerX,
            float centerY,
            float radius,
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (toggle == null || radius <= 0)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get icon path
            string iconPath = GetIconPath(
                toggleStyle,
                isOn,
                toggle.OnIconPath,
                toggle.OffIconPath);

            if (string.IsNullOrEmpty(iconPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(toggle, isOn, state, theme, useThemeColors);

            // Paint icon in circle using StyledImagePainter
            StyledImagePainter.PaintInCircle(
                g,
                centerX,
                centerY,
                radius,
                iconPath,
                iconColor,
                1f);
        }

        /// <summary>
        /// Paints an icon with a GraphicsPath using StyledImagePainter
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            GraphicsPath path,
            BeepToggle toggle,
            ToggleStyle toggleStyle,
            bool isOn,
            ControlState state,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            if (path == null || toggle == null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get icon path
            string iconPath = GetIconPath(
                toggleStyle,
                isOn,
                toggle.OnIconPath,
                toggle.OffIconPath);

            if (string.IsNullOrEmpty(iconPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(toggle, isOn, state, theme, useThemeColors);

            // Paint icon with path using StyledImagePainter
            StyledImagePainter.PaintWithTint(
                g,
                path,
                iconPath,
                iconColor,
                1f,
                0);
        }

        /// <summary>
        /// Resolves icon path from various sources
        /// Handles SvgsUI properties, file paths, and embedded resources
        /// </summary>
        public static string ResolveIconPath(
            string iconPath,
            ToggleStyle toggleStyle,
            bool isOn)
        {
            if (string.IsNullOrEmpty(iconPath))
            {
                // Fallback to style-specific default
                return GetIconPath(toggleStyle, isOn);
            }

            // If it's already a valid path (contains / or \ or has extension), return as-is
            if (iconPath.Contains("/") || iconPath.Contains("\\") || 
                iconPath.Contains(".svg") || iconPath.Contains(".png") || iconPath.Contains(".jpg"))
            {
                return iconPath;
            }

            // Try to resolve from SvgsUI static properties using reflection
            // This handles cases where iconPath is a property name like "Check" or "Lock"
            try
            {
                var svgsUIType = typeof(SvgsUI);
                var property = svgsUIType.GetProperty(iconPath, 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.Static | 
                    System.Reflection.BindingFlags.IgnoreCase);
                
                if (property != null)
                {
                    var value = property.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }
            catch
            {
                // Reflection failed, continue with fallback
            }

            // Fallback to style-specific default
            return GetIconPath(toggleStyle, isOn);
        }

        /// <summary>
        /// Gets recommended icon paths for common toggle use cases
        /// </summary>
        public static (string onIcon, string offIcon) GetRecommendedIcons(string useCase)
        {
            return useCase.ToLowerInvariant() switch
            {
                "playpause" or "media" => (SvgsUI.Pause, SvgsUI.Play),
                "lock" or "security" => (SvgsUI.Lock, SvgsUI.Unlock),
                "wifi" or "network" => (SvgsUI.Wifi ?? SvgsUI.Signal, SvgsUI.WifiOff ?? SvgsUI.SignalOff),
                "volume" or "sound" => (SvgsUI.Volume, SvgsUI.VolumeX),
                "mic" or "microphone" => (SvgsUI.Mic, SvgsUI.MicOff),
                "eye" or "visibility" => (SvgsUI.Eye, SvgsUI.EyeOff),
                "heart" or "favorite" => (SvgsUI.Heart, SvgsUI.HeartOff ?? SvgsUI.Heart),
                "like" or "thumbs" => (SvgsUI.ThumbsUp, SvgsUI.ThumbsDown),
                "power" or "onoff" => (SvgsUI.Power, SvgsUI.PowerOff ?? SvgsUI.ZapOff),
                "theme" or "darkmode" => (SvgsUI.Moon ?? SvgsUI.Moon, SvgsUI.Sun ?? SvgsUI.Sun),
                "settings" or "gear" => (SvgsUI.Settings ?? SvgsUI.Sliders, SvgsUI.Settings ?? SvgsUI.Sliders),
                _ => (SvgsUI.Check, SvgsUI.X) // Default
            };
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle)
        {
            var path = new GraphicsPath();
            
            // Most icons are circular or square
            // Use circle for most styles, square for some
            bool useCircle = controlStyle switch
            {
                BeepControlStyle.Material3 => true,
                BeepControlStyle.iOS15 => true,
                BeepControlStyle.MacOSBigSur => true,
                BeepControlStyle.NeoBrutalist => false, // Square for brutalist
                BeepControlStyle.HighContrast => false, // Square for high contrast
                _ => true // Default to circle
            };

            if (useCircle)
            {
                float centerX = bounds.X + bounds.Width / 2f;
                float centerY = bounds.Y + bounds.Height / 2f;
                float radius = Math.Min(bounds.Width, bounds.Height) / 2f;
                path.AddEllipse(bounds);
            }
            else
            {
                path.AddRectangle(bounds);
            }

            return path;
        }

        /// <summary>
        /// Calculates icon bounds within thumb bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateIconBounds(
            Rectangle thumbBounds,
            BeepToggle toggle,
            ToggleStyle toggleStyle)
        {
            if (thumbBounds.IsEmpty || toggle == null)
                return Rectangle.Empty;

            Size iconSize = GetIconSize(toggle, toggleStyle, thumbBounds);
            
            int x = thumbBounds.X + (thumbBounds.Width - iconSize.Width) / 2;
            int y = thumbBounds.Y + (thumbBounds.Height - iconSize.Height) / 2;
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }
    }
}

