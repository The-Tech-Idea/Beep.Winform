using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Images.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region StyledImagePainter Integration

        /// <summary>
        /// Paints the image using StyledImagePainter with shape-based clipping
        /// This provides better integration with the styling system
        /// </summary>
        public void PaintWithStyledPainter(Graphics g, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(_imagepath))
                return;

            // Use StyledImagePainter for shape-based painting
            if (_clipShape != ImageClipShape.None)
            {
                // Create clip path
                using (GraphicsPath clipPath = ImageClipHelpers.CreateClipPath(bounds, _clipShape, _cornerRadius, _customClipPath))
                {
                    // Use StyledImagePainter with tinting if needed
                    if (fillColor != Color.Empty && fillColor != Color.Black)
                    {
                        StyledImagePainter.PaintWithTint(g, clipPath, _imagepath, fillColor, _opacity, (int)_cornerRadius);
                    }
                    else
                    {
                        StyledImagePainter.Paint(g, clipPath, _imagepath);
                    }
                }
            }
            else
            {
                // No clipping - use standard painting
                if (fillColor != Color.Empty && fillColor != Color.Black)
                {
                    StyledImagePainter.PaintWithTint(g, bounds, _imagepath, fillColor, _opacity, (int)_cornerRadius);
                }
                else
                {
                    StyledImagePainter.Paint(g, bounds, _imagepath, ControlStyle);
                }
            }
        }

        /// <summary>
        /// Paints the image in a specific styled shape using StyledImagePainter
        /// </summary>
        public void PaintInStyledShape(Graphics g, Rectangle bounds, StyledImagePainter.StyledShape shape, Color? tint = null)
        {
            if (string.IsNullOrEmpty(_imagepath))
                return;

            StyledImagePainter.PaintInShape(g, bounds, _imagepath, shape, tint ?? fillColor, _opacity);
        }

        /// <summary>
        /// Paints the image in a circle using StyledImagePainter
        /// </summary>
        public void PaintInCircle(Graphics g, float centerX, float centerY, float radius, Color? tint = null)
        {
            if (string.IsNullOrEmpty(_imagepath))
                return;

            StyledImagePainter.PaintInCircle(g, centerX, centerY, radius, _imagepath, tint ?? fillColor, _opacity);
        }

        /// <summary>
        /// Paints the image in a triangle using StyledImagePainter
        /// </summary>
        public void PaintInTriangle(Graphics g, float centerX, float centerY, float size, float rotation = 0f, Color? tint = null)
        {
            if (string.IsNullOrEmpty(_imagepath))
                return;

            StyledImagePainter.PaintInTriangle(g, centerX, centerY, size, _imagepath, rotation, tint ?? fillColor, _opacity);
        }

        /// <summary>
        /// Paints the image in a hexagon using StyledImagePainter
        /// </summary>
        public void PaintInHexagon(Graphics g, float centerX, float centerY, float size, float rotation = 0f, Color? tint = null)
        {
            if (string.IsNullOrEmpty(_imagepath))
                return;

            StyledImagePainter.PaintInHexagon(g, centerX, centerY, size, _imagepath, rotation, tint ?? fillColor, _opacity);
        }

        /// <summary>
        /// Paints the image in a custom shape using StyledImagePainter
        /// </summary>
        public void PaintInCustomShape(Graphics g, GraphicsPath customPath, Color? tint = null)
        {
            if (string.IsNullOrEmpty(_imagepath) || customPath == null)
                return;

            StyledImagePainter.PaintInCustomShape(g, customPath, _imagepath, tint ?? fillColor, _opacity, (int)_cornerRadius);
        }

        #endregion
    }
}
