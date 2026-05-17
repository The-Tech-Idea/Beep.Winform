// BeepDocumentHostDesigner.cs
// Design-time designer for BeepDocumentHost.
// Smart-tag actions are provided by DocumentHostActionList (ActionLists/).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Behaviors;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Editors;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time designer for <see cref="BeepDocumentHost"/>.
    /// Provides smart-tag action lists, property grid filtering,
    /// snap-line integration, and design-time verbs.
    /// </summary>
    public partial class BeepDocumentHostDesigner : ParentControlDesigner
    {
        private const string DesignTimeDocumentsPropertyName = nameof(BeepDocumentHost.DesignTimeDocuments);
        private const string DesignTimeLayoutJsonPropertyName = nameof(BeepDocumentHost.DesignTimeLayoutJson);

        // ── Internal tab-strip / content-area panel names must not be moved ──
        private static readonly HashSet<string> _lockedChildTypes
            = new HashSet<string>(StringComparer.Ordinal)
        {
            "BeepDocumentTabStrip",
            nameof(Panel)           // the _contentArea
        };

        private readonly Stack<DocumentDescriptor> _designTimeClosedDocuments = new();
        private readonly HashSet<Control> _contextMenuSurfaces = new();
        private BeepDocumentHost? _wiredHost;

        // Stored so we can unhook it in Dispose (anonymous lambdas cannot be removed).
        private EventHandler? _handleCreatedHandler;
        // Stored so we can unsubscribe from ComponentRemoving in Dispose.
        private IComponentChangeService? _changeSvcSubscribed;
        // Phase 11: ActiveDocumentChanged handler — design-time tab click → selection sync.
        private EventHandler<DocumentEventArgs>? _activeDocumentChangedHandler;
        // Phase 11: guard against re-entrant selection updates while we are
        // applying selection from inside ActiveDocumentChanged.
        private bool _syncingActiveDocumentSelection;

        // ── Smart-tag action lists + design-time verbs ───────────────────────
        // Moved to Designers/BeepDocumentHostDesigner.Verbs.cs.

        // ── Initialise ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Lock internal child controls so they cannot be accidentally
            // moved, resized, or deleted through the design surface.
            if (component is BeepDocumentHost host)
            {
                _wiredHost = host;
                foreach (Control child in host.Controls)
                    LockChild(child);

                WireDesignContextMenuSurfaces(host);
                host.ControlAdded += Host_ControlAdded;
                host.ControlRemoved += Host_ControlRemoved;

                // Store as a named field so Dispose can unhook it.
                // Anonymous lambdas cannot be removed from event subscriptions.
                _handleCreatedHandler = (s, e) =>
                {
                    try
                    {
                        var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                        if (selSvc?.PrimarySelection == host)
                        {
                            selSvc.SetSelectedComponents(null);
                            selSvc.SetSelectedComponents(new object[] { host });
                        }

                        WireDesignContextMenuSurfaces(host);

                        // Phase 02: seeded panels from ApplyDesignTimeDocuments need siting so
                        // that the Properties window can show their BeepDocumentPanelDesigner
                        // and tab-header clicks can route selection to them.
                        SiteAllDesignPanels();

                        // Phase 11: after panels are sited, push the currently
                        // active document into the selection service so the
                        // property grid lands on the visible document tab
                        // (not the host shell) on first form load.
                        SyncInitialActiveDocumentSelection();
                    }
                    catch { /* safe at design-time */ }
                };
                host.HandleCreated += _handleCreatedHandler;

                // Subscribe ComponentRemoving so we can do designer-side scrub
                // BEFORE the host is disposed (prevents paint-during-teardown crash).
                if (GetService(typeof(IComponentChangeService)) is IComponentChangeService changeSvc)
                {
                    _changeSvcSubscribed = changeSvc;
                    changeSvc.ComponentRemoving += OnComponentRemoving;
                }

                // ── Phase 11 — Design-Time Tab Selection ─────────────────────
                // When a tab is clicked at design time the strip raises TabSelected
                // → the host's OnTabSelected switches active panel → raises
                // ActiveDocumentChanged. Until this wiring existed the property
                // grid and toolbox-drop target stayed on the host so the user
                // could not "select a tab document" the way DevExpress/Telerik
                // surfaces let them. Forwarding here pushes the active panel
                // into ISelectionService → property grid swaps automatically.
                _activeDocumentChangedHandler = OnHostActiveDocumentChanged;
                host.ActiveDocumentChanged += _activeDocumentChangedHandler;
            }

            // Sprint 17.1: register the docking-guide compass adorner
            InitializeDockAdorner();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe ComponentRemoving before anything else.
                if (_changeSvcSubscribed != null)
                {
                    try { _changeSvcSubscribed.ComponentRemoving -= OnComponentRemoving; }
                    catch { /* safe */ }
                    _changeSvcSubscribed = null;
                }

                if (_wiredHost != null)
                {
                    try { _wiredHost.ControlAdded -= Host_ControlAdded; } catch { }
                    try { _wiredHost.ControlRemoved -= Host_ControlRemoved; } catch { }

                    if (_handleCreatedHandler != null)
                    {
                        try { _wiredHost.HandleCreated -= _handleCreatedHandler; } catch { }
                        _handleCreatedHandler = null;
                    }

                    if (_activeDocumentChangedHandler != null)
                    {
                        try { _wiredHost.ActiveDocumentChanged -= _activeDocumentChangedHandler; } catch { }
                        _activeDocumentChangedHandler = null;
                    }

                    foreach (Control control in _contextMenuSurfaces.ToList())
                    {
                        try { UnhookDesignContextMenuSurface(control); } catch { }
                    }

                    // Phase 02 + Phase 04: release sited design-time panels so they
                    // don't leak in the designer container and don't get serialised as
                    // orphans. The _isUnsiting guard tells UnsiteAllDesignPanels to skip
                    // the explicit nested.Remove(panel) calls so the change-service
                    // doesn't fire synthetic ComponentRemoved events into the undo stack
                    // during teardown.
                    _isUnsiting = true;
                    try { UnsiteAllDesignPanels(); } catch { /* must never throw in Dispose */ }
                    finally { _isUnsiting = false; }

                    _contextMenuSurfaces.Clear();
                    _wiredHost = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Fired by <see cref="IComponentChangeService.ComponentRemoving"/> when any component
        /// is about to be removed from the designer host. When <em>this</em> host is the one
        /// being removed we do a pre-disposal scrub so the host's own <c>Dispose</c> runs clean.
        /// </summary>
        private void OnComponentRemoving(object? sender, ComponentEventArgs e)
        {
            if (!ReferenceEquals(e.Component, _wiredHost)) return;

            try
            {
                // Signal the host that the designer is detaching before dispose.
                if (_wiredHost != null)
                {
                    // Unhook the HandleCreated handler now (before the handle is destroyed).
                    if (_handleCreatedHandler != null)
                    {
                        _wiredHost.HandleCreated -= _handleCreatedHandler;
                        _handleCreatedHandler = null;
                    }

                    _wiredHost.ControlAdded -= Host_ControlAdded;
                    _wiredHost.ControlRemoved -= Host_ControlRemoved;

                    foreach (Control control in _contextMenuSurfaces.ToList())
                        try { UnhookDesignContextMenuSurface(control); } catch { }
                    _contextMenuSurfaces.Clear();

                    _wiredHost = null;
                }

                // Unsubscribe ourselves now — the Dispose will no-op on the null ref.
                if (_changeSvcSubscribed != null)
                {
                    _changeSvcSubscribed.ComponentRemoving -= OnComponentRemoving;
                    _changeSvcSubscribed = null;
                }
            }
            catch { /* must never throw inside ComponentRemoving */ }
        }

        private void LockChild(Control child)
        {
            var svc = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (svc == null) return;

            var childDesigner = svc.GetDesigner(child) as ControlDesigner;
            if (childDesigner == null) return;

            TypeDescriptor.AddAttributes(child,
                new DesignerAttribute(typeof(ControlDesigner)));
        }

        // ── Property grid filtering ───────────────────────────────────────────

        /// <summary>
        /// Hides low-level Panel infrastructure properties from the Properties
        /// grid so it only shows meaningful BeepDocumentHost properties.
        /// All remaining visible properties have an explicit <c>[Category]</c>
        /// attribute so none land in the default "Misc" group.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // ── Inherited Panel / ScrollableControl plumbing ─────────────────
            // These are not relevant to BeepDocumentHost; hide them so the
            // Properties window only shows meaningful document-host properties.
            string[] hiddenProps =
            {
                // ScrollableControl
                "AutoScroll",
                "AutoScrollMargin",
                "AutoScrollMinSize",
                "AutoScrollOffset",
                "AutoScrollPosition",
                "HorizontalScroll",
                "VerticalScroll",

                // Panel
                "BorderStyle",

                // Layout
                "AutoSize",
                "AutoSizeMode",
                "Padding",
                "Margin",
                "MinimumSize",
                "MaximumSize",

                // Appearance — theme-controlled, direct editor not useful
                "BackColor",
                "BackgroundImage",
                "BackgroundImageLayout",
                "ForeColor",
                "Font",
                "Cursor",
                "RightToLeft",
                "UseCompatibleTextRendering",

                // Behavior — not applicable
                "ImeMode",
                "CausesValidation",
                "UseWaitCursor",
                "AllowDrop",

                // Misc-bound properties
                "Text",
                "Tag",

                // Accessibility — not tuned per-host
                "AccessibleDescription",
                "AccessibleName",
                "AccessibleRole",
            };

            foreach (var name in hiddenProps)
            {
                if (properties[name] is PropertyDescriptor pd)
                    properties[name] = TypeDescriptor.CreateProperty(
                        pd.ComponentType, pd,
                        new BrowsableAttribute(false));
            }
        }

        // ── Snap lines ────────────────────────────────────────────────────────

        /// <summary>
        /// Provides snap lines at the content area edges so other controls
        /// on the parent form align cleanly against the document host.
        /// </summary>
        public override IList<SnapLine> SnapLines
        {
            get
            {
                var lines = new List<SnapLine>(base.SnapLines);

                if (Component is BeepDocumentHost host && host.Controls.Count >= 2)
                {
                    // The content area is always the first control added
                    // (Controls.Add order: _contentArea first, _tabStrip second).
                    // Snap to its edges to let sibling controls align.
                    foreach (Control child in host.Controls)
                    {
                        if (child is Panel contentArea && !(child is BeepDocumentTabStrip))
                        {
                            var r = contentArea.Bounds;
                            lines.Add(new SnapLine(SnapLineType.Top,    r.Top,    SnapLinePriority.Medium));
                            lines.Add(new SnapLine(SnapLineType.Bottom, r.Bottom, SnapLinePriority.Medium));
                            lines.Add(new SnapLine(SnapLineType.Left,   r.Left,   SnapLinePriority.Medium));
                            lines.Add(new SnapLine(SnapLineType.Right,  r.Right,  SnapLinePriority.Medium));
                            break;
                        }
                    }
                }

                return lines;
            }
        }

        // ── DoDefaultAction ───────────────────────────────────────────────────

        /// <summary>
        /// Called when the developer double-clicks the control on the design
        /// surface. Adds a placeholder document so the control is never empty.
        /// </summary>
        public override void DoDefaultAction()
        {
            AddDesignTimeDocument();
        }

        // ── OnPaintAdornments ─────────────────────────────────────────────────

        /// <summary>
        /// Paints a hint overlay when no documents are loaded, so the developer
        /// knows the control is live and how to interact with it.
        /// </summary>
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            if (Component is not BeepDocumentHost host) return;

            // Dock compass (17.1) — rendered during drag operations
            if (_dragActive)
            {
                PaintDockCompass(pe.Graphics, host);
                return;   // skip empty-state hint while dragging
            }

            if (host.DocumentCount > 0) return;   // only draw when empty

            var g   = pe.Graphics;
            var rc  = host.ClientRectangle;

            // Dashed border
            using (var dashPen = new Pen(SystemColors.ControlDark, 1f))
            {
                dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(dashPen, rc.X + 2, rc.Y + 2, rc.Width - 5, rc.Height - 5);
            }

            // Centred hint text
            const string Line1 = "BeepDocumentHost";
            const string Line2 = "Double-click to create a document surface";
            const string Line3 = "Toolbox drops go to the active document  |  Smart-Tag (►) for split/layout actions";

            using var titleFont = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var hintFont  = new Font("Segoe UI",  8f);
            using var titleBrush = new SolidBrush(SystemColors.ControlText);
            using var hintBrush  = new SolidBrush(SystemColors.GrayText);

            using var sf = new StringFormat
            {
                Alignment     = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var cy  = rc.Height / 2;
            var mid = new Rectangle(rc.X, cy - 40, rc.Width, 24);
            g.DrawString(Line1, titleFont, titleBrush, mid, sf);

            mid.Y += 26;
            g.DrawString(Line2, hintFont, hintBrush, mid, sf);

            mid.Y += 18;
            g.DrawString(Line3, hintFont, hintBrush, mid, sf);
        }

        // ── PreFilterEvents ───────────────────────────────────────────────────

        /// <summary>
        /// Hides low-level WinForms plumbing events from the Properties / Events
        /// grid so developers only see events that are meaningful for this control.
        /// </summary>
        protected override void PreFilterEvents(IDictionary events)
        {
            base.PreFilterEvents(events);

            string[] removedEvents =
            {
                "Layout",
                "Paint",
                "PaintBackground",
                "Resize",
                "SizeChanged",
                "Move",
                "LocationChanged",
                "BackColorChanged",
                "ForeColorChanged",
                "FontChanged",
                "CursorChanged",
                "RegionChanged",
                "RightToLeftChanged",
                "ImeModeChanged",
                "Scroll",
                "ControlAdded",
                "ControlRemoved",
                "DockChanged",
                "MarginChanged",
                "PaddingChanged",
                "TabIndexChanged",
                "TabStopChanged",
                "Validated",
                "Validating"
            };

            foreach (var name in removedEvents)
                events.Remove(name);
        }

        // ── Design-time helpers called by verbs ───────────────────────────────

        /// <summary>Opens the DesignTimeDocuments collection editor.</summary>
        private void EditDesignTimeDocuments(BeepDocumentHost host)
        {
            var prop = TypeDescriptor.GetProperties(host)["DesignTimeDocuments"];
            if (prop == null) return;

            var ctx = new DesignTimeDocumentsEditorContext(host, prop,
                GetService(typeof(IServiceProvider)) as IServiceProvider ?? (IServiceProvider)GetService(typeof(IDesignerHost))!);

            var editor = new Editors.DesignTimeDocumentsEditor(
                typeof(System.Collections.ObjectModel.Collection<
                    TheTechIdea.Beep.Winform.Controls.DocumentHost.DocumentDescriptor>));

            var current = prop.GetValue(host);
            var svc     = GetService(typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
                          as System.Windows.Forms.Design.IWindowsFormsEditorService;

            editor.EditValue(ctx, ctx, current);

            ExecuteDesignTimeDocumentsAction("Edit Design-Time Documents", (h, docs) =>
            {
                SyncHostWithDesignTimeDocuments(h, docs);
            });
        }

        /// <summary>Shows a dialog to edit per-group tab positions for nested splits.</summary>
        private void EditGroupTabPositions(BeepDocumentHost host)
        {
            using var dlg = new GroupTabPositionDialog(host);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ExecuteAction("Set Group Tab Positions", h =>
                {
                    foreach (var (groupId, position) in dlg.ChangedPositions)
                        h.SetGroupTabPosition(groupId, position);
                });
            }
        }

        /// <summary>Shows the layout tree structure in a read-only dialog.</summary>
        private void ShowLayoutTreeDialog(BeepDocumentHost host)
        {
            using var dlg = new LayoutTreeDialog(host);
            dlg.ShowDialog();
        }

        /// <summary>Shows the layout preset picker and applies the chosen preset.</summary>
        private void PickAndApplyLayoutPreset(BeepDocumentHost host)
        {
            using var dlg = new LayoutPresetPickerDialog();
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            ApplyDesignTimeLayoutPreset(dlg.SelectedPreset);
        }

        /// <summary>
        /// Routes toolbox-dropped controls into the active document surface, creating
        /// a design-time document when the host is empty.
        /// </summary>
        protected override Control GetParentForComponent(IComponent component)
        {
            if (Component is not BeepDocumentHost)
            {
                return base.GetParentForComponent(component);
            }

            BeepDocumentPanel? targetPanel = EnsureDesignSurfaceForDroppedContent();
            if (targetPanel != null)
            {
                return targetPanel;
            }

            return base.GetParentForComponent(component);
        }

        /// <summary>
        /// Prevents the document host's internal infrastructure controls from being
        /// added directly from the toolbox, but allows any other control so that
        /// toolbox drops are routed to the active document surface.
        /// </summary>
        public override bool CanParent(Control control)
        {
            // Block internal plumbing — these are managed exclusively by BeepDocumentHost itself
            if (control?.GetType().Name is nameof(BeepDocumentPanel)
                                        or nameof(BeepDocumentTabStrip)
                                        or "BeepDocumentDockOverlay"
                                        or "BeepAutoHideStrip")
            {
                return false;
            }

            // Allow everything else so the designer can route toolbox drops to the
            // active document panel surface (P7-004).
            return true;
        }

        // ── Drag-and-drop pipeline (OnDragEnter/Over/Drop/Leave + dock compass)
        // Moved to Designers/BeepDocumentHostDesigner.DragDrop.cs.

        // ── Generic property helpers (used by DocumentHostActionList) ─────────

        /// <summary>Reads a property value from the hosted component.</summary>
        public T GetProperty<T>(string propertyName)
        {
            if (Component == null) return default!;
            var prop = TypeDescriptor.GetProperties(Component)[propertyName];
            if (prop != null)
            {
                var v = prop.GetValue(Component);
                if (v is T typed) return typed;
            }
            return default!;
        }

        /// <summary>Writes a property value to the hosted component (undo-aware).</summary>
        /// <remarks>
        /// Phase 04: each smart-tag property write is wrapped in its own
        /// <see cref="DesignerTransaction"/> so the Edit menu surfaces a
        /// descriptive undo entry (e.g. "Set TabStyle") and concurrent writes
        /// are atomic. The previous implementation raised
        /// <see cref="IComponentChangeService"/> events without a transaction,
        /// which left the change visible but not always reversible from the
        /// Edit menu UI.
        /// </remarks>
        public void SetProperty(string propertyName, object? value)
        {
            if (Component == null) return;
            var prop = TypeDescriptor.GetProperties(Component)[propertyName];
            if (prop == null || prop.IsReadOnly) return;

            var designerHost = GetDesignerHost();
            var svc          = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var oldValue     = prop.GetValue(Component);

            DesignerTransaction? txn = null;
            try
            {
                txn = designerHost?.CreateTransaction($"Set {propertyName}");
                svc?.OnComponentChanging(Component, prop);
                prop.SetValue(Component, value);
                svc?.OnComponentChanged(Component, prop, oldValue, value);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }
        }

        /// <summary>
        /// Executes a designer action inside a <see cref="DesignerTransaction"/> so it
        /// participates fully in the VS undo/redo stack.
        /// </summary>
        /// <param name="description">
        /// The human-readable description shown in the Edit menu (e.g. "Undo Add Document").
        /// </param>
        /// <param name="action">The action to run. Called only if the host is non-null.</param>
        public void ExecuteAction(string description, Action<BeepDocumentHost> action)
        {
            if (Component is not BeepDocumentHost host) return;

            var designerHost = GetDesignerHost();
            var changeSvc    = GetChangeService();

            DesignerTransaction? txn = null;
            try
            {
                txn = designerHost?.CreateTransaction(description);
                changeSvc?.OnComponentChanging(host, null);

                action(host);

                changeSvc?.OnComponentChanged(host, null, null, null);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }

            RefreshDesignerActionUI();
        }

        // ── Public design-time document CRUD ─────────────────────────────────
        // Moved to Designers/BeepDocumentHostDesigner.DesignTimeDocuments.cs.

        private IDesignerHost? GetDesignerHost()
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

        /// <summary>
        /// Returns the <see cref="BeepDocumentPanel"/> that is currently active in the host,
        /// or <c>null</c> if none is available.  Used by <see cref="OnDragDrop"/> to redirect
        /// toolbox drops to the correct inner panel.
        /// </summary>
        private static BeepDocumentPanel? GetActiveDocumentPanel(BeepDocumentHost host)
        {
            // Fix (Phase 02): previous implementation walked host.Controls and matched panel.Name
            // against ActiveDocumentId. Panels are not direct children of the host (they live
            // under _contentArea) and Name is never set to DocumentId, so the lookup always
            // returned null and broke toolbox-drop redirection. Use the public accessor.
            var activeId = host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return null;
            return host.GetPanel(activeId);
        }

        private IComponentChangeService? GetChangeService()
            => GetService(typeof(IComponentChangeService)) as IComponentChangeService;

        private ISelectionService? GetSelectionService()
            => GetService(typeof(ISelectionService)) as ISelectionService;

        private DesignerActionUIService? GetDesignerActionUiService()
            => GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;

        private PropertyDescriptor? GetDesignTimeDocumentsProperty()
            => Component == null ? null : TypeDescriptor.GetProperties(Component)[DesignTimeDocumentsPropertyName];

        private PropertyDescriptor? GetDesignTimeLayoutProperty()
            => Component == null ? null : TypeDescriptor.GetProperties(Component)[DesignTimeLayoutJsonPropertyName];

        private void RefreshDesignerActionUI()
        {
            if (Component == null)
            {
                return;
            }

            GetDesignerActionUiService()?.Refresh(Component);
        }

        private void SyncDesignerSelection(object? selectionTarget)
        {
            if (selectionTarget == null)
            {
                return;
            }

            GetSelectionService()?.SetSelectedComponents(new object[] { selectionTarget }, SelectionTypes.Replace);
            RefreshDesignerActionUI();
        }

        /// <summary>
        /// Phase 11 — Forwards the host's ActiveDocumentChanged event into the
        /// designer's selection service so a tab click at design time selects
        /// the corresponding <see cref="BeepDocumentPanel"/> in the property
        /// grid and routes subsequent toolbox drops onto that panel.
        /// </summary>
        /// <remarks>
        /// Guarded with <see cref="_syncingActiveDocumentSelection"/> to avoid
        /// recursive selection notifications and to no-op when the panel is
        /// not (yet) sited via INestedContainer — siting happens in
        /// <c>SiteAllDesignPanels</c> from the HandleCreated callback so the
        /// very first activation right after form load can fire before the
        /// panel has been siteed; the next user click will re-fire correctly.
        /// </remarks>
        private void OnHostActiveDocumentChanged(object? sender, DocumentEventArgs e)
        {
            if (_syncingActiveDocumentSelection)
            {
                return;
            }

            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            BeepDocumentPanel? panel = host.ActivePanel;
            if (panel == null)
            {
                return;
            }

            // Panels that have not yet been sited (no Site / null Container) cannot
            // be promoted into the ISelectionService selection — fall back to a
            // selection of the host so the property grid at least stays consistent.
            object selection = panel.Site != null ? (object)panel : host;

            _syncingActiveDocumentSelection = true;
            try
            {
                SyncDesignerSelection(selection);
            }
            finally
            {
                _syncingActiveDocumentSelection = false;
            }
        }

        /// <summary>
        /// Phase 11 — Pushes the currently active document panel into the
        /// designer selection one time on load. Called from the HandleCreated
        /// callback after <c>SiteAllDesignPanels</c> so the property grid
        /// lands on the visible document instead of the host shell.
        /// </summary>
        private void SyncInitialActiveDocumentSelection()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            if (host.ActivePanel is not BeepDocumentPanel panel)
            {
                return;
            }

            // Only sync if the user has not already clicked elsewhere — never
            // steal the current selection if it is already in the host's tree.
            var selSvc = GetSelectionService();
            if (selSvc?.PrimarySelection is Control selected
                && (selected == host || IsDescendantOfHost(selected, host)))
            {
                _syncingActiveDocumentSelection = true;
                try
                {
                    SyncDesignerSelection(panel.Site != null ? (object)panel : host);
                }
                finally
                {
                    _syncingActiveDocumentSelection = false;
                }
            }
        }

        private static bool IsDescendantOfHost(Control candidate, BeepDocumentHost host)
        {
            Control? c = candidate;
            while (c != null)
            {
                if (ReferenceEquals(c, host))
                {
                    return true;
                }
                c = c.Parent;
            }

            return false;
        }

        // ── ExecuteDesignTimeDocumentsAction (transactional core) ────────────
        // ── ApplyLayoutAssistant / preset core ───────────────────────────────
        // ── SyncHostWithDesignTimeDocuments / descriptor sync ────────────────
        // ── EnsureActiveDesignDocumentSurface and CRUD helpers ───────────────
        // ── CloneDescriptor / CaptureDocumentDescriptor / FindDesignTime ─────
        // ── DragDrop compass + OnDragXxx overrides ───────────────────────────
        //
        // All moved to:
        //   Designers/BeepDocumentHostDesigner.DesignTimeDocuments.cs
        //   Designers/BeepDocumentHostDesigner.LayoutPresets.cs
        //   Designers/BeepDocumentHostDesigner.DragDrop.cs
    }

    // Phase 03 partial-class refactor (workspace rule: one file, one class).
    // The following helper classes were extracted from this monolith:
    //   • DocumentHostDesignerMenuRenderer / DocumentHostDesignerColorTable
    //       → Designers/DocumentHostDesignerMenuTheming.cs
    //   • DesignTimeDocumentsEditorContext
    //       → Designers/DesignTimeDocumentsEditorContext.cs
    //   • GroupTabPositionDialog
    //       → Dialogs/GroupTabPositionDialog.cs
    //   • LayoutTreeDialog
    //       → Dialogs/LayoutTreeDialog.cs
}
