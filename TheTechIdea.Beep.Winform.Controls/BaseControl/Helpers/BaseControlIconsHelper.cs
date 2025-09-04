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

            int pad = Math.Max(0, _owner.MaterialIconPadding);
            int zoneLeft = drawingRect.Left + 8;
            int zoneRight = drawingRect.Right - 8;
            int size = Math.Min(_owner.MaterialIconSize, Math.Max(12, drawingRect.Height - 8));

            // Calculate leading icon position
            if (!string.IsNullOrEmpty(_owner.LeadingIconPath) || !string.IsNullOrEmpty(_owner.LeadingImagePath))
            {
                _leadingRect = new Rectangle(zoneLeft, drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneLeft = _leadingRect.Right + pad;
            }

            // Calculate trailing icon position
            if (!string.IsNullOrEmpty(_owner.TrailingIconPath) || !string.IsNullOrEmpty(_owner.TrailingImagePath) ||
                (_owner.ShowClearButton && HasContent()))
            {
                _trailingRect = new Rectangle(Math.Max(zoneLeft, zoneRight - size), drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneRight = _trailingRect.Left - pad;
            }

            // Calculate adjusted content rectangle that excludes icon areas
            _adjustedContentRect = new Rectangle(
                zoneLeft,
                drawingRect.Top,
                Math.Max(0, zoneRight - zoneLeft),
                drawingRect.Height
            );
        }

        private bool HasContent()
        {
            // Check if control has content (this would need to be implemented based on control type)
            // For now, return false - derived controls should override this logic
            return false;
        }

        public void Draw(Graphics g)
        {
            // Draw leading icon
            if (!_leadingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.LeadingIconPath) ? _owner.LeadingIconPath : _owner.LeadingImagePath;
                if (_leadingimage == null)
                {
                    _leadingimage = new BeepImage();
                }
                _leadingimage.BackColor = _owner.BackColor;
                _leadingimage.ImagePath = path;
                _leadingimage.Size = _leadingRect.Size;
                _leadingimage.IsChild = true;
                _leadingimage.Draw(g, _leadingRect);
            }

            // Draw trailing icon
            if (!_trailingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.TrailingIconPath) ? _owner.TrailingIconPath : _owner.TrailingImagePath;
                if (_trailingimage == null)
                {
                    _trailingimage = new BeepImage();
                }
                Console.WriteLine(_trailingimage.BackColor + " " + _owner.BackColor);
                _trailingimage.BackColor = _owner.BackColor;
                
                _trailingimage.ImagePath = path;
                _trailingimage.Size = _trailingRect.Size;
                _trailingimage.IsChild = true;
                _trailingimage.Draw(g, _trailingRect);
            }
        }

        public Rectangle LeadingRect => _leadingRect;
        public Rectangle TrailingRect => _trailingRect;

        // New: Get the adjusted content rectangle that excludes icon areas
        public Rectangle AdjustedContentRect => _adjustedContentRect;
    }
}
