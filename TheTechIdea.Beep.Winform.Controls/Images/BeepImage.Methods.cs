using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region Designer Support

        // Ensure the control reports that it should be visible
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Ensure minimum size
            if (width < 16) width = 16;
            if (height < 16) height = 16;

            base.SetBoundsCore(x, y, width, height, specified);
        }

        // Override CreateParams if needed
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x10000000; // WS_VISIBLE
                return cp;
            }
        }

        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return _scaleMode switch
            {
                ImageScaleMode.Stretch => Math.Min(scaleX, scaleY), // Fit within bounds, stretching as needed
                ImageScaleMode.KeepAspectRatio => Math.Min(scaleX, scaleY), // Maintain aspect ratio
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => 1.0f // Default to no scaling
            };
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            float scaleFactor = GetScaleFactor(imageSize, targetRect.Size);

            float newWidth = imageSize.Width * scaleFactor;
            float newHeight = imageSize.Height * scaleFactor;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;  // Center horizontally
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2; // Center vertically

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        public float GetScaleFactor(SizeF imageSize)
        {
            float availableWidth = ClientSize.Width;
            float availableHeight = ClientSize.Height;

            switch (_scaleMode)
            {
                case ImageScaleMode.Stretch:
                    return 1.0f; // No scaling, just stretch
                case ImageScaleMode.KeepAspectRatioByWidth:
                    return availableWidth / imageSize.Width;
                case ImageScaleMode.KeepAspectRatioByHeight:
                    return availableHeight / imageSize.Height;
                case ImageScaleMode.KeepAspectRatio:
                default:
                    float scaleX = availableWidth / imageSize.Width;
                    float scaleY = availableHeight / imageSize.Height;
                    return Math.Min(scaleX, scaleY); // Maintain aspect ratio
            }
        }

        public RectangleF GetScaledBounds(SizeF imageSize)
        {
            float controlWidth = DrawingRect.Width;
            float controlHeight = DrawingRect.Height;

            float scaleX = controlWidth / imageSize.Width;
            float scaleY = controlHeight / imageSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = (controlWidth - newWidth) / 2;
            float yOffset = (controlHeight - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        #endregion

        #region Event Handlers for Hover and Pressed State

        protected override void OnMouseEnter(EventArgs e)
        {
            if (IsStillImage)
            {
                return;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsStillImage)
            {
                return;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            // ForeColor affects SVG recoloring when ImageEmbededin = Button or other contexts
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            if (_applyThemeOnImage && svgDocument != null)
            {
                ApplyThemeToSvg();
            }
            Invalidate();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            // BackColor can affect recoloring in some embedding contexts
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            if (_applyThemeOnImage && svgDocument != null)
            {
                ApplyThemeToSvg();
            }
            Invalidate();
        }

        #endregion

        #region Rotate

        public void Rotate90Clockwise()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 90f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        public void Rotate90CounterClockwise()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle - 90f + 360f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate270FlipNone);
        }

        public void Rotate180()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 180f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate180FlipNone);
        }

        public void FlipHorizontal()
        {
            if (isSvg && svgDocument != null)
            {
                _flipX = !_flipX;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
            else if (regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipX);
            }
        }

        public void FlipVertical()
        {
            if (isSvg && svgDocument != null)
            {
                _flipY = !_flipY;
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
            else if (regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipY);
            }
        }

        /// <summary>
        /// Rotate or flip only regular images using built-in support
        /// </summary>
        public void RotateImage(RotateFlipType rotateFlipType)
        {
            if (regularImage != null)
            {
                regularImage.RotateFlip(rotateFlipType);
                _stateChanged = true;
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = null;
                Invalidate();
            }
            else
            {
                MessageBox.Show("Rotation is only supported for regular images (PNG, JPG, BMP).", "Rotate Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Resets all rotation and flip states
        /// </summary>
        public void ResetTransformations()
        {
            ManualRotationAngle = 0;
            _flipX = false;
            _flipY = false;
            if (regularImage != null)
            {
                // Reload the original image if needed
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    LoadImage(ImagePath);
                }
            }
            _stateChanged = true;
            _cachedRenderedImage?.Dispose();
            _cachedRenderedImage = null;
            Invalidate();
        }

        #endregion

        #region IBeep UI Component Implementation

        public override void SetValue(object value)
        {
            ImagePath = string.Empty;
            if (value != null)
            {
                ImagePath = value.ToString();
            }
        }

        public override object GetValue()
        {
            return ImagePath;
        }

        public override void ClearValue()
        {
            ImagePath = "";
        }

        public override bool ValidateData(out string messege)
        {
            messege = "";
            return true;
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // Use the provided rectangle in DrawingRect for our drawing area
            DrawingRect = rectangle;

            // Draw the image at the specified rectangle coordinates
            DrawImage(graphics, rectangle);
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    foreach (var kv in _rasterizedSvgCache)
                        kv.Value.Dispose();
                    _rasterizedSvgCache.Clear();
                }
                catch { }
                _cachedRenderedImage?.Dispose();
                DisposeImages();
                _spinTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DisposeImages()
        {
            regularImage?.Dispose();
            regularImage = null;
            svgDocument = null;
        }

        #endregion
    }
}
