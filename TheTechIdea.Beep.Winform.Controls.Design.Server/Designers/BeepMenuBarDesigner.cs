// BeepMenuBarDesigner.cs
// Phase 08 — Designer Integration.
//
// Hosts the BeepMenuBar design-time experience. The action list lives in
// BeepMenuBarActionList.cs (one class per file, per project convention).
//
// Phase 08 promotes this designer from "height presets only" (91 LOC) to a
// commercial-grade smart-tag experience matching the BeepDocumentHost
// designer pattern:
//   - Load Sample Items / Clear All Items verbs (wrapped in
//     DesignerTransaction so VS Undo surfaces descriptive entries)
//   - Style cycler (Material / Fluent / Office)
//   - Item-count read-out
//   - Quick access to appearance + behaviour properties
//
// All component mutations go through ExecuteAction (transaction-wrapped)
// or SetPropertyWithTransaction (per-property), never raw property writes.
// This guarantees clean Undo/Redo behaviour in the VS Edit menu.
//
// See .plans/Menus-Phase-08-DesignerIntegration.md.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for <see cref="BeepMenuBar"/>. Hooks the
    /// commercial-grade smart tag action list and provides the
    /// transaction-wrapped action chokepoints that keep VS Undo /
    /// Redo entries descriptive.
    /// </summary>
    public sealed class BeepMenuBarDesigner : BaseBeepControlDesigner
    {
        /// <summary>The hosted menubar, or <c>null</c> if not yet sited.</summary>
        public BeepMenuBar? MenuBar => Component as BeepMenuBar;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepMenuBarActionList(this));
            return lists;
        }

        // ─────────────────────────────────────────────────────────────────
        // Transaction-wrapped action chokepoint.
        //
        // Every smart-tag mutation that touches the component runs through
        // ExecuteAction so the VS Edit menu shows a descriptive Undo entry
        // (e.g. "Undo Load Sample Menu Items"). The IComponentChangeService
        // events are raised inside the transaction so the designer host
        // emits a single coherent change for nested mutations.
        //
        // Mirrors BeepDocumentHostDesigner.ExecuteAction (Phase 04 of the
        // DocumentHost MDI program).
        // ─────────────────────────────────────────────────────────────────

        internal void ExecuteAction(string description, Action<BeepMenuBar> action)
        {
            if (MenuBar is not BeepMenuBar bar) return;

            var host       = GetService(typeof(IDesignerHost))            as IDesignerHost;
            var changeSvc  = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            DesignerTransaction? txn = null;
            try
            {
                txn = host?.CreateTransaction(description);
                changeSvc?.OnComponentChanging(bar, null);

                action(bar);

                changeSvc?.OnComponentChanged(bar, null, null, null);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }

            RefreshDesignerActionUI();
        }

        /// <summary>
        /// Transaction-wrapped per-property writer. The base class
        /// <see cref="BaseBeepControlDesigner.SetProperty"/> raises change
        /// events but does NOT open a transaction — so Undo surfaces an
        /// anonymous entry rather than the descriptive name. This shim
        /// adds the transaction layer.
        /// </summary>
        internal void SetPropertyWithTransaction(string propertyName, object? value, string description)
        {
            if (MenuBar is not BeepMenuBar bar) return;

            var prop = TypeDescriptor.GetProperties(bar)[propertyName];
            if (prop == null || prop.IsReadOnly) return;

            var host       = GetService(typeof(IDesignerHost))            as IDesignerHost;
            var changeSvc  = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            var oldValue   = prop.GetValue(bar);

            if (Equals(oldValue, value)) return;

            DesignerTransaction? txn = null;
            try
            {
                txn = host?.CreateTransaction(description);
                changeSvc?.OnComponentChanging(bar, prop);
                prop.SetValue(bar, value);
                changeSvc?.OnComponentChanged(bar, prop, oldValue, value);
                txn?.Commit();
            }
            catch
            {
                txn?.Cancel();
                throw;
            }

            RefreshDesignerActionUI();
        }

        private void RefreshDesignerActionUI()
        {
            if (Component == null) return;
            (GetService(typeof(DesignerActionUIService)) as DesignerActionUIService)?.Refresh(Component);
        }
    }
}
