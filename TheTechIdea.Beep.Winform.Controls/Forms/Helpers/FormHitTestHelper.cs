using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Helpers
{
    /// <summary>
    /// Centralizes edge and caption hit-testing for a borderless custom form.
    /// </summary>
    internal sealed class FormHitTestHelper
    {
        private readonly IBeepModernFormHost _host;
        private readonly Func<bool> _captionEnabled;
        private readonly Func<int> _captionHeight;
        private readonly Func<bool> _isOverSystemButton;
        private int _resizeMargin;

        public FormHitTestHelper(IBeepModernFormHost host, Func<bool> captionEnabled, Func<int> captionHeight, Func<bool> isOverSystemButton, int resizeMargin)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _captionEnabled = captionEnabled;
            _captionHeight = captionHeight;
            _isOverSystemButton = isOverSystemButton;
            _resizeMargin = resizeMargin;
        }

        public void SetResizeMargin(int margin) => _resizeMargin = Math.Max(0, margin);

        /// <summary>
        /// Performs WM_NCHITTEST logic and writes result to message if handled.
        /// </summary>
        public bool HandleNcHitTest(ref Message m)
        {
            var form = _host.AsForm;
            Point pos = form.PointToClient(new Point(m.LParam.ToInt32()));
            int margin = _resizeMargin;
            var cs = form.ClientSize;

            // Edges / corners (priority first)
            if (pos.X <= margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPLEFT; return true; }
            if (pos.X >= cs.Width - margin && pos.Y <= margin) { m.Result = (IntPtr)HTTOPRIGHT; return true; }
            if (pos.X <= margin && pos.Y >= cs.Height - margin) { m.Result = (IntPtr)HTBOTTOMLEFT; return true; }
            if (pos.X >= cs.Width - margin && pos.Y >= cs.Height - margin) { m.Result = (IntPtr)HTBOTTOMRIGHT; return true; }
            if (pos.X <= margin) { m.Result = (IntPtr)HTLEFT; return true; }
            if (pos.X >= cs.Width - margin) { m.Result = (IntPtr)HTRIGHT; return true; }
            if (pos.Y <= margin) { m.Result = (IntPtr)HTTOP; return true; }
            if (pos.Y >= cs.Height - margin) { m.Result = (IntPtr)HTBOTTOM; return true; }

            // Caption drag area (exclude system buttons)
            if (_captionEnabled() && pos.Y <= _captionHeight())
            {
                if (_isOverSystemButton()) { m.Result = (IntPtr)HTCLIENT; return true; }
                m.Result = (IntPtr)HTCAPTION; return true;
            }

            return false; // not handled
        }

        #region HT constants
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;
        private const int HTCAPTION = 2;
        private const int HTCLIENT = 1;
        #endregion
    }
}
