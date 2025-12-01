using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Ratings;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Helpers
{
    /// <summary>
    /// Centralized icon management for Rating controls
    /// Integrates with StyledImagePainter for consistent icon rendering
    /// Supports SVG icons from SvgsUI, custom paths, and theme-based tinting
    /// </summary>
    public static class RatingIconHelpers
    {
        #region Icon Path Resolution

        /// <summary>
        /// Get icon path for a rating style and state
        /// Resolves icons from SvgsUI, custom paths, or fallback icons
        /// </summary>
        public static string GetRatingIconPath(
            RatingStyle style,
            bool isFilled,
            int ratingIndex = 0,
            string customIconPath = null)
        {
            // Priority 1: Custom icon path (if provided)
            if (!string.IsNullOrEmpty(customIconPath))
                return customIconPath;

            // Priority 2: Style-specific icons from SvgsUI
            return style switch
            {
                RatingStyle.ClassicStar => isFilled ? SvgsUI.Star ?? "star.svg" : SvgsUI.Star ?? "star-outline.svg",
                RatingStyle.ModernStar => isFilled ? SvgsUI.Star ?? "star-rounded.svg" : SvgsUI.Star ?? "star-outline.svg",
                RatingStyle.Heart => isFilled ? SvgsUI.Heart ?? "heart.svg" : SvgsUI.Heart ?? SvgsUI.Heart ?? "heart-outline.svg",
                RatingStyle.Thumb => isFilled ? SvgsUI.ThumbsUp ?? "thumb-up.svg" : SvgsUI.ThumbsDown ?? "thumb-down.svg",
                RatingStyle.Circle => isFilled ? SvgsUI.Circle ?? "circle-filled.svg" : SvgsUI.Circle ?? "circle-outline.svg",
                RatingStyle.Emoji => GetEmojiIconPath(ratingIndex),
                RatingStyle.Bar => "bar.svg", // Bars are drawn, not icons
                RatingStyle.GradientStar => isFilled ? SvgsUI.Star ?? "star.svg" : SvgsUI.Star ?? "star-outline.svg",
                RatingStyle.Minimal => isFilled ? SvgsUI.Circle ?? "circle-filled.svg" : SvgsUI.Circle ?? "circle-outline.svg",
                _ => isFilled ? SvgsUI.Star ?? "star.svg" : SvgsUI.Star ?? "star-outline.svg"
            };
        }

        /// <summary>
        /// Get emoji icon path (returns emoji character or SVG path)
        /// </summary>
        private static string GetEmojiIconPath(int ratingIndex)
        {
            // Emoji ratings: 😀 (5), 😊 (4), 😐 (3), 😕 (2), 😢 (1)
            return ratingIndex switch
            {
                4 => "😀", // Excellent
                3 => "😊", // Very Good
                2 => "😐", // Good
                1 => "😕", // Fair
                0 => "😢", // Poor
                _ => "😐"  // Default
            };
        }

        /// <summary>
        /// Resolve icon path from multiple sources
        /// </summary>
        public static string ResolveIconPath(
            string iconPath,
            RatingStyle style,
            bool isFilled,
            int ratingIndex = 0)
        {
            // If custom path provided, use it
            if (!string.IsNullOrEmpty(iconPath))
                return iconPath;

            // Otherwise, get recommended icon for style
            return GetRatingIconPath(style, isFilled, ratingIndex);
        }

        /// <summary>
        /// Get recommended icon for a rating style
        /// </summary>
        public static string GetRecommendedIcon(RatingStyle style, bool isFilled)
        {
            return GetRatingIconPath(style, isFilled);
        }

        #endregion

        #region Icon Color Management

        /// <summary>
        /// Get icon color based on rating state and theme
        /// Integrates with RatingThemeHelpers for theme-aware colors
        /// </summary>
        public static Color GetIconColor(
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            bool isFilled,
            bool isHovered,
            Color? customFilledColor = null,
            Color? customEmptyColor = null,
            Color? customHoverColor = null)
        {
            // Hover state takes priority
            if (isHovered && customHoverColor.HasValue)
                return customHoverColor.Value;

            if (isHovered)
            {
                return RatingThemeHelpers.GetHoverRatingColor(theme, useThemeColors, style, null);
            }

            // Filled vs empty
            if (isFilled)
            {
                if (customFilledColor.HasValue)
                    return customFilledColor.Value;

                return RatingThemeHelpers.GetFilledRatingColor(theme, useThemeColors, style, null);
            }
            else
            {
                if (customEmptyColor.HasValue)
                    return customEmptyColor.Value;

                return RatingThemeHelpers.GetEmptyRatingColor(theme, useThemeColors, style, null);
            }
        }

        #endregion

        #region Icon Sizing

        /// <summary>
        /// Get icon size based on star size and rating style
        /// </summary>
        public static Size GetIconSize(
            int starSize,
            RatingStyle style)
        {
            if (starSize <= 0)
                return new Size(24, 24);

            // Icons are typically 80-100% of star size
            float iconRatio = style switch
            {
                RatingStyle.Heart => 0.9f,        // Hearts: 90% of star size
                RatingStyle.Thumb => 0.85f,        // Thumbs: 85% of star size
                RatingStyle.Circle => 0.8f,        // Circles: 80% of star size
                RatingStyle.Emoji => 1.0f,         // Emojis: 100% of star size (text-based)
                RatingStyle.Bar => starSize,       // Bars: full height
                RatingStyle.Minimal => 0.7f,        // Minimal: 70% of star size
                _ => 0.95f                         // Stars: 95% of star size
            };

            int iconSize = (int)(starSize * iconRatio);
            return new Size(iconSize, iconSize);
        }

        /// <summary>
        /// Calculate icon bounds within rating bounds
        /// </summary>
        public static Rectangle CalculateIconBounds(
            Rectangle ratingBounds,
            Size iconSize,
            int index,
            int starCount,
            int spacing,
            int starSize)
        {
            // Calculate star positions (same as in painters)
            int totalWidth = (starSize * starCount) + (spacing * (starCount - 1));
            int startX = ratingBounds.Left + (ratingBounds.Width - totalWidth) / 2;
            int startY = ratingBounds.Top + (ratingBounds.Height - starSize) / 2;

            int starX = startX + index * (starSize + spacing);
            int starY = startY;

            // Center icon within star bounds
            int iconX = starX + (starSize - iconSize.Width) / 2;
            int iconY = starY + (starSize - iconSize.Height) / 2;

            return new Rectangle(iconX, iconY, iconSize.Width, iconSize.Height);
        }

        #endregion

        #region Icon Painting

        /// <summary>
        /// Paint icon using StyledImagePainter
        /// Supports SVG, caching, and theme-aware tinting
        /// </summary>
        public static void PaintIcon(
            Graphics g,
            Rectangle bounds,
            string iconPath,
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            bool isFilled,
            bool isHovered,
            Color? tintColor = null,
            float rotation = 0f)
        {
            if (g == null || bounds.IsEmpty || string.IsNullOrEmpty(iconPath))
                return;

            // Get icon color
            Color iconColor = tintColor ?? GetIconColor(
                theme,
                useThemeColors,
                style,
                isFilled,
                isHovered);

            // For emoji, draw as text instead of icon
            if (style == RatingStyle.Emoji && iconPath.Length == 1 && char.IsSurrogate(iconPath[0]))
            {
                PaintEmoji(g, bounds, iconPath, iconColor);
                return;
            }

            // Use StyledImagePainter for SVG/icons
            try
            {
                StyledImagePainter.PaintWithTint(
                    g,
                    bounds,
                    iconPath,
                    iconColor);
            }
            catch
            {
                // Fallback: draw a simple shape if icon fails to load
                PaintFallbackIcon(g, bounds, style, iconColor, isFilled);
            }
        }

        /// <summary>
        /// Paint icon in a circle background
        /// </summary>
        public static void PaintIconInCircle(
            Graphics g,
            Rectangle bounds,
            string iconPath,
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            bool isFilled,
            bool isHovered,
            Color? circleBackColor = null,
            Color? circleBorderColor = null)
        {
            if (g == null || bounds.IsEmpty)
                return;

            // Draw circle background
            Color backColor = circleBackColor ?? Color.Transparent;
            if (backColor != Color.Transparent)
            {
                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    g.FillEllipse(brush, bounds);
                }
            }

            // Draw circle border
            if (circleBorderColor.HasValue)
            {
                using (Pen pen = new Pen(circleBorderColor.Value, 1))
                {
                    g.DrawEllipse(pen, bounds);
                }
            }

            // Draw icon (slightly smaller to fit in circle)
            Rectangle iconBounds = new Rectangle(
                bounds.X + bounds.Width / 8,
                bounds.Y + bounds.Height / 8,
                bounds.Width * 3 / 4,
                bounds.Height * 3 / 4);

            PaintIcon(g, iconBounds, iconPath, theme, useThemeColors, style, isFilled, isHovered);
        }

        /// <summary>
        /// Paint icon with a GraphicsPath (for custom shapes)
        /// </summary>
        public static void PaintIconWithPath(
            Graphics g,
            Rectangle bounds,
            GraphicsPath path,
            IBeepTheme theme,
            bool useThemeColors,
            RatingStyle style,
            bool isFilled,
            bool isHovered,
            Color? fillColor = null,
            Color? borderColor = null)
        {
            if (g == null || bounds.IsEmpty || path == null)
                return;

            // Get colors
            Color fill = fillColor ?? GetIconColor(theme, useThemeColors, style, isFilled, isHovered);
            Color border = borderColor ?? RatingThemeHelpers.GetRatingBorderColor(theme, useThemeColors, style, null);

            // Scale path to bounds
            RectangleF pathBounds = path.GetBounds();
            if (!pathBounds.IsEmpty)
            {
                float scaleX = bounds.Width / pathBounds.Width;
                float scaleY = bounds.Height / pathBounds.Height;
                float scale = Math.Min(scaleX, scaleY);

                using (Matrix matrix = new Matrix())
                {
                    matrix.Translate(bounds.X + bounds.Width / 2f - pathBounds.Width * scale / 2f,
                                    bounds.Y + bounds.Height / 2f - pathBounds.Height * scale / 2f);
                    matrix.Scale(scale, scale);
                    path.Transform(matrix);
                }
            }

            // Fill path
            using (SolidBrush brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            // Draw border
            using (Pen pen = new Pen(border, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Paint emoji as text
        /// </summary>
        private static void PaintEmoji(Graphics g, Rectangle bounds, string emoji, Color color)
        {
            if (string.IsNullOrEmpty(emoji))
                return;

            using (SolidBrush brush = new SolidBrush(color))
            {
                // Use a large font for emoji
                Font emojiFont = new Font("Segoe UI Emoji", bounds.Height * 0.8f, FontStyle.Regular);
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(emoji, emojiFont, brush, bounds, format);
                emojiFont.Dispose();
            }
        }

        /// <summary>
        /// Paint fallback icon when icon path fails
        /// </summary>
        private static void PaintFallbackIcon(
            Graphics g,
            Rectangle bounds,
            RatingStyle style,
            Color color,
            bool isFilled)
        {
            switch (style)
            {
                case RatingStyle.Circle:
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillEllipse(brush, bounds);
                    }
                    break;

                case RatingStyle.Heart:
                    // Draw simple heart shape
                    using (GraphicsPath heartPath = CreateHeartPath(bounds))
                    {
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            g.FillPath(brush, heartPath);
                        }
                    }
                    break;

                default:
                    // Draw simple star
                    using (GraphicsPath starPath = CreateStarPath(bounds))
                    {
                        using (SolidBrush brush = new SolidBrush(color))
                        {
                            g.FillPath(brush, starPath);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Create a simple heart path
        /// </summary>
        private static GraphicsPath CreateHeartPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float radius = Math.Min(bounds.Width, bounds.Height) / 2f;

            // Simple heart shape
            path.AddBezier(
                centerX, centerY + radius * 0.3f,
                centerX - radius * 0.5f, centerY - radius * 0.2f,
                centerX - radius * 0.3f, centerY - radius * 0.5f,
                centerX, centerY - radius * 0.3f);
            path.AddBezier(
                centerX, centerY - radius * 0.3f,
                centerX + radius * 0.3f, centerY - radius * 0.5f,
                centerX + radius * 0.5f, centerY - radius * 0.2f,
                centerX, centerY + radius * 0.3f);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Create a simple star path
        /// </summary>
        private static GraphicsPath CreateStarPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float outerRadius = Math.Min(bounds.Width, bounds.Height) / 2f;
            float innerRadius = outerRadius * 0.4f;
            int points = 5;

            PointF[] starPoints = new PointF[points * 2];
            double angleStep = Math.PI / points;
            double angle = -Math.PI / 2; // Start at top

            for (int i = 0; i < starPoints.Length; i++)
            {
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                starPoints[i] = new PointF(
                    centerX + (float)(Math.Cos(angle) * radius),
                    centerY + (float)(Math.Sin(angle) * radius));
                angle += angleStep;
            }

            path.AddPolygon(starPoints);
            return path;
        }

        #endregion
    }
}

