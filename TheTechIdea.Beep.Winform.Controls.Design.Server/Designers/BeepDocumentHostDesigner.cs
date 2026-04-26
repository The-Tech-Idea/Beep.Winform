// BeepDocumentHostDesigner.cs
// Design-time designer for BeepDocumentHost.
// Smart-tag actions are provided by DocumentHostActionList (ActionLists/).
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Behaviors;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;
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
        // ── Internal tab-strip / content-area panel names must not be moved ──
        private static readonly HashSet<string> _lockedChildTypes
            = new HashSet<string>(StringComparer.Ordinal)
        {
            "BeepDocumentTabStrip",
            nameof(Panel)           // the _contentArea
        };

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

        public override DesignerVerbCollection Verbs
        {
            get
            {
                _verbs ??= new DesignerVerbCollection
                {
                    new DesignerVerb("Add Document", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host)
                        {
                            int    idx   = host.DocumentCount + 1;
                            string id    = $"doc{idx}";
                            string title = $"Document {idx}";
                            ExecuteAction($"Add Document '{title}'",
                                h => h.AddDocument(id, title, activate: true));
                        }
                    }),
                    new DesignerVerb("Close Active Document", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host
                            && !string.IsNullOrEmpty(host.ActiveDocumentId))
                        {
                            var id = host.ActiveDocumentId!;
                            ExecuteAction($"Close Document '{id}'",
                                h => h.CloseDocument(id));
                        }
                    }),
                    new DesignerVerb("Split Horizontal \u2194", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host
                            && !string.IsNullOrEmpty(host.ActiveDocumentId))
                        {
                            var id = host.ActiveDocumentId!;
                            ExecuteAction("Split Horizontal",
                                h => h.SplitDocumentHorizontal(id));
                        }
                    }),
                    new DesignerVerb("Split Vertical \u2195", (s, e) =>
                    {
                        if (Component is BeepDocumentHost host
                            && !string.IsNullOrEmpty(host.ActiveDocumentId))
                        {
                            var id = host.ActiveDocumentId!;
                            ExecuteAction("Split Vertical",
                                h => h.SplitDocumentVertical(id));
                        }
                    }),
                    new DesignerVerb("Merge All Groups", (s, e) =>
                    {
                        ExecuteAction("Merge All Groups",
                            h => h.MergeAllGroups());
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
                };
                return _verbs;
            }
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
                foreach (Control child in host.Controls)
                    LockChild(child);

                // When the control gets its handle, force the designer surface
                // to recalculate glyph adorners (ensures snap points are fresh)
                host.HandleCreated += (s, e) =>
                {
                    try
                    {
                        var selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
                        if (selSvc?.PrimarySelection == host)
                        {
                            selSvc.SetSelectedComponents(null);
                            selSvc.SetSelectedComponents(new object[] { host });
                        }
                    }
                    catch { /* safe at design-time */ }
                };
            }

            // Sprint 17.1: register the docking-guide compass adorner
            InitializeDockAdorner();
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
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            // Hide verbose base-Panel plumbing that is not relevant to the
            // BeepDocumentHost abstraction.
            string[] hiddenProps =
            {
                "AutoScroll",
                "AutoScrollMargin",
                "AutoScrollMinSize",
                "AutoScrollOffset",
                "AutoScrollPosition",
                "BorderStyle",
                "HorizontalScroll",
                "VerticalScroll",
                "Padding",
                "AutoSize",
                "AutoSizeMode",
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
            if (Component is BeepDocumentHost host)
            {
                int    idx   = host.DocumentCount + 1;
                string id    = $"doc{idx}";
                string title = $"Document {idx}";
                ExecuteAction($"Add Document '{title}'",
                    h => h.AddDocument(id, title, activate: true));
            }
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
            const string Line2 = "Double-click to add a document";
            const string Line3 = "Drag to split  |  Smart-Tag (►) for all options  |  Nested splits supported";

            using var titleFont = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var hintFont  = new Font("Segoe UI",  8f);
            using var titleBrush = new SolidBrush(SystemColors.ControlText);
            using var hintBrush  = new SolidBrush(SystemColors.GrayText);

            var sf = new StringFormat
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

            var newVal = editor.EditValue(ctx, ctx, current);
            if (!ReferenceEquals(newVal, current))
                SetProperty("DesignTimeDocuments", newVal);
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

            ExecuteAction($"Apply Layout Preset: {dlg.SelectedPreset}", h =>
            {
                switch (dlg.SelectedPreset)
                {
                    case LayoutPreset.SideBySide:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.Stacked:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.ThreeWay:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.ThreeWayNested:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.FourUp:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.ThreeColumn:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    case LayoutPreset.FiveWay:
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentVertical(h.ActiveDocumentId);
                        if (!string.IsNullOrEmpty(h.ActiveDocumentId))
                            h.SplitDocumentHorizontal(h.ActiveDocumentId);
                        break;
                    default: // Single — merge back
                        h.MergeAllGroups();
                        break;
                }
            });
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

            var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            var changeSvc    = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

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
            _dragActive = true;
            _dragZone   = DockZone.None;
            de.Effect   = DragDropEffects.Move;
            InvalidateHost();
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            base.OnDragOver(de);

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

        protected override void OnDragDrop(DragEventArgs de)
        {
            base.OnDragDrop(de);

            if (Component is BeepDocumentHost host)
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

            // Five compass zones: Center, Left, Right, Top, Bottom
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz,     Sz * 2, Sz * 2), "●", DockZone.Center);
            DrawCompassArrow(g, new Rectangle(cx - Sz * 3, cy - Sz,     Sz * 2, Sz * 2), "◄", DockZone.Left);
            DrawCompassArrow(g, new Rectangle(cx + Sz,     cy - Sz,     Sz * 2, Sz * 2), "►", DockZone.Right);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz * 3, Sz * 2, Sz * 2), "▲", DockZone.Top);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy + Sz,     Sz * 2, Sz * 2), "▼", DockZone.Bottom);
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
