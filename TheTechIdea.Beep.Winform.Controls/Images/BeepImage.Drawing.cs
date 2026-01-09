using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Images.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Images
{
    public partial class BeepImage
    {
        #region Image Drawing Methods

        /// <summary>
        /// Creates a clip path using ImageClipHelpers for better shape creation
        /// </summary>
        private GraphicsPath CreateClipPath(Rectangle bounds)
        {
            return ImageClipHelpers.CreateClipPath(bounds, _clipShape, _cornerRadius, _customClipPath);
        }

        /// <summary>
        /// Draws the image with clipping to the specified rectangle.
        /// Enhanced with region-based clipping support.
        /// </summary>
        /// <param name="g">The Graphics object to draw on</param>
        /// <param name="destRect">The destination rectangle in the target surface</param>
        /// <param name="drawRect">The source rectangle from the image to draw</param>
        public void Draw(Graphics g, Rectangle destRect, Rectangle drawRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var originalTransform = g.Transform;
            var originalClip = g.Clip;

            try
            {
                // Calculate rotation parameters
                float effectiveRotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);

                // Get center point of destination rectangle
                PointF center = new PointF(destRect.X + destRect.Width / 2f, destRect.Y + destRect.Height / 2f);

                // Apply clipping if needed - use region-based clipping for better performance
                if (ClipShape != ImageClipShape.None)
                {
                    if (_useRegionClipping)
                    {
                        // Use Region-based clipping (more efficient for complex shapes)
                        using (Region clipRegion = ImageClipHelpers.CreateClipRegion(destRect, _clipShape, _cornerRadius, _customClipPath))
                        {
                            ImageClipHelpers.ApplyClipRegion(g, clipRegion);
                            DrawImageContent(g, destRect, drawRect, effectiveRotation, center);
                        }
                    }
                    else
                    {
                        // Use GraphicsPath-based clipping (standard approach)
                        using (GraphicsPath clipPath = CreateClipPath(destRect))
                        {
                            ImageClipHelpers.ApplyClipPath(g, clipPath);
                            DrawImageContent(g, destRect, drawRect, effectiveRotation, center);
                        }
                    }
                }
                else
                {
                    // No clipping - draw directly
                    DrawImageContent(g, destRect, drawRect, effectiveRotation, center);
                }
            }
            finally
            {
                // Restore original graphics state
                g.Clip = originalClip;
                g.Transform = originalTransform;
            }
        }

        /// <summary>
        /// Draws the actual image content with transformations
        /// </summary>
        private void DrawImageContent(Graphics g, Rectangle destRect, Rectangle drawRect, float effectiveRotation, PointF center)
        {
            // Set up transformation for rotation and flipping
            g.TranslateTransform(center.X, center.Y);

            if (_flipX || _flipY)
            {
                g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
            }

            if (effectiveRotation != 0)
            {
                g.RotateTransform(effectiveRotation);
            }

            g.TranslateTransform(-center.X, -center.Y);

            // Now draw the image
            if (isSvg && svgDocument != null)
            {
                var imageSize = svgDocument.GetDimensions();

                // Calculate the scaling factors
                float scaleX = (float)destRect.Width / drawRect.Width;
                float scaleY = (float)destRect.Height / drawRect.Height;

                // Create a transformation matrix for the SVG
                g.TranslateTransform(destRect.X - drawRect.X * scaleX, destRect.Y - drawRect.Y * scaleY);
                g.ScaleTransform(scaleX, scaleY);

                svgDocument.Draw(g);
            }
            else if (regularImage != null)
            {
                // For bitmap images, we can use DrawImage with source and destination rectangles
                if (_grayscale || _opacity < 1.0f)
                {
                    using (ImageAttributes imageAttr = new ImageAttributes())
                    {
                        ColorMatrix colorMatrix = new ColorMatrix();

                        if (_grayscale)
                        {
                            // Grayscale conversion matrix
                            colorMatrix.Matrix00 = colorMatrix.Matrix11 = colorMatrix.Matrix22 = 0.299f;
                            colorMatrix.Matrix01 = colorMatrix.Matrix12 = 0.587f;
                            colorMatrix.Matrix02 = colorMatrix.Matrix21 = 0.114f;
                            colorMatrix.Matrix10 = colorMatrix.Matrix20 = 0;
                        }

                        // Apply opacity
                        colorMatrix.Matrix33 = _opacity;

                        imageAttr.SetColorMatrix(colorMatrix);

                        g.DrawImage(regularImage, destRect,
                            drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height,
                            GraphicsUnit.Pixel, imageAttr);
                    }
                }
                else
                {
                    g.DrawImage(regularImage, destRect,
                        drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height,
                        GraphicsUnit.Pixel);
                }
            }
        }

        public void DrawImage(Graphics g, Rectangle imageRect)
        {
            // Check if we need to regenerate the cached image
            bool needsRegeneration = CheckIfStateChanged(imageRect);

            // If nothing changed and we have a cached image, just draw it
            if (!needsRegeneration && _cachedRenderedImage != null && _cachedImageRect == imageRect)
            {
                g.DrawImage(_cachedRenderedImage, imageRect);
                return;
            }

            // Only perform expensive operations if state changed
            if (needsRegeneration)
            {
                RegenerateImage(imageRect);
            }

            // Draw the cached image
            if (_cachedRenderedImage != null)
            {
                g.DrawImage(_cachedRenderedImage, imageRect);
            }
        }

        private bool CheckIfStateChanged(Rectangle imageRect)
        {
            // Calculate current state
            float currentRotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);
            float currentScale = (_isPulsing || _isBouncing) ? _pulseScale : 1.0f;
            float currentAlpha = _isFading ? _fadeAlpha : 1.0f;
            int currentShakeOffset = _isShaking ? _shakeOffset : 0;

            // IMPORTANT: Also consider color-affecting changes by monitoring ForeColor/BackColor hash and ImageEmbededin
            int colorSignature = HashCode.Combine(ForeColor, BackColor, _imageEmbededin);

            // Check if any state has changed
            bool changed = _stateChanged ||
                           _lastImagePath != _imagepath ||
                           _lastImageRect != imageRect ||
                           Math.Abs(_lastRotation - currentRotation) > 0.1f ||
                           Math.Abs(_lastScale - currentScale) > 0.01f ||
                           Math.Abs(_lastAlpha - currentAlpha) > 0.01f ||
                           _lastShakeOffset != currentShakeOffset ||
                           _lastClipShape != _clipShape ||
                           _lastFlipX != _flipX ||
                           _lastFlipY != _flipY ||
                           _lastColorSignature != colorSignature;

            if (changed)
            {
                // Update last known state
                _lastImagePath = _imagepath;
                _lastImageRect = imageRect;
                _lastRotation = currentRotation;
                _lastScale = currentScale;
                _lastAlpha = currentAlpha;
                _lastShakeOffset = currentShakeOffset;
                _lastClipShape = _clipShape;
                _lastFlipX = _flipX;
                _lastFlipY = _flipY;
                _lastColorSignature = colorSignature;
                _stateChanged = false;
            }

            return changed;
        }

        // Track last color-related signature for cache invalidation
        private int _lastColorSignature;

        private void RegenerateImage(Rectangle imageRect)
        {
            // Dispose old cached image
            _cachedRenderedImage?.Dispose();

            // Create new bitmap for caching
            _cachedRenderedImage = new Bitmap(imageRect.Width, imageRect.Height, PixelFormat.Format32bppArgb);
            _cachedImageRect = imageRect;

            using (Graphics cacheGraphics = Graphics.FromImage(_cachedRenderedImage))
            {
                // Set high quality settings only when regenerating
                cacheGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                cacheGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                cacheGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                cacheGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Create a local rectangle for drawing to the cache
                Rectangle localRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);

                // Perform all the expensive transformations on the cache
                DrawToCache(cacheGraphics, localRect);
            }
        }

        private void DrawToCache(Graphics g, Rectangle rect)
        {
            // Store original graphics state
            var originalTransform = g.Transform;
            var originalCompositingMode = g.CompositingMode;
            var originalClip = g.Clip;

            try
            {
                // Calculate current animation values
                float rotation = _lastRotation;
                float scale = _lastScale;
                float alpha = _lastAlpha;
                int shakeOffset = _lastShakeOffset;

                // Center point for transformations
                PointF center = new PointF(rect.Width / 2f, rect.Height / 2f);

                // Apply clipping if needed - use region-based clipping for better performance
                if (_clipShape != ImageClipShape.None)
                {
                    if (_useRegionClipping)
                    {
                        using (Region clipRegion = ImageClipHelpers.CreateClipRegion(rect, _clipShape, _cornerRadius, _customClipPath))
                        {
                            ImageClipHelpers.ApplyClipRegion(g, clipRegion);
                            DrawCachedImageContent(g, rect, rotation, scale, alpha, shakeOffset, center);
                        }
                    }
                    else
                    {
                        using (GraphicsPath clipPath = CreateClipPath(rect))
                        {
                            ImageClipHelpers.ApplyClipPath(g, clipPath);
                            DrawCachedImageContent(g, rect, rotation, scale, alpha, shakeOffset, center);
                        }
                    }
                }
                else
                {
                    DrawCachedImageContent(g, rect, rotation, scale, alpha, shakeOffset, center);
                }
            }
            finally
            {
                // Restore graphics state
                g.Clip = originalClip;
                g.Transform = originalTransform;
                g.CompositingMode = originalCompositingMode;
            }
        }

        /// <summary>
        /// Draws the cached image content with transformations
        /// </summary>
        private void DrawCachedImageContent(Graphics g, Rectangle rect, float rotation, float scale, float alpha, int shakeOffset, PointF center)
        {
            // Apply transformations
            g.TranslateTransform(center.X, center.Y);

            if (_flipX || _flipY)
            {
                g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
            }

            g.ScaleTransform(scale, scale);
            g.RotateTransform(rotation);
            g.TranslateTransform(-center.X, -center.Y);

            if (shakeOffset != 0)
            {
                g.TranslateTransform(shakeOffset, 0);
            }

            // Draw the actual image
            if (isSvg && svgDocument != null)
            {
                SizeF imageSize = svgDocument.GetDimensions();
                RectangleF scaledBounds = GetScaledBounds(imageSize, rect);

                if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                {
                    g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                    g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);

                    if (alpha < 1.0f)
                        g.CompositingMode = CompositingMode.SourceOver;

                    svgDocument.Draw(g);
                }
            }
            else if (regularImage != null)
            {
                SizeF imageSize = regularImage.Size;
                RectangleF scaledBounds = GetScaledBounds(imageSize, rect);

                if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                {
                    if (alpha < 1.0f || _grayscale)
                    {
                        ColorMatrix matrix = new ColorMatrix();

                        if (_grayscale)
                        {
                            matrix.Matrix00 = matrix.Matrix11 = matrix.Matrix22 = 0.299f;
                            matrix.Matrix01 = matrix.Matrix12 = 0.587f;
                            matrix.Matrix02 = matrix.Matrix21 = 0.114f;
                            matrix.Matrix10 = matrix.Matrix20 = 0;
                        }

                        matrix.Matrix33 = alpha * _opacity;

                        using (ImageAttributes attr = new ImageAttributes())
                        {
                            attr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            g.DrawImage(regularImage, Rectangle.Round(scaledBounds), 0, 0,
                                      regularImage.Width, regularImage.Height, GraphicsUnit.Pixel, attr);
                        }
                    }
                    else
                    {
                        g.DrawImage(regularImage, scaledBounds);
                    }
                }
            }
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            DrawImage(
               g,
               DrawingRect
           );
        }

        #endregion

        #region Preview Generation Methods

        /// <summary>
        /// Applies the current clipping shape to an image and returns the result
        /// </summary>
        /// <param name="sourceImage">The source image to clip</param>
        /// <returns>A new image with the clipping shape applied</returns>
        public Image GenerateShapedImage(Image sourceImage)
        {
            if (sourceImage == null)
                return null;

            Bitmap result = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Create clip path based on the image dimensions
                Rectangle imageBounds = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                using (GraphicsPath clipPath = CreateClipPath(imageBounds))
                {
                    // Set clip region
                    g.SetClip(clipPath);

                    // Draw the image
                    g.DrawImage(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an image with the specified shape, color, and size
        /// </summary>
        public static Image CreateShapedImage(ImageClipShape shape, Color fillColor, Size size, float cornerRadius = 10f)
        {
            Bitmap result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle bounds = new Rectangle(0, 0, size.Width, size.Height);

                using (GraphicsPath path = ImageClipHelpers.CreateClipPath(bounds, shape, cornerRadius, null))
                {
                    using (Brush brush = new SolidBrush(fillColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
