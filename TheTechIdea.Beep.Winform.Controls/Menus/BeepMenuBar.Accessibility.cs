// BeepMenuBar.Accessibility.cs
// Phase 07 — Accessibility, Keyboard, Mnemonics.
//
// Exposes the menubar to UIA / MSAA via a custom AccessibleObject tree:
//
//   BeepMenuBar          → MenuBarAccessibleObject (Role = MenuBar)
//     └─ each top-level  → MenuItemAccessibleObject (Role = MenuItem,
//                          State = Focused|Selected when matched,
//                          State = HasPopup when item has children)
//
// Bounds resolve through the existing Layout partial's
// CalculateMenuItemRects() helper (visibility relaxed to internal in this
// phase) and PointToScreen, so screen readers see the same geometry the
// user clicks. Activation / Popup partials raise UIA events
// (AccessibleEvents.Focus / MenuPopupStart / MenuPopupEnd) via the
// internal RaiseItemAccessibleEvent helper.
//
// See .plans/Menus-Phase-07-AccessibilityKeyboard.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMenuBar
    {
        // ─────────────────────────────────────────────────────────────────
        // AccessibleObject factory — WinForms calls this once per control
        // and caches the result.
        // ─────────────────────────────────────────────────────────────────

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new MenuBarAccessibleObject(this);
        }

        // ─────────────────────────────────────────────────────────────────
        // UIA event helpers — exposed to the Activation + Popup partials.
        // AccessibilityNotifyClients is protected on Control, so this
        // shim makes it reachable from same-class partials without
        // surfacing a public-or-internal protected wrapper.
        // ─────────────────────────────────────────────────────────────────

        internal void RaiseItemAccessibleEvent(AccessibleEvents accEvent, int itemIndex)
        {
            if (itemIndex < 0) return;
            try { AccessibilityNotifyClients(accEvent, itemIndex); }
            catch { /* never let an AT subscriber error break the menubar */ }
        }

        // ─────────────────────────────────────────────────────────────────
        // MenuBarAccessibleObject — the root AO. WinForms walks here from
        // the parent form's AO and asks for our role, name, and children.
        // ─────────────────────────────────────────────────────────────────

        internal sealed class MenuBarAccessibleObject : ControlAccessibleObject
        {
            private readonly BeepMenuBar _owner;

            public MenuBarAccessibleObject(BeepMenuBar owner) : base(owner)
            {
                _owner = owner;
            }

            public override AccessibleRole Role => AccessibleRole.MenuBar;

            public override string Name
                => string.IsNullOrEmpty(_owner.AccessibleName) ? "Menu bar" : _owner.AccessibleName;

            public override int GetChildCount() => _owner.items?.Count ?? 0;

            public override AccessibleObject GetChild(int index)
            {
                if (_owner.items == null) return null;
                if (index < 0 || index >= _owner.items.Count) return null;
                return new MenuItemAccessibleObject(_owner, index);
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                if (_owner.items == null || _owner.items.Count == 0) return this;

                var clientPoint = _owner.PointToClient(new Point(x, y));
                var rects = _owner.CalculateMenuItemRects();
                for (int i = 0; i < rects.Count && i < _owner.items.Count; i++)
                {
                    if (rects[i].Contains(clientPoint))
                    {
                        return new MenuItemAccessibleObject(_owner, i);
                    }
                }
                return this;
            }

            public override AccessibleObject GetFocused()
            {
                int idx = _owner._selectedIndex;
                if (idx < 0 || idx >= (_owner.items?.Count ?? 0)) return null;
                if (_owner.Activation == MenubarActivation.Inactive) return null;
                return new MenuItemAccessibleObject(_owner, idx);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // MenuItemAccessibleObject — represents one top-level menubar
        // entry. Lightweight: holds only the owner + index so a fresh
        // instance can be returned on every GetChild call without keeping
        // a sticky reference.
        // ─────────────────────────────────────────────────────────────────

        internal sealed class MenuItemAccessibleObject : AccessibleObject
        {
            private readonly BeepMenuBar _owner;
            private readonly int _index;

            public MenuItemAccessibleObject(BeepMenuBar owner, int index)
            {
                _owner = owner;
                _index = index;
            }

            private Models.SimpleItem Item
                => (_owner.items != null && _index >= 0 && _index < _owner.items.Count)
                    ? _owner.items[_index]
                    : null;

            public override AccessibleRole Role => AccessibleRole.MenuItem;

            public override AccessibleObject Parent => _owner.AccessibilityObject;

            public override string Name
            {
                get
                {
                    var item = Item;
                    if (item == null) return string.Empty;
                    // Strip the '&' mnemonic prefix so screen readers don't
                    // pronounce "ampersand F file" — they say "File".
                    return StripMnemonic(item.Text);
                }
            }

            public override string Description
            {
                get
                {
                    var item = Item;
                    if (item == null) return string.Empty;
                    return item.Description ?? string.Empty;
                }
            }

            public override string KeyboardShortcut
            {
                get
                {
                    var item = Item;
                    if (item == null) return null;
                    // Surface the Alt+letter mnemonic if one is authored.
                    char m = GetMnemonicChar(item.Text);
                    return m == '\0' ? null : "Alt+" + char.ToUpperInvariant(m);
                }
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (_owner.items == null) return Rectangle.Empty;
                    var rects = _owner.CalculateMenuItemRects();
                    if (_index < 0 || _index >= rects.Count) return Rectangle.Empty;
                    var client = rects[_index];
                    return _owner.RectangleToScreen(client);
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    var s = AccessibleStates.Focusable | AccessibleStates.Selectable;
                    var item = Item;
                    if (item != null && item.Children != null && item.Children.Count > 0)
                    {
                        s |= AccessibleStates.HasPopup;
                    }
                    if (_owner._selectedIndex == _index)
                    {
                        s |= AccessibleStates.Selected;
                        if (_owner.Activation != MenubarActivation.Inactive)
                        {
                            s |= AccessibleStates.Focused;
                        }
                    }
                    if (_owner.Activation == MenubarActivation.ActiveWithPopup
                        && _owner.OpenTopLevelIndex == _index)
                    {
                        s |= AccessibleStates.Expanded;
                    }
                    return s;
                }
            }

            public override string DefaultAction
            {
                get
                {
                    var item = Item;
                    if (item == null) return null;
                    return (item.Children != null && item.Children.Count > 0) ? "Open" : "Press";
                }
            }

            public override void DoDefaultAction()
            {
                var item = Item;
                if (item == null) return;
                try { _owner.HandleMenuItemClick(item, _index); } catch { }
            }

            public override void Select(AccessibleSelection flags)
            {
                if ((flags & AccessibleSelection.TakeFocus) != 0 ||
                    (flags & AccessibleSelection.TakeSelection) != 0)
                {
                    _owner._selectedIndex = _index;
                    if (_owner.Activation == MenubarActivation.Inactive)
                    {
                        _owner.SetActivation(MenubarActivation.ActiveNoPopup);
                    }
                    _owner.Invalidate();
                    _owner.RaiseItemAccessibleEvent(AccessibleEvents.Focus, _index);
                }
            }

            // ── helpers ──────────────────────────────────────────────

            private static string StripMnemonic(string text)
            {
                if (string.IsNullOrEmpty(text)) return string.Empty;
                if (text.IndexOf('&') < 0) return text;

                var sb = new System.Text.StringBuilder(text.Length);
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == '&' && i + 1 < text.Length)
                    {
                        if (text[i + 1] == '&') { sb.Append('&'); i++; continue; }
                        continue;
                    }
                    sb.Append(c);
                }
                return sb.ToString();
            }

            private static char GetMnemonicChar(string text)
            {
                if (string.IsNullOrEmpty(text)) return '\0';
                for (int i = 0; i < text.Length - 1; i++)
                {
                    if (text[i] != '&') continue;
                    if (text[i + 1] == '&') { i++; continue; }
                    return text[i + 1];
                }
                return '\0';
            }
        }
    }
}
