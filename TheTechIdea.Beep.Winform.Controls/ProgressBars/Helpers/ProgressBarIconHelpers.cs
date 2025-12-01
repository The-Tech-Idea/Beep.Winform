using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers
{
    /// <summary>
    /// Helper class for managing icons in progress bar controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class ProgressBarIconHelpers
    {
        /// <summary>
        /// Gets the recommended icon path for a specific progress bar painter kind
        /// Resolves icons from SvgsUI or returns custom paths
        /// </summary>
        public static string GetIconPath(
            ProgressPainterKind painterKind,
            string customIconPath = null)
        {
            // Priority 1: Custom icon path (if provided)
            if (!string.IsNullOrEmpty(customIconPath))
                return customIconPath;

            // Priority 2: Painter-specific icons from SvgsUI
            return painterKind switch
            {
                ProgressPainterKind.RingCenterImage => SvgsUI.Activity ?? SvgsUI.Loader ?? SvgsUI.Circle,
                ProgressPainterKind.LinearTrackerIcon => SvgsUI.ArrowRight ?? SvgsUI.ChevronRight ?? SvgsUI.Navigation,
                ProgressPainterKind.LinearBadge => SvgsUI.CheckCircle ?? SvgsUI.Circle,
                ProgressPainterKind.StepperCircles => SvgsUI.Circle ?? SvgsUI.CheckCircle,
                ProgressPainterKind.ChevronSteps => SvgsUI.ChevronRight ?? SvgsUI.ArrowRight,
                ProgressPainterKind.DotsLoader => SvgsUI.Circle ?? SvgsUI.Loader,
                ProgressPainterKind.ArrowStripe => SvgsUI.ArrowRight ?? SvgsUI.ChevronRight,
                ProgressPainterKind.ArrowHeadAnimated => SvgsUI.ArrowRight ?? SvgsUI.Navigation,
                _ => SvgsUI.Circle // Default fallback
            };
        }

        /// <summary>
        /// Gets the icon color based on progress bar state and theme
        /// Integrates with ProgressBarThemeHelpers for theme-aware colors
        /// </summary>
        public static Color GetIconColor(
            BeepProgressBar progressBar,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (progressBar == null)
                return Color.Gray;

            // Use theme colors if available
            if (useThemeColors && theme != null)
            {
                // Icons typically use the progress color (foreground)
                return ProgressBarThemeHelpers.GetProgressBarForeColor(theme, useThemeColors, progressBar.ProgressColor);
            }

            // Use progress bar's progress color
            return progressBar.ProgressColor != Color.Empty ? progressBar.ProgressColor : Color.FromArgb(52, 152, 219);
        }

        /// <summary>
        /// Calculates the appropriate icon size for a progress bar
        /// Icons are sized based on ProgressBarSize and bounds
        /// </summary>
        public static Size GetIconSize(
            BeepProgressBar progressBar,
            ProgressPainterKind painterKind,
            Rectangle bounds)
        {
            if (progressBar == null || bounds.IsEmpty)
                return new Size(16, 16);

            // Base icon size as percentage of bounds
            float iconRatio = painterKind switch
            {
                ProgressPainterKind.RingCenterImage => 0.3f,        // 30% of ring height (center)
                ProgressPainterKind.LinearTrackerIcon => 1.2f,       // 120% of bar height (above bar)
                ProgressPainterKind.LinearBadge => 0.8f,            // 80% of bar height
                ProgressPainterKind.StepperCircles => 0.6f,         // 60% of step height
                ProgressPainterKind.ChevronSteps => 0.5f,           // 50% of step height
                ProgressPainterKind.DotsLoader => 0.7f,             // 70% of bar height
                ProgressPainterKind.ArrowStripe => 0.4f,            // 40% of bar height
                ProgressPainterKind.ArrowHeadAnimated => 0.6f,      // 60% of bar height
                _ => 0.5f // Default 50%
            };

            // Adjust based on ProgressBarSize
            float sizeMultiplier = progressBar.BarSize switch
            {
                ProgressBarSize.Thin => 0.7f,
                ProgressBarSize.Small => 0.85f,
                ProgressBarSize.Medium => 1.0f,
                ProgressBarSize.Large => 1.2f,
                ProgressBarSize.ExtraLarge => 1.4f,
                _ => 1.0f
            };

            int iconSize = (int)(Math.Min(bounds.Width, bounds.Height) * iconRatio * sizeMultiplier);
            
            // Ensure minimum and maximum sizes
            iconSize = Math.Max(12, Math.Min(iconSize, 64));
            
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Calculates icon bounds within progress bar bounds
        /// Centers the icon and applies appropriate sizing
        /// </summary>
        public static Rectangle CalculateIconBounds(
            Rectangle bounds,
            BeepProgressBar progressBar,
            ProgressPainterKind painterKind,
            string position = "center")
        {
            if (bounds.IsEmpty || progressBar == null)
                return Rectangle.Empty;

            Size iconSize = GetIconSize(progressBar, painterKind, bounds);
            
            int x, y;

            switch (position.ToLowerInvariant())
            {
                case "center":
                    x = bounds.X + (bounds.Width - iconSize.Width) / 2;
                    y = bounds.Y + (bounds.Height - iconSize.Height) / 2;
                    break;
                case "top":
                    x = bounds.X + (bounds.Width - iconSize.Width) / 2;
                    y = bounds.Y;
                    break;
                case "bottom":
                    x = bounds.X + (bounds.Width - iconSize.Width) / 2;
                    y = bounds.Bottom - iconSize.Height;
                    break;
                case "left":
                    x = bounds.X;
                    y = bounds.Y + (bounds.Height - iconSize.Height) / 2;
                    break;
                case "right":
                    x = bounds.Right - iconSize.Width;
                    y = bounds.Y + (bounds.Height - iconSize.Height) / 2;
                    break;
                default:
                    x = bounds.X + (bounds.Width - iconSize.Width) / 2;
                    y = bounds.Y + (bounds.Height - iconSize.Height) / 2;
                    break;
            }
            
            return new Rectangle(x, y, iconSize.Width, iconSize.Height);
        }

        /// <summary>
        /// Paints an icon in a rectangle using StyledImagePainter
        /// Handles tinting, theme colors, and state-based styling
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle iconBounds,
            BeepProgressBar progressBar,
            ProgressPainterKind painterKind,
            string iconPath,
            IBeepTheme theme = null,
            bool useThemeColors = false,
            BeepControlStyle controlStyle = BeepControlStyle.Material3)
        {
            if (iconBounds.IsEmpty || progressBar == null || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath, painterKind);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(progressBar, theme, useThemeColors);

            // Create GraphicsPath for icon bounds (square or circle based on style)
            using (var iconPathShape = CreateIconPath(iconBounds, controlStyle))
            {
                // Paint icon with tinting using StyledImagePainter
                StyledImagePainter.PaintWithTint(
                    g,
                    iconPathShape,
                    resolvedPath,
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
            BeepProgressBar progressBar,
            string iconPath,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (progressBar == null || radius <= 0 || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath, ProgressPainterKind.RingCenterImage);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(progressBar, theme, useThemeColors);

            // Paint icon in circle using StyledImagePainter
            StyledImagePainter.PaintInCircle(
                g,
                centerX,
                centerY,
                radius,
                resolvedPath,
                iconColor,
                1f);
        }

        /// <summary>
        /// Paints an icon with a GraphicsPath using StyledImagePainter
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            GraphicsPath path,
            BeepProgressBar progressBar,
            string iconPath,
            IBeepTheme theme = null,
            bool useThemeColors = false)
        {
            if (path == null || progressBar == null || string.IsNullOrEmpty(iconPath))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Resolve icon path
            string resolvedPath = ResolveIconPath(iconPath, ProgressPainterKind.RingCenterImage);

            if (string.IsNullOrEmpty(resolvedPath))
                return;

            // Get icon color
            Color iconColor = GetIconColor(progressBar, theme, useThemeColors);

            // Paint icon with path using StyledImagePainter
            StyledImagePainter.PaintWithTint(
                g,
                path,
                resolvedPath,
                iconColor,
                1f,
                0);
        }

        /// <summary>
        /// Resolves icon path from various sources
        /// Handles SvgsUI properties, file paths, and embedded resources
        /// </summary>
        public static string ResolveIconPath(string iconPath, ProgressPainterKind painterKind)
        {
            if (string.IsNullOrEmpty(iconPath))
            {
                // Fallback to recommended icon for painter kind
                return GetIconPath(painterKind);
            }

            // If it's already a valid path (contains / or \ or has extension), return as-is
            if (iconPath.Contains("/") || iconPath.Contains("\\") || 
                iconPath.Contains(".svg") || iconPath.Contains(".png") || iconPath.Contains(".jpg"))
            {
                return iconPath;
            }

            // Try to resolve from SvgsUI static properties using reflection
            // This handles cases where iconPath is a property name like "Activity" or "Loader"
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

            // Fallback to recommended icon for painter kind
            return GetIconPath(painterKind);
        }

        /// <summary>
        /// Creates a GraphicsPath for icon bounds based on ControlStyle
        /// </summary>
        private static GraphicsPath CreateIconPath(Rectangle bounds, BeepControlStyle controlStyle)
        {
            var path = new GraphicsPath();
            
            // Most icons are square or circular
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
                path.AddEllipse(bounds);
            }
            else
            {
                path.AddRectangle(bounds);
            }

            return path;
        }

        /// <summary>
        /// Gets recommended icon paths for common progress bar use cases
        /// </summary>
        public static string GetRecommendedIcon(string useCase)
        {
            return useCase.ToLowerInvariant() switch
            {
                "loading" or "spinner" => SvgsUI.Loader ?? SvgsUI.Activity,
                "progress" or "activity" => SvgsUI.Activity ?? SvgsUI.Loader,
                "check" or "complete" => SvgsUI.CheckCircle ?? SvgsUI.Check,
                "arrow" or "next" => SvgsUI.ArrowRight ?? SvgsUI.ChevronRight,
                "tracker" or "marker" => SvgsUI.Navigation ?? SvgsUI.ArrowRight,
                "badge" or "label" => SvgsUI.Circle ?? SvgsUI.CheckCircle,
                "step" or "circle" => SvgsUI.Circle ?? SvgsUI.CheckCircle,
                "dot" => SvgsUI.Circle,
                _ => SvgsUI.Circle // Default
            };
        }
    }
}

