using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Keeps a visible tooltip attached to its anchor while the anchor moves.
    /// <para>
    /// A tooltip is a top-level window, so unlike a child control it does not move with its parent
    /// for free. Without this, a tooltip positioned once at show time stays where it was while the
    /// user scrolls the container, drags the window, maximises it, or moves it to another monitor —
    /// floating over unrelated content and pointing at nothing.
    /// </para>
    /// <para>
    /// This is the WinForms equivalent of Floating UI's <c>autoUpdate</c>, minus the observers the
    /// web has: it subscribes to the anchor, its scrollable ancestors, and its top-level form.
    /// </para>
    /// </summary>
    internal sealed class ToolTipAutoUpdate : IDisposable
    {
        private readonly Control _anchor;
        private readonly Action<Rectangle> _reposition;
        private readonly Action _hide;
        private readonly List<Control> _subscribed = new();
        private readonly Timer _coalesce;
        private Rectangle _lastAnchorRect = Rectangle.Empty;
        private bool _disposed;

        /// <param name="anchor">The control the tooltip describes.</param>
        /// <param name="reposition">Called with the anchor's new screen rectangle.</param>
        /// <param name="hide">Called when the anchor can no longer host a tooltip.</param>
        public ToolTipAutoUpdate(Control anchor, Action<Rectangle> reposition, Action hide)
        {
            _anchor = anchor ?? throw new ArgumentNullException(nameof(anchor));
            _reposition = reposition ?? throw new ArgumentNullException(nameof(reposition));
            _hide = hide ?? throw new ArgumentNullException(nameof(hide));

            // Scroll and resize arrive in bursts. Coalescing to roughly one reposition per frame
            // is the same change-gating rule that took BeepGridPro from 12 repaints per mouse-move
            // down to 2.
            _coalesce = new Timer { Interval = 16 };
            _coalesce.Tick += (_, _) => { _coalesce.Stop(); Apply(); };

            Subscribe(_anchor);

            for (Control c = _anchor.Parent; c != null; c = c.Parent)
            {
                Subscribe(c);
                if (c is ScrollableControl sc) sc.Scroll += OnScroll;
            }

            if (_anchor.TopLevelControl is Form form)
            {
                form.Move += OnAnchorChanged;
                form.ResizeEnd += OnAnchorChanged;
                form.Deactivate += OnDeactivate;
                _subscribed.Add(form);
            }

            _lastAnchorRect = CurrentAnchorRect();
        }

        private void Subscribe(Control c)
        {
            c.LocationChanged += OnAnchorChanged;
            c.SizeChanged += OnAnchorChanged;
            c.VisibleChanged += OnAnchorChanged;
            c.Disposed += OnAnchorGone;
            _subscribed.Add(c);
        }

        private void OnScroll(object sender, ScrollEventArgs e) => Schedule();
        private void OnAnchorChanged(object sender, EventArgs e) => Schedule();
        private void OnAnchorGone(object sender, EventArgs e) => _hide();
        private void OnDeactivate(object sender, EventArgs e) => _hide();

        private void Schedule()
        {
            if (_disposed) return;
            _coalesce.Stop();
            _coalesce.Start();
        }

        private void Apply()
        {
            if (_disposed) return;

            if (!CanStillShow())
            {
                _hide();
                return;
            }

            var rect = CurrentAnchorRect();
            if (rect == _lastAnchorRect) return;   // nothing actually moved

            _lastAnchorRect = rect;
            _reposition(rect);
        }

        /// <summary>
        /// False once the anchor cannot meaningfully host a tooltip — disposed, hidden, collapsed,
        /// its form minimised, or scrolled fully out of an ancestor's client area. That last case
        /// is Floating UI's <c>hide</c> middleware, and it needs an explicit test because WinForms
        /// still reports a screen rectangle for a control that is scrolled out of view.
        /// </summary>
        private bool CanStillShow()
        {
            if (_anchor.IsDisposed || !_anchor.Visible) return false;
            if (_anchor.Width <= 0 || _anchor.Height <= 0) return false;
            if (_anchor.TopLevelControl is Form f && (f.WindowState == FormWindowState.Minimized || !f.Visible))
                return false;

            var rect = CurrentAnchorRect();
            for (Control c = _anchor.Parent; c != null; c = c.Parent)
            {
                if (c is ScrollableControl || c.Parent == null)
                {
                    var clip = c.RectangleToScreen(c.ClientRectangle);
                    if (!clip.IntersectsWith(rect)) return false;
                }
            }
            return true;
        }

        private Rectangle CurrentAnchorRect()
        {
            try { return _anchor.RectangleToScreen(_anchor.ClientRectangle); }
            catch { return Rectangle.Empty; }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _coalesce.Stop();
            _coalesce.Dispose();

            foreach (var c in _subscribed)
            {
                try
                {
                    c.LocationChanged -= OnAnchorChanged;
                    c.SizeChanged -= OnAnchorChanged;
                    c.VisibleChanged -= OnAnchorChanged;
                    c.Disposed -= OnAnchorGone;
                    if (c is ScrollableControl sc) sc.Scroll -= OnScroll;
                    if (c is Form form)
                    {
                        form.Move -= OnAnchorChanged;
                        form.ResizeEnd -= OnAnchorChanged;
                        form.Deactivate -= OnDeactivate;
                    }
                }
                catch { /* control may already be disposed */ }
            }
            _subscribed.Clear();
        }
    }
}
