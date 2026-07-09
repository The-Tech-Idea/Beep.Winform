using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal sealed class BaseControlIconsHelper
    {
        private readonly BaseControl _owner;
        private Rectangle _leadingRect;
        private Rectangle _trailingRect;
        private Rectangle _adjustedContentRect;
        private BeepImage _leadingimage;
        private BeepImage _trailingimage;

        public BaseControlIconsHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _leadingimage = new BeepImage();
            _trailingimage = new BeepImage();
        }

        /// <summary>DPI-scales a pixel value using the owner control, for layout-only use.</summary>
        private int Sc(int px) => DpiScalingHelper.ScaleValue(px, _owner);

        /// <summary>
        /// Computes leading and trailing icon rectangles within the given drawing area.
        /// Icons are vertically centered. Their size is bounded by the owner's
        /// <see cref="BaseControl.IconSize"/> (DPI-scaled) and the available height.
        /// When both icons are present and the control is too narrow, the trailing
        /// icon takes priority and the leading icon shrinks to fit.
        /// </summary>
        public void UpdateLayout(Rectangle drawingRect)
        {
            _leadingRect = Rectangle.Empty;
            _trailingRect = Rectangle.Empty;

            int pad   = Sc(_owner.IconPadding);
            int maxSz = Sc(_owner.IconSize);
            int availH = drawingRect.Height - pad * 2;
            if (availH < 4) return; // too small to render any icon

            // Icon size: the desired logical size, bounded by available height.
            int size = Math.Min(maxSz, availH);

            bool hasLeading  = !string.IsNullOrEmpty(_owner.LeadingIconPath)  || !string.IsNullOrEmpty(_owner.LeadingImagePath);
            bool hasTrailing = !string.IsNullOrEmpty(_owner.TrailingIconPath) || !string.IsNullOrEmpty(_owner.TrailingImagePath) || _owner.ShowClearButton;

            int zoneLeft  = drawingRect.Left  + pad;
            int zoneRight = drawingRect.Right - pad;

            if (hasTrailing)
            {
                _trailingRect = new Rectangle(zoneRight - size, drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneRight = _trailingRect.Left - pad;
            }

            if (hasLeading)
            {
                // Constrain the leading icon so it doesn't collide with the trailing icon.
                int availW = Math.Max(size, zoneRight - zoneLeft);
                int leadSize = Math.Min(size, availW);
                _leadingRect = new Rectangle(zoneLeft, drawingRect.Top + (drawingRect.Height - leadSize) / 2, leadSize, leadSize);
                zoneLeft = _leadingRect.Right + pad;
            }

            _adjustedContentRect = new Rectangle(zoneLeft, drawingRect.Top, Math.Max(0, zoneRight - zoneLeft), drawingRect.Height);
        }

        public void Draw(Graphics g)
        {
            if (!_leadingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.LeadingIconPath) ? _owner.LeadingIconPath : _owner.LeadingImagePath;
                EnsureImageConfigured(_leadingimage, path, _leadingRect.Size, isLeading: true);
                _leadingimage.DrawImage(g, _leadingRect);
            }

            if (!_trailingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.TrailingIconPath) ? _owner.TrailingIconPath : _owner.TrailingImagePath;
                if (string.IsNullOrEmpty(path) && _owner.ShowClearButton)
                    path = "ClearButton"; // placeholder — BeepImage renders a themed X
                EnsureImageConfigured(_trailingimage, path, _trailingRect.Size, isLeading: false);
                _trailingimage.DrawImage(g, _trailingRect);
            }
        }

        private void EnsureImageConfigured(BeepImage target, string imagePath, Size size, bool isLeading)
        {
            if (target == null) return;
            target.IsChild = true;
            target.BackColor = _owner.BackColor;
            target.ApplyThemeOnImage = true; // apply theme tint so icons match text colour
            target.PreserveSvgBackgrounds = true;
            target.Size = size;

            // Leading icon uses the owner's ForeColor; trailing uses a muted/accent variant.
            target.ForeColor = isLeading
                ? _owner.ForeColor
                : (_owner._currentTheme?.AccentColor ?? _owner.ForeColor);

            if (!string.Equals(target.ImagePath, imagePath, StringComparison.OrdinalIgnoreCase))
            {
                target.ImagePath = imagePath;
            }
        }

        public Rectangle LeadingRect => _leadingRect;
        public Rectangle TrailingRect => _trailingRect;
        public Rectangle AdjustedContentRect => _adjustedContentRect;
    }
}
