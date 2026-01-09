using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Svg;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ImageManagement;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region Loading Images

        public bool IsSvgPath(string path)
        {
            return Path.GetExtension(path)?.ToLower() == ".svg";
        }

        /// <summary>
        /// Normalizes the image path to handle both strongly-typed references (Option 1) 
        /// and direct embedded resource paths (Option 2).
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Normalized path suitable for loading</returns>
        private string NormalizeImagePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // Option 1: Handle strongly-typed static references from SvgsUI, Svgs, SvgsDatasources
            // These classes expose static readonly strings like "TheTechIdea.Beep.Winform.Controls.GFX.Icons.UI.key.svg"
            // The value is already the full embedded resource path, so just return it
            // This is detected by checking if the path already looks like a full embedded resource path
            if (path.StartsWith("TheTechIdea.Beep.", StringComparison.OrdinalIgnoreCase) &&
                (path.Contains(".GFX.") || path.Contains(".Fonts.")))
            {
                // Already a full embedded resource path (Option 1 or Option 2)
                return path;
            }

            // Option 2: Direct embedded resource path - user typed the full path
            // Just return as-is if it contains typical embedded resource namespace patterns
            if (path.Contains("TheTechIdea", StringComparison.OrdinalIgnoreCase) && path.Contains("."))
            {
                return path;
            }

            // Handle short names - try to resolve from ImageListHelper
            bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
            if (isJustFileName)
            {
                string resolvedPath = ImageListHelper.GetImagePathFromName(path);
                if (!string.IsNullOrWhiteSpace(resolvedPath))
                {
                    return resolvedPath;
                }
            }

            // Return original path for file system paths or unresolved names
            return path;
        }

        /// <summary>
        /// Load the image from the provided path (checks if it's a file path or embedded resource).
        /// Supports both strongly-typed references (Option 1: SvgsUI.Key) and 
        /// direct embedded resource paths (Option 2: "TheTechIdea.Beep.Winform.Controls.GFX.Icons.UI.key.svg")
        /// </summary>
        private bool LoadImage(string path)
        {
            bool retval = false;
            try
            {
                // Normalize the path to handle both Option 1 and Option 2
                path = NormalizeImagePath(path);

                if (string.IsNullOrWhiteSpace(path))
                {
                    return false;
                }

                if (IsEmbeddedResource(path))
                {
                    // Attempt to load from embedded resources
                    retval = LoadImageFromEmbeddedResource(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        // Load from file system
                        retval = LoadImageFromFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepImage LoadImage Error: {ex.Message}");
                return false;
            }
            return retval;
        }

        /// <summary>
        /// Load an image from the file system (SVG, PNG, JPG, BMP)
        /// </summary>
        private bool LoadImageFromFile(string path)
        {
            bool retval = false;
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".svg":
                    retval = LoadSvg(path);
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    retval = LoadRegularImage(path);
                    break;
                default:
                    break;
            }

            return retval;
        }

        public bool IsEmbeddedResource(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // Normalize the path by replacing slashes with dots (CASE-INSENSITIVE comparison)
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

        /// <summary>
        /// Diagnostics method to list all embedded SVG resources
        /// </summary>
        public static List<string> GetAllEmbeddedSvgResources()
        {
            var svgResources = new List<string>();
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
        /// Useful for debugging why certain icons can't be loaded.
        /// </summary>
        /// <param name="searchPattern">The pattern to search for (e.g., "key.svg" or "UI")</param>
        /// <returns>List of matching resource names</returns>
        public static List<string> FindEmbeddedResources(string searchPattern)
        {
            var matchingResources = new List<string>();
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

        /// <summary>
        /// Load an image from the embedded resources (checks the current assembly).
        /// </summary>
        public bool LoadImageFromEmbeddedResource(string resourcePath)
        {
            try
            {
                Stream? stream = null;
                string? matchedResource = null;

                // Normalize resource path (remove starting dots or extra spaces)
                string normalizedResourcePath = resourcePath.Trim().Replace("\\", ".").Replace("/", ".");

                // Debug output - list similar resources if not found
                var allSvgs = new List<string>();

                // Search all assemblies for the resource (CASE-INSENSITIVE)
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        var resourceNames = assembly.GetManifestResourceNames();

                        // Perform a case-insensitive lookup
                        matchedResource = resourceNames
                            .FirstOrDefault(name => name.Equals(normalizedResourcePath, StringComparison.OrdinalIgnoreCase));

                        if (matchedResource != null)
                        {
                            stream = assembly.GetManifestResourceStream(matchedResource);
                            break;
                        }

                        // Collect SVG resources for debugging if we haven't found a match yet
                        if (stream == null && Path.GetExtension(resourcePath).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                        {
                            allSvgs.AddRange(resourceNames.Where(r => r.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)));
                        }
                    }
                    catch
                    {
                        // Skip problematic assemblies
                        continue;
                    }
                }

                // If not found, try to find similar resources for better error message
                if (stream == null)
                {
                    string fileName = Path.GetFileName(resourcePath);
                    var similarResources = allSvgs
                        .Where(r => r.Contains(fileName, StringComparison.OrdinalIgnoreCase))
                        .Take(5)
                        .ToList();

                    if (similarResources.Any())
                    {
                        System.Diagnostics.Debug.WriteLine($"BeepImage: Resource '{resourcePath}' not found. Similar resources:");
                        foreach (var similar in similarResources)
                        {
                            System.Diagnostics.Debug.WriteLine($"  - {similar}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"BeepImage: Resource '{resourcePath}' not found. No similar resources found.");
                    }
                }

                if (stream != null)
                {
                    // Determine image type by file extension
                    string extension = Path.GetExtension(resourcePath).ToLower();
                    if (extension == ".svg")
                    {
                        // Read the stream into a string for sanitization
                        using (var reader = new StreamReader(stream))
                        {
                            string svgContent = reader.ReadToEnd();

                            // Sanitize invalid attribute values
                            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

                            // Convert the sanitized string back to a stream
                            using (var sanitizedStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent)))
                            {
                                svgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                                svgDocument.CustomAttributes.Remove("Style");
                                svgDocument.FlushStyles();
                                isSvg = true;
                            }
                        }
                    }
                    else
                    {
                        regularImage = Image.FromStream(stream);
                        isSvg = false;
                    }

                    _stateChanged = true;
                    _cachedRenderedImage?.Dispose();
                    _cachedRenderedImage = null;
                    Invalidate(); // Trigger a repaint
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ClearImage()
        {
            svgDocument = null;
            regularImage?.Dispose();
            regularImage = null;
            isSvg = false;
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            Invalidate();
        }

        public SvgDocument LoadSanitizedSvg(string svgFilePath)
        {
            // Read the raw SVG content
            string svgContent = File.ReadAllText(svgFilePath);

            // Replace invalid attribute values
            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

            // Parse the sanitized content into an SvgDocument
            return SvgDocument.FromSvg<SvgDocument>(svgContent);
        }

        public bool LoadSvg(string svgPath)
        {
            try
            {
                DisposeImages();
                svgDocument = LoadSanitizedSvg(svgPath);
                svgDocument.CustomAttributes.Remove("Style");
                svgDocument.FlushStyles();
                isSvg = true;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate(); // Trigger repaint
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading SVG: {ex.Message}", "SVG Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public bool LoadRegularImage(string imagePath)
        {
            try
            {
                DisposeImages();
                regularImage = Image.FromFile(imagePath);
                isSvg = false;
                regularImage.Tag = imagePath; // Store path for future reference
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate(); // Trigger repaint
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "ImagePath Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        #endregion
    }
}
