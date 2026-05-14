// BeepDockManager.cs
// Non-visual component that manages named tool-window panels (Solution Explorer,
// Properties, Output, etc.) on the edges of a BeepDocumentHost.
//
// Usage (designer):
//   1. Drop BeepDockManager onto the form's component tray.
//   2. Set BeepDockManager.Host to your BeepDocumentHost.
//   3. Use "Edit Dock Panels…" smart-tag to define panels at design time.
//   4. In Form_Load, assign panel.Content = yourControl and call manager.ShowPanel(id).
//
// Usage (code):
//   var desc = manager.AddPanel("SolutionExplorer", DockEdge.Left, treeView);
//   manager.HidePanel(desc.Id);
//   manager.AutoHidePanel(desc.Id);
//   manager.FloatPanel(desc.Id);
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Non-visual component that manages named tool-window dock panels on the edges
    /// of a <see cref="BeepDocumentHost"/> — the Beep equivalent of DevExpress DockManager.
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDockManager), "BeepDockManager.bmp")]
    [Description("Non-visual manager for tool-window dock panels on a BeepDocumentHost.")]
    [Designer("TheTechIdea.Beep.Winform.Controls.DocumentHost.BeepDockManagerDesigner, TheTechIdea.Beep.Winform.Controls")]
    [DefaultProperty("Panels")]
    public partial class BeepDockManager : Component, ISupportInitialize
    {
        // ─────────────────────────────────────────────────────────────────────
        // Fields
        // ─────────────────────────────────────────────────────────────────────

        private BeepDocumentHost?                   _host;
        private readonly BindingList<DockPanelDescriptor> _panels = new();
        private bool                                _inInit;

        // One BeepDockRail per edge — created lazily.
        private BeepDockRail? _railLeft, _railRight, _railTop, _railBottom;

        // Float windows: panelId → floating form.
        private readonly Dictionary<string, Form> _floatWindows =
            new(StringComparer.Ordinal);

        // ─────────────────────────────────────────────────────────────────────
        // Constructors
        // ─────────────────────────────────────────────────────────────────────

        public BeepDockManager() { WireListEvents(); InitCrossHost(); }

        public BeepDockManager(IContainer container)
        {
            container?.Add(this);
            WireListEvents();
            InitCrossHost();
        }

        // Cross-host partial hooks (implemented in BeepDockManager.CrossHost.cs)
        partial void InitCrossHost();
        partial void CleanupCrossHost();

        // ─────────────────────────────────────────────────────────────────────
        // ISupportInitialize
        // ─────────────────────────────────────────────────────────────────────

        public void BeginInit() => _inInit = true;

        public void EndInit()
        {
            _inInit = false;
            ApplyAllPanels();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The <see cref="BeepDocumentHost"/> whose edges this manager populates.
        /// Must be set before <see cref="ISupportInitialize.EndInit"/> runs (i.e.
        /// before <c>InitializeComponent()</c> completes).
        /// </summary>
        [Category("Document Manager")]
        [Description("The BeepDocumentHost whose edges this manager populates.")]
        [DefaultValue(null)]
        public BeepDocumentHost? Host
        {
            get => _host;
            set
            {
                if (ReferenceEquals(_host, value)) return;
                DetachHost();
                _host = value;
                AttachHost();
                if (!_inInit) ApplyAllPanels();
            }
        }

        /// <summary>
        /// The collection of tool-window panels managed by this component.
        /// Modify at design time via the <b>Edit Dock Panels…</b> smart-tag action.
        /// </summary>
        [Category("Document Manager")]
        [Description("Tool-window panels managed by this component.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<DockPanelDescriptor> Panels => _panels;

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised after a panel's visibility, pin state, edge, or float state changes.</summary>
        public event EventHandler<DockPanelEventArgs>? PanelStateChanged;

        // ─────────────────────────────────────────────────────────────────────
        // Public API — panel lifecycle
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a new tool panel to the collection and immediately shows it on the
        /// specified edge (pinned).
        /// </summary>
        /// <param name="title">Display title.</param>
        /// <param name="edge">Target edge.</param>
        /// <param name="content">Control hosted inside the panel. May be null and set later.</param>
        /// <param name="iconPath">Optional icon path.</param>
        /// <returns>The new descriptor.</returns>
        public DockPanelDescriptor AddPanel(
            string    title,
            DockEdge  edge    = DockEdge.Left,
            Control?  content = null,
            string?   iconPath = null)
        {
            var desc = new DockPanelDescriptor
            {
                Title    = title,
                Edge     = edge,
                Content  = content,
                IconPath = iconPath,
                IsVisible    = true,
                IsAutoHidden = false
            };
            _panels.Add(desc);          // triggers OnPanelAdded via list-changed event
            return desc;
        }

        /// <summary>Shows a panel in pinned (permanently visible) state.</summary>
        public void ShowPanel(string id)
        {
            var desc = Find(id);
            if (desc is null) return;
            desc.IsAutoHidden = false;
            desc.IsVisible    = true;
            if (desc.Edge != DockEdge.Floating)
                EnsureRail(desc.Edge).ShowPinned(desc);
            RaisePanelStateChanged(desc);
        }

        /// <summary>Hides the panel entirely (removes from UI).</summary>
        public void HidePanel(string id)
        {
            var desc = Find(id);
            if (desc is null) return;
            desc.IsVisible = false;
            GetRail(desc.Edge)?.HidePanel(id);
            GetFloatWindow(id)?.Close();
            RaisePanelStateChanged(desc);
        }

        /// <summary>Collapses the panel into the edge auto-hide strip.</summary>
        public void AutoHidePanel(string id)
        {
            var desc = Find(id);
            if (desc is null) return;
            desc.IsAutoHidden = true;
            desc.IsVisible    = true;
            if (desc.Edge == DockEdge.Floating)
                DockPanel(id, DockEdge.Left);   // dock before auto-hiding
            EnsureRail(desc.Edge).ShowAutoHidden(desc);
            RaisePanelStateChanged(desc);
        }

        /// <summary>Tears the panel off into a floating window.</summary>
        public void FloatPanel(string id)
        {
            var desc = Find(id);
            if (desc is null || desc.Content is null) return;

            // Remove from any existing rail
            GetRail(desc.Edge)?.HidePanel(id);

            // Close an existing float window for this panel if any
            GetFloatWindow(id)?.Close();

            var prevEdge = desc.Edge;
            desc.Edge        = DockEdge.Floating;
            desc.IsVisible   = true;
            desc.IsAutoHidden = false;

            var floatForm = BuildFloatWindow(desc);
            _floatWindows[id] = floatForm;
            floatForm.Show(_host?.FindForm());

            RaisePanelStateChanged(desc);
        }

        /// <summary>Docks a floating panel back to the specified edge in pinned state.</summary>
        public void DockPanel(string id, DockEdge edge)
        {
            var desc = Find(id);
            if (desc is null) return;

            GetFloatWindow(id)?.Close();
            _floatWindows.Remove(id);

            desc.Edge         = edge;
            desc.IsAutoHidden = false;
            desc.IsVisible    = true;
            EnsureRail(edge).ShowPinned(desc);
            RaisePanelStateChanged(desc);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Internal — host attach/detach
        // ─────────────────────────────────────────────────────────────────────

        private void AttachHost()
        {
            if (_host is null) return;
            _host.Disposed += OnHostDisposed;
        }

        private void DetachHost()
        {
            if (_host is null) return;
            _host.Disposed -= OnHostDisposed;
            RemoveAllRails();
        }

        private void OnHostDisposed(object? sender, EventArgs e) => _host = null;

        // ─────────────────────────────────────────────────────────────────────
        // Internal — apply all panels (called after EndInit or Host change)
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyAllPanels()
        {
            if (_host is null) return;
            foreach (var desc in _panels)
                ApplyPanel(desc);
        }

        private void ApplyPanel(DockPanelDescriptor desc)
        {
            if (_host is null) return;
            if (!desc.IsVisible)
            {
                GetRail(desc.Edge)?.HidePanel(desc.Id);
                return;
            }
            if (desc.Edge == DockEdge.Floating)
            {
                FloatPanel(desc.Id);
                return;
            }
            if (desc.IsAutoHidden)
                EnsureRail(desc.Edge).ShowAutoHidden(desc);
            else
                EnsureRail(desc.Edge).ShowPinned(desc);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Internal — rail management
        // ─────────────────────────────────────────────────────────────────────

        private BeepDockRail EnsureRail(DockEdge edge)
        {
            if (_host is null)
                throw new InvalidOperationException("BeepDockManager.Host must be set before showing panels.");

            ref BeepDockRail? slot = ref GetRailRef(edge);
            if (slot is null)
            {
                slot = new BeepDockRail(edge);
                // Wire rail header events directly to the manager so pin/close
                // buttons in overlay headers feed back without a round-trip.
                slot.PanelPinned += OnRailPanelPinned;
                slot.PanelClosed += OnRailPanelClosed;
                _host.RegisterDockRail(edge, slot);
            }
            return slot;
        }

        private void OnRailPanelPinned(object? sender, DockRailPanelEventArgs e)
            => ShowPanel(e.Id);

        private void OnRailPanelClosed(object? sender, DockRailPanelEventArgs e)
            => HidePanel(e.Id);

        private BeepDockRail? GetRail(DockEdge edge) => edge switch
        {
            DockEdge.Left   => _railLeft,
            DockEdge.Right  => _railRight,
            DockEdge.Top    => _railTop,
            DockEdge.Bottom => _railBottom,
            _               => null
        };

        private ref BeepDockRail? GetRailRef(DockEdge edge)
        {
            switch (edge)
            {
                case DockEdge.Left:   return ref _railLeft;
                case DockEdge.Right:  return ref _railRight;
                case DockEdge.Top:    return ref _railTop;
                default:              return ref _railBottom;
            }
        }

        private void RemoveAllRails()
        {
            RemoveRail(ref _railLeft,   DockEdge.Left);
            RemoveRail(ref _railRight,  DockEdge.Right);
            RemoveRail(ref _railTop,    DockEdge.Top);
            RemoveRail(ref _railBottom, DockEdge.Bottom);
        }

        private void RemoveRail(ref BeepDockRail? slot, DockEdge edge)
        {
            if (slot is null) return;
            slot.PanelPinned -= OnRailPanelPinned;
            slot.PanelClosed -= OnRailPanelClosed;
            _host?.UnregisterDockRail(edge);
            slot.Dispose();
            slot = null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Internal — float window
        // ─────────────────────────────────────────────────────────────────────

        private Form BuildFloatWindow(DockPanelDescriptor desc)
        {
            var win = new Form
            {
                Text            = desc.Title,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                StartPosition   = FormStartPosition.CenterParent,
                Size            = new Size(350, 450),
                MinimumSize     = new Size(200, 120),
                ShowInTaskbar   = false
            };

            if (desc.Content is not null)
            {
                desc.Content.Dock = DockStyle.Fill;
                win.Controls.Add(desc.Content);
            }

            win.FormClosing += (_, _) =>
            {
                win.Controls.Remove(desc.Content);
                DockPanel(desc.Id, DockEdge.Left);
                _floatWindows.Remove(desc.Id);
            };

            AttachCrossHostDrag(win, desc);
            return win;
        }

        private Form? GetFloatWindow(string id) =>
            _floatWindows.TryGetValue(id, out var f) ? f : null;

        // ─────────────────────────────────────────────────────────────────────
        // Internal — list event wiring
        // ─────────────────────────────────────────────────────────────────────

        private void WireListEvents()
        {
            _panels.ListChanged += OnPanelListChanged;
        }

        private void OnPanelListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_inInit || _host is null) return;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ApplyPanel(_panels[e.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                    // Index already removed — we can't get the descriptor; just refresh all rails
                    ApplyAllPanels();
                    break;
                case ListChangedType.ItemChanged:
                    if (e.NewIndex >= 0 && e.NewIndex < _panels.Count)
                        ApplyPanel(_panels[e.NewIndex]);
                    break;
                case ListChangedType.Reset:
                    RemoveAllRails();
                    ApplyAllPanels();
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private DockPanelDescriptor? Find(string id) =>
            _panels.FirstOrDefault(p => p.Id == id);

        private void RaisePanelStateChanged(DockPanelDescriptor desc) =>
            PanelStateChanged?.Invoke(this, new DockPanelEventArgs(desc));

        // ─────────────────────────────────────────────────────────────────────
        // Dispose
        // ─────────────────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachHost();
                foreach (var win in _floatWindows.Values.ToList())
                    win.Dispose();
                _floatWindows.Clear();
            }
            CleanupCrossHost();
            base.Dispose(disposing);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Event args
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Event arguments carrying the affected <see cref="DockPanelDescriptor"/>.</summary>
    public sealed class DockPanelEventArgs : EventArgs
    {
        public DockPanelDescriptor Panel { get; }
        public DockPanelEventArgs(DockPanelDescriptor panel) =>
            Panel = panel ?? throw new ArgumentNullException(nameof(panel));
    }
}
