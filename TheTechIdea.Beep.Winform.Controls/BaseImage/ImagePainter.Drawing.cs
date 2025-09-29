using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.BaseImage
{
    public partial class ImagePainter
    {
        public void DrawImage(Graphics g, string imagePath, Rectangle bounds)
        {
            if (!HasImage || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            if (!UseCaching)
            {
                DrawToCache(g, bounds);
                return;
            }

            bool needsRegeneration = CheckIfStateChanged(bounds);

            if (!needsRegeneration && _cachedRenderedImage != null && _cachedImageRect == bounds)
            {
                g.DrawImage(_cachedRenderedImage, bounds);
                return;
            }

            if (needsRegeneration)
            {
                RegenerateImage(bounds);
            }

            if (_cachedRenderedImage != null)
            {
                g.DrawImage(_cachedRenderedImage, bounds);
            }
        }

        public void DrawImage(Graphics g, Rectangle bounds)
        {
            if (!HasImage || bounds.Width <= 0 || bounds.Height <= 0)
                return;

            if (!UseCaching)
            {
                DrawToCache(g, bounds);
                return;
            }

            bool needsRegeneration = CheckIfStateChanged(bounds);

            if (!needsRegeneration && _cachedRenderedImage != null && _cachedImageRect == bounds)
            {
                g.DrawImage(_cachedRenderedImage, bounds);
                return;
            }

            if (needsRegeneration)
            {
                RegenerateImage(bounds);
            }

            if (_cachedRenderedImage != null)
            {
                g.DrawImage(_cachedRenderedImage, bounds);
            }
        }

        public void DrawImage(Graphics g, Rectangle destRect, Rectangle sourceRect)
        {
            if (!HasImage)
                return;

            g.SmoothingMode = Smoothing;
            g.InterpolationMode = Interpolation;
            g.PixelOffsetMode = PixelOffset;
            g.TextRenderingHint = TextRendering;

            var originalTransform = g.Transform;
            var originalClip = g.Clip;

            try
            {
                float effectiveRotation = _manualRotationAngle;
                PointF center = new PointF(destRect.X + destRect.Width / 2f, destRect.Y + destRect.Height / 2f);

                GraphicsPath clipPath = null;
                if (_clipShape != ImageClipShape.None)
                {
                    clipPath = CreateClipPath(destRect);
                    if (_drawBackground && _backgroundColor.A > 0)
                    {
                        using var bgBrush = new SolidBrush(_backgroundColor);
                        g.FillPath(bgBrush, clipPath);
                    }
                    g.SetClip(clipPath);
                }

                g.TranslateTransform(center.X, center.Y);
                if (_flipX || _flipY) g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
                if (effectiveRotation != 0) g.RotateTransform(effectiveRotation);
                g.TranslateTransform(-center.X, -center.Y);

                if (_isSvg && _svgDocument != null)
                {
                    float scaleX = (float)destRect.Width / sourceRect.Width;
                    float scaleY = (float)destRect.Height / sourceRect.Height;
                    g.TranslateTransform(destRect.X - sourceRect.X * scaleX, destRect.Y - sourceRect.Y * scaleY);
                    g.ScaleTransform(scaleX, scaleY);
                    _svgDocument.Draw(g);
                }
                else if (_regularImage != null)
                {
                    if (_grayscale || _opacity < 1.0f)
                    {
                        using (ImageAttributes imageAttr = new ImageAttributes())
                        {
                            ColorMatrix colorMatrix = new ColorMatrix();
                            if (_grayscale)
                            {
                                colorMatrix.Matrix00 = colorMatrix.Matrix11 = colorMatrix.Matrix22 = 0.299f;
                                colorMatrix.Matrix01 = colorMatrix.Matrix12 = 0.587f;
                                colorMatrix.Matrix02 = colorMatrix.Matrix21 = 0.114f;
                                colorMatrix.Matrix10 = colorMatrix.Matrix20 = 0;
                            }
                            colorMatrix.Matrix33 = _opacity;
                            imageAttr.SetColorMatrix(colorMatrix);
                            g.DrawImage(_regularImage, destRect, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, GraphicsUnit.Pixel, imageAttr);
                        }
                    }
                    else
                    {
                        g.DrawImage(_regularImage, destRect, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height, GraphicsUnit.Pixel);
                    }
                }

                if (clipPath != null && _drawBorder && _borderThickness > 0 && _borderColor.A > 0)
                {
                    g.ResetClip();
                    using var pen = new Pen(_borderColor, _borderThickness);
                    g.DrawPath(pen, clipPath);
                }

                clipPath?.Dispose();
            }
            finally
            {
                g.Clip = originalClip;
                g.Transform = originalTransform;
            }
        }

        public void PreRender(Size size)
        {
            if (size.Width <= 0 || size.Height <= 0) return;
            RegenerateImage(new Rectangle(Point.Empty, size));
        }

        private bool CheckIfStateChanged(Rectangle imageRect)
        {
            float currentRotation = _manualRotationAngle;
            float currentScale = _pulseScale * _scaleFactor;
            float currentAlpha = _fadeAlpha * _opacity;
            int currentShakeOffset = _shakeOffset;
            int colorSignature = HashCode.Combine(_fillColor, _strokeColor, _imageEmbededin, _backgroundColor, _borderColor);

            bool changed = _stateChanged ||
                           _lastImagePath != _imagePath ||
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
                _lastImagePath = _imagePath;
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

        private void RegenerateImage(Rectangle imageRect)
        {
            lock (_cacheLock)
            {
                _cachedRenderedImage?.Dispose();
                _cachedRenderedImage = new Bitmap(imageRect.Width, imageRect.Height, PixelFormat.Format32bppArgb);
                _cachedImageRect = imageRect;

                using (Graphics cacheGraphics = Graphics.FromImage(_cachedRenderedImage))
                {
                    cacheGraphics.SmoothingMode = Smoothing;
                    cacheGraphics.InterpolationMode = Interpolation;
                    cacheGraphics.PixelOffsetMode = PixelOffset;
                    cacheGraphics.TextRenderingHint = TextRendering;

                    Rectangle localRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);
                    DrawToCache(cacheGraphics, localRect);
                }
            }
        }

        private RectangleF GetAlignedScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            var baseBounds = this.GetScaledBounds(imageSize, targetRect);
            float dx = 0, dy = 0;
            if (baseBounds.Width < targetRect.Width)
            {
                switch (_alignment)
                {
                    case System.Drawing.ContentAlignment.TopLeft:
                    case System.Drawing.ContentAlignment.MiddleLeft:
                    case System.Drawing.ContentAlignment.BottomLeft:
                        dx = -(targetRect.Width - baseBounds.Width) / 2; break;
                    case System.Drawing.ContentAlignment.TopRight:
                    case System.Drawing.ContentAlignment.MiddleRight:
                    case System.Drawing.ContentAlignment.BottomRight:
                        dx = (targetRect.Width - baseBounds.Width) / 2; break;
                }
            }
            if (baseBounds.Height < targetRect.Height)
            {
                switch (_alignment)
                {
                    case System.Drawing.ContentAlignment.TopLeft:
                    case System.Drawing.ContentAlignment.TopCenter:
                    case System.Drawing.ContentAlignment.TopRight:
                        dy = -(targetRect.Height - baseBounds.Height) / 2; break;
                    case System.Drawing.ContentAlignment.BottomLeft:
                    case System.Drawing.ContentAlignment.BottomCenter:
                    case System.Drawing.ContentAlignment.BottomRight:
                        dy = (targetRect.Height - baseBounds.Height) / 2; break;
                }
            }
            return new RectangleF(baseBounds.X + dx, baseBounds.Y + dy, baseBounds.Width, baseBounds.Height);
        }

        private void DrawToCache(Graphics g, Rectangle rect)
        {
            var originalTransform = g.Transform;
            var originalCompositingMode = g.CompositingMode;
            var originalClip = g.Clip;

            try
            {
                g.SmoothingMode = Smoothing;
                g.InterpolationMode = Interpolation;
                g.PixelOffsetMode = PixelOffset;
                g.TextRenderingHint = TextRendering;

                var paddedRect = new Rectangle(
                    rect.X + _contentPadding.Left,
                    rect.Y + _contentPadding.Top,
                    Math.Max(0, rect.Width - _contentPadding.Horizontal),
                    Math.Max(0, rect.Height - _contentPadding.Vertical)
                );

                float rotation = _lastRotation;
                float scale = _lastScale;
                float alpha = _lastAlpha;
                int shakeOffset = _lastShakeOffset;

                GraphicsPath clipPath = null;
                if (_clipShape != ImageClipShape.None)
                {
                    clipPath = CreateClipPath(paddedRect);
                    if (_drawBackground && _backgroundColor.A > 0)
                    {
                        using var bgBrush = new SolidBrush(_backgroundColor);
                        g.FillPath(bgBrush, clipPath);
                    }
                    g.SetClip(clipPath);
                }

                RectangleF scaledBounds;
                if (_isSvg && _svgDocument != null)
                {
                    SizeF imageSize = _svgDocument.GetDimensions();
                    scaledBounds = GetAlignedScaledBounds(imageSize, paddedRect);
                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        PointF center = new PointF(paddedRect.X + paddedRect.Width / 2f, paddedRect.Y + paddedRect.Height / 2f);
                        g.TranslateTransform(center.X, center.Y);
                        if (_flipX || _flipY) g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
                        g.ScaleTransform(scale, scale);
                        g.RotateTransform(rotation);
                        g.TranslateTransform(-center.X, -center.Y);
                        if (shakeOffset != 0) g.TranslateTransform(shakeOffset, 0);

                        g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                        g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);
                        if (alpha < 1.0f) g.CompositingMode = CompositingMode.SourceOver;
                        _svgDocument.Draw(g);
                    }
                }
                else if (_regularImage != null)
                {
                    SizeF imageSize = _regularImage.Size;
                    scaledBounds = GetAlignedScaledBounds(imageSize, paddedRect);
                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        PointF center = new PointF(paddedRect.X + paddedRect.Width / 2f, paddedRect.Y + paddedRect.Height / 2f);
                        g.TranslateTransform(center.X, center.Y);
                        if (_flipX || _flipY) g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
                        g.ScaleTransform(scale, scale);
                        g.RotateTransform(rotation);
                        g.TranslateTransform(-center.X, -center.Y);
                        if (shakeOffset != 0) g.TranslateTransform(shakeOffset, 0);

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
                            matrix.Matrix33 = alpha;
                            using (ImageAttributes attr = new ImageAttributes())
                            {
                                attr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                g.DrawImage(_regularImage, Rectangle.Round(scaledBounds), 0, 0, _regularImage.Width, _regularImage.Height, GraphicsUnit.Pixel, attr);
                            }
                        }
                        else
                        {
                            g.DrawImage(_regularImage, scaledBounds);
                        }
                    }
                }

                if (clipPath != null && _drawBorder && _borderThickness > 0 && _borderColor.A > 0)
                {
                    g.ResetClip();
                    using var pen = new Pen(_borderColor, _borderThickness);
                    g.DrawPath(pen, clipPath);
                }

                clipPath?.Dispose();
            }
            finally
            {
                g.Clip = originalClip;
                g.Transform = originalTransform;
                g.CompositingMode = originalCompositingMode;
            }
        }

        private GraphicsPath CreateClipPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            switch (_clipShape)
            {
                case ImageClipShape.Circle:
                    int diameter = Math.Min(bounds.Width, bounds.Height);
                    int offsetX = (bounds.Width - diameter) / 2;
                    int offsetY = (bounds.Height - diameter) / 2;
                    path.AddEllipse(bounds.X + offsetX, bounds.Y + offsetY, diameter, diameter);
                    break;

                case ImageClipShape.RoundedRect:
                    path = GetRoundedRectPath(bounds, (int)_cornerRadius);
                    break;

                case ImageClipShape.Ellipse:
                    path.AddEllipse(bounds);
                    break;

                case ImageClipShape.Diamond:
                    Point[] diamondPoints = new Point[4];
                    diamondPoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);
                    diamondPoints[1] = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2);
                    diamondPoints[2] = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);
                    diamondPoints[3] = new Point(bounds.X, bounds.Y + bounds.Height / 2);
                    path.AddPolygon(diamondPoints);
                    break;

                case ImageClipShape.Triangle:
                    Point[] trianglePoints = new Point[3];
                    trianglePoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);
                    trianglePoints[1] = new Point(bounds.X, bounds.Y + bounds.Height);
                    trianglePoints[2] = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height);
                    path.AddPolygon(trianglePoints);
                    break;

                case ImageClipShape.Hexagon:
                    Point[] hexagonPoints = new Point[6];
                    int quarterHeight = bounds.Height / 4;
                    hexagonPoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);
                    hexagonPoints[1] = new Point(bounds.X + bounds.Width, bounds.Y + quarterHeight);
                    hexagonPoints[2] = new Point(bounds.X + bounds.Width, bounds.Y + 3 * quarterHeight);
                    hexagonPoints[3] = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);
                    hexagonPoints[4] = new Point(bounds.X, bounds.Y + 3 * quarterHeight);
                    hexagonPoints[5] = new Point(bounds.X, bounds.Y + quarterHeight);
                    path.AddPolygon(hexagonPoints);
                    break;

                case ImageClipShape.Custom:
                    if (_customClipPath != null)
                    {
                        path.AddPath(_customClipPath, false);
                    }
                    else
                    {
                        path.AddRectangle(bounds);
                    }
                    break;

                case ImageClipShape.None:
                default:
                    path.AddRectangle(bounds);
                    break;
            }

            return path;
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);
            int d = r * 2;

            if (r > 0)
            {
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }
            return path;
        }

        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;
            return _scaleMode switch
            {
                ImageScaleMode.Stretch => Math.Min(scaleX, scaleY),
                ImageScaleMode.KeepAspectRatio => Math.Min(scaleX, scaleY),
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => 1.0f
            };
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            float scaleFactor = GetScaleFactor(imageSize, targetRect.Size);
            float newWidth = imageSize.Width * scaleFactor;
            float newHeight = imageSize.Height * scaleFactor;
            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2;
            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }
    }
}
