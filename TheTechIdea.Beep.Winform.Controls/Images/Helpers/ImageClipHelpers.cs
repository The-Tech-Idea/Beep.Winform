using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Images.Helpers
{
    /// <summary>
    /// Helper class for creating clip paths and regions for image clipping
    /// Uses GraphicsExtensions for shape creation and supports region-based clipping
    /// </summary>
    public static class ImageClipHelpers
    {
        /// <summary>
        /// Creates a GraphicsPath for the specified clip shape
        /// Uses GraphicsExtensions methods for better shape creation
        /// </summary>
        public static GraphicsPath CreateClipPath(Rectangle bounds, ImageClipShape clipShape, float cornerRadius = 10f, GraphicsPath customPath = null)
        {
            if (clipShape == ImageClipShape.Custom && customPath != null)
            {
                return (GraphicsPath)customPath.Clone();
            }

            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float size = Math.Min(bounds.Width, bounds.Height) / 2f;

            return clipShape switch
            {
                ImageClipShape.Circle => GraphicsExtensions.CreateCircle(centerX, centerY, size),
                ImageClipShape.Ellipse => CreateEllipsePath(bounds),
                ImageClipShape.RoundedRect => GraphicsExtensions.GetRoundedRectPath(bounds, (int)cornerRadius),
                ImageClipShape.RoundedRectangle => GraphicsExtensions.GetRoundedRectPath(bounds, (int)cornerRadius),
                ImageClipShape.Diamond => GraphicsExtensions.CreateDiamond(centerX, centerY, bounds.Width, bounds.Height),
                ImageClipShape.Triangle => GraphicsExtensions.CreateTriangle(centerX, centerY, size),
                ImageClipShape.Hexagon => GraphicsExtensions.CreateHexagon(centerX, centerY, size, 0f),
                ImageClipShape.None => CreateRectanglePath(bounds),
                _ => CreateRectanglePath(bounds)
            };
        }

        /// <summary>
        /// Creates a Region for the specified clip shape (more efficient for complex shapes)
        /// </summary>
        public static Region CreateClipRegion(Rectangle bounds, ImageClipShape clipShape, float cornerRadius = 10f, GraphicsPath customPath = null)
        {
            using (var path = CreateClipPath(bounds, clipShape, cornerRadius, customPath))
            {
                return new Region(path);
            }
        }

        /// <summary>
        /// Creates an ellipse path that fills the bounds
        /// </summary>
        private static GraphicsPath CreateEllipsePath(Rectangle bounds)
        {
            var path = new GraphicsPath();
            path.AddEllipse(bounds);
            return path;
        }

        /// <summary>
        /// Creates a rectangle path
        /// </summary>
        private static GraphicsPath CreateRectanglePath(Rectangle bounds)
        {
            var path = new GraphicsPath();
            path.AddRectangle(bounds);
            return path;
        }

        /// <summary>
        /// Applies clipping to graphics using a GraphicsPath
        /// </summary>
        public static void ApplyClipPath(Graphics g, GraphicsPath clipPath)
        {
            if (clipPath != null && !clipPath.GetBounds().IsEmpty)
            {
                g.SetClip(clipPath);
            }
        }

        /// <summary>
        /// Applies clipping to graphics using a Region (more efficient for complex shapes)
        /// </summary>
        public static void ApplyClipRegion(Graphics g, Region clipRegion)
        {
            if (clipRegion != null && !clipRegion.IsEmpty(g))
            {
                g.Clip = clipRegion;
            }
        }

        /// <summary>
        /// Creates additional shape paths that can be used for clipping
        /// </summary>
        public static GraphicsPath CreatePentagonPath(Rectangle bounds, float rotation = 0f)
        {
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float size = Math.Min(bounds.Width, bounds.Height) / 2f;
            return GraphicsExtensions.CreatePentagon(centerX, centerY, size, rotation);
        }

        /// <summary>
        /// Creates an octagon path
        /// </summary>
        public static GraphicsPath CreateOctagonPath(Rectangle bounds, float rotation = 0f)
        {
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float size = Math.Min(bounds.Width, bounds.Height) / 2f;
            return GraphicsExtensions.CreateOctagon(centerX, centerY, size, rotation);
        }

        /// <summary>
        /// Creates a star path
        /// </summary>
        public static GraphicsPath CreateStarPath(Rectangle bounds, int points = 5, float rotation = 0f)
        {
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float outerRadius = Math.Min(bounds.Width, bounds.Height) / 2f;
            float innerRadius = outerRadius * 0.5f;
            return GraphicsExtensions.CreateStar(centerX, centerY, outerRadius, innerRadius, points, rotation);
        }

        /// <summary>
        /// Creates a pill/capsule path
        /// </summary>
        public static GraphicsPath CreatePillPath(Rectangle bounds)
        {
            return GraphicsExtensions.CreatePill(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        /// <summary>
        /// Creates a heart path
        /// </summary>
        public static GraphicsPath CreateHeartPath(Rectangle bounds)
        {
            float centerX = bounds.X + bounds.Width / 2f;
            float centerY = bounds.Y + bounds.Height / 2f;
            float size = Math.Min(bounds.Width, bounds.Height) / 2f;
            return GraphicsExtensions.CreateHeart(centerX, centerY, size);
        }

        /// <summary>
        /// Gets recommended corner radius for rounded shapes based on bounds
        /// </summary>
        public static float GetRecommendedCornerRadius(Rectangle bounds, float ratio = 0.1f)
        {
            return Math.Min(bounds.Width, bounds.Height) * ratio;
        }
    }
}
