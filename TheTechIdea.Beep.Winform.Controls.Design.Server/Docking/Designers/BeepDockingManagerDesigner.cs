using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers
{
    /// <summary>
    /// Design-time support for BeepDockingManager.
    ///
    /// Follows Krypton's KryptonDockingManager designer pattern:
    /// - Non-visual ComponentDesigner (manager is a tray component, not a control)
    /// - Smart tags let the developer add panels at any dock edge
    /// - Designer verbs provide the same actions from the right-click context menu
    /// - All mutations go through IComponentChangeService (undo + .designer.cs codegen)
    /// - No DesignMode guards that would reduce functionality
    /// </summary>
    internal sealed class BeepDockingManagerDesigner : ComponentDesigner
    {
        private BeepDockingManager _manager;
        private DesignerActionListCollection _actionLists;
        private DesignerVerbCollection _verbs;

        private BeepDockingManager Manager => _manager ?? (_manager = (BeepDockingManager)Component);

        // ── Smart tags ──────────────────────────────────────────────────────────
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new BeepDockingManagerActionList(Component, this));
                }
                return _actionLists;
            }
        }

        // ── Verbs ───────────────────────────────────────────────────────────────
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Add Panel &Left",   (s, e) => AddPanel(DockPosition.Left)),
                            new DesignerVerb("Add Panel &Right",  (s, e) => AddPanel(DockPosition.Right)),
                            new DesignerVerb("Add Panel &Top",    (s, e) => AddPanel(DockPosition.Top)),
                        new DesignerVerb("Add Panel &Bottom", (s, e) => AddPanel(DockPosition.Bottom)),
                        new DesignerVerb("Add Panel &Fill",   (s, e) => AddPanel(DockPosition.Fill)),
                        new DesignerVerb("Move Selected Panel &Left",   (s, e) => MoveSelectedPanel(DockPosition.Left)),
                        new DesignerVerb("Move Selected Panel &Right",  (s, e) => MoveSelectedPanel(DockPosition.Right)),
                        new DesignerVerb("Move Selected Panel &Top",    (s, e) => MoveSelectedPanel(DockPosition.Top)),
                        new DesignerVerb("Move Selected Panel &Bottom", (s, e) => MoveSelectedPanel(DockPosition.Bottom)),
                        new DesignerVerb("Move Selected Panel &Fill",   (s, e) => MoveSelectedPanel(DockPosition.Fill)),
                        new DesignerVerb("Stack Selected with Previous Panel", (s, e) => StackSelectedWithPreviousPanel()),
                        new DesignerVerb("Stack Selected with Next Panel",     (s, e) => StackSelectedWithNextPanel()),
                            new DesignerVerb("&Validate Panels",  (s, e) => ValidatePanels()),
                            new DesignerVerb("Attach &Host Form", (s, e) => AttachHostForm())
                    };
                }
                return _verbs;
            }
        }

        // ── Lifecycle ────────────────────────────────────────────────────────────

        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            // Auto-attach the parent form as HostForm when the manager is first dropped onto a form.
            // Mirrors Krypton's KryptonDockingManager designer: the manager immediately knows its host.
            AttachHostForm();

            Debug.WriteLine("[BeepDockingManagerDesigner] Initialized");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager = null;
            base.Dispose(disposing);
        }

        // ── Public helpers used by action list ───────────────────────────────────

        /// <summary>
        /// Adds a new DockPanel component at the specified dock edge.
        /// Routed through BeepDockingDesignerWiring for proper undo support.
        /// </summary>
        public DockPanel AddPanel(DockPosition position)
        {
            try
            {
                return BeepDockingDesignerWiring.AddPanel(Manager, position, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerDesigner] Error adding panel at {position}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Removes a DockPanel component from the designer container.
        /// </summary>
        public void RemovePanel(DockPanel panel)
        {
            try
            {
                BeepDockingDesignerWiring.RemovePanel(panel, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerDesigner] Error removing panel: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns all DockPanel components in the designer that belong to this manager.
        /// </summary>
        public List<DockPanel> GetAssociatedPanels() =>
            BeepDockingDesignerWiring.GetPanelsFor(Manager, AsServiceProvider);

        public void MoveSelectedPanel(DockPosition position)
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            if (panel != null)
                BeepDockingDesignerWiring.MovePanel(panel, position, AsServiceProvider);
        }

        public void StackSelectedWithPreviousPanel()
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            DockPanel target = BeepDockingDesignerWiring.GetPreviousPanel(panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(panel, target, AsServiceProvider);
        }

        public void StackSelectedWithNextPanel()
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            DockPanel target = BeepDockingDesignerWiring.GetNextPanel(panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(panel, target, AsServiceProvider);
        }

        /// <summary>
        /// Validates all associated panels and logs warnings.
        /// </summary>
        public void ValidatePanels()
        {
            try
            {
                var panels = GetAssociatedPanels();
                int issues = 0;

                foreach (DockPanel p in panels)
                {
                    if (string.IsNullOrWhiteSpace(p.Key))
                    {
                        Debug.WriteLine($"[BeepDockingManagerDesigner] Warning: panel has no Key");
                        issues++;
                    }
                }

                Debug.WriteLine(issues == 0
                    ? $"[BeepDockingManagerDesigner] {panels.Count} panel(s) OK"
                    : $"[BeepDockingManagerDesigner] {issues} issue(s) across {panels.Count} panel(s)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerDesigner] Validation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Locates the parent Form in the designer host and assigns it to the manager's
        /// HostForm property, mirroring Krypton's KryptonDockingManager designer pattern
        /// where the host form is auto-detected during InitializeNewComponent.
        /// </summary>
        public void AttachHostForm()
        {
            try
            {
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host == null) return;

                // Walk up from the manager's own site container to find the root Form.
                Form parentForm = host.RootComponent as Form;
                if (parentForm == null) return;

                IComponentChangeService svc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                PropertyDescriptor prop = TypeDescriptor.GetProperties(Manager)["HostForm"];
                if (prop == null) return;

                svc?.OnComponentChanging(Manager, prop);
                prop.SetValue(Manager, parentForm);
                svc?.OnComponentChanged(Manager, prop, null, parentForm);

                Debug.WriteLine($"[BeepDockingManagerDesigner] HostForm attached: {parentForm.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerDesigner] AttachHostForm error: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifies the designer host that the manager component has changed,
        /// triggering .designer.cs regeneration for the manager's properties.
        /// </summary>
        public void CommitChanges()
        {
            IComponentChangeService svc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            svc?.OnComponentChanged(Manager, null, null, null);
        }

        private object GetService(Type serviceType) => base.GetService(serviceType);

        // Adapter so callers that need IServiceProvider can consume this designer's GetService.
        private IServiceProvider AsServiceProvider => new ServiceProviderAdapter(base.GetService);
    }

    file sealed class ServiceProviderAdapter : IServiceProvider
    {
        private readonly Func<Type, object> _getter;
        internal ServiceProviderAdapter(Func<Type, object> getter) => _getter = getter;
        public object GetService(Type serviceType) => _getter(serviceType);
    }
}
