using System;
using System.Drawing;
using System.Windows.Forms;

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

        public void UpdateLayout(Rectangle drawingRect)
        {
            _leadingRect = Rectangle.Empty;
            _trailingRect = Rectangle.Empty;

            int pad = Math.Max(0, _owner.IconPadding);
            int zoneLeft = drawingRect.Left + pad;
            int zoneRight = drawingRect.Right - pad;
            int size = Math.Min(_owner.IconSize, Math.Max(12, drawingRect.Height - pad - pad));

            if (!string.IsNullOrEmpty(_owner.LeadingIconPath) || !string.IsNullOrEmpty(_owner.LeadingImagePath))
            {
                _leadingRect = new Rectangle(zoneLeft, drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneLeft = _leadingRect.Right + pad;
            }

            if (!string.IsNullOrEmpty(_owner.TrailingIconPath) || !string.IsNullOrEmpty(_owner.TrailingImagePath) || _owner.ShowClearButton)
            {
                _trailingRect = new Rectangle(Math.Max(zoneLeft, zoneRight - size), drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneRight = _trailingRect.Left - pad;
            }

            _adjustedContentRect = new Rectangle(zoneLeft, drawingRect.Top, Math.Max(0, zoneRight - zoneLeft), drawingRect.Height);
        }

        public void Draw(Graphics g)
        {
            if (!_leadingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.LeadingIconPath) ? _owner.LeadingIconPath : _owner.LeadingImagePath;
                EnsureBeepImageConfigured(_leadingimage, path, _leadingRect.Size);
                _leadingimage.DrawImage(g, _leadingRect);
            }

            if (!_trailingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.TrailingIconPath) ? _owner.TrailingIconPath : _owner.TrailingImagePath;
                EnsureBeepImageConfigured(_trailingimage, path, _trailingRect.Size);
                _trailingimage.DrawImage(g, _trailingRect);
            }
        }

        private void EnsureBeepImageConfigured(BeepImage target, string imagePath, Size size)
        {
            if (target == null) return;
            target.IsChild = true;
            target.BackColor = _owner.BackColor;
            target.ForeColor = _owner.ForeColor;
            target.ApplyThemeOnImage = false; // allow theme tint
            target.PreserveSvgBackgrounds = true; // keep background shapes (e.g., circle) from the SVG
            target.Size = size;
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
