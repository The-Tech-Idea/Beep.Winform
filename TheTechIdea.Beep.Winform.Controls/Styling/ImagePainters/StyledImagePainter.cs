using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters
{
    /// <summary>
    /// Image painter that uses cache and works with image paths
    /// Paints images with rounded corners based on Style
    /// </summary>
    public static class StyledImagePainter
    {
        // Thread-safe caches
        private static ConcurrentDictionary<string, Image> _imageCache = new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private static ConcurrentDictionary<string, ImagePainter> _painterCache = new ConcurrentDictionary<string, ImagePainter>(StringComparer.OrdinalIgnoreCase);
        private static ConcurrentDictionary<string, Image> _tintedCache = new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        // Pre-render coalescing
        private static CancellationTokenSource _preRenderCts = null;
        private static readonly object _preRenderLock = new object();
        #region Paint Methods with GraphicsPath and not rectangle

        public static void Paint(Graphics g, GraphicsPath path, string imagePath, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }

            int radius = StyleBorders.GetRadius(style);

            using (var roundedPath = GraphicsExtensions.GetRoundedRectPath(path.GetBoundsRect(), radius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(roundedPath);
                painter.DrawImage(g, roundedPath.GetBoundsRect());
                g.ResetClip();
            }
            
        }

        public static void Paint(Graphics g, GraphicsPath path, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }

    
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                painter.DrawImage(g, path.GetBoundsRect());
                g.ResetClip();
            
        }

        public static void PaintWithTint(Graphics g, GraphicsPath path, string imagePath, Color tint, float opacity = 1f, int cornerRadius = 0)
        {
            if (string.IsNullOrEmpty(imagePath) ) return;
            Rectangle bounds = Rectangle.Ceiling(path.GetBounds());
            string key = GetTintCacheKey(imagePath, tint, opacity, bounds.Size);
            if (_tintedCache.TryGetValue(key, out var tinted) && tinted != null)
            {
                using (var roundedPath = GraphicsExtensions.GetRoundedRectPath(bounds, cornerRadius))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SetClip(roundedPath);
                    g.DrawImage(tinted, roundedPath.GetBoundsRect());
                    g.ResetClip();
                }
                return;
            }

            Image baseImage = LoadImage(imagePath);
            if (baseImage == null)
            {
                var painter = GetOrCreatePainter(imagePath);
                if (painter != null)
                {
                    var oldApply = painter.ApplyThemeOnImage;
                    var oldFill = painter.FillColor;
                    var oldOpacity = painter.Opacity;
                    try
                    {
                        painter.ApplyThemeOnImage = true;
                        painter.FillColor = tint;
                        painter.Opacity = opacity;
                        using (var bounds1   = GraphicsExtensions.GetRoundedRectPath(path.GetBoundsRect(), cornerRadius))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SetClip(path);
                            painter.DrawImage(g, bounds1.GetBoundsRect());
                            g.ResetClip();
                        }
                    }
                    finally
                    {
                        painter.ApplyThemeOnImage = oldApply;
                        painter.FillColor = oldFill;
                        painter.Opacity = oldOpacity;
                    }
                }
                return;
            }

            Bitmap bmp = new Bitmap(Math.Max(1, bounds.Width), Math.Max(1, bounds.Height));
            using (var tg = Graphics.FromImage(bmp))
            {
                tg.Clear(Color.Transparent);
                tg.SmoothingMode = SmoothingMode.HighQuality;
                tg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                tg.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle dest = new Rectangle(0, 0, bmp.Width, bmp.Height);

                float rFactor = tint.R / 255f;
                float gFactor = tint.G / 255f;
                float bFactor = tint.B / 255f;
                float aFactor = opacity;

                var cm = new ColorMatrix(new float[][] {
                    new float[] { rFactor, 0, 0, 0, 0 },
                    new float[] { 0, gFactor, 0, 0, 0 },
                    new float[] { 0, 0, bFactor, 0, 0 },
                    new float[] { 0, 0, 0, aFactor, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                using (var ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    tg.DrawImage(baseImage, dest, 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia);
                }
            }

            _tintedCache[key] = bmp;

            using (var bound2 = GraphicsExtensions.GetRoundedRectPath(bounds, cornerRadius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                g.DrawImage(bmp, bound2.GetBoundsRect());
                g.ResetClip();
            }
        }

        #endregion

        #region "Advanced Shape-Based Image Painting"

        /// <summary>
        /// Paints image inside a circle
        /// </summary>
        public static void PaintInCircle(Graphics g, float centerX, float centerY, float radius, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateCircle(centerX, centerY, radius))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a triangle
        /// </summary>
        public static void PaintInTriangle(Graphics g, float centerX, float centerY, float size, string imagePath, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateTriangle(centerX, centerY, size, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a star shape
        /// </summary>
        public static void PaintInStar(Graphics g, float centerX, float centerY, float outerRadius, float innerRadius, string imagePath, int points = 5, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateStar(centerX, centerY, outerRadius, innerRadius, points, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a hexagon
        /// </summary>
        public static void PaintInHexagon(Graphics g, float centerX, float centerY, float size, string imagePath, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateHexagon(centerX, centerY, size, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a pentagon
        /// </summary>
        public static void PaintInPentagon(Graphics g, float centerX, float centerY, float size, string imagePath, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreatePentagon(centerX, centerY, size, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside an octagon
        /// </summary>
        public static void PaintInOctagon(Graphics g, float centerX, float centerY, float size, string imagePath, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateOctagon(centerX, centerY, size, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a regular polygon with any number of sides
        /// </summary>
        public static void PaintInPolygon(Graphics g, float centerX, float centerY, float size, int sides, string imagePath, float rotation = 0f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateRegularPolygon(centerX, centerY, size, sides, rotation))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a diamond/rhombus shape
        /// </summary>
        public static void PaintInDiamond(Graphics g, float centerX, float centerY, float width, float height, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateDiamond(centerX, centerY, width, height))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a pill/capsule shape
        /// </summary>
        public static void PaintInPill(Graphics g, float x, float y, float width, float height, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreatePill(x, y, width, height))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a heart shape
        /// </summary>
        public static void PaintInHeart(Graphics g, float centerX, float centerY, float size, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateHeart(centerX, centerY, size))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a cloud shape
        /// </summary>
        public static void PaintInCloud(Graphics g, float x, float y, float width, float height, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateCloud(x, y, width, height))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a gear/cog shape
        /// </summary>
        public static void PaintInGear(Graphics g, float centerX, float centerY, float outerRadius, float innerRadius, string imagePath, int teeth = 12, float toothDepth = 0.3f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateGear(centerX, centerY, outerRadius, innerRadius, teeth, toothDepth))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a tab shape (for tab controls)
        /// </summary>
        public static void PaintInTab(Graphics g, float x, float y, float width, float height, string imagePath, float cornerRadius = 8f, GraphicsExtensions.TabPosition position = GraphicsExtensions.TabPosition.Top, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateTab(x, y, width, height, cornerRadius, position))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a tag/label shape
        /// </summary>
        public static void PaintInTag(Graphics g, float x, float y, float width, float height, string imagePath, float cornerRadius = 5f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateTag(x, y, width, height, cornerRadius))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a breadcrumb item shape
        /// </summary>
        public static void PaintInBreadcrumb(Graphics g, float x, float y, float width, float height, string imagePath, float arrowSize = 10f, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateBreadcrumb(x, y, width, height, arrowSize))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a badge (circular notification badge)
        /// </summary>
        public static void PaintInBadge(Graphics g, float x, float y, float size, string imagePath, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateBadge(x, y, size))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside a speech bubble shape
        /// </summary>
        public static void PaintInSpeechBubble(Graphics g, float x, float y, float width, float height, string imagePath, float cornerRadius = 10f, float tailSize = 15f, GraphicsExtensions.SpeechBubbleTailPosition tailPosition = GraphicsExtensions.SpeechBubbleTailPosition.BottomLeft, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateSpeechBubble(x, y, width, height, cornerRadius, tailSize, tailPosition))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside an arrow shape
        /// </summary>
        public static void PaintInArrow(Graphics g, float x, float y, float width, float height, string imagePath, GraphicsExtensions.ArrowDirection direction = GraphicsExtensions.ArrowDirection.Right, Color? tint = null, float opacity = 1f)
        {
            using (var path = GraphicsExtensions.CreateArrow(x, y, width, height, direction))
            {
                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
        }

        /// <summary>
        /// Paints image inside any custom GraphicsPath shape
        /// </summary>
        public static void PaintInCustomShape(Graphics g, GraphicsPath customPath, string imagePath, Color? tint = null, float opacity = 1f, int cornerRadius = 0)
        {
            if (customPath == null) return;
            
            if (tint.HasValue)
                PaintWithTint(g, customPath, imagePath, tint.Value, opacity, cornerRadius);
            else
                Paint(g, customPath, imagePath);
        }

        /// <summary>
        /// Paints image with a glow effect inside a shape
        /// </summary>
        public static void PaintWithGlow(Graphics g, GraphicsPath path, string imagePath, Color glowColor, int glowSize = 10, Color? tint = null, float opacity = 1f)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            // Draw glow first
            GraphicsExtensions.DrawGlow(g, path, glowColor, glowSize);

            // Then draw image
            if (tint.HasValue)
                PaintWithTint(g, path, imagePath, tint.Value, opacity);
            else
                Paint(g, path, imagePath);
        }

        /// <summary>
        /// Paints image with a shadow effect
        /// </summary>
        public static void PaintWithShadow(Graphics g, GraphicsPath path, string imagePath, float shadowOffsetX, float shadowOffsetY, Color shadowColor, Color? tint = null, float opacity = 1f)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            // Draw shadow first
            using (var shadowPath = GraphicsExtensions.CreateShadowPath(path, shadowOffsetX, shadowOffsetY))
            {
                using (var shadowBrush = new SolidBrush(shadowColor))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Then draw image
            if (tint.HasValue)
                PaintWithTint(g, path, imagePath, tint.Value, opacity);
            else
                Paint(g, path, imagePath);
        }

        /// <summary>
        /// Paints image with both gradient overlay and shape clipping
        /// </summary>
        public static void PaintWithGradientOverlay(Graphics g, GraphicsPath path, string imagePath, Color gradientStart, Color gradientEnd, float gradientAngle = 90f, float overlayOpacity = 0.5f)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            // Paint base image
            Paint(g, path, imagePath);

            // Apply gradient overlay
            using (var gradientBrush = GraphicsExtensions.CreateLinearGradientFromPath(path, 
                Color.FromArgb((int)(255 * overlayOpacity), gradientStart),
                Color.FromArgb((int)(255 * overlayOpacity), gradientEnd),
                gradientAngle))
            {
                g.SetClip(path);
                g.FillPath(gradientBrush, path);
                g.ResetClip();
            }
        }

        /// <summary>
        /// Paints image rotated inside a shape
        /// </summary>
        public static void PaintRotated(Graphics g, GraphicsPath path, string imagePath, float rotationDegrees, Color? tint = null, float opacity = 1f)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            var bounds = path.GetBounds();
            var center = new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);

            var state = g.Save();
            try
            {
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(rotationDegrees);
                g.TranslateTransform(-center.X, -center.Y);

                if (tint.HasValue)
                    PaintWithTint(g, path, imagePath, tint.Value, opacity);
                else
                    Paint(g, path, imagePath);
            }
            finally
            {
                g.Restore(state);
            }
        }

        /// <summary>
        /// Paints image tiled inside a shape
        /// </summary>
        public static void PaintTiled(Graphics g, GraphicsPath path, string imagePath, float tileWidth, float tileHeight)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            var painter = GetOrCreatePainter(imagePath);
            if (painter == null) return;

            var bounds = path.GetBounds();
            g.SetClip(path);

            for (float y = bounds.Y; y < bounds.Bottom; y += tileHeight)
            {
                for (float x = bounds.X; x < bounds.Right; x += tileWidth)
                {
                    var tileRect = new Rectangle(
                        (int)x, (int)y,
                        (int)Math.Min(tileWidth, bounds.Right - x),
                        (int)Math.Min(tileHeight, bounds.Bottom - y)
                    );
                    painter.DrawImage(g, tileRect);
                }
            }

            g.ResetClip();
        }

        /// <summary>
        /// Paints image with aspect ratio preserved inside shape
        /// </summary>
        public static void PaintAspectFit(Graphics g, GraphicsPath path, string imagePath, Color? tint = null, float opacity = 1f)
        {
            if (path == null || string.IsNullOrEmpty(imagePath)) return;

            var painter = GetOrCreatePainter(imagePath);
            if (painter == null) return;

            var bounds = path.GetBounds();
            var image = LoadImage(imagePath);
            if (image == null) return;

            // Calculate aspect-fit rectangle
            float imageAspect = (float)image.Width / image.Height;
            float boundsAspect = bounds.Width / bounds.Height;
            
            RectangleF destRect;
            if (imageAspect > boundsAspect)
            {
                // Image is wider - fit to width
                float height = bounds.Width / imageAspect;
                float yOffset = (bounds.Height - height) / 2;
                destRect = new RectangleF(bounds.X, bounds.Y + yOffset, bounds.Width, height);
            }
            else
            {
                // Image is taller - fit to height
                float width = bounds.Height * imageAspect;
                float xOffset = (bounds.Width - width) / 2;
                destRect = new RectangleF(bounds.X + xOffset, bounds.Y, width, bounds.Height);
            }

            using (var fitPath = GraphicsExtensions.CreateRectanglePath(destRect))
            {
                g.SetClip(path); // Clip to original shape
                
                if (tint.HasValue)
                    PaintWithTint(g, fitPath, imagePath, tint.Value, opacity);
                else
                    Paint(g, fitPath, imagePath);
                    
                g.ResetClip();
            }
        }

        #endregion


        public static void Paint(Graphics g, Rectangle bounds, string imagePath, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }

            int radius = StyleBorders.GetRadius(style);

            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, radius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                painter.DrawImage(g, path.GetBoundsRect());
                g.ResetClip();
            }
        }

        public static void Paint(Graphics g, Rectangle bounds, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }

            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, 0))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                painter.DrawImage(g, bounds);
                g.ResetClip();
            }
        }

        public static void PaintWithTint(Graphics g, Rectangle bounds, string imagePath, Color tint, float opacity = 1f, int cornerRadius = 0)
        {
            if (string.IsNullOrEmpty(imagePath) || bounds.IsEmpty) return;

            string key = GetTintCacheKey(imagePath, tint, opacity, bounds.Size);
            if (_tintedCache.TryGetValue(key, out var tinted) && tinted != null)
            {
                using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, cornerRadius))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SetClip(path);
                    g.DrawImage(tinted, bounds);
                    g.ResetClip();
                }
                return;
            }

            Image baseImage = LoadImage(imagePath);
            if (baseImage == null)
            {
                var painter = GetOrCreatePainter(imagePath);
                if (painter != null)
                {
                    var oldApply = painter.ApplyThemeOnImage;
                    var oldFill = painter.FillColor;
                    var oldOpacity = painter.Opacity;
                    try
                    {
                        painter.ApplyThemeOnImage = true;
                        painter.FillColor = tint;
                        painter.Opacity = opacity;
                        using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, cornerRadius))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SetClip(path);
                            painter.DrawImage(g, bounds);
                            g.ResetClip();
                        }
                    }
                    finally
                    {
                        painter.ApplyThemeOnImage = oldApply;
                        painter.FillColor = oldFill;
                        painter.Opacity = oldOpacity;
                    }
                }
                return;
            }

            Bitmap bmp = new Bitmap(Math.Max(1, bounds.Width), Math.Max(1, bounds.Height));
            using (var tg = Graphics.FromImage(bmp))
            {
                tg.Clear(Color.Transparent);
                tg.SmoothingMode = SmoothingMode.HighQuality;
                tg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                tg.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Rectangle dest = new Rectangle(0, 0, bmp.Width, bmp.Height);

                float rFactor = tint.R / 255f;
                float gFactor = tint.G / 255f;
                float bFactor = tint.B / 255f;
                float aFactor = opacity;

                var cm = new ColorMatrix(new float[][] {
                    new float[] { rFactor, 0, 0, 0, 0 },
                    new float[] { 0, gFactor, 0, 0, 0 },
                    new float[] { 0, 0, bFactor, 0, 0 },
                    new float[] { 0, 0, 0, aFactor, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                using (var ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    tg.DrawImage(baseImage, dest, 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia);
                }
            }

            _tintedCache[key] = bmp;

            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, cornerRadius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                g.DrawImage(bmp, bounds);
                g.ResetClip();
            }
        }

        public static void PreRenderTintedToCache(string imagePath, Color tint, float opacity, Size size)
        {
            if (string.IsNullOrEmpty(imagePath) || size.Width <= 0 || size.Height <= 0) return;
            string key = GetTintCacheKey(imagePath, tint, opacity, size);
            if (_tintedCache.ContainsKey(key)) return;

            Image baseImage = LoadImage(imagePath);
            if (baseImage == null) return;

            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (var tg = Graphics.FromImage(bmp))
            {
                tg.Clear(Color.Transparent);
                tg.SmoothingMode = SmoothingMode.HighQuality;
                tg.InterpolationMode = InterpolationMode.HighQualityBicubic;
                tg.PixelOffsetMode = PixelOffsetMode.HighQuality;

                float rFactor = tint.R / 255f;
                float gFactor = tint.G / 255f;
                float bFactor = tint.B / 255f;
                float aFactor = opacity;

                var cm = new ColorMatrix(new float[][] {
                    new float[] { rFactor, 0, 0, 0, 0 },
                    new float[] { 0, gFactor, 0, 0, 0 },
                    new float[] { 0, 0, bFactor, 0, 0 },
                    new float[] { 0, 0, 0, aFactor, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                using (var ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    tg.DrawImage(baseImage, new Rectangle(0, 0, size.Width, size.Height), 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia);
                }
            }

            _tintedCache[key] = bmp;
        }

        /// <summary>
        /// Schedule pre-rendering in background, canceling any previous scheduled pre-render.
        /// This coalesces rapid theme changes.
        /// </summary>
        public static void SchedulePreRender(string imagePath, Color tint, float opacity, int[] sizes)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            lock (_preRenderLock)
            {
                try
                {
                    _preRenderCts?.Cancel();
                }
                catch { }
                _preRenderCts = new CancellationTokenSource();
                var token = _preRenderCts.Token;
                Task.Run(() =>
                {
                    foreach (var s in sizes)
                    {
                        if (token.IsCancellationRequested) break;
                        try
                        {
                            PreRenderTintedToCache(imagePath, tint, opacity, new Size(s, s));
                        }
                        catch { }
                    }
                }, token);
            }
        }

        public static void ClearTintCache()
        {
            foreach (var img in _tintedCache.Values)
            {
                try { img?.Dispose(); } catch { }
            }
            _tintedCache.Clear();
        }

        public static void RemoveTintedFromCache(string imagePath)
        {
            var keys = new List<string>(_tintedCache.Keys);
            foreach (var k in keys)
            {
                if (k.StartsWith(imagePath + "|", StringComparison.OrdinalIgnoreCase))
                {
                    try { _tintedCache[k]?.Dispose(); } catch { }
                    _tintedCache.TryRemove(k, out _);
                }
            }
        }

        private static string GetTintCacheKey(string path, Color tint, float opacity, Size size)
        {
            return $"{path}|{tint.ToArgb()}|{opacity}|{size.Width}x{size.Height}";
        }

        public static void PaintDisabled(Graphics g, Rectangle bounds, string imagePath, Color disabledBackColor)
        {
            if (string.IsNullOrEmpty(imagePath) || bounds.IsEmpty) return;

            using (var temp = new Bitmap(Math.Max(1, bounds.Width), Math.Max(1, bounds.Height)))
            {
                using (var tg = Graphics.FromImage(temp))
                {
                    tg.Clear(Color.Transparent);
                    try
                    {
                        Paint(tg, new Rectangle(0, 0, temp.Width, temp.Height), imagePath);
                    }
                    catch { }
                }

                ControlPaint.DrawImageDisabled(g, temp, bounds.X, bounds.Y, disabledBackColor);
            }
        }

        private static ImagePainter GetOrCreatePainter(string imagePath)
        {
            if (_painterCache.TryGetValue(imagePath, out var existing)) return existing;

            var painter = new ImagePainter(imagePath);
            if (!painter.HasImage)
            {
                try
                {
                    bool looksLikePath = imagePath.Contains("/") || imagePath.Contains("\\") || Path.GetExtension(imagePath).Length > 0;
                    if (!looksLikePath)
                    {
                        string mapped = ImageManagement.ImageListHelper.GetImagePathFromName(imagePath);
                        if (!string.IsNullOrEmpty(mapped))
                        {
                            painter.ImagePath = mapped;
                        }
                    }
                }
                catch { }
            }

            if (!painter.HasImage) return null;

            _painterCache.TryAdd(imagePath, painter);
            return painter;
        }

        private static Image LoadImage(string imagePath)
        {
            if (_imageCache.TryGetValue(imagePath, out var cached)) return cached;

            try
            {
                if (File.Exists(imagePath))
                {
                    Image image = Image.FromFile(imagePath);
                    _imageCache.TryAdd(imagePath, image);
                    return image;
                }

                bool looksLikePath = imagePath.Contains("/") || imagePath.Contains("\\") || Path.GetExtension(imagePath).Length > 0;
                if (!looksLikePath)
                {
                    string mapped = ImageManagement.ImageListHelper.GetImagePathFromName(imagePath);
                    if (!string.IsNullOrEmpty(mapped) && File.Exists(mapped))
                    {
                        Image image = Image.FromFile(mapped);
                        _imageCache.TryAdd(imagePath, image);
                        return image;
                    }
                }
            }
            catch { }
            return null;
        }

        public static void ClearCache()
        {
            foreach (var image in _imageCache.Values)
            {
                try { image?.Dispose(); } catch { }
            }
            _imageCache.Clear();

            foreach (var painter in _painterCache.Values)
            {
                try { painter?.Dispose(); } catch { }
            }
            _painterCache.Clear();

            ClearTintCache();
        }

        public static void RemoveFromCache(string imagePath)
        {
            if (_imageCache.TryRemove(imagePath, out var img))
            {
                try { img?.Dispose(); } catch { }
            }

            if (_painterCache.TryRemove(imagePath, out var painter))
            {
                try { painter?.Dispose(); } catch { }
            }

            RemoveTintedFromCache(imagePath);
        }

        /// <summary>
        /// Invalidate caches explicitly (useful when theme assets or icons change)
        /// </summary>
        public static void InvalidateCaches(string imagePath = null)
        {
            if (string.IsNullOrEmpty(imagePath)) ClearCache(); else RemoveFromCache(imagePath);
        }

    
    }
}
