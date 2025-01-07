using System.Reflection;
using Svg;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Desktop.Common
{
    public static class ImageLoader
    {
        /// <summary>
        /// Loads an SVG from a file path and returns the SvgDocument.
        /// </summary>
        public static SvgDocument LoadSvg(string svgPath)
        {
            if (string.IsNullOrEmpty(svgPath) || !File.Exists(svgPath))
                throw new FileNotFoundException($"SVG file not found: {svgPath}");

            try
            {
                return SvgDocument.Open(svgPath);
            }
            catch (Exception ex)
            {
                // Could throw or show a message, depending on library vs. app usage
                throw new Exception($"Error loading SVG from '{svgPath}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads an image (PNG, JPG, BMP, etc.) from a file.
        /// </summary>
        public static Image LoadRegularImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                throw new FileNotFoundException($"Image file not found: {imagePath}");

            try
            {
                return Image.FromFile(imagePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading image from '{imagePath}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads from embedded resource. Returns an (isSvg, object) tuple.
        /// The second item is either the loaded object (SvgDocument or Image).
        /// </summary>
        public static (bool isSvg, object result) LoadFromEmbeddedResource(string resourcePath, Assembly assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly(); // default

            using Stream stream = assembly.GetManifestResourceStream(resourcePath)
                ?? throw new FileNotFoundException($"Embedded resource not found: {resourcePath}");

            string extension = Path.GetExtension(resourcePath).ToLower();
            if (extension == ".svg")
            {
                var svgDoc = SvgDocument.Open<SvgDocument>(stream);
                return (true, svgDoc);
            }
            else
            {
                var image = Image.FromStream(stream);
                return (false, image);
            }
        }

        /// <summary>
       

    }
}
