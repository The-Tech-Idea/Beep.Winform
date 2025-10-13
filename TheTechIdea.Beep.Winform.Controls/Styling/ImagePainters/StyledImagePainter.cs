using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.BaseImage;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters
{
    /// <summary>
    /// Image painter that uses cache and works with image paths
    /// Paints images with rounded corners based on style
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

            using (var path = CreateRoundedRectangle(bounds, radius))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SetClip(path);
                painter.DrawImage(g, bounds);
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

            using (var path = CreateRoundedRectangle(bounds, 0))
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
                using (var path = CreateRoundedRectangle(bounds, cornerRadius))
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
                        using (var path = CreateRoundedRectangle(bounds, cornerRadius))
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

            using (var path = CreateRoundedRectangle(bounds, cornerRadius))
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
