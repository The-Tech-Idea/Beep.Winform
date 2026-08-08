using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ToolTips.Helpers
{
    /// <summary>
    /// Dismisses the visible tooltip when Escape is pressed, wherever focus happens to be.
    /// <para>
    /// WCAG 1.4.13 requires hover/focus content to be <em>dismissible without moving the pointer</em>
    /// — i.e. from the keyboard, while focus is still on the trigger. The tooltip window's own
    /// <c>ProcessCmdKey</c> cannot do that: a hover-triggered tooltip never takes focus, so its key
    /// handler is never in the focused control's chain and Escape does nothing.
    /// </para>
    /// <para>
    /// A message filter sees the key regardless of which control has focus, which is the only way
    /// to honour the criterion for a tooltip that is deliberately non-focusable.
    /// </para>
    /// </summary>
    internal sealed class ToolTipEscapeFilter : IMessageFilter, IDisposable
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_ESCAPE = 0x1B;

        private readonly Action _onEscape;
        private bool _installed;

        private ToolTipEscapeFilter(Action onEscape) => _onEscape = onEscape;

        /// <summary>Installs a filter that calls <paramref name="onEscape"/> once, on Escape.</summary>
        public static ToolTipEscapeFilter Install(Action onEscape)
        {
            if (onEscape == null) throw new ArgumentNullException(nameof(onEscape));
            var filter = new ToolTipEscapeFilter(onEscape);
            Application.AddMessageFilter(filter);
            filter._installed = true;
            return filter;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg != WM_KEYDOWN || (int)m.WParam != VK_ESCAPE) return false;

            try { _onEscape(); }
            catch (Exception ex)
            {
                BeepLog.FailureOnce("ToolTip.escapeFilter", this, "handle Escape in message filter", ex);
            }

            // Not consumed: Escape may also mean something to the focused control (closing a
            // dialog, cancelling an edit). Dismissing a tooltip should not swallow it.
            return false;
        }

        public void Dispose()
        {
            if (!_installed) return;
            Application.RemoveMessageFilter(this);
            _installed = false;
        }
    }
}
