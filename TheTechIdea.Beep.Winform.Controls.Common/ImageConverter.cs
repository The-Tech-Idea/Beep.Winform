using System.Drawing.Imaging;
using Svg;
using TheTechIdea.Beep.Vis.Modules;

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
        public static float GetScaleFactor(SizeF imageSize, Size targetSize, ImageScaleMode scaleMode)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return scaleMode switch
            {
                ImageScaleMode.Stretch => Math.Min(scaleX, scaleY), // Fit within bounds, stretching as needed
                ImageScaleMode.KeepAspectRatio => Math.Min(scaleX, scaleY), // Maintain aspect ratio
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => 1.0f // Default to no scaling
            };
        }
        public static RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect, ImageScaleMode scaleMode)
        {
            float scaleFactor = GetScaleFactor(imageSize, targetRect.Size, scaleMode);

            float newWidth = imageSize.Width * scaleFactor;
            float newHeight = imageSize.Height * scaleFactor;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;  // Center horizontally
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2; // Center vertically

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }
        public static Image ScaleSvgImage(Graphics g, SvgDocument svgDocument, Rectangle imageRect, float _manualRotationAngle,ImageScaleMode scaleMode)
        {
            var originalTransform = g.Transform;
            try
            {
               
                float effectiveRotation = _manualRotationAngle;
                PointF center = new PointF(imageRect.X + imageRect.Width / 2f, imageRect.Y + imageRect.Height / 2f);

                // Apply rotation transformations
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(effectiveRotation);
                g.TranslateTransform(-center.X, -center.Y);

                if (svgDocument != null)
                {
                    var imageSize = svgDocument.GetDimensions();
                    var scaledBounds = GetScaledBounds(new SizeF(imageSize.Width, imageSize.Height), imageRect, scaleMode);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                        g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);
                        svgDocument.Draw(g);
                    }
                    // create new scaled image from svg document
                    Bitmap bmp = new Bitmap((int)scaledBounds.Width, (int)scaledBounds.Height);
                    using (Graphics g2 = Graphics.FromImage(bmp))
                    {
                        g2.Clear(Color.Transparent);
                        g2.TranslateTransform(-scaledBounds.X, -scaledBounds.Y);
                        g2.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);
                        svgDocument.Draw(g2);
                    }

                }

            }
            finally
            {
                // Restore the original transformation state
                g.Transform = originalTransform;
            }
            return null;
        }
        public static Image ScaleRegularImage(Graphics g, Image regularImage, Rectangle imageRect, float _manualRotationAngle, ImageScaleMode scaleMode)
        {
            var originalTransform = g.Transform;
            try
            {
                float effectiveRotation = _manualRotationAngle;
                PointF center = new PointF(imageRect.X + imageRect.Width / 2f, imageRect.Y + imageRect.Height / 2f);

                // Apply rotation transformations
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(effectiveRotation);
                g.TranslateTransform(-center.X, -center.Y);
                if (regularImage != null)
                {
                    var scaledBounds = GetScaledBounds(new SizeF(regularImage.Width, regularImage.Height), imageRect, scaleMode);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.DrawImage(
                            regularImage,
                            new Rectangle((int)scaledBounds.X, (int)scaledBounds.Y, (int)scaledBounds.Width, (int)scaledBounds.Height),
                            0,
                            0,
                            regularImage.Width,
                            regularImage.Height,
                            GraphicsUnit.Pixel
                        );
                        // create new scaled image from regular image
                        Bitmap bmp = new Bitmap((int)scaledBounds.Width, (int)scaledBounds.Height);
                        using (Graphics g2 = Graphics.FromImage(bmp))
                        {
                            g2.Clear(Color.Transparent);
                            g2.DrawImage(
                                regularImage,
                                new Rectangle(0, 0, (int)scaledBounds.Width, (int)scaledBounds.Height),
                                0,
                                0,
                                regularImage.Width,
                                regularImage.Height,
                                GraphicsUnit.Pixel
                            );
                        }
                        return bmp;
                    }
                }
            }
            finally
            {
                // Restore the original transformation state
                g.Transform = originalTransform;
            }
            return null;
        }
    }
}
