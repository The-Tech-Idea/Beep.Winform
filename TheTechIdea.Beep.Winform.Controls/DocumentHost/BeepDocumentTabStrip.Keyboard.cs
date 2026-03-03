// BeepDocumentTabStrip.Keyboard.cs
// Keyboard shortcut handling for BeepDocumentTabStrip.
// Active when the strip (or a child it focuses) receives keyboard input.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Windows.Forms;
namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // Focus change — repaint so the focus rectangle appears / disappears
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnGotFocus(EventArgs e)  { base.OnGotFocus(e);  Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

        // ─────────────────────────────────────────────────────────────────────
        // ProcessCmdKey override (fires when strip is focused)
        // ─────────────────────────────────────────────────────────────────────

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            => HandleShortcut(keyData) || base.ProcessCmdKey(ref msg, keyData);

        // ─────────────────────────────────────────────────────────────────────
        // Internal forwarder used by BeepDocumentHost.ProcessCmdKey
        // (ProcessCmdKey is protected, so the host cannot call it directly)
        // ─────────────────────────────────────────────────────────────────────

        internal bool HandleShortcut(Keys keyData)
        {
            if (!_keyboardShortcutsEnabled) return false;

            switch (keyData)
            {
                case Keys.Control | Keys.Tab:              CycleTab(+1);            return true;
                case Keys.Control | Keys.Shift | Keys.Tab: CycleTab(-1);            return true;
                case Keys.Control | Keys.W:
                case Keys.Control | Keys.F4:               CloseActiveByKeyboard(); return true;

                // Arrow navigation — cycle through tabs without Ctrl modifier
                case Keys.Right:
                case Keys.Down:  CycleTab(+1); return true;
                case Keys.Left:
                case Keys.Up:    CycleTab(-1); return true;
                case Keys.Home:  JumpToTab(0);                   return true;
                case Keys.End:   JumpToTab(_tabs.Count - 1);     return true;
            }

            // Ctrl+1 through Ctrl+9
            for (int n = 1; n <= 9; n++)
            {
                if (keyData == (Keys.Control | (Keys.D0 + n)))
                { JumpToTab(n - 1); return true; }
            }

            return false;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Navigation helpers
        // ─────────────────────────────────────────────────────────────────────

        private void CycleTab(int delta)
        {
            if (_tabs.Count == 0) return;
            int next = (_activeTabIndex + delta + _tabs.Count) % _tabs.Count;
            ActiveTabIndex = next;
            TabSelected?.Invoke(this, new TabEventArgs(next, _tabs[next]));
        }

        private void JumpToTab(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            ActiveTabIndex = index;
            TabSelected?.Invoke(this, new TabEventArgs(index, _tabs[index]));
        }

        private void CloseActiveByKeyboard()
        {
            if (_activeTabIndex < 0 || _activeTabIndex >= _tabs.Count) return;
            var tab = _tabs[_activeTabIndex];
            if (tab.CanClose && !tab.IsPinned)
                RequestClose(_activeTabIndex, tab);
        }
    }
}
