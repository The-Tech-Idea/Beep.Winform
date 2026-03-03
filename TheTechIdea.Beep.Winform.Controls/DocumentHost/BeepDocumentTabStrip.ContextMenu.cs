// BeepDocumentTabStrip.ContextMenu.cs
// Right-click context menu, pin/unpin logic, and themed ContextMenuStrip helpers
// for BeepDocumentTabStrip.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // Cache the index of the last right-clicked tab so closure lambdas stay correct
        // even if the Tabs list changes before the user makes a selection.
        private int _contextTabIndex = -1;

        // ─────────────────────────────────────────────────────────────────────
        // Public entry point — called from Mouse.cs OnMouseDown
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Builds and displays the context menu for the tab at <paramref name="tabIndex"/> (-1 = empty area).</summary>
        internal void ShowTabContextMenu(int tabIndex, Point screenPoint)
        {
            _contextTabIndex = tabIndex;
            bool hasTab = tabIndex >= 0 && tabIndex < _tabs.Count;
            var  tab    = hasTab ? _tabs[tabIndex] : null;

            var menu = new ContextMenuStrip();
            ApplyThemeToMenu(menu);

            // ── Close ─────────────────────────────────────────────────────
            var closeItem = new ToolStripMenuItem("Close")
                { Enabled = hasTab && (tab?.CanClose ?? false) };
            closeItem.Click += (s, e) => FireContextClose(_contextTabIndex);
            menu.Items.Add(closeItem);

            // ── Close All But This ────────────────────────────────────────
            var closeOthers = new ToolStripMenuItem("Close All But This")
                { Enabled = hasTab && _tabs.Count(t => t.CanClose) > 1 };
            closeOthers.Click += (s, e) =>
            {
                int snapshot = _contextTabIndex;
                var toClose  = _tabs.Where((t, i) => i != snapshot && t.CanClose).ToList();
                foreach (var t in toClose)
                {
                    int idx = _tabs.IndexOf(t);
                    if (idx >= 0) RequestClose(idx, t);
                }
            };
            menu.Items.Add(closeOthers);

            // ── Close All to the Right ────────────────────────────────────
            var closeRight = new ToolStripMenuItem("Close All to the Right")
                { Enabled = hasTab && tabIndex < _tabs.Count - 1 };
            closeRight.Click += (s, e) =>
            {
                var toClose = _tabs.Skip(_contextTabIndex + 1).Where(t => t.CanClose).ToList();
                foreach (var t in toClose)
                {
                    int idx = _tabs.IndexOf(t);
                    if (idx >= 0) RequestClose(idx, t);
                }
            };
            menu.Items.Add(closeRight);

            // ── Close All ─────────────────────────────────────────────────
            var closeAll = new ToolStripMenuItem("Close All")
                { Enabled = _tabs.Any(t => t.CanClose) };
            closeAll.Click += (s, e) =>
            {
                var toClose = _tabs.Where(t => t.CanClose).ToList();
                foreach (var t in toClose)
                {
                    int idx = _tabs.IndexOf(t);
                    if (idx >= 0) RequestClose(idx, t);
                }
            };
            menu.Items.Add(closeAll);

            menu.Items.Add(new ToolStripSeparator());

            // ── Pin / Unpin ───────────────────────────────────────────────
            if (hasTab)
            {
                string pinLabel = (tab?.IsPinned ?? false) ? "Unpin Tab" : "Pin Tab";
                var pinItem = new ToolStripMenuItem(pinLabel);
                pinItem.Click += (s, e) =>
                {
                    if (_contextTabIndex >= 0 && _contextTabIndex < _tabs.Count)
                        TogglePin(_contextTabIndex);
                };
                menu.Items.Add(pinItem);
            }

            // ── Float ─────────────────────────────────────────────────────
            if (hasTab)
            {
                var floatItem = new ToolStripMenuItem("Float")
                    { Enabled = !(tab?.IsPinned ?? true) };
                floatItem.Click += (s, e) =>
                {
                    if (_contextTabIndex >= 0 && _contextTabIndex < _tabs.Count)
                        TabFloatRequested?.Invoke(this,
                            new TabEventArgs(_contextTabIndex, _tabs[_contextTabIndex]));
                };
                menu.Items.Add(floatItem);
            }

            // ── Consumer extension point ───────────────────────────────────
            var args = new TabContextMenuEventArgs(_contextTabIndex, tab, menu);
            TabContextMenuOpening?.Invoke(this, args);
            if (args.Cancel) { menu.Dispose(); return; }

            menu.Show(this, PointToClient(screenPoint));
            menu.Closed += (s, e) => menu.Dispose();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private void FireContextClose(int index)
        {
            if (index >= 0 && index < _tabs.Count)
                RequestClose(index, _tabs[index]);
        }

        private void ApplyThemeToMenu(ContextMenuStrip menu)
        {
            if (_theme == null) return;
            menu.BackColor = _theme.PanelBackColor;
            menu.ForeColor = _theme.ForeColor;
            menu.Renderer  = new BeepTabMenuRenderer(_theme);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Pin / Unpin
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Toggles the pinned state of the tab at <paramref name="index"/>.</summary>
        internal void TogglePin(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;

            var  tab      = _tabs[index];
            bool wasPinned = tab.IsPinned;
            tab.IsPinned  = !wasPinned;

            // Maintain invariant: pinned tabs always occupy the left-most positions
            _tabs.RemoveAt(index);
            if (!wasPinned)
            {
                // Newly pinned → insert at end of existing pinned group
                int insertAt = _tabs.TakeWhile(t => t.IsPinned).Count();
                _tabs.Insert(insertAt, tab);
                if (_activeTabIndex == index) _activeTabIndex = insertAt;
                else if (_activeTabIndex > index && _activeTabIndex <= insertAt) _activeTabIndex--;
                else if (_activeTabIndex < index && _activeTabIndex >= insertAt) _activeTabIndex++;
            }
            else
            {
                // Newly unpinned → insert just after the remaining pinned group
                int insertAt = _tabs.TakeWhile(t => t.IsPinned).Count();
                _tabs.Insert(insertAt, tab);
                if (_activeTabIndex == index) _activeTabIndex = insertAt;
                else if (_activeTabIndex > index && _activeTabIndex <= insertAt) _activeTabIndex--;
                else if (_activeTabIndex < index && _activeTabIndex >= insertAt) _activeTabIndex++;
            }

            CalculateTabLayout();
            Invalidate();

            int newIndex = _tabs.IndexOf(tab);
            TabPinToggled?.Invoke(this, new TabEventArgs(newIndex, tab));
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Themed ToolStrip renderer
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class BeepTabMenuRenderer : ToolStripProfessionalRenderer
    {
        public BeepTabMenuRenderer(IBeepTheme theme)
            : base(new BeepTabColorTable(theme)) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled
                ? e.Item.ForeColor
                : Color.FromArgb(120, e.Item.ForeColor);
            base.OnRenderItemText(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Enabled) return;
            if (e.Item.Selected)
            {
                var ct = (BeepTabColorTable)ColorTable;
                using var br = new SolidBrush(ct.HighlightColor);
                e.Graphics.FillRectangle(br, e.Item.ContentRectangle);
            }
            // else transparent — parent ToolStrip background shows through
        }
    }

    internal sealed class BeepTabColorTable : ProfessionalColorTable
    {
        private readonly IBeepTheme _t;
        public Color HighlightColor { get; }

        public BeepTabColorTable(IBeepTheme t)
        {
            _t             = t;
            HighlightColor = Color.FromArgb(50, t.PrimaryColor);
        }

        public override Color MenuBorder                 => _t.BorderColor;
        public override Color MenuItemBorder             => Color.Transparent;
        public override Color MenuItemSelected           => HighlightColor;
        public override Color MenuItemSelectedGradientBegin => HighlightColor;
        public override Color MenuItemSelectedGradientEnd   => HighlightColor;
        public override Color ToolStripDropDownBackground    => _t.PanelBackColor;
        public override Color MenuStripGradientBegin         => _t.PanelBackColor;
        public override Color MenuStripGradientEnd           => _t.PanelBackColor;
        public override Color SeparatorDark                  => _t.BorderColor;
        public override Color SeparatorLight                 => _t.PanelBackColor;
        public override Color ImageMarginGradientBegin       => _t.PanelBackColor;
        public override Color ImageMarginGradientMiddle      => _t.PanelBackColor;
        public override Color ImageMarginGradientEnd         => _t.PanelBackColor;
    }
}
