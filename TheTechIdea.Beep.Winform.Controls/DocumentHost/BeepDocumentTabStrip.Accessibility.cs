// BeepDocumentTabStrip.Accessibility.cs
// Accessibility (UIA / MSAA) support for BeepDocumentTabStrip.
// Exposes the strip as a TabControl with individual PageTab children so that
// screen readers (Narrator, NVDA, JAWS) can announce the active tab and allow
// the user to navigate between tabs with the virtual cursor.
// ─────────────────────────────────────────────────────────────────────────────────────────
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
                }
            }
        }
    }
}
