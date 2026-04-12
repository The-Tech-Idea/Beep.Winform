// BeepDocumentTabStrip.Accessibility.cs
// WCAG 2.1 AA accessibility for BeepDocumentTabStrip.
// • Screen-reader announcements via AccessibilityObject.NotifyClients
// • Keyboard arrow navigation (Left/Right/Home/End, Tab/Shift-Tab)
// • Focus ring drawn via DrawFocusRectangle on the active tab rect
// • Every tab exposes role, state, bounds, help-text, and name
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ── Factory ──────────────────────────────────────────────────────────

        /// <summary>
        /// Returns a custom <see cref="AccessibleObject"/> that describes the tab
        /// strip to accessibility tools.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new BeepTabStripAccessible(this);

        // ── Screen-reader notification ────────────────────────────────────────

        /// <summary>
        /// Called after the active tab index changes.  Notifies the accessibility
        /// tree so that screen readers announce the newly selected tab.
        /// </summary>
        internal void NotifyActiveTabChanged()
        {
            int idx = _activeTabIndex;
            if (idx < 0 || idx >= _tabs.Count) return;

            var obj = new BeepTabAccessible(this, idx);
            if (AccessibilityObject is Control.ControlAccessibleObject coa)
            {
                coa.NotifyClients(AccessibleEvents.Selection, idx);
                coa.NotifyClients(AccessibleEvents.Focus, idx);
            }
        }

        // ── Focus ring ────────────────────────────────────────────────────────

        /// <summary>
        /// Draws a keyboard focus rectangle around the active tab when the control
        /// has focus, satisfying WCAG 2.1 SC 2.4.7 (Focus Visible).
        /// Called at the end of <c>DrawContent</c> in the painting partial.
        /// </summary>
        internal void DrawAccessibilityFocusRing(System.Drawing.Graphics g)
        {
            if (!Focused) return;
            int idx = _activeTabIndex;
            if (idx < 0 || idx >= _tabs.Count) return;
            var rect = _tabs[idx].TabRect;
            if (rect.IsEmpty) return;
            rect.Inflate(-1, -1);
            System.Windows.Forms.ControlPaint.DrawFocusRectangle(g, rect);
        }

        // ── Keyboard navigation ───────────────────────────────────────────────

        protected override bool IsInputKey(Keys keyData)
        {
            if (!Focused) return base.IsInputKey(keyData);
            return keyData switch
            {
                Keys.Left or Keys.Right or Keys.Home or Keys.End => true,
                _ => base.IsInputKey(keyData)
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_tabs.Count == 0) return;

            int next = _activeTabIndex;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    next = next > 0 ? next - 1 : _tabs.Count - 1;
                    break;
                case Keys.Right:
                    next = next < _tabs.Count - 1 ? next + 1 : 0;
                    break;
                case Keys.Home:
                    next = 0;
                    break;
                case Keys.End:
                    next = _tabs.Count - 1;
                    break;
                default:
                    return;
            }

            if (next != _activeTabIndex)
            {
                ActiveTabIndex = next;
                TabSelected?.Invoke(this, new TabEventArgs(next, _tabs[next]));
                NotifyActiveTabChanged();
                e.Handled = true;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // BeepTabStripAccessible — root accessible object for the strip
        // ════════════════════════════════════════════════════════════════════

        private sealed class BeepTabStripAccessible : Control.ControlAccessibleObject
        {
            private readonly BeepDocumentTabStrip _strip;

            internal BeepTabStripAccessible(BeepDocumentTabStrip strip) : base(strip)
                => _strip = strip;

            // ── Identification ───────────────────────────────────────────────

            public override AccessibleRole Role        => AccessibleRole.PageTabList;
            public override string?        Name        => _strip.Name ?? "Document Tabs";
            public override string?        Description => "Tab strip for document windows";
            public override string?        Help        =>
                "Use Left/Right arrows to navigate tabs. Home/End for first/last. Enter to activate.";

            // ── State ────────────────────────────────────────────────────────

            public override AccessibleStates State
                => _strip.Focused
                   ? AccessibleStates.Focused | AccessibleStates.Focusable
                   : AccessibleStates.Focusable;

            // ── Children ─────────────────────────────────────────────────────

            public override int GetChildCount() => _strip._tabs.Count;

            public override AccessibleObject? GetChild(int index)
            {
                if (index < 0 || index >= _strip._tabs.Count) return null;
                return new BeepTabAccessible(_strip, index);
            }

            // ── Focused child ────────────────────────────────────────────────

            public override AccessibleObject? GetFocused()
            {
                int ai = _strip._activeTabIndex;
                if (ai >= 0 && ai < _strip._tabs.Count && _strip.Focused)
                    return new BeepTabAccessible(_strip, ai);
                return null;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        // BeepTabAccessible — accessible object for a single tab
        // ════════════════════════════════════════════════════════════════════

        private sealed class BeepTabAccessible : AccessibleObject
        {
            private readonly BeepDocumentTabStrip _strip;
            private readonly int _index;

            internal BeepTabAccessible(BeepDocumentTabStrip strip, int index)
            {
                _strip = strip;
                _index = index;
            }

            private BeepDocumentTab? Tab
                => (_index >= 0 && _index < _strip._tabs.Count)
                   ? _strip._tabs[_index] : null;

            // ── Identification ───────────────────────────────────────────────

            public override AccessibleRole Role        => AccessibleRole.PageTab;

            /// <summary>
            /// Returns a screen-reader-friendly name in the format
            /// "Tab N of M: Title [Modified] [Pinned]".
            /// </summary>
            public override string? Name
            {
                get
                {
                    var tab = Tab;
                    if (tab == null) return null;
                    int total = _strip._tabs.Count;
                    var sb = new System.Text.StringBuilder();
                    sb.Append("Tab ").Append(_index + 1).Append(" of ").Append(total)
                      .Append(": ").Append(tab.Title ?? string.Empty);
                    if (tab.IsModified) sb.Append(" [Modified]");
                    if (tab.IsPinned)   sb.Append(" [Pinned]");
                    return sb.ToString();
                }
            }

            public override string?        Description => Tab?.TooltipText ?? Tab?.Title;
            public override string?        Value       => Tab?.IsModified == true ? "Modified" : null;
            public override string?        Help        => "Press Enter or Space to activate this tab. Right-click for more options.";
            public override string?        DefaultAction => "Activate";

            // ── Bounds ───────────────────────────────────────────────────────

            public override System.Drawing.Rectangle Bounds
                => Tab is { } t && !t.TabRect.IsEmpty
                   ? _strip.RectangleToScreen(t.TabRect)
                   : System.Drawing.Rectangle.Empty;

            // ── State ────────────────────────────────────────────────────────

            public override AccessibleStates State
            {
                get
                {
                    var s = AccessibleStates.Selectable | AccessibleStates.Focusable;
                    if (Tab?.IsActive  == true) s |= AccessibleStates.Selected | AccessibleStates.Focused;
                    if (Tab?.IsPinned  == true) s |= AccessibleStates.ReadOnly;
                    if (Tab?.IsModified== true) s |= AccessibleStates.Mixed;
                    return s;
                }
            }

            // ── Navigation ───────────────────────────────────────────────────

            public override AccessibleObject? Navigate(AccessibleNavigation navdir)
            {
                int count = _strip._tabs.Count;
                return navdir switch
                {
                    AccessibleNavigation.Next        => _index + 1 < count
                                                        ? new BeepTabAccessible(_strip, _index + 1) : null,
                    AccessibleNavigation.Previous    => _index - 1 >= 0
                                                        ? new BeepTabAccessible(_strip, _index - 1) : null,
                    AccessibleNavigation.FirstChild  => null,
                    AccessibleNavigation.LastChild   => null,
                    AccessibleNavigation.Up          => _strip.AccessibilityObject,
                    _                                => null
                };
            }

            // ── Select ───────────────────────────────────────────────────────

            public override void Select(AccessibleSelection flags)
            {
                if (flags.HasFlag(AccessibleSelection.TakeSelection) ||
                    flags.HasFlag(AccessibleSelection.AddSelection))
                {
                    _strip.ActiveTabIndex = _index;
                    _strip.TabSelected?.Invoke(_strip,
                        new TabEventArgs(_index, _strip._tabs[_index]));
                    _strip.Focus();
                    _strip.NotifyActiveTabChanged();
                }
            }

            public override void DoDefaultAction() => Select(AccessibleSelection.TakeSelection);
        }
    }
}
