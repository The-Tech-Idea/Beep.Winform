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

            // Convert the LParam into screen coordinates and compare to the
            // popover's screen rectangle. Clicks on the popover itself fall
            // inside the rectangle and are NOT treated as outside clicks.
            int x = unchecked((short)(long)m.LParam);
            int y = unchecked((short)((long)m.LParam >> 16));
            var screenPoint = new Point(x, y);

            // If the target has been disposed (popover closed via Cancel/Esc),
            // unregister and stop filtering.
            if (_target.IsDisposed || !_target.IsHandleCreated)
                return false;

            var popoverScreen = _target.RectangleToScreen(_target.ClientRectangle);
            if (popoverScreen.Contains(screenPoint))
                return false;

            // Click landed outside the popover — dismiss.
            // Marshal back to the UI thread to keep Control.* state safe.
            if (_target.IsHandleCreated)
            {
                _target.BeginInvoke(_onOutsideClick);
            }
            return false;
        }
    }
}
