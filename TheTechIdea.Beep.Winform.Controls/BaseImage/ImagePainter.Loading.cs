using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Svg;
 

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    public partial class ImagePainter
    {
        public bool UseThemeColors { get; set; } = false;

        public bool LoadFromStream(Stream stream, string extensionHint)
        {
            try
            {
                string ext = Path.GetExtension(extensionHint);
                if (string.Equals(ext, ".svg", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
                    string svgContent = reader.ReadToEnd();
                    svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                           .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                           .Replace("stroke=\"null\"", "stroke=\"none\"");
                    using var sanitizedStream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent));
                    SvgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                }
                else
                {
                    using var img = Image.FromStream(stream, useEmbeddedColorManagement: true, validateImageData: true);
                    Image = new Bitmap(img); // decouple from source stream
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void DisposeImages()
        {
            _regularImage?.Dispose();
            _regularImage = null;
            _svgDocument = null;
            _isSvg = false;
        }

        protected bool LoadImage(string path)
        {
            try
            {
                // Heuristic: if caller passed a short name (e.g., "icon.svg") or an embedded resource key,
                // try mapping via ImageListHelper first, then fall back to embedded-resource probing.
                if (IsEmbeddedResource(path))
                {
                    bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
                    if (isJustFileName)
                    {
                        string mapped = ImageListHelper.GetImagePathFromName(path);
                        if (!string.IsNullOrEmpty(mapped))
                        {
                            // If the mapping resolves to a physical file, load from disk.
                            if (File.Exists(mapped))
                            {
                                return LoadImageFromFile(mapped);
                            }

                            // Otherwise, the mapping might still be an embedded resource key.
                            if (LoadImageFromEmbeddedResource(mapped))
                            {
                                return true;
                            }
                        }
                    }

                    // Try original path as embedded resource (supports dotted resource names)
                    if (LoadImageFromEmbeddedResource(path))
                    {
                        return true;
                    }

                    // As a last resort, if the original path points to a file, try loading it.
                    if (File.Exists(path))
                    {
                        return LoadImageFromFile(path);
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        return LoadImageFromFile(path);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private bool LoadImageFromFile(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".svg":
                    return LoadSvg(path);
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    return LoadRegularImage(path);
                default:
                    return false;
            }
        }

        private bool LoadSvg(string svgPath)
        {
            try
            {
                DisposeImages();
                _svgDocument = LoadSanitizedSvg(svgPath);
                _svgDocument.CustomAttributes.Remove("Style");
                _svgDocument.FlushStyles();
                _isSvg = true;
                _stateChanged = true;
                InvalidateCache();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LoadRegularImage(string imagePath)
        {
            try
            {
                DisposeImages();
                // Avoid file locking by cloning the image
                using (var img = Image.FromFile(imagePath))
                {
                    _regularImage = new Bitmap(img);
                }
                _isSvg = false;
                _stateChanged = true;
                InvalidateCache();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private SvgDocument LoadSanitizedSvg(string svgFilePath)
        {
            string svgContent = File.ReadAllText(svgFilePath);
            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                   .Replace("stroke=\"null\"", "stroke=\"none\"");
            return SvgDocument.FromSvg<SvgDocument>(svgContent);
        }

        private bool IsEmbeddedResource(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            string normalizedPath = path.Trim().Replace("\\", ".").Replace("/", ".");

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var resourceNames = assembly.GetManifestResourceNames();
                if (resourceNames.Any(name => name.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
            if (isJustFileName)
            {
                return true;
            }

            if (File.Exists(path) || Directory.Exists(path))
            {
                return false;
            }

            return string.IsNullOrEmpty(Path.GetExtension(path));
        }

        private bool LoadImageFromEmbeddedResource(string resourcePath)
        {
            try
            {
                string matchedResource = null;
                string normalizedResourcePath = resourcePath.Trim().Replace("\\", ".").Replace("/", ".");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    matchedResource = resourceNames
                        .FirstOrDefault(name => name.Equals(normalizedResourcePath, StringComparison.OrdinalIgnoreCase));

                    if (matchedResource != null)
                    {
                        using var stream = assembly.GetManifestResourceStream(matchedResource);
                        if (stream == null) break;

                        string extension = Path.GetExtension(resourcePath).ToLower();
                        if (extension == ".svg")
                        {
                            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: false);
                            string svgContent = reader.ReadToEnd();
                            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

                            using var sanitizedStream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent));
                            _svgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                            _svgDocument.CustomAttributes.Remove("Style");
                            _svgDocument.FlushStyles();
                            _isSvg = true;
                        }
                        else
                        {
                            using var img = Image.FromStream(stream, useEmbeddedColorManagement: true, validateImageData: true);
                            _regularImage = new Bitmap(img); // decouple from stream
                            _isSvg = false;
                        }

                        _stateChanged = true;
                        InvalidateCache();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Convenience: draw an SVG/icon by key or path with color and opacity
        public void DrawSvg(Graphics g, string iconKeyOrPath, Rectangle destRect, Color color, float opacity = 1.0f)
        {
            if (g == null || destRect.Width <= 0 || destRect.Height <= 0) return;
            if (string.IsNullOrWhiteSpace(iconKeyOrPath)) return;

            // Resolve to a resource path or file path
            string path = iconKeyOrPath;
            try
            {
                bool looksLikePath = iconKeyOrPath.Contains("/") || iconKeyOrPath.Contains("\\") || Path.GetExtension(iconKeyOrPath).Length > 0;
                if (!looksLikePath)
                {
                    string mapped = ImageListHelper.GetImagePathFromName(iconKeyOrPath);
                    if (!string.IsNullOrEmpty(mapped)) path = mapped;
                }
            }
            catch { /* best-effort mapping */ }

            // Use a temporary painter to avoid mutating this instance's cached state
            using var painter = new ImagePainter();
            painter.CurrentTheme = this.CurrentTheme;
            painter.ImageEmbededin = this.ImageEmbededin;
            painter.ApplyThemeOnImage = true; // allow recolor
            painter.FillColor = color;
            painter.StrokeColor = color;
            painter.Opacity = Math.Max(0f, Math.Min(1f, opacity));
            painter.ImagePath = path; // auto-load
            painter.Alignment = this.Alignment;
            painter.ScaleMode = this.ScaleMode;
            painter.DrawImage(g, destRect);
        }
    }
}
