using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Svg;
using TheTechIdea.Beep.Desktop.Common.Util;

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    public partial class ImagePainter
    {
        public bool LoadFromStream(Stream stream, string extensionHint)
        {
            try
            {
                if (string.Equals(Path.GetExtension(extensionHint), ".svg", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(stream);
                    string svgContent = reader.ReadToEnd();
                    svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                           .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                           .Replace("stroke=\"null\"", "stroke=\"none\"");
                    using var sanitizedStream = new MemoryStream(Encoding.UTF8.GetBytes(svgContent));
                    SvgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                }
                else
                {
                    Image = Image.FromStream(stream);
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
                if (IsEmbeddedResource(path))
                {
                    bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
                    if (isJustFileName)
                    {
                        string newpath = ImageListHelper.GetImagePathFromName(path);
                        if (newpath != null)
                        {
                            path = newpath;
                        }
                    }
                    return LoadImageFromEmbeddedResource(path);
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
                _svgDocument.CustomAttributes.Remove("style");
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
                _regularImage = Image.FromFile(imagePath);
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
                Stream stream = null;
                string matchedResource = null;
                string normalizedResourcePath = resourcePath.Trim().Replace("\\", ".").Replace("/", ".");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var resourceNames = assembly.GetManifestResourceNames();
                    matchedResource = resourceNames
                        .FirstOrDefault(name => name.Equals(normalizedResourcePath, StringComparison.OrdinalIgnoreCase));

                    if (matchedResource != null)
                    {
                        stream = assembly.GetManifestResourceStream(matchedResource);
                        break;
                    }
                }

                if (stream != null)
                {
                    string extension = Path.GetExtension(resourcePath).ToLower();
                    if (extension == ".svg")
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string svgContent = reader.ReadToEnd();
                            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

                            using (var sanitizedStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent)))
                            {
                                _svgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                                _svgDocument.CustomAttributes.Remove("style");
                                _svgDocument.FlushStyles();
                                _isSvg = true;
                            }
                        }
                    }
                    else
                    {
                        _regularImage = Image.FromStream(stream);
                        _isSvg = false;
                    }

                    _stateChanged = true;
                    InvalidateCache();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
