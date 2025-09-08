using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Provides unified layout rectangles so painting and child layout stay consistent.
    /// </summary>
    internal sealed class FormLayoutHelper
    {
        private readonly IBeepModernFormHost _host;

        public FormLayoutHelper(IBeepModernFormHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        /// <summary>
        /// Full paint bounds inside current Padding.
        /// </summary>
        public Rectangle GetPaintBounds()
        {
            var form = _host.AsForm;
            if (form.ClientSize.Width <= 0 || form.ClientSize.Height <= 0) return Rectangle.Empty;
            var pad = _host.Padding;
            return new Rectangle(pad.Left, pad.Top,
                Math.Max(0, form.ClientSize.Width - pad.Horizontal),
                Math.Max(0, form.ClientSize.Height - pad.Vertical));
        }

        /// <summary>
        /// Content bounds (same as paint bounds for now; can add caption/ribbon offsets later).
        /// </summary>
        public Rectangle GetContentBounds() => GetPaintBounds();
    }
}
