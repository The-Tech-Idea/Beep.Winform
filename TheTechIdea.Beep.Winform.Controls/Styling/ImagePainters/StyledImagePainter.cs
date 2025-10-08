using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.BaseImage;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters
{
    /// <summary>
    /// Image painter that uses cache and works with image paths
    /// Paints images with rounded corners based on style
    /// </summary>
    public static class StyledImagePainter
    {
        // Cache for loaded images to avoid repeated disk I/O
        private static Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();
        
        // Cache for ImagePainter instances
        private static Dictionary<string, ImagePainter> _painterCache = new Dictionary<string, ImagePainter>();
        
        public static void Paint(Graphics g, Rectangle bounds, string imagePath, BeepControlStyle style)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;
            
            // Get or create ImagePainter from cache
            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }
            
            int radius = StyleBorders.GetRadius(style);
            
            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                
                // Use ImagePainter to draw
                painter.DrawImage(g, bounds);
                
                g.ResetClip();
            }
        }

        public static void Paint(Graphics g, Rectangle bounds, string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            // Get or create ImagePainter from cache
            ImagePainter painter = GetOrCreatePainter(imagePath);
            if (painter == null)
            {
                System.Diagnostics.Debug.WriteLine($"[StyledImagePainter] Unable to resolve image '{imagePath}'");
                return;
            }
            using (var path = CreateRoundedRectangle(bounds, 0))
                {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                
                // Use ImagePainter to draw
                painter.DrawImage(g, bounds);
                
                g.ResetClip();
            }

        }

        /// <summary>
        /// Get or create ImagePainter from cache
        /// </summary>
        private static ImagePainter GetOrCreatePainter(string imagePath)
        {
            if (_painterCache.ContainsKey(imagePath))
                return _painterCache[imagePath];

            // Prefer ImagePainter's own resolution logic (handles filenames, mapped paths, and embedded resources)
            var painter = new ImagePainter(imagePath);
            if (!painter.HasImage)
            {
                try
                {
                    // Attempt name-to-path mapping via ImageListHelper if a short name was provided
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
                catch { /* best-effort mapping */ }
            }

            if (!painter.HasImage)
            {
                // Give up if painter still couldn't resolve an image
                return null;
            }

            _painterCache[imagePath] = painter;
            return painter;
        }
        
        /// <summary>
        /// Load image from path with caching
        /// </summary>
        private static Image LoadImage(string imagePath)
        {
            // Retained for backward compatibility; prefer ImagePainter-based loading.
            if (_imageCache.ContainsKey(imagePath))
                return _imageCache[imagePath];

            try
            {
                if (File.Exists(imagePath))
                {
                    Image image = Image.FromFile(imagePath);
                    _imageCache[imagePath] = image;
                    return image;
                }

                // Try to map short names to physical paths
                bool looksLikePath = imagePath.Contains("/") || imagePath.Contains("\\") || Path.GetExtension(imagePath).Length > 0;
                if (!looksLikePath)
                {
                    string mapped = ImageManagement.ImageListHelper.GetImagePathFromName(imagePath);
                    if (!string.IsNullOrEmpty(mapped) && File.Exists(mapped))
                    {
                        Image image = Image.FromFile(mapped);
                        _imageCache[imagePath] = image;
                        return image;
                    }
                }
            }
            catch
            {
                // swallow; caller will fall back to ImagePainter path
            }
            return null;
        }
        
        /// <summary>
        /// Clear image cache to free memory
        /// </summary>
        public static void ClearCache()
        {
            foreach (var image in _imageCache.Values)
            {
                image?.Dispose();
            }
            _imageCache.Clear();
            
            foreach (var painter in _painterCache.Values)
            {
                painter?.Dispose();
            }
            _painterCache.Clear();
        }
        
        /// <summary>
        /// Remove specific image from cache
        /// </summary>
        public static void RemoveFromCache(string imagePath)
        {
            if (_imageCache.ContainsKey(imagePath))
            {
                _imageCache[imagePath]?.Dispose();
                _imageCache.Remove(imagePath);
            }
            
            if (_painterCache.ContainsKey(imagePath))
            {
                _painterCache[imagePath]?.Dispose();
                _painterCache.Remove(imagePath);
            }
        }
        
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }
            
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
    }
}
