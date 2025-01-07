using System.Drawing.Imaging;
using Svg;

namespace TheTechIdea.Beep.Desktop.Common
{
    /// <summary>
    /// Provides functionality for converting SVG or PNG images into .ico (Icon) format.
    /// </summary>
    public static class ImageConverters
    {
        /// <summary>
        /// Converts an SVG file at the given path to an Icon of specified size (in pixels).
        /// </summary>
        /// <param name="svgPath">Path to the .svg file on disk.</param>
        /// <param name="iconSize">Desired width/height of the icon (in pixels).</param>
        /// <returns>An Icon object. Throws if svgPath is invalid or file not found.</returns>
        public static Icon ConvertSvgToIcon(string svgPath, int iconSize = 64)
        {
            if (string.IsNullOrWhiteSpace(svgPath) || !File.Exists(svgPath))
                throw new FileNotFoundException($"SVG file not found: {svgPath}");

            SvgDocument svgDoc = SvgDocument.Open(svgPath);
            return ConvertSvgToIcon(svgDoc, iconSize);
        }

        /// <summary>
        /// Converts an in-memory SvgDocument to an Icon of specified size (in pixels).
        /// </summary>
        /// <param name="svgDoc">An already-loaded SvgDocument object.</param>
        /// <param name="iconSize">Desired width/height of the icon (in pixels).</param>
        /// <returns>An Icon object created from the rendered SVG bitmap.</returns>
        public static Icon ConvertSvgToIcon(SvgDocument svgDoc, int iconSize = 64)
        {
            if (svgDoc == null)
                throw new ArgumentNullException(nameof(svgDoc));

            // Render the SVG to a Bitmap at the specified size
            using Bitmap bitmap = svgDoc.Draw(iconSize, iconSize);
            return CreateIconFromBitmap(bitmap);
        }

        /// <summary>
        /// Converts a PNG file at the given path to an Icon of specified size (in pixels).
        /// </summary>
        /// <param name="pngPath">Path to the .png file on disk.</param>
        /// <param name="iconSize">Desired width/height of the icon (in pixels).</param>
        /// <returns>An Icon object. Throws if pngPath is invalid or file not found.</returns>
        public static Icon ConvertPngToIcon(string pngPath, int iconSize = 64)
        {
            if (string.IsNullOrWhiteSpace(pngPath) || !File.Exists(pngPath))
                throw new FileNotFoundException($"PNG file not found: {pngPath}");

            using Bitmap sourceBitmap = new Bitmap(pngPath);
            // Resize the bitmap if needed
            using Bitmap resized = new Bitmap(sourceBitmap, new Size(iconSize, iconSize));

            return CreateIconFromBitmap(resized);
        }

        /// <summary>
        /// Helper method to convert a Bitmap into an Icon via PNG stream technique.
        /// </summary>
        private static Icon CreateIconFromBitmap(Bitmap bitmap)
        {
            // Convert the bitmap to a PNG in memory
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);

            return CreateIconFromPngStream(ms, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Creates an Icon from in-memory PNG data by constructing a minimal .ico file header.
        /// </summary>
        private static Icon CreateIconFromPngStream(Stream pngStream, int width, int height)
        {
            using BinaryReader reader = new BinaryReader(pngStream);
            byte[] pngData = reader.ReadBytes((int)pngStream.Length);

            using MemoryStream icoStream = new MemoryStream();
            // Write a simple ICO header for one icon image
            icoStream.Write(new byte[] { 0, 0, 1, 0, 1, 0 }, 0, 6);

            // Icon entry: Width & Height
            icoStream.WriteByte((byte)width);   // 0 => 256px, but we use the direct dimension if < 256
            icoStream.WriteByte((byte)height);
            icoStream.WriteByte(0); // No color palette
            icoStream.WriteByte(0); // Reserved
            // Planes = 1, BitCount = 32
            icoStream.Write(new byte[] { 1, 0, 32, 0 }, 0, 4);

            // Write the size of the raw PNG data
            icoStream.Write(BitConverter.GetBytes(pngData.Length), 0, 4);

            // Write the offset of the image data (22 bytes for this simple ICO header)
            icoStream.Write(BitConverter.GetBytes(22), 0, 4);

            // Write the actual PNG data
            icoStream.Write(pngData, 0, pngData.Length);
            icoStream.Seek(0, SeekOrigin.Begin);

            // Construct the Icon from the in-memory .ico structure
            return new Icon(icoStream);
        }
    }
}
