using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips
{
    /// <summary>
    /// C5: Global message filter that watches for left-mouse-button-down
    /// anywhere outside a target control's screen bounds and invokes a
    /// callback (typically dismiss).
    ///
    /// This replaces the old <c>OnDeactivate</c>-based dismissal in
    /// <see cref="BeepPopover"/>, which used to fire when the user clicked
    /// the popover's own action buttons (the click moved focus and the
    /// deactivate message arrived before the button's <c>Click</c> handler).
    /// The new filter ignores clicks that fall inside the popover's bounds,
    /// so button clicks fire their action first; only clicks outside the
    /// popover trigger dismissal.
    /// </summary>
    internal sealed class OutsideClickMessageFilter : IMessageFilter
    {
        private const int WM_LBUTTONDOWN = 0x0201;

        private readonly Control _target;
        private readonly Action _onOutsideClick;

        public OutsideClickMessageFilter(Control target, Action onOutsideClick)
        {
            _target         = target ?? throw new ArgumentNullException(nameof(target));
            _onOutsideClick = onOutsideClick ?? throw new ArgumentNullException(nameof(onOutsideClick));
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_LBUTTONDOWN)
                return false;

            // Self-remove once the target is gone. The original comment claimed this happened
            // ("unregister and stop filtering") but the code only returned false, so a popover
            // disposed without closing left this filter installed in Application's global filter
            // list for the lifetime of the process, holding a reference to a dead Control.
            if (_target.IsDisposed || !_target.IsHandleCreated)
            {
                Application.RemoveMessageFilter(this);
                return false;
            }

            // WM_LBUTTONDOWN carries CLIENT coordinates of the window that received it — not
            // screen coordinates. The previous code unpacked LParam and compared it directly
            // against the popover's screen rectangle, so the "did the click land inside?" test was
            // comparing two different coordinate spaces and only worked when the clicked window
            // happened to sit near the origin. Control.MousePosition is already in screen space.
            var screenPoint = Control.MousePosition;

            var popoverScreen = _target.RectangleToScreen(_target.ClientRectangle);
            if (popoverScreen.Contains(screenPoint))
                return false;

            // Click landed outside the popover — dismiss.
            // Marshal back to the UI thread to keep Control.* state safe.
            try
            {
                _target.BeginInvoke(_onOutsideClick);
            }
            catch (ObjectDisposedException)
            {
                Application.RemoveMessageFilter(this);
            }
            return false;
        }
    }
}
