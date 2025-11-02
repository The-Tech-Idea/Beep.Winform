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
                if (string.IsNullOrWhiteSpace(path))
                    return false;

                // OPTION 1 SUPPORT: Check if this looks like a strongly-typed static field value
                // Example: "TheTechIdea.Beep.Winform.Controls.GFX.Icons.UI.key.svg"
                // These typically have multiple dots and end with a file extension
                bool looksLikeStronglyTypedPath = path.Contains(".") && 
                                                   (path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                                                    path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                    path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                    path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                    path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase));

                // OPTION 2 SUPPORT: Check if it's a direct embedded resource path or file
                // Try as embedded resource first if it looks like a strongly-typed path OR is identified as embedded
                if (looksLikeStronglyTypedPath || IsEmbeddedResource(path))
                {
                    // Handle just filename case (e.g., "icon.svg")
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
                    // This handles BOTH Option 1 (strongly-typed) and Option 2 (direct embedded path)
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
                    // Standard file path
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

            // Normalize the path by replacing slashes with dots (case-insensitive comparison)
            string normalizedPath = path.Trim().Replace("\\", ".").Replace("/", ".");

            // Check all loaded assemblies for matching embedded resources (CASE-INSENSITIVE)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    // Use case-insensitive comparison
                    if (resourceNames.Any(name => name.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }

                    // Also try partial matching for short names (e.g., "key.svg" might match "*.UI.key.svg")
                    bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
                    if (isJustFileName)
                    {
                        // Check if any resource ends with this filename (case-insensitive)
                        if (resourceNames.Any(name => name.EndsWith("." + normalizedPath, StringComparison.OrdinalIgnoreCase)))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that can't be queried
                    continue;
                }
            }

            // If it's just a filename without path separators, assume it might be embedded
            bool isShortName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
            if (isShortName)
            {
                return true;
            }

            // If it's a valid file system path, it's NOT an embedded resource
            if (File.Exists(path) || Directory.Exists(path))
            {
                return false;
            }

            // If it has no valid file extension and looks like a namespace, assume it's embedded
            return string.IsNullOrEmpty(Path.GetExtension(path)) || path.Contains(".");
        }

        private bool LoadImageFromEmbeddedResource(string resourcePath)
        {
            try
            {
                string? matchedResource = null;
                string normalizedResourcePath = resourcePath.Trim().Replace("\\", ".").Replace("/", ".");
                
                // Debug list to track similar resources if not found
                var allSvgResources = new System.Collections.Generic.List<string>();

                // Search all assemblies for the resource (CASE-INSENSITIVE)
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var resourceNames = assembly.GetManifestResourceNames();
                        
                        // Perform case-insensitive lookup
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

                        // Collect SVG resources for debugging if we haven't found a match yet
                        if (matchedResource == null && Path.GetExtension(resourcePath).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                        {
                            allSvgResources.AddRange(resourceNames.Where(r => r.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)));
                        }
                    }
                    catch
                    {
                        // Skip problematic assemblies
                        continue;
                    }
                }

                // Debug output if resource not found
                if (matchedResource == null)
                {
                    string fileName = Path.GetFileName(resourcePath);
                    var similarResources = allSvgResources
                        .Where(r => r.Contains(fileName, StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .ToList();

                    if (similarResources.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"ImagePainter: Resource '{resourcePath}' not found. Similar resources:");
                        foreach (var similar in similarResources)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {similar}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"ImagePainter: Resource '{resourcePath}' not found. No similar resources found.");
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ImagePainter: Error loading embedded resource '{resourcePath}': {ex.Message}");
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

        /// <summary>
        /// Diagnostics method to list all embedded SVG resources across all loaded assemblies.
        /// Useful for debugging why certain icons can't be loaded.
        /// </summary>
        public static System.Collections.Generic.List<string> GetAllEmbeddedSvgResources()
        {
            var svgResources = new System.Collections.Generic.List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    svgResources.AddRange(resourceNames.Where(r => r.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)));
                }
                catch { }
            }
            return svgResources;
        }

        /// <summary>
        /// Diagnostics method to find all embedded resources matching a specific pattern (case-insensitive).
        /// </summary>
        public static System.Collections.Generic.List<string> FindEmbeddedResources(string searchPattern)
        {
            var matchingResources = new System.Collections.Generic.List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    matchingResources.AddRange(resourceNames.Where(r => 
                        r.Contains(searchPattern, StringComparison.OrdinalIgnoreCase)));
                }
                catch { }
            }
            return matchingResources;
        }
    }
}
