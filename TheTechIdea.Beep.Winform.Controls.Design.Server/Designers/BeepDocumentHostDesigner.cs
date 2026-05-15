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
    public class BeepDocumentHostDesigner : ParentControlDesigner
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

        // ── Action list ───────────────────────────────────────────────────────

        private DesignerActionListCollection? _actionLists;

        /// <inheritdoc/>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                _actionLists ??= new DesignerActionListCollection
                {
                    new DocumentHostActionList(this)
                };
                return _actionLists;
            }
        }

        // ── Design-time verbs ─────────────────────────────────────────────────

        private DesignerVerbCollection? _verbs;
        /// <summary>
        /// Cached reference so we can toggle <see cref="DesignerVerb.Enabled"/>
        /// when a <see cref="BeepDocumentManager"/> already surfaces this verb.
        /// </summary>
        private DesignerVerb? _shortcutsVerb;

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                    new DesignerVerb("Add Document", (s, e) =>
                    {
                        AddDesignTimeDocument();
                    }),
                    new DesignerVerb("Close Active Document", (s, e) =>
                    {
                        CloseActiveDesignTimeDocument();
                    }),
                    new DesignerVerb("Split With New Document \u2194", (s, e) =>
                    {
                        CreateSplitDesignTimeDocument(horizontal: true);
                    }),
                    new DesignerVerb("Split With New Document \u2195", (s, e) =>
                    {
                        CreateSplitDesignTimeDocument(horizontal: false);
                    }),
                    new DesignerVerb("Select Active Document Surface", (s, e) =>
                    {
                        SelectActiveDocumentSurface();
                    }),
                    new DesignerVerb("Reopen Last Closed Document", (s, e) =>
                    {
                        ReopenLastClosedDesignTimeDocument();
                    }),
                    new DesignerVerb("Layout Assistant\u2026", (s, e) =>
                    {
                        ShowLayoutAssistant();
                    }),
                    new DesignerVerb("Merge All Groups", (s, e) =>
                    {
                        MergeAllDesignTimeGroups();
                    }),
                    new DesignerVerb("Edit Design-Time Documents\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                            EditDesignTimeDocuments(host);
                    }),
                    new DesignerVerb("Apply Layout Preset\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                            PickAndApplyLayoutPreset(host);
                    }),
                    // Sprint 19: Nested split verbs
                    new DesignerVerb("Set Group Tab Position\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host && host.Groups.Count > 1)
                            EditGroupTabPositions(host);
                    }),
                    new DesignerVerb("View Layout Tree\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                            ShowLayoutTreeDialog(host);
                    }),
                    // Phase 7 designer verbs (P7-003)
                    new DesignerVerb("Export Layout Snapshot\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                        {
                            var json = host.SaveLayout();
                            System.Windows.Forms.Clipboard.SetText(json);
                            System.Windows.Forms.MessageBox.Show(
                                "Layout snapshot copied to clipboard.",
                                "Export Layout Snapshot",
                                System.Windows.Forms.MessageBoxButtons.OK,
                                System.Windows.Forms.MessageBoxIcon.Information);
                        }
                    }),
                    new DesignerVerb("Clear All Documents", (s, e) =>
                    {
                        var r = System.Windows.Forms.MessageBox.Show(
                            "Remove all design-time documents from the host?",
                            "Clear All Documents",
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Warning);
                        if (r == System.Windows.Forms.DialogResult.Yes)
                            CloseAllDesignTimeDocuments();
                    }),
                    new DesignerVerb("Customize Keyboard Shortcuts\u2026", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                        {
                            var registry = host.CommandRegistry;
                            using var dlg = new DocumentHostShortcutEditorDialog(registry);
                            dlg.ShowDialog();
                        }
                    }),
                };

                    _shortcutsVerb = (DesignerVerb)_verbs[_verbs.Count - 1];
                }

                // A4: suppress host-level verb when BeepDocumentManager on the
                // same form already provides "Customize Keyboard Shortcuts…".
                if (_shortcutsVerb != null)
                    _shortcutsVerb.Enabled = !HasBoundManager();

                return _verbs;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> when a <see cref="BeepDocumentManager"/>
        /// on the same design surface has its <see cref="BeepTabbedView.Host"/>
        /// set to this <see cref="BeepDocumentHost"/>.
        /// </summary>
        private bool HasBoundManager()
        {
            if (Component is not BeepDocumentHost host) return false;
            var container = host.Site?.Container;
            if (container == null) return false;

            foreach (IComponent comp in container.Components)
            {
                if (comp is BeepDocumentManager mgr &&
                    (mgr.View as BeepTabbedView)?.Host == host)
                    return true;
            }
            return false;
        }

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

                    foreach (Control control in _contextMenuSurfaces.ToList())
                    {
                        try { UnhookDesignContextMenuSurface(control); } catch { }
                    }

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

        /// <summary>
        /// Intercepts drag-drop from the toolbox or component tray and redirects
        /// the dropped control to the currently active design-time document panel
        /// rather than to the BeepDocumentHost root.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs de)
        {
            bool toolboxDrag = IsToolboxDrag(de);

            // For toolbox drops, try to redirect to the active document panel designer
            // so the dropped control lands in the document surface, not the host root.
            if (toolboxDrag && Component is BeepDocumentHost hostForToolbox)
            {
                var activePanel = GetActiveDocumentPanel(hostForToolbox);
                if (activePanel != null)
                {
                    var designerHost = GetDesignerHost();
                    if (designerHost?.GetDesigner(activePanel) is ControlDesigner panelDesigner)
                    {
                        panelDesigner.GetType()
                            .GetMethod("OnDragDrop",
                                System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public,
                                null,
                                new[] { typeof(DragEventArgs) }, null)
                            ?.Invoke(panelDesigner, new object[] { de });
                        _dragActive = false;
                        _dragZone   = DockZone.None;
                        InvalidateHost();
                        return;
                    }
                }
            }

            base.OnDragDrop(de);

            // For non-toolbox (document tab) drags, handle dock-zone-based splitting.
            if (!toolboxDrag && Component is BeepDocumentHost host)
            {
                var screenPt = new Point(de.X, de.Y);
                var clientPt = host.PointToClient(screenPt);
                var zone     = HitTestCompass(host.ClientRectangle, clientPt);

                switch (zone)
                {
                    case DockZone.Left:
                    case DockZone.Right:
                        if (!string.IsNullOrEmpty(host.ActiveDocumentId))
                            ExecuteAction("Split Horizontal",
                                h => h.SplitDocumentHorizontal(h.ActiveDocumentId!));
                        break;

                    case DockZone.Top:
                    case DockZone.Bottom:
                        if (!string.IsNullOrEmpty(host.ActiveDocumentId))
                            ExecuteAction("Split Vertical",
                                h => h.SplitDocumentVertical(h.ActiveDocumentId!));
                        break;

                    // DockZone.Center → no split; tab dropped into existing group
                }
            }

            _dragActive = false;
            _dragZone   = DockZone.None;
            InvalidateHost();
        }

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
        public void SetProperty(string propertyName, object? value)
        {
            if (Component == null) return;
            var prop = TypeDescriptor.GetProperties(Component)[propertyName];
            if (prop == null || prop.IsReadOnly) return;

            var svc      = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var oldValue = prop.GetValue(Component);

            svc?.OnComponentChanging(Component, prop);
            prop.SetValue(Component, value);
            svc?.OnComponentChanged(Component, prop, oldValue, value);
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

        public void AddDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Add Document", (h, docs) =>
            {
                AddDesignTimeDocumentInternal(h, docs, activate: true, selectSurface: true);
            });
        }

        public void CloseActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Close Document '{activeDocumentId}'", (h, docs) =>
            {
                DocumentDescriptor snapshot = CaptureDocumentDescriptor(h, docs, activeDocumentId);
                if (!CloseDesignTimeDocument(h, activeDocumentId))
                {
                    return;
                }

                DocumentDescriptor? existing = FindDesignTimeDocument(docs, activeDocumentId);
                if (existing != null)
                {
                    docs.Remove(existing);
                }

                _designTimeClosedDocuments.Push(snapshot);
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
            });
        }

        public void CloseAllDesignTimeDocuments()
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Close All Documents", (h, docs) =>
            {
                foreach (DocumentDescriptor descriptor in docs.ToList())
                {
                    if (string.IsNullOrWhiteSpace(descriptor.Id))
                    {
                        continue;
                    }

                    DocumentDescriptor snapshot = CloneDescriptor(descriptor);
                    if (CloseDesignTimeDocument(h, descriptor.Id))
                    {
                        _designTimeClosedDocuments.Push(snapshot);
                        docs.Remove(descriptor);
                    }
                }

                SyncDesignerSelection(h);
            });
        }

        public void ReopenLastClosedDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost || _designTimeClosedDocuments.Count == 0)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Reopen Last Closed Document", (h, docs) =>
            {
                DocumentDescriptor descriptor = CloneDescriptor(_designTimeClosedDocuments.Pop());
                if (FindDesignTimeDocument(docs, descriptor.Id) == null)
                {
                    docs.Add(descriptor);
                }

                BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(h, descriptor, activate: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void CreateSplitDesignTimeDocument(bool horizontal)
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            string description = horizontal ? "Split Horizontal" : "Split Vertical";
            ExecuteDesignTimeDocumentsAction(description, (h, docs) =>
            {
                CreateSplitDesignTimeDocumentInternal(h, docs, horizontal, selectSurface: true);
            });
        }

        public void ApplyDesignTimeLayoutPreset(LayoutPreset preset)
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction($"Apply Layout Preset: {preset}", (h, docs) =>
            {
                ApplyDesignTimeLayoutPresetCore(h, docs, preset, selectSurface: true);
            });
        }

        public void SelectActiveDocumentSurface()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            if (host.ActivePanel != null)
            {
                SyncDesignerSelection(host.ActivePanel);
                return;
            }

            ExecuteDesignTimeDocumentsAction("Select Active Document Surface", (h, docs) =>
            {
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public string GetActiveDocumentTitle()
            => (Component as BeepDocumentHost)?.ActivePanel?.DocumentTitle ?? string.Empty;

        public void SetActiveDocumentTitle(string value)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string title = string.IsNullOrWhiteSpace(value) ? "Document" : value.Trim();
            string activeDocumentId = host.ActiveDocumentId!;

            ExecuteDesignTimeDocumentsAction($"Rename Document '{activeDocumentId}'", (h, docs) =>
            {
                DocumentDescriptor descriptor = FindDesignTimeDocument(docs, activeDocumentId)
                    ?? AddDescriptorSnapshotToDesignTimeDocuments(h, docs, activeDocumentId);

                descriptor.Title = title;

                if (h.GetPanel(activeDocumentId) is BeepDocumentPanel panel)
                {
                    panel.DocumentTitle = title;
                }

                foreach (BeepDocumentGroup group in h.Groups)
                {
                    BeepDocumentTab? tab = group.TabStrip.FindTabById(activeDocumentId);
                    if (tab != null)
                    {
                        tab.Title = title;
                        group.TabStrip.Invalidate();
                    }
                }

                h.RecalculateLayout();
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
            });
        }

        public void SetActiveDocumentPinned(bool pinned)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            string description = pinned ? $"Pin Document '{activeDocumentId}'" : $"Unpin Document '{activeDocumentId}'";
            ExecuteDesignTimeDocumentsAction(description, (h, docs) =>
            {
                DocumentDescriptor descriptor = FindDesignTimeDocument(docs, activeDocumentId)
                    ?? AddDescriptorSnapshotToDesignTimeDocuments(h, docs, activeDocumentId);
                descriptor.IsPinned = pinned;
                h.PinDocument(activeDocumentId, pinned);
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
            });
        }

        public void MergeAllDesignTimeGroups()
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Merge All Groups", (h, docs) =>
            {
                h.MergeAllGroups();
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void FloatActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Float Document '{activeDocumentId}'", (h, docs) =>
            {
                h.FloatDocument(activeDocumentId);
                SyncDesignerSelection(h);
            });
        }

        public void DockBackActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Dock Document '{activeDocumentId}'", (h, docs) =>
            {
                h.DockBackDocument(activeDocumentId);
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void AutoHideActiveDesignTimeDocument(AutoHideSide side)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Auto Hide Document '{activeDocumentId}'", (h, docs) =>
            {
                h.AutoHideDocument(activeDocumentId, side);
                SyncDesignerSelection(h);
            });
        }

        public void RestoreAutoHideActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Restore Auto Hide Document '{activeDocumentId}'", (h, docs) =>
            {
                h.RestoreAutoHideDocument(activeDocumentId);
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void ShowLayoutAssistant()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            using var dialog = new DocumentHostLayoutAssistantDialog(host.DesignTimeDocuments);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Layout Assistant", (h, docs) =>
            {
                ApplyLayoutAssistant(h, docs, dialog.SelectedPreset, dialog.Documents);
            });
        }

        public void RenameActiveDesignTimeDocumentWithPrompt()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            using var dialog = new DocumentHostTextPromptDialog(
                "Rename Document",
                "Document title:",
                GetActiveDocumentTitle());

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetActiveDocumentTitle(dialog.Value);
            }
        }

        public void OpenDesignTimeDocumentsEditor()
        {
            if (Component is BeepDocumentHost host)
            {
                EditDesignTimeDocuments(host);
            }
        }

        public bool CanReopenLastClosedDesignTimeDocument => _designTimeClosedDocuments.Count > 0;

        public DocumentDockState GetActiveDocumentDockState()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return DocumentDockState.None;
            }

            return host.GetDocumentDockState(host.ActiveDocumentId!);
        }

        private IDesignerHost? GetDesignerHost()
            => GetService(typeof(IDesignerHost)) as IDesignerHost;

        /// <summary>
        /// Returns the <see cref="BeepDocumentPanel"/> that is currently active in the host,
        /// or <c>null</c> if none is available.  Used by <see cref="OnDragDrop"/> to redirect
        /// toolbox drops to the correct inner panel.
        /// </summary>
        private static BeepDocumentPanel? GetActiveDocumentPanel(BeepDocumentHost host)
        {
            var activeId = host.ActiveDocumentId;
            if (string.IsNullOrEmpty(activeId)) return null;

            // Walk the host's Controls to find the matching panel
            foreach (Control c in host.Controls)
            {
                if (c is BeepDocumentPanel panel &&
                    panel.Name == activeId)
                    return panel;
            }
            return null;
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

        private void ExecuteDesignTimeDocumentsAction(string description, Action<BeepDocumentHost, Collection<DocumentDescriptor>> action)
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            IDesignerHost? designerHost = GetDesignerHost();
            IComponentChangeService? changeService = GetChangeService();
            PropertyDescriptor? property = GetDesignTimeDocumentsProperty();
            PropertyDescriptor? layoutProperty = GetDesignTimeLayoutProperty();
            string previousLayout = host.DesignTimeLayoutJson;

            DesignerTransaction? transaction = null;
            try
            {
                transaction = designerHost?.CreateTransaction(description);
                changeService?.OnComponentChanging(host, property);
                if (layoutProperty != null)
                    changeService?.OnComponentChanging(host, layoutProperty);

                action(host, host.DesignTimeDocuments);

                string currentLayout = CaptureDesignTimeLayout(host, host.DesignTimeDocuments);
                host.DesignTimeLayoutJson = currentLayout;

                changeService?.OnComponentChanged(host, property, null, host.DesignTimeDocuments);
                if (layoutProperty != null)
                    changeService?.OnComponentChanged(host, layoutProperty, previousLayout, host.DesignTimeLayoutJson);
                transaction?.Commit();
            }
            catch
            {
                transaction?.Cancel();
                throw;
            }

            WireDesignContextMenuSurfaces(host);
            RefreshDesignerActionUI();
        }

        private void Host_ControlAdded(object? sender, ControlEventArgs e)
            => HookDesignContextMenuSurface(e.Control);

        private void Host_ControlRemoved(object? sender, ControlEventArgs e)
            => UnhookDesignContextMenuSurface(e.Control);

        private void WireDesignContextMenuSurfaces(BeepDocumentHost host)
        {
            HookDesignContextMenuSurface(host);
            foreach (Control child in host.Controls)
            {
                HookDesignContextMenuSurface(child);
            }
        }

        private void HookDesignContextMenuSurface(Control control)
        {
            if (!IsDesignContextMenuSurface(control) || !_contextMenuSurfaces.Add(control))
            {
                return;
            }

            control.MouseUp += DesignContextSurface_MouseUp;

            // Left-click on a tab header → select that document's panel in the Properties window
            if (control is BeepDocumentTabStrip)
                control.MouseDown += DesignTabStrip_MouseDown;
        }

        private void UnhookDesignContextMenuSurface(Control control)
        {
            if (!_contextMenuSurfaces.Remove(control))
            {
                return;
            }

            control.MouseUp -= DesignContextSurface_MouseUp;

            if (control is BeepDocumentTabStrip)
                control.MouseDown -= DesignTabStrip_MouseDown;
        }

        private static bool IsDesignContextMenuSurface(Control control)
            => control is BeepDocumentHost or BeepDocumentTabStrip || control.Parent is BeepDocumentHost;

        /// <summary>
        /// Left-click on a tab header selects the corresponding <see cref="BeepDocumentPanel"/>
        /// in the Visual Studio Properties window — identical to DevExpress XtraTabbedView behaviour.
        /// </summary>
        private void DesignTabStrip_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left
                || sender is not BeepDocumentTabStrip strip
                || Component is not BeepDocumentHost host)
                return;

            var hit = strip.HitTestTab(e.Location);
            if (!hit.Hit || hit.TabIndex < 0 || hit.TabIndex >= strip.Tabs.Count)
                return;

            // Ignore close-button / add-button / scroll-button clicks
            if (hit.IsCloseButton || hit.IsAddButton || hit.IsScrollLeft || hit.IsScrollRight || hit.IsOverflowButton)
                return;

            string tabId = strip.Tabs[hit.TabIndex].Id;
            if (string.IsNullOrWhiteSpace(tabId)) return;

            var panel = host.GetPanel(tabId);
            if (panel == null) return;

            // Route selection to the panel so the Properties window shows its properties
            GetSelectionService()?.SetSelectedComponents(
                new object[] { panel },
                SelectionTypes.Replace);
        }

        private void DesignContextSurface_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || Component is not BeepDocumentHost host)
            {
                return;
            }

            Control surface = sender as Control ?? host;
            if (surface is BeepDocumentTabStrip strip
                && strip.ActiveTabIndex >= 0
                && strip.ActiveTabIndex < strip.Tabs.Count)
            {
                string tabId = strip.Tabs[strip.ActiveTabIndex].Id;
                if (!string.IsNullOrWhiteSpace(tabId))
                {
                    host.SetActiveDocument(tabId);
                }
            }

            ShowDesignContextMenu(host, surface, e.Location);
        }

        private void ShowDesignContextMenu(BeepDocumentHost host, Control surface, Point location)
        {
            var menu = BuildDesignContextMenu(host);
            ApplyThemeToDesignMenu(host, menu);
            menu.Closed += (s, e) => menu.Dispose();
            menu.Show(surface, location);
        }

        private ContextMenuStrip BuildDesignContextMenu(BeepDocumentHost host)
        {
            var menu = new ContextMenuStrip { ShowImageMargin = false };
            bool hasActiveDocument = !string.IsNullOrWhiteSpace(host.ActiveDocumentId);
            DocumentDockState dockState = GetActiveDocumentDockState();
            bool isPinned = hasActiveDocument
                && FindDesignTimeDocument(host.DesignTimeDocuments, host.ActiveDocumentId!)?.IsPinned == true;

            if (hasActiveDocument)
            {
                string title = GetActiveDocumentTitle();
                menu.Items.Add(new ToolStripMenuItem($"Active Document: {title}") { Enabled = false });
                menu.Items.Add(new ToolStripMenuItem($"Dock State: {dockState}") { Enabled = false });
                menu.Items.Add(new ToolStripSeparator());
            }

            menu.Items.Add(CreateDesignMenuItem("Add Document", AddDesignTimeDocument));
            menu.Items.Add(CreateDesignMenuItem("Layout Assistant…", ShowLayoutAssistant));
            menu.Items.Add(CreateDesignMenuItem("Edit Design-Time Documents…", () =>
            {
                if (Component is BeepDocumentHost currentHost)
                {
                    EditDesignTimeDocuments(currentHost);
                }
            }));

            if (hasActiveDocument)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateDesignMenuItem("Rename Active Document…", RenameActiveDesignTimeDocumentWithPrompt));
                menu.Items.Add(CreateDesignMenuItem("Select Active Document Surface", SelectActiveDocumentSurface));
                menu.Items.Add(CreateDesignMenuItem("Close Active Document", CloseActiveDesignTimeDocument));
                menu.Items.Add(CreateDesignMenuItem(
                    dockState == DocumentDockState.Floating ? "Dock Back" : "Float",
                    dockState == DocumentDockState.Floating ? DockBackActiveDesignTimeDocument : FloatActiveDesignTimeDocument,
                    dockState is DocumentDockState.Docked or DocumentDockState.Floating));
                menu.Items.Add(CreateDesignMenuItem(
                    isPinned ? "Unpin Active Document" : "Pin Active Document",
                    () => SetActiveDocumentPinned(!isPinned),
                    dockState != DocumentDockState.None));

                if (dockState == DocumentDockState.AutoHide)
                {
                    menu.Items.Add(CreateDesignMenuItem("Restore Auto Hide", RestoreAutoHideActiveDesignTimeDocument));
                }

                if (dockState == DocumentDockState.Docked)
                {
                    menu.Items.Add(BuildAutoHideMenu());
                }
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(CreateDesignMenuItem("Split With New Document Right", () => CreateSplitDesignTimeDocument(horizontal: true)));
            menu.Items.Add(CreateDesignMenuItem("Split With New Document Down", () => CreateSplitDesignTimeDocument(horizontal: false)));
            menu.Items.Add(CreateDesignMenuItem("Merge All Groups", MergeAllDesignTimeGroups, host.Groups.Count > 1));
            menu.Items.Add(BuildLayoutPresetMenu());

            if (CanReopenLastClosedDesignTimeDocument)
            {
                menu.Items.Add(CreateDesignMenuItem("Reopen Last Closed", ReopenLastClosedDesignTimeDocument));
            }

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(CreateDesignMenuItem("View Layout Tree…", () => ShowLayoutTreeDialog(host)));
            menu.Items.Add(CreateDesignMenuItem("Set Group Tab Position…", () => EditGroupTabPositions(host), host.Groups.Count > 1));

            return menu;
        }

        private ToolStripMenuItem BuildAutoHideMenu()
        {
            var menuItem = new ToolStripMenuItem("Auto Hide");
            foreach (AutoHideSide side in Enum.GetValues(typeof(AutoHideSide)))
            {
                menuItem.DropDownItems.Add(CreateDesignMenuItem($"{GetAutoHideLabel(side)}", () => AutoHideActiveDesignTimeDocument(side)));
            }

            return menuItem;
        }

        private ToolStripMenuItem BuildLayoutPresetMenu()
        {
            var menuItem = new ToolStripMenuItem("Quick Layout Presets");
            foreach (LayoutPreset preset in GetLayoutPresetOrder())
            {
                menuItem.DropDownItems.Add(CreateDesignMenuItem(GetLayoutPresetDisplayName(preset), () => ApplyDesignTimeLayoutPreset(preset)));
            }

            return menuItem;
        }

        private static ToolStripMenuItem CreateDesignMenuItem(string text, Action action, bool enabled = true)
        {
            var menuItem = new ToolStripMenuItem(text) { Enabled = enabled };
            menuItem.Click += (s, e) => action();
            return menuItem;
        }

        private void ApplyThemeToDesignMenu(BeepDocumentHost host, ContextMenuStrip menu)
        {
            menu.BackColor = host.BackColor;
            menu.ForeColor = host.ForeColor;
            menu.Renderer = new DocumentHostDesignerMenuRenderer(host.BackColor, host.ForeColor);

            foreach (ToolStripItem item in menu.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.DropDownOpening += (s, e) =>
                    {
                        if (menuItem.DropDown is ContextMenuStrip dropDown)
                        {
                            ApplyThemeToDesignMenu(host, dropDown);
                        }
                    };
                }
            }
        }

        private void ApplyLayoutAssistant(BeepDocumentHost host,
                                          Collection<DocumentDescriptor> docs,
                                          LayoutPreset preset,
                                          IReadOnlyList<DocumentLayoutAssistantItem> documents)
        {
            ApplyAssistantDocumentSpecifications(host, docs, documents);
            SyncHostWithDesignTimeDocuments(host, docs);
            ApplyDesignTimeLayoutPresetCore(host, docs, preset, selectSurface: false);

            BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: true);
            SyncDesignerSelection((object?)panel ?? host);
        }

        private void ApplyAssistantDocumentSpecifications(BeepDocumentHost host,
                                                          Collection<DocumentDescriptor> docs,
                                                          IReadOnlyList<DocumentLayoutAssistantItem> documents)
        {
            int requiredCount = Math.Max(1, documents.Count);
            while (docs.Count < requiredCount)
            {
                docs.Add(CreateNextDesignTimeDocumentDescriptor(host, docs));
            }

            for (int index = 0; index < requiredCount; index++)
            {
                DocumentLayoutAssistantItem specification = documents[index];
                DocumentDescriptor descriptor = docs[index];
                descriptor.Title = NormalizeDocumentTitle(specification.Title, index + 1);
                descriptor.InitialContent = specification.InitialContent;
            }

            for (int index = docs.Count - 1; index >= requiredCount; index--)
            {
                DocumentDescriptor descriptor = docs[index];
                _designTimeClosedDocuments.Push(CloneDescriptor(descriptor));
                CloseDesignTimeDocument(host, descriptor.Id);
                docs.RemoveAt(index);
            }
        }

        private void ApplyDesignTimeLayoutPresetCore(BeepDocumentHost host,
                                                     Collection<DocumentDescriptor> docs,
                                                     LayoutPreset preset,
                                                     bool selectSurface)
        {
            switch (preset)
            {
                case LayoutPreset.Single:
                    EnsureDesignDocumentCount(host, docs, 1);
                    host.MergeAllGroups();
                    break;

                case LayoutPreset.SideBySide:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    break;

                case LayoutPreset.Stacked:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("stacked", host);
                    break;

                case LayoutPreset.ThreeWay:
                    EnsureDesignDocumentCount(host, docs, 3);
                    host.TemplateLibrary.ApplyTemplate("three-way", host);
                    break;

                case LayoutPreset.FourUp:
                    EnsureDesignDocumentCount(host, docs, 4);
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    break;

                case LayoutPreset.ThreeWayNested:
                    EnsureDesignDocumentCount(host, docs, 2);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    EnsureDesignDocumentCount(host, docs, 3);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: false, selectSurface: false);
                    break;

                case LayoutPreset.ThreeColumn:
                    EnsureDesignDocumentCount(host, docs, 3);
                    host.TemplateLibrary.ApplyTemplate("side-by-side", host);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false);
                    break;

                case LayoutPreset.FiveWay:
                    EnsureDesignDocumentCount(host, docs, 4);
                    host.TemplateLibrary.ApplyTemplate("four-up", host);
                    EnsureDesignDocumentCount(host, docs, 5);
                    CreateSplitDesignTimeDocumentInternal(host, docs, horizontal: true, selectSurface: false);
                    break;
            }

            BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: selectSurface);
            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }
        }

        private void SyncHostWithDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs)
        {
            var desiredDescriptors = docs
                .Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id))
                .ToList();
            var desiredIds = new HashSet<string>(desiredDescriptors.Select(descriptor => descriptor.Id), StringComparer.OrdinalIgnoreCase);

            foreach (string openDocumentId in GetOpenDocumentIds(host))
            {
                if (!desiredIds.Contains(openDocumentId))
                {
                    CloseDesignTimeDocument(host, openDocumentId);
                }
            }

            foreach (DocumentDescriptor descriptor in desiredDescriptors)
            {
                EnsureDesignTimeDocumentOpen(host, descriptor, activate: false);
                ApplyDescriptorToOpenDocument(host, descriptor);
            }

            if (string.IsNullOrWhiteSpace(host.ActiveDocumentId) && desiredDescriptors.Count > 0)
            {
                host.SetActiveDocument(desiredDescriptors[0].Id);
            }
        }

        private void ApplyDescriptorToOpenDocument(BeepDocumentHost host, DocumentDescriptor descriptor)
        {
            if (host.GetPanel(descriptor.Id) is BeepDocumentPanel panel)
            {
                panel.DocumentTitle = descriptor.Title;
                panel.IconPath = descriptor.IconPath;
                panel.CanClose = descriptor.CanClose;
                panel.IsModified = descriptor.IsModified;
            }

            foreach (BeepDocumentGroup group in host.Groups)
            {
                BeepDocumentTab? tab = group.TabStrip.FindTabById(descriptor.Id);
                if (tab == null)
                {
                    continue;
                }

                tab.Title = descriptor.Title;
                tab.IconPath = descriptor.IconPath;
                tab.CanClose = descriptor.CanClose;
                tab.IsModified = descriptor.IsModified;
                tab.IsPinned = descriptor.IsPinned;
                group.TabStrip.Invalidate();
            }

            if (host.GetDocumentDockState(descriptor.Id) == DocumentDockState.Docked)
            {
                host.PinDocument(descriptor.Id, descriptor.IsPinned);
            }
        }

        private bool CloseDesignTimeDocument(BeepDocumentHost host, string documentId)
        {
            switch (host.GetDocumentDockState(documentId))
            {
                case DocumentDockState.Floating:
                    host.DockBackDocument(documentId);
                    break;
                case DocumentDockState.AutoHide:
                    host.RestoreAutoHideDocument(documentId);
                    break;
            }

            return host.CloseDocument(documentId);
        }

        private static IReadOnlyList<string> GetOpenDocumentIds(BeepDocumentHost host)
        {
            try
            {
                using JsonDocument json = JsonDocument.Parse(host.SaveLayout());
                var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                CollectDocumentIds(json.RootElement, ids);
                return ids.ToList();
            }
            catch
            {
                return host.Groups
                    .SelectMany(group => group.DocumentIds)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        private static void CollectDocumentIds(JsonElement element, HashSet<string> ids)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        if ((property.NameEquals("id") || property.NameEquals("documentId"))
                            && property.Value.ValueKind == JsonValueKind.String)
                        {
                            string? value = property.Value.GetString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                ids.Add(value);
                            }
                        }

                        CollectDocumentIds(property.Value, ids);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        CollectDocumentIds(item, ids);
                    }
                    break;
            }
        }

        private static string CaptureDesignTimeLayout(BeepDocumentHost host, Collection<DocumentDescriptor> docs)
        {
            if (!docs.Any(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id)))
            {
                return string.Empty;
            }

            try
            {
                return host.SaveLayout();
            }
            catch
            {
                return host.DesignTimeLayoutJson;
            }
        }

        private static string NormalizeDocumentTitle(string? title, int ordinal)
            => string.IsNullOrWhiteSpace(title) ? $"Document {ordinal}" : title.Trim();

        private static IReadOnlyList<LayoutPreset> GetLayoutPresetOrder()
            => new[]
            {
                LayoutPreset.Single,
                LayoutPreset.SideBySide,
                LayoutPreset.Stacked,
                LayoutPreset.ThreeWay,
                LayoutPreset.ThreeWayNested,
                LayoutPreset.FourUp,
                LayoutPreset.ThreeColumn,
                LayoutPreset.FiveWay
            };

        private static string GetLayoutPresetDisplayName(LayoutPreset preset)
            => preset switch
            {
                LayoutPreset.SideBySide => "Side-by-Side",
                LayoutPreset.ThreeWay => "Three-Way",
                LayoutPreset.ThreeWayNested => "Three-Way Nested",
                LayoutPreset.FourUp => "Four-Up",
                LayoutPreset.ThreeColumn => "Three Column",
                LayoutPreset.FiveWay => "Five-Way",
                _ => preset.ToString()
            };

        private static string GetAutoHideLabel(AutoHideSide side)
            => side switch
            {
                AutoHideSide.Left => "Left",
                AutoHideSide.Right => "Right",
                AutoHideSide.Top => "Top",
                AutoHideSide.Bottom => "Bottom",
                _ => side.ToString()
            };

        private BeepDocumentPanel? EnsureDesignSurfaceForDroppedContent()
        {
            if (Component is not BeepDocumentHost host)
            {
                return null;
            }

            if (host.ActivePanel != null)
            {
                return host.ActivePanel;
            }

            BeepDocumentPanel? panel = null;
            ExecuteDesignTimeDocumentsAction("Create Document For Dropped Control", (h, docs) =>
            {
                panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: false);
            });

            return (Component as BeepDocumentHost)?.ActivePanel ?? panel;
        }

        private BeepDocumentPanel? EnsureActiveDesignDocumentSurface(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool selectSurface)
        {
            if (host.ActivePanel != null)
            {
                if (selectSurface)
                {
                    SyncDesignerSelection(host.ActivePanel);
                }

                return host.ActivePanel;
            }

            if (docs.Count == 0)
            {
                return AddDesignTimeDocumentInternal(host, docs, activate: true, selectSurface: selectSurface);
            }

            host.ApplyDesignTimeDocuments();

            DocumentDescriptor? descriptor = docs.FirstOrDefault(doc => !string.IsNullOrWhiteSpace(doc.Id));
            if (descriptor == null)
            {
                return AddDesignTimeDocumentInternal(host, docs, activate: true, selectSurface: selectSurface);
            }

            BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(host, descriptor, activate: true);
            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }

            return panel;
        }

        private void EnsureDesignDocumentCount(BeepDocumentHost host, Collection<DocumentDescriptor> docs, int count)
        {
            while (docs.Count(doc => !string.IsNullOrWhiteSpace(doc.Id)) < count)
            {
                AddDesignTimeDocumentInternal(host, docs, activate: false, selectSurface: false);
            }
        }

        private BeepDocumentPanel? AddDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool activate, bool selectSurface)
        {
            DocumentDescriptor descriptor = CreateNextDesignTimeDocumentDescriptor(host, docs);
            docs.Add(descriptor);

            BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(host, descriptor, activate);
            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }

            return panel;
        }

        private BeepDocumentPanel? CreateSplitDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool horizontal, bool selectSurface)
        {
            BeepDocumentPanel? anchorPanel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: false);
            if (anchorPanel == null)
            {
                return null;
            }

            DocumentDescriptor descriptor = CreateNextDesignTimeDocumentDescriptor(host, docs);
            docs.Add(descriptor);

            BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(host, descriptor, activate: true);
            if (panel == null)
            {
                return null;
            }

            if (horizontal)
            {
                host.SplitDocumentHorizontal(descriptor.Id);
            }
            else
            {
                host.SplitDocumentVertical(descriptor.Id);
            }

            host.SetActiveDocument(descriptor.Id);

            if (selectSurface)
            {
                SyncDesignerSelection(panel);
            }

            return panel;
        }

        private BeepDocumentPanel? EnsureDesignTimeDocumentOpen(BeepDocumentHost host, DocumentDescriptor descriptor, bool activate)
        {
            if (host.GetPanel(descriptor.Id) == null)
            {
                host.ApplyDesignTimeDocuments();
            }

            if (activate)
            {
                host.SetActiveDocument(descriptor.Id);
            }

            return host.GetPanel(descriptor.Id);
        }

        private DocumentDescriptor CreateNextDesignTimeDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs)
        {
            var usedIds = new HashSet<string>(
                docs.Where(doc => !string.IsNullOrWhiteSpace(doc.Id)).Select(doc => doc.Id),
                StringComparer.OrdinalIgnoreCase);

            int index = 1;
            string id;
            do
            {
                id = $"doc{index}";
                index++;
            }
            while (usedIds.Contains(id) || host.GetPanel(id) != null);

            return new DocumentDescriptor
            {
                Id = id,
                Title = $"Document {index - 1}",
                InitialContent = DocumentInitialContent.Empty
            };
        }

        private static DocumentDescriptor? FindDesignTimeDocument(IEnumerable<DocumentDescriptor> docs, string documentId)
            => docs.FirstOrDefault(doc => string.Equals(doc.Id, documentId, StringComparison.OrdinalIgnoreCase));

        private DocumentDescriptor AddDescriptorSnapshotToDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor descriptor = CaptureDocumentDescriptor(host, docs, documentId);
            docs.Add(descriptor);
            return descriptor;
        }

        private DocumentDescriptor CaptureDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor? existing = FindDesignTimeDocument(docs, documentId);
            if (existing != null)
            {
                return CloneDescriptor(existing);
            }

            BeepDocumentPanel? panel = host.GetPanel(documentId);
            return new DocumentDescriptor
            {
                Id = documentId,
                Title = panel?.DocumentTitle ?? documentId,
                IconPath = panel?.IconPath,
                IsModified = panel?.IsModified ?? false,
                CanClose = panel?.CanClose ?? true,
                InitialContent = DocumentInitialContent.Empty
            };
        }

        private static DocumentDescriptor CloneDescriptor(DocumentDescriptor source)
        {
            var clone = new DocumentDescriptor
            {
                Id = source.Id,
                Title = source.Title,
                IconPath = source.IconPath,
                IsModified = source.IsModified,
                IsPinned = source.IsPinned,
                CanClose = source.CanClose,
                Category = source.Category,
                TooltipText = source.TooltipText,
                Tag = source.Tag,
                BadgeText = source.BadgeText,
                BadgeColor = source.BadgeColor,
                TabColor = source.TabColor,
                AccentColor = source.AccentColor,
                InitialContent = source.InitialContent
            };

            foreach (KeyValuePair<string, string> pair in source.CustomData)
            {
                clone.CustomData[pair.Key] = pair.Value;
            }

            return clone;
        }

        // ─────────────────────────────────────────────────────────────────────
        // 17.1 — Docking guide adorner  (drag-and-drop compass in the designer)
        // ─────────────────────────────────────────────────────────────────────

        private bool      _dragActive;
        private DockZone  _dragZone = DockZone.None;

        private enum DockZone { None, Center, Left, Right, Top, Bottom }

        /// <summary>Called at the end of Initialize() to opt into DragDrop on the host.</summary>
        private void InitializeDockAdorner()
        {
            if (Component is BeepDocumentHost host)
                host.AllowDrop = true;   // ensure the host accepts drops at design-time
        }

        // ── drag overrides ────────────────────────────────────────────────────

        protected override void OnDragEnter(DragEventArgs de)
        {
            base.OnDragEnter(de);

            if (IsToolboxDrag(de))
            {
                _dragActive = false;
                _dragZone = DockZone.None;
                InvalidateHost();
                return;
            }

            _dragActive = true;
            _dragZone   = DockZone.None;
            de.Effect   = DragDropEffects.Move;
            InvalidateHost();
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            base.OnDragOver(de);

            if (IsToolboxDrag(de))
            {
                _dragActive = false;
                _dragZone = DockZone.None;
                InvalidateHost();
                return;
            }

            if (Component is BeepDocumentHost host)
            {
                var screenPt = new Point(de.X, de.Y);
                var clientPt = host.PointToClient(screenPt);
                _dragZone   = HitTestCompass(host.ClientRectangle, clientPt);
                de.Effect   = _dragZone == DockZone.None ? DragDropEffects.None : DragDropEffects.Move;
                InvalidateHost();
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            _dragActive = false;
            _dragZone   = DockZone.None;
            InvalidateHost();
        }

        private static bool IsToolboxDrag(DragEventArgs de)
        {
            try
            {
                return de.Data
                    .GetFormats()
                    .Any(format => format.IndexOf("Toolbox", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Determines which compass zone contains <paramref name="pt"/>.</summary>
        private static DockZone HitTestCompass(Rectangle hostRc, Point pt)
        {
            const int ArrowSize = 28;
            int cx = hostRc.Width  / 2;
            int cy = hostRc.Height / 2;

            if (new Rectangle(cx - ArrowSize,     cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Center;
            if (new Rectangle(cx - ArrowSize * 3, cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Left;
            if (new Rectangle(cx + ArrowSize,     cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Right;
            if (new Rectangle(cx - ArrowSize,     cy - ArrowSize * 3, ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Top;
            if (new Rectangle(cx - ArrowSize,     cy + ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Bottom;
            return DockZone.None;
        }

        private void InvalidateHost()
        {
            try { (Component as System.Windows.Forms.Control)?.Invalidate(); }
            catch { }
        }

        /// <summary>Paints the docking compass when a drag is in progress.</summary>
        private void PaintDockCompass(Graphics g, BeepDocumentHost host)
        {
            if (!_dragActive) return;

            var rc = host.ClientRectangle;
            int cx = rc.Width  / 2;
            int cy = rc.Height / 2;
            const int Sz = 28;

            // Semi-transparent overlay
            using var dimBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0));
            g.FillRectangle(dimBrush, rc);

            // Draw split preview if a zone is active
            if (_dragZone != DockZone.None && _dragZone != DockZone.Center)
            {
                DrawSplitPreview(g, rc, _dragZone);
            }

            // Five compass zones: Center, Left, Right, Top, Bottom
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz,     Sz * 2, Sz * 2), "●", DockZone.Center);
            DrawCompassArrow(g, new Rectangle(cx - Sz * 3, cy - Sz,     Sz * 2, Sz * 2), "◄", DockZone.Left);
            DrawCompassArrow(g, new Rectangle(cx + Sz,     cy - Sz,     Sz * 2, Sz * 2), "►", DockZone.Right);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz * 3, Sz * 2, Sz * 2), "▲", DockZone.Top);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy + Sz,     Sz * 2, Sz * 2), "▼", DockZone.Bottom);

            // Draw zone labels below compass
            DrawCompassLabels(g, rc);
        }

        private void DrawSplitPreview(Graphics g, Rectangle hostRc, DockZone zone)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int splitX = hostRc.X + hostRc.Width / 2;
            int splitY = hostRc.Y + hostRc.Height / 2;
            const int PreviewMargin = 40;
            var previewRc = new Rectangle(
                hostRc.X + PreviewMargin,
                hostRc.Y + PreviewMargin,
                hostRc.Width - (PreviewMargin * 2),
                hostRc.Height - (PreviewMargin * 2));

            using var previewPen = new Pen(Color.FromArgb(200, 100, 180, 255), 2f);
            using var fillBrush = new SolidBrush(Color.FromArgb(40, 100, 180, 255));

            switch (zone)
            {
                case DockZone.Left:
                    // Vertical split: show left pane as active
                    int splitLineX = previewRc.X + previewRc.Width / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y, previewRc.Width / 2, previewRc.Height);
                    g.DrawLine(previewPen, splitLineX, previewRc.Y, splitLineX, previewRc.Y + previewRc.Height);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Left Split Preview", previewRc);
                    break;

                case DockZone.Right:
                    // Vertical split: show right pane as active
                    splitLineX = previewRc.X + previewRc.Width / 2;
                    g.FillRectangle(fillBrush, previewRc.X + previewRc.Width / 2, previewRc.Y, previewRc.Width / 2, previewRc.Height);
                    g.DrawLine(previewPen, splitLineX, previewRc.Y, splitLineX, previewRc.Y + previewRc.Height);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Right Split Preview", previewRc);
                    break;

                case DockZone.Top:
                    // Horizontal split: show top pane as active
                    int splitLineY = previewRc.Y + previewRc.Height / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y, previewRc.Width, previewRc.Height / 2);
                    g.DrawLine(previewPen, previewRc.X, splitLineY, previewRc.X + previewRc.Width, splitLineY);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Top Split Preview", previewRc);
                    break;

                case DockZone.Bottom:
                    // Horizontal split: show bottom pane as active
                    splitLineY = previewRc.Y + previewRc.Height / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y + previewRc.Height / 2, previewRc.Width, previewRc.Height / 2);
                    g.DrawLine(previewPen, previewRc.X, splitLineY, previewRc.X + previewRc.Width, splitLineY);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Bottom Split Preview", previewRc);
                    break;
            }
        }

        private static void DrawPreviewLabel(Graphics g, string text, Rectangle previewRc)
        {
            using var font = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var labelRc = new Rectangle(previewRc.X, previewRc.Y + 8, previewRc.Width, 24);
            g.DrawString(text, font, brush, labelRc, sf);
        }

        private void DrawCompassLabels(Graphics g, Rectangle hostRc)
        {
            int cx = hostRc.X + hostRc.Width / 2;
            int cy = hostRc.Y + hostRc.Height / 2;
            const int Distance = 100;

            using var font = new Font("Segoe UI", 9f);
            using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Only show labels when hovering over compass zones
            if (_dragZone != DockZone.None)
            {
                string label = _dragZone switch
                {
                    DockZone.Center => "Drop here to add to group",
                    DockZone.Left => "Split Left",
                    DockZone.Right => "Split Right",
                    DockZone.Top => "Split Top",
                    DockZone.Bottom => "Split Bottom",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(label))
                {
                    var labelRc = new Rectangle(cx - 80, cy + Distance, 160, 24);
                    g.DrawString(label, font, brush, labelRc, sf);
                }
            }
        }

        private void DrawCompassArrow(Graphics g, Rectangle rc, string symbol, DockZone zone)
        {
            bool active = zone == _dragZone;

            using var bgBrush = new SolidBrush(active
                ? Color.FromArgb(220, 0, 120, 215)
                : Color.FromArgb(160, 40, 40, 40));
            using var fg  = new SolidBrush(Color.White);
            using var pen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f);
            using var fnt = new Font("Segoe UI", 13f, FontStyle.Bold);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(bgBrush, rc);
            g.DrawEllipse(pen, rc);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(symbol, fnt, fg, rc, sf);
        }
    }

    internal sealed class DocumentHostDesignerMenuRenderer : ToolStripProfessionalRenderer
    {
        public DocumentHostDesignerMenuRenderer(Color backColor, Color foreColor)
            : base(new DocumentHostDesignerColorTable(backColor, foreColor))
        {
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Enabled
                ? e.Item.ForeColor
                : Color.FromArgb(140, e.Item.ForeColor);
            base.OnRenderItemText(e);
        }
    }

    internal sealed class DocumentHostDesignerColorTable : ProfessionalColorTable
    {
        private readonly Color _background;
        private readonly Color _foreground;
        private readonly Color _highlight;

        public DocumentHostDesignerColorTable(Color background, Color foreground)
        {
            _background = background;
            _foreground = foreground;
            _highlight = Blend(background, SystemColors.Highlight, 0.22f);
        }

        public override Color MenuBorder => Blend(_background, _foreground, 0.15f);
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuItemSelected => _highlight;
        public override Color MenuItemSelectedGradientBegin => _highlight;
        public override Color MenuItemSelectedGradientEnd => _highlight;
        public override Color ToolStripDropDownBackground => _background;
        public override Color MenuStripGradientBegin => _background;
        public override Color MenuStripGradientEnd => _background;
        public override Color SeparatorDark => Blend(_background, _foreground, 0.18f);
        public override Color SeparatorLight => _background;
        public override Color ImageMarginGradientBegin => _background;
        public override Color ImageMarginGradientMiddle => _background;
        public override Color ImageMarginGradientEnd => _background;

        private static Color Blend(Color a, Color b, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            return Color.FromArgb(
                255,
                (int)Math.Round(a.R + ((b.R - a.R) * amount)),
                (int)Math.Round(a.G + ((b.G - a.G) * amount)),
                (int)Math.Round(a.B + ((b.B - a.B) * amount)));
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper: ITypeDescriptorContext for CollectionEditor invocation
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Minimal <see cref="ITypeDescriptorContext"/> implementation required to
    /// invoke a <see cref="System.ComponentModel.Design.CollectionEditor"/> from
    /// the designer verb / smart-tag action outside of the Properties grid.
    /// </summary>
    internal sealed class DesignTimeDocumentsEditorContext
        : ITypeDescriptorContext, IServiceProvider
    {
        private readonly object                _instance;
        private readonly PropertyDescriptor    _property;
        private readonly IServiceProvider      _services;

        public DesignTimeDocumentsEditorContext(
            object instance, PropertyDescriptor property, IServiceProvider services)
        {
            _instance = instance;
            _property = property;
            _services = services;
        }

        // ITypeDescriptorContext
        public IContainer?         Container  => (_services.GetService(typeof(IDesignerHost)) as IDesignerHost)?.Container;
        public object              Instance   => _instance;
        public PropertyDescriptor  PropertyDescriptor => _property;

        public bool OnComponentChanging()
        {
            var svc = _services.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            try { svc?.OnComponentChanging(_instance, _property); return true; }
            catch { return false; }
        }

        public void OnComponentChanged()
        {
            var svc = _services.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            svc?.OnComponentChanged(_instance, _property, null, _property.GetValue(_instance));
        }

        // IServiceProvider
        public object? GetService(Type serviceType) => _services.GetService(serviceType);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Sprint 19: Per-group tab position editor dialog
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class GroupTabPositionDialog : Form
    {
        public Dictionary<string, TabStripPosition> ChangedPositions { get; } = new();

        private readonly Dictionary<string, ComboBox> _positionCombos = new();

        public GroupTabPositionDialog(BeepDocumentHost host)
        {
            Text = "Set Group Tab Positions";
            Size = new Size(420, 160 + host.Groups.Count * 36);
            MinimumSize = new Size(380, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = SystemColors.Window;
            Font = new Font("Segoe UI", 9f);

            var titleLabel = new Label
            {
                Text = "Per-group tab strip position:",
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(8, 4, 0, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(12, 8, 12, 0)
            };

            foreach (var grp in host.Groups)
            {
                var row = new Panel { Height = 32, Width = 360 };
                var label = new Label
                {
                    Text = $"Group {grp.GroupId.Substring(0, 8)}{(grp.IsPrimary ? " (Primary)" : "")}:",
                    Location = new Point(0, 6),
                    Width = 180,
                    TextAlign = ContentAlignment.MiddleRight
                };
                var combo = new ComboBox
                {
                    Location = new Point(190, 4),
                    Width = 150,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                combo.Items.AddRange(Enum.GetNames<TabStripPosition>());
                combo.SelectedItem = grp.TabPosition.ToString();
                _positionCombos[grp.GroupId] = combo;

                row.Controls.Add(label);
                row.Controls.Add(combo);
                panel.Controls.Add(row);
            }

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var okBtn = new Button { Text = "OK", Width = 80, Height = 28, DialogResult = DialogResult.OK };
            var cancelBtn = new Button { Text = "Cancel", Width = 80, Height = 28, DialogResult = DialogResult.Cancel };
            okBtn.Location = new Point(btnPanel.Width - 176, 10);
            cancelBtn.Location = new Point(btnPanel.Width - 88, 10);
            okBtn.Click += (s, e) => CollectChanges(host);
            btnPanel.Controls.Add(okBtn);
            btnPanel.Controls.Add(cancelBtn);

            AcceptButton = okBtn;
            CancelButton = cancelBtn;

            Controls.Add(panel);
            Controls.Add(btnPanel);
            Controls.Add(titleLabel);
        }

        private void CollectChanges(BeepDocumentHost host)
        {
            foreach (var grp in host.Groups)
            {
                if (_positionCombos.TryGetValue(grp.GroupId, out var combo)
                    && Enum.TryParse<TabStripPosition>(combo.SelectedItem?.ToString(), out var pos)
                    && pos != grp.TabPosition)
                {
                    ChangedPositions[grp.GroupId] = pos;
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Sprint 19: Layout tree viewer dialog
    // ─────────────────────────────────────────────────────────────────────────

    internal sealed class LayoutTreeDialog : Form
    {
        public LayoutTreeDialog(BeepDocumentHost host)
        {
            Text = "Layout Tree Structure";
            Size = new Size(480, 360);
            MinimumSize = new Size(380, 280);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = SystemColors.Window;
            Font = new Font("Segoe UI", 9f);

            var titleLabel = new Label
            {
                Text = $"Layout Tree ({host.Groups.Count} groups)",
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(8, 4, 0, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var treeBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9f),
                BackColor = SystemColors.ControlLightLight,
                Text = FormatLayoutTree(host.LayoutRoot)
            };

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };
            var closeBtn = new Button { Text = "Close", Width = 80, Height = 28, DialogResult = DialogResult.OK };
            closeBtn.Location = new Point(btnPanel.Width - 92, 10);
            btnPanel.Controls.Add(closeBtn);
            AcceptButton = closeBtn;

            Controls.Add(treeBox);
            Controls.Add(btnPanel);
            Controls.Add(titleLabel);
        }

        private static string FormatLayoutTree(ILayoutNode node, int depth = 0)
        {
            var indent = new string(' ', depth * 3);
            if (node is GroupLayoutNode g)
            {
                return $"{indent}[Group] {g.DocumentIds.Count} docs{(g.SelectedDocumentId != null ? $" (active: {g.SelectedDocumentId})" : "")}";
            }
            if (node is SplitLayoutNode s)
            {
                var orient = s.Orientation == Orientation.Horizontal ? "Horizontal" : "Vertical";
                return $"{indent}[Split] {orient} ({s.Ratio:P0})\n{FormatLayoutTree(s.First, depth + 1)}\n{FormatLayoutTree(s.Second, depth + 1)}";
            }
            return $"{indent}[Unknown]";
        }
    }
}
