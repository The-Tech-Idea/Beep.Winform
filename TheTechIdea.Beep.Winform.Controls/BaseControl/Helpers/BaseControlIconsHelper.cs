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

        public BaseControlIconsHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public void UpdateLayout(Rectangle drawingRect)
        {
            _leadingRect = Rectangle.Empty;
            _trailingRect = Rectangle.Empty;

            int pad = Math.Max(0, _owner.MaterialIconPadding);
            int zoneLeft = drawingRect.Left + 8;
            int zoneRight = drawingRect.Right - 8;
            int size = Math.Min(_owner.MaterialIconSize, Math.Max(12, drawingRect.Height - 8));

            if (!string.IsNullOrEmpty(_owner.LeadingIconPath) || !string.IsNullOrEmpty(_owner.LeadingImagePath))
            {
                _leadingRect = new Rectangle(zoneLeft, drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
                zoneLeft = _leadingRect.Right + pad;
            }

            if (!string.IsNullOrEmpty(_owner.TrailingIconPath) || !string.IsNullOrEmpty(_owner.TrailingImagePath))
            {
                _trailingRect = new Rectangle(Math.Max(zoneLeft, zoneRight - size), drawingRect.Top + (drawingRect.Height - size) / 2, size, size);
            }
        }

        public void Draw(Graphics g)
        {
            if (!_leadingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.LeadingIconPath) ? _owner.LeadingIconPath : _owner.LeadingImagePath;
                using var img = new BeepImage { ImagePath = path, Size = _leadingRect.Size, IsChild = true };
                img.Draw(g, _leadingRect);
            }
            if (!_trailingRect.IsEmpty)
            {
                string path = !string.IsNullOrEmpty(_owner.TrailingIconPath) ? _owner.TrailingIconPath : _owner.TrailingImagePath;
                using var img = new BeepImage { ImagePath = path, Size = _trailingRect.Size, IsChild = true };
                img.Draw(g, _trailingRect);
            }
        }

        public Rectangle LeadingRect => _leadingRect;
        public Rectangle TrailingRect => _trailingRect;
    }
}
