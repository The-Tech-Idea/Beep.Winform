// BeepDocumentGroup.cs
// Represents a single split pane inside a BeepDocumentHost (feature 2.1 — Document Groups).
// Each group owns exactly one BeepDocumentTabStrip + one content Panel.
// BeepDocumentHost manages the collection of groups and arranges them in a split layout.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Represents one split pane within a <see cref="BeepDocumentHost"/>.
    /// A host always has at least one group (the primary group).  Calling
    /// <see cref="BeepDocumentHost.SplitDocumentHorizontal"/> or
    /// <see cref="BeepDocumentHost.SplitDocumentVertical"/> creates additional groups.
    /// </summary>
    public sealed class BeepDocumentGroup : IDisposable
    {
        // ─────────────────────────────────────────────────────────────────────
        // Private double-buffered panel — avoids reflection on Panel internals
        // ─────────────────────────────────────────────────────────────────────
        private sealed class DoubleBufferedContentPanel : Panel
        {
            public DoubleBufferedContentPanel() => DoubleBuffered = true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Identity
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Unique identifier for this group (GUID string).</summary>
        public string GroupId { get; }

        /// <summary>
        /// True for the first (legacy) group, which wraps the original <c>_tabStrip</c>
        /// and <c>_contentArea</c> created in the <see cref="BeepDocumentHost"/> constructor.
        /// </summary>
        public bool IsPrimary { get; }

        // ─────────────────────────────────────────────────────────────────────
        // Child controls — owned by this group (secondary) or borrowed (primary)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>The tab strip owned by this group.</summary>
        internal BeepDocumentTabStrip TabStrip { get; }

        /// <summary>The content panel that hosts document panels belonging to this group.</summary>
        internal Panel ContentArea { get; }

        // ─────────────────────────────────────────────────────────────────────
        // Document registry
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Ordered list of document IDs belonging to this group.</summary>
        internal List<string> DocumentIds { get; } = new List<string>();

        /// <summary>True when this group contains no documents.</summary>
        public bool IsEmpty => DocumentIds.Count == 0;

        // ─────────────────────────────────────────────────────────────────────
        // Layout state
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Bounds (relative to the host) assigned by <see cref="BeepDocumentHost.RecalculateLayout"/>.</summary>
        internal Rectangle GroupBounds { get; set; }

        /// <summary>
        /// Per-group tab strip position.  Defaults to the host's global <c>TabPosition</c>
        /// but can be overridden per group (e.g. left group tabs on top, right group tabs on bottom).
        /// </summary>
        public TabStripPosition TabPosition { get; set; } = TabStripPosition.Top;

        // ─────────────────────────────────────────────────────────────────────
        // Events forwarded to BeepDocumentHost
        // ─────────────────────────────────────────────────────────────────────

        internal event EventHandler<TabEventArgs>?        TabSelected;
        internal event EventHandler<TabEventArgs>?        TabCloseRequested;
        internal event EventHandler<TabClosingEventArgs>? TabClosing;
        internal event EventHandler<TabEventArgs>?        TabFloatRequested;
        internal event EventHandler<TabEventArgs>? TabPinToggled;
        internal event EventHandler<TabReorderArgs>? TabReordered;
        internal event EventHandler? AddButtonClicked;

        // ─────────────────────────────────────────────────────────────────────
        // Constructors
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Constructor for the PRIMARY group — wraps the pre-existing strip and content
        /// area that were created in <see cref="BeepDocumentHost"/>'s constructor.
        /// </summary>
        internal BeepDocumentGroup(string groupId,
                                   BeepDocumentTabStrip tabStrip,
                                   Panel contentArea)
        {
            GroupId     = groupId;
            IsPrimary   = true;
            TabStrip    = tabStrip;
            ContentArea = contentArea;
            WireEvents();
        }

        /// <summary>
        /// Constructor for SECONDARY groups — creates fresh controls.
        /// </summary>
        internal BeepDocumentGroup(string groupId,
                                   string themeName,
                                   bool showAddButton,
                                   TabCloseMode closeMode,
                                   DocumentTabStyle tabStyle,
                                   IBeepTheme theme)
        {
            GroupId   = groupId;
            IsPrimary = false;

            TabStrip = new BeepDocumentTabStrip
            {
                Dock         = DockStyle.None,
                ThemeName    = themeName,
                ShowAddButton = showAddButton,
                CloseMode    = closeMode,
                TabStyle     = tabStyle
            };

            ContentArea = new DoubleBufferedContentPanel
            {
                Dock      = DockStyle.None,
                BackColor = theme?.BackgroundColor ?? DocumentGroupThemeHelpers.ThemeAwareWindow()
            };

            WireEvents();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Event wiring
        // ─────────────────────────────────────────────────────────────────────

        private void WireEvents()
        {
            TabStrip.TabSelected       += (s, e) => TabSelected?.Invoke(this, e);
            TabStrip.TabCloseRequested += (s, e) => TabCloseRequested?.Invoke(this, e);
            TabStrip.TabClosing        += (s, e) => TabClosing?.Invoke(this, e);
            TabStrip.TabFloatRequested += (s, e) => TabFloatRequested?.Invoke(this, e);
            TabStrip.TabPinToggled     += (s, e) => TabPinToggled?.Invoke(this, e);
            TabStrip.TabReordered      += (s, e) => TabReordered?.Invoke(this, e);
            TabStrip.AddButtonClicked  += (s, e) => AddButtonClicked?.Invoke(this, e);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Layout application
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Positions the tab strip and content area within <see cref="GroupBounds"/>
        /// according to this group's <see cref="TabPosition"/>.
        /// </summary>
        internal void ApplyBounds(int stripH)
        {
            var b = GroupBounds;
            if (b.Width <= 0 || b.Height <= 0) return;

            var tabPos = TabPosition;
            switch (tabPos)
            {
                case TabStripPosition.Top:
                    TabStrip.IsVertical = false;
                    TabStrip.SetBounds(b.Left, b.Top, b.Width, stripH);
                    ContentArea.SetBounds(b.Left, b.Top + stripH, b.Width, Math.Max(0, b.Height - stripH));
                    TabStrip.Visible = true;
                    break;

                case TabStripPosition.Bottom:
                    TabStrip.IsVertical = false;
                    ContentArea.SetBounds(b.Left, b.Top, b.Width, Math.Max(0, b.Height - stripH));
                    TabStrip.SetBounds(b.Left, b.Top + b.Height - stripH, b.Width, stripH);
                    TabStrip.Visible = true;
                    break;

                case TabStripPosition.Left:
                    TabStrip.IsVertical = true;
                    TabStrip.SetBounds(b.Left, b.Top, stripH, b.Height);
                    ContentArea.SetBounds(b.Left + stripH, b.Top, Math.Max(0, b.Width - stripH), b.Height);
                    TabStrip.Visible = true;
                    break;

                case TabStripPosition.Right:
                    TabStrip.IsVertical = true;
                    ContentArea.SetBounds(b.Left, b.Top, Math.Max(0, b.Width - stripH), b.Height);
                    TabStrip.SetBounds(b.Left + b.Width - stripH, b.Top, stripH, b.Height);
                    TabStrip.Visible = true;
                    break;

                case TabStripPosition.Hidden:
                    TabStrip.IsVertical = false;
                    TabStrip.Visible = false;
                    ContentArea.SetBounds(b.Left, b.Top, b.Width, b.Height);
                    break;
            }

            TabStrip.CalculateTabLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Theme
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Applies the theme to this group's controls.</summary>
        internal void ApplyTheme(string themeName, IBeepTheme theme)
        {
            TabStrip.ThemeName      = themeName;
            ContentArea.BackColor   = theme?.BackgroundColor
                                      ?? DocumentGroupThemeHelpers.ThemeAwareWindow();
        }

        // ─────────────────────────────────────────────────────────────────────
        // IDisposable
        // ─────────────────────────────────────────────────────────────────────

        private bool _disposed;

        /// <summary>
        /// Disposes owned controls.  Does NOT dispose primary-group controls
        /// (those are owned by <see cref="BeepDocumentHost"/>).
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (!IsPrimary)
            {
                TabStrip.Dispose();
                ContentArea.Dispose();
            }
        }
    }

    internal static class DocumentGroupThemeHelpers
    {
        internal static Color ThemeAwareWindow()
        {
            return SystemInformation.HighContrast ? SystemColors.Window : ColorUtils.MapSystemColor(SystemColors.Window);
        }
    }
}
