// BeepMenuBar.Keyboard.cs
// Phase 04A — Commercial-Grade Menubar UX.
//
// Owns keyboard activation: Alt → enter mnemonic mode, Alt+letter →
// open by mnemonic, Esc → leave activation, Left/Right → traverse top
// level, Down/Enter → open popup. Hooks go through ProcessCmdKey (for
// system keys like Alt/Esc/arrows that bypass focus) and
// ProcessMnemonic (the standard WinForms entry-point that WPF/Win32
// also use for Alt+letter handling).
//
// The state machine lives in BeepMenuBar.Activation.cs; this file just
// drives transitions. Mnemonic *rendering* is owned by Drawing.cs.
//
// See .plans/Menus-Phase-04-CommercialMenuBarUX.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // ProcessCmdKey — system-key entry point.
        //
        // Fires BEFORE focus dispatch and BEFORE OnKeyDown. The correct
        // hook for Alt / Esc / arrow keys when the control itself can't
        // take focus (CanBeFocused = false in the constructor).
        //
        // Returns true to indicate "we handled the key, don't propagate".
        // ─────────────────────────────────────────────────────────────────
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (DesignMode) return base.ProcessCmdKey(ref msg, keyData);

            // Strip modifier flags except Alt for the Alt-toggle test.
            var key       = keyData & Keys.KeyCode;
            var modifiers = keyData & Keys.Modifiers;

            // First Alt press → activate; second Alt press → deactivate.
            // We listen for the KeyUp-equivalent SYSKEY by checking the
            // bare Alt key (Keys.Menu) — pressing Alt alone arrives here
            // with keyData == Keys.Menu (no other modifiers set).
            if (keyData == Keys.Menu)
            {
                ToggleAltActivation();
                return true;
            }

            // Esc — any state except Inactive: deactivate.
            if (keyData == Keys.Escape && Activation != MenubarActivation.Inactive)
            {
                Debug.WriteLine("BeepMenuBar.Keyboard: Escape -> Inactive");
                // Close any open popup first so the user isn't left
                // staring at an orphan after pressing Esc.
                try { ContextMenus.ContextMenuManager.CloseAllMenus(); }
                catch { /* non-fatal */ }
                _openTopLevelIndex = -1;
                DeactivateKeyboard();
                return true;
            }

            // Arrow keys + Enter only apply while keyboard is active.
            if (Activation == MenubarActivation.Inactive)
                return base.ProcessCmdKey(ref msg, keyData);

            // Only handle bare arrow / Enter (no Ctrl/Shift modifiers).
            if (modifiers != Keys.None)
                return base.ProcessCmdKey(ref msg, keyData);

            switch (key)
            {
                case Keys.Left:
                    MoveHighlight(delta: -1);
                    return true;

                case Keys.Right:
                    MoveHighlight(delta: +1);
                    return true;

                case Keys.Down:
                case Keys.Enter:
                    // Open popup at the currently highlighted top-level
                    // item — but only if it actually has children;
                    // otherwise treat as a command activation.
                    OpenHighlightedTopLevel();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ─────────────────────────────────────────────────────────────────
        // ProcessMnemonic — Alt+letter entry point.
        //
        // Called automatically by WinForms when an `&`-prefixed character
        // is matched. We resolve against SimpleItem.Text and open the
        // popup (or invoke the item's bound method when no children).
        // ─────────────────────────────────────────────────────────────────
        protected override bool ProcessMnemonic(char charCode)
        {
            if (DesignMode) return false;
            if (items == null || items.Count == 0) return false;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == null) continue;

                char mnemonic = GetMnemonicChar(item.Text);
                if (mnemonic == '\0') continue;

                if (char.ToUpperInvariant(mnemonic) == char.ToUpperInvariant(charCode))
                {
                    Debug.WriteLine($"BeepMenuBar.Keyboard: mnemonic '{charCode}' -> index {i} ({item.Text})");

                    // Ensure activation flips on so the next keystroke
                    // already sees the menubar in arrow-traversal mode.
                    if (Activation == MenubarActivation.Inactive)
                    {
                        SetActivation(MenubarActivation.ActiveNoPopup);
                    }

                    _selectedIndex = i;
                    Invalidate();
                    HandleMenuItemClick(item, i);
                    return true;
                }
            }
            return false;
        }

        // ─────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────

        private void ToggleAltActivation()
        {
            if (Activation == MenubarActivation.Inactive)
            {
                // First Alt press: enter keyboard mode, highlight first item.
                if (items != null && items.Count > 0)
                {
                    _selectedIndex = 0;
                }
                SetActivation(MenubarActivation.ActiveNoPopup);
                Debug.WriteLine("BeepMenuBar.Keyboard: Alt -> ActiveNoPopup");
            }
            else
            {
                // Second Alt or Alt-while-popup-open: leave keyboard mode.
                try { ContextMenus.ContextMenuManager.CloseAllMenus(); }
                catch { /* non-fatal */ }
                _openTopLevelIndex = -1;
                _selectedIndex     = -1;
                DeactivateKeyboard();
                Debug.WriteLine("BeepMenuBar.Keyboard: Alt -> Inactive");
            }
        }

        private void MoveHighlight(int delta)
        {
            if (items == null || items.Count == 0) return;

            int count = items.Count;
            int index = _selectedIndex;
            if (index < 0) index = 0;

            // Skip null/disabled items so wrap-around still lands on a
            // valid target. Bound the loop by `count` to prevent
            // infinite spin when every item is null.
            int next = index;
            for (int step = 0; step < count; step++)
            {
                next = ((next + delta) % count + count) % count;
                if (items[next] != null) break;
            }

            if (next == _selectedIndex) return;
            _selectedIndex = next;
            Invalidate();

            // Phase 07-E — keep screen readers tracking arrow navigation.
            RaiseItemAccessibleEvent(AccessibleEvents.Focus, _selectedIndex);

            // While a popup is up, arrow keys should *swap* popups —
            // depends on Phase 04B's non-blocking infrastructure, so
            // for 04A we only re-issue the popup if Activation is
            // already ActiveWithPopup (compatible with current
            // blocking Show path).
            if (Activation == MenubarActivation.ActiveWithPopup)
            {
                OpenHighlightedTopLevel();
            }
        }

        private void OpenHighlightedTopLevel()
        {
            if (items == null) return;
            if (_selectedIndex < 0 || _selectedIndex >= items.Count) return;

            var item = items[_selectedIndex];
            if (item == null) return;

            HandleMenuItemClick(item, _selectedIndex);
        }

        /// <summary>
        /// Returns the mnemonic character for <paramref name="text"/>.
        /// Honours the standard `&letter` convention (with `&&` as an
        /// escaped ampersand). Falls back to the first letter when no
        /// `&` is present, matching Win32 menu auto-mnemonics.
        /// </summary>
        private static char GetMnemonicChar(string text)
        {
            if (string.IsNullOrEmpty(text)) return '\0';

            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] != '&') continue;
                // "&&" is an escaped literal ampersand — skip both.
                if (text[i + 1] == '&') { i++; continue; }
                return text[i + 1];
            }

            // No explicit mnemonic: fall back to the first letter so
            // Alt+F still finds "File" even when authored as "File".
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c)) return c;
            }
            return '\0';
        }
    }
}
