// BeepDockManagerDesigner.cs
// Design-time support for BeepDockManager.
// Provides:
//   • Designer verb "Edit Dock Panels…" — opens the DockPanelDescriptor collection editor.
//   • Designer verb "Auto-pair with DocumentHost" — finds the first BeepDocumentHost on the
//     parent form and assigns it to BeepDockManager.Host.
//   • DesignerActionList smart-tag — shortcut Add Panel commands per edge, plus a Host link.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Designer for <see cref="BeepDockManager"/> — adds verbs and a smart-tag list.
    /// </summary>
    internal sealed class BeepDockManagerDesigner : ComponentDesigner
    {
        private DesignerActionListCollection? _actionLists;

        // ─────────────────────────────────────────────────────────────────────
        // Verbs
        // ─────────────────────────────────────────────────────────────────────

        public override DesignerVerbCollection Verbs { get; } = new DesignerVerbCollection();

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            Verbs.Add(new DesignerVerb("Edit Dock Panels…",    OnEditPanels));
            Verbs.Add(new DesignerVerb("Auto-pair with DocumentHost", OnAutoPair));
        }

        private void OnEditPanels(object? sender, EventArgs e)
        {
            var mgr = (BeepDockManager)Component;

            var propDescriptor = TypeDescriptor.GetProperties(mgr)[nameof(BeepDockManager.Panels)];
            if (propDescriptor == null) return;

            var editor = propDescriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
            if (editor == null) return;

            var host = GetService<IDesignerHost>();
            var ctx  = new DesignerTypeDescriptorContext(host, propDescriptor, mgr);
            editor.EditValue(ctx, ctx, propDescriptor.GetValue(mgr));
        }

        // Minimal ITypeDescriptorContext + IServiceProvider bridge for UITypeEditor.EditValue.
        private sealed class DesignerTypeDescriptorContext : ITypeDescriptorContext, IServiceProvider
        {
            private readonly IDesignerHost?       _host;
            private readonly PropertyDescriptor   _prop;
            private readonly object               _instance;

            internal DesignerTypeDescriptorContext(IDesignerHost? host, PropertyDescriptor prop, object instance)
            {
                _host     = host;
                _prop     = prop;
                _instance = instance;
            }

            public IContainer?         Container           => _host?.Container;
            public object              Instance            => _instance;
            public PropertyDescriptor  PropertyDescriptor  => _prop;
            public void                OnComponentChanged() { }
            public bool                OnComponentChanging() => true;

            public object? GetService(Type serviceType)
                => _host?.GetService(serviceType);
        }

        private void OnAutoPair(object? sender, EventArgs e)
        {
            var mgr  = (BeepDockManager)Component;
            var host = FindDocumentHostOnForm();
            if (host is null)
            {
                var uiSvc = GetService<IUIService>();
                uiSvc?.ShowMessage(
                    "No BeepDocumentHost found on the parent form.",
                    "Auto-pair",
                    System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }

            var svc = GetService<IDesignerHost>()!;
            using (var tx = svc.CreateTransaction("Auto-pair BeepDockManager with BeepDocumentHost"))
            {
                var prop = TypeDescriptor.GetProperties(mgr)[nameof(BeepDockManager.Host)];
                prop?.SetValue(mgr, host);
                tx.Commit();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Smart-tag
        // ─────────────────────────────────────────────────────────────────────

        public override DesignerActionListCollection ActionLists =>
            _actionLists ??= new DesignerActionListCollection { new DockManagerActionList(this) };

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private T? GetService<T>() where T : class => GetService(typeof(T)) as T;

        private BeepDocumentHost? FindDocumentHostOnForm()
        {
            var svc = GetService<IDesignerHost>();
            if (svc is null) return null;

            return svc.Container.Components
                .OfType<BeepDocumentHost>()
                .FirstOrDefault();
        }

        private void AddPanelOnEdge(DockEdge edge)
        {
            var mgr = (BeepDockManager)Component;
            var svc = GetService<IDesignerHost>()!;
            using var tx = svc.CreateTransaction($"Add dock panel on {edge}");
            mgr.AddPanel($"Panel ({edge})", edge, content: null, iconPath: null);
            tx.Commit();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Nested — DesignerActionList
        // ─────────────────────────────────────────────────────────────────────

        private sealed class DockManagerActionList : DesignerActionList
        {
            private readonly BeepDockManagerDesigner _designer;

            public DockManagerActionList(BeepDockManagerDesigner designer)
                : base(designer.Component)
            {
                _designer = designer;
            }

            // Smart-tag items
            public override DesignerActionItemCollection GetSortedActionItems()
            {
                var items = new DesignerActionItemCollection();

                items.Add(new DesignerActionHeaderItem("Host"));
                items.Add(new DesignerActionPropertyItem(
                    nameof(Host),
                    "DocumentHost",
                    "Host",
                    "The BeepDocumentHost this manager attaches to."));
                items.Add(new DesignerActionMethodItem(
                    this,
                    nameof(AutoPair),
                    "Auto-pair with DocumentHost",
                    "Host",
                    "Find and assign the first BeepDocumentHost on this form.",
                    includeAsDesignerVerb: false));

                items.Add(new DesignerActionHeaderItem("Add Panels"));
                items.Add(new DesignerActionMethodItem(this, nameof(AddLeft),   "Add Left Panel",   "Add Panels", includeAsDesignerVerb: false));
                items.Add(new DesignerActionMethodItem(this, nameof(AddRight),  "Add Right Panel",  "Add Panels", includeAsDesignerVerb: false));
                items.Add(new DesignerActionMethodItem(this, nameof(AddTop),    "Add Top Panel",    "Add Panels", includeAsDesignerVerb: false));
                items.Add(new DesignerActionMethodItem(this, nameof(AddBottom), "Add Bottom Panel", "Add Panels", includeAsDesignerVerb: false));

                items.Add(new DesignerActionHeaderItem("Edit"));
                items.Add(new DesignerActionMethodItem(this, nameof(EditPanels), "Edit Dock Panels…", "Edit", includeAsDesignerVerb: false));

                return items;
            }

            // ── Properties bound to smart-tag ──

            public BeepDocumentHost? Host
            {
                get => ((BeepDockManager)Component!).Host;
                set
                {
                    var svc = _designer.GetService<IDesignerHost>()!;
                    using var tx = svc.CreateTransaction("Set BeepDockManager.Host");
                    TypeDescriptor.GetProperties(Component!)[nameof(BeepDockManager.Host)]?.SetValue(Component, value);
                    tx.Commit();
                }
            }

            // ── Methods bound to smart-tag ──

            public void AutoPair()   => _designer.OnAutoPair(this, EventArgs.Empty);
            public void EditPanels() => _designer.OnEditPanels(this, EventArgs.Empty);
            public void AddLeft()    => _designer.AddPanelOnEdge(DockEdge.Left);
            public void AddRight()   => _designer.AddPanelOnEdge(DockEdge.Right);
            public void AddTop()     => _designer.AddPanelOnEdge(DockEdge.Top);
            public void AddBottom()  => _designer.AddPanelOnEdge(DockEdge.Bottom);
        }
    }
}
