using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{

    internal class ControlDpiHelper
    {
        private readonly BaseControl _owner;

        public ControlDpiHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region DPI Scaling Properties
        public float DpiScaleFactor { get; private set; } = 1.0f;
        #endregion

        #region DPI Scaling Methods
        public void UpdateDpiScaling(Graphics g)
        {
            DpiScaleFactor = DpiScalingHelper.GetDpiScaleFactor(g);
        }

        public int ScaleValue(int value)
        {
            return DpiScalingHelper.ScaleValue(value, DpiScaleFactor);
        }

        public Size ScaleSize(Size size)
        {
            return DpiScalingHelper.ScaleSize(size, DpiScaleFactor);
        }

        public Font ScaleFont(Font font)
        {
            return DpiScalingHelper.ScaleFont(font, DpiScaleFactor);
        }

        public void SafeApplyFont(Font newFont, bool preserveLocation = true)
        {
            if (newFont == null) return;

            Point originalLocation = Point.Empty;
            Size originalSize = Size.Empty;
            if (preserveLocation)
            {
                originalLocation = _owner.Location;
                originalSize = _owner.Size;
            }

            Font scaledFont = newFont;
            if (DpiScaleFactor != 1.0f)
            {
                scaledFont = ScaleFont(newFont);
            }

            _owner.SuspendLayout();
            try
            {
                _owner.Font = scaledFont;

                if (preserveLocation)
                {
                    _owner.Location = originalLocation;
                    _owner.Size = originalSize;
                }
            }
            finally
            {
                _owner.ResumeLayout(false);
            }

            if (preserveLocation)
            {
                _owner.Location = originalLocation;
            }
        }
        #endregion

        #region Font Scaling Helper Methods
        public Font GetScaledFont(Graphics g, string text, Size maxSize, Font originalFont)
        {
            if (originalFont == null)
            {
                originalFont = _owner.Font;
            }

            if (Fits(g, text, originalFont, maxSize))
                return originalFont;

            float minSize = 6.0f;
            float lower = minSize;
            float upper = originalFont.Size;
            float finalSize = lower;

            while ((upper - lower) > 0.5f)
            {
                float mid = (lower + upper) / 2f;
                using (var testFont = new Font(originalFont.FontFamily, mid, originalFont.Style))
                {
                    if (Fits(g, text, testFont, maxSize))
                    {
                        finalSize = mid;
                        upper = mid;
                    }
                    else
                    {
                        lower = mid;
                    }
                }
            }

            return new Font(originalFont.FontFamily, finalSize, originalFont.Style);
        }

        private bool Fits(Graphics g, string text, Font font, Size maxSize)
        {
            var measured = TextRenderer.MeasureText(g, text, font);
            return (measured.Width <= maxSize.Width && measured.Height <= maxSize.Height);
        }
        #endregion

        #region Image Scaling
        public float GetScaleFactor(SizeF imageSize, Size targetSize, ImageScaleMode scaleMode)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return scaleMode switch
            {
                ImageScaleMode.Stretch => 1.0f,
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => Math.Min(scaleX, scaleY)
            };
        }

        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect, ImageScaleMode scaleMode)
        {
            float scale = GetScaleFactor(imageSize, targetRect.Size, scaleMode);

            if (scale <= 0)
                return RectangleF.Empty;

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        public RectangleF GetScaledBounds(SizeF imageSize, ImageScaleMode scaleMode)
        {
            float controlWidth = _owner.ClientSize.Width;
            float controlHeight = _owner.ClientSize.Height;

            float scaleX = controlWidth / imageSize.Width;
            float scaleY = controlHeight / imageSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = (controlWidth - newWidth) / 2;
            float yOffset = (controlHeight - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        public Size GetSuitableSizeForTextAndImage(Size imageSize, Size maxImageSize, TextImageRelation textImageRelation)
        {
            Size textSize = TextRenderer.MeasureText(_owner.Text, _owner.Font);

            if (imageSize.Width > maxImageSize.Width || imageSize.Height > maxImageSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)maxImageSize.Width / imageSize.Width,
                    (float)maxImageSize.Height / imageSize.Height);

                imageSize = new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }

            int width = 0;
            int height = 0;

            switch (textImageRelation)
            {
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    width = imageSize.Width + textSize.Width + _owner.Padding.Left + _owner.Padding.Right;
                    height = Math.Max(imageSize.Height, textSize.Height) + _owner.Padding.Top + _owner.Padding.Bottom;
                    break;

                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    width = Math.Max(imageSize.Width, textSize.Width) + _owner.Padding.Left + _owner.Padding.Right;
                    height = imageSize.Height + textSize.Height + _owner.Padding.Top + _owner.Padding.Bottom;
                    break;

                case TextImageRelation.Overlay:
                    width = Math.Max(imageSize.Width, textSize.Width) + _owner.Padding.Left + _owner.Padding.Right;
                    height = Math.Max(imageSize.Height, textSize.Height) + _owner.Padding.Top + _owner.Padding.Bottom;
                    break;
            }

            return new Size(width, height);
        }
        #endregion
    }
}