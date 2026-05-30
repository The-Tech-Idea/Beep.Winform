using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.ActionLists
{
    /// <summary>
    /// Smart-tag action list for BeepDockingManager.
    /// Provides quick actions to add panels at any dock edge and validate layout.
    /// All panel creation is routed through BeepDockingDesignerWiring so that
    /// the designer records the changes and regenerates .designer.cs.
    /// </summary>
    internal sealed class BeepDockingManagerActionList : DesignerActionList
    {
        private readonly BeepDockingManager _manager;
        private readonly BeepDockingManagerDesigner _designer;

        public BeepDockingManagerActionList(IComponent component, BeepDockingManagerDesigner designer)
            : base(component)
        {
            _manager = (BeepDockingManager)component;
            _designer = designer;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Add Panel"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPanelLeft),   "Add Panel Left",   "Add Panel", "Add a new dockable panel on the left edge",   true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPanelRight),  "Add Panel Right",  "Add Panel", "Add a new dockable panel on the right edge",  true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPanelTop),    "Add Panel Top",    "Add Panel", "Add a new dockable panel on the top edge",    true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPanelBottom), "Add Panel Bottom", "Add Panel", "Add a new dockable panel on the bottom edge", true));
            items.Add(new DesignerActionMethodItem(this, nameof(AddPanelFill),   "Add Panel Fill",   "Add Panel", "Add a new dockable panel in the fill area",   true));

            items.Add(new DesignerActionHeaderItem("Selected Panel"));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedLeft),   "Move Left",   "Selected Panel", "Move the selected panel to the left edge", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedRight),  "Move Right",  "Selected Panel", "Move the selected panel to the right edge", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedTop),    "Move Top",    "Selected Panel", "Move the selected panel to the top edge", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedBottom), "Move Bottom", "Selected Panel", "Move the selected panel to the bottom edge", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedFill),   "Move Fill",   "Selected Panel", "Move the selected panel to the fill area", true));
            items.Add(new DesignerActionMethodItem(this, nameof(StackSelectedWithPrevious), "Stack with Previous", "Selected Panel", "Stack the selected panel with the previous panel", true));
            items.Add(new DesignerActionMethodItem(this, nameof(StackSelectedWithNext),     "Stack with Next",     "Selected Panel", "Stack the selected panel with the next panel", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedEarlier),       "Move Earlier",        "Selected Panel", "Move the selected panel earlier in its stack", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveSelectedLater),         "Move Later",          "Selected Panel", "Move the selected panel later in its stack", true));

            items.Add(new DesignerActionHeaderItem("Layout"));
            items.Add(new DesignerActionMethodItem(this, nameof(RefreshLayout), "Refresh Layout", "Layout",
                "Refresh the design-time docking layout", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ValidatePanels), "Validate Panels", "Layout",
                "Check that all DockPanel components are properly configured", true));
            items.Add(new DesignerActionMethodItem(this, nameof(AttachHostForm), "Attach Host Form", "Layout",
                "Assign the parent form as the docking manager's host form", true));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Style), "Style", "Appearance",
                "Control style applied to docking chrome (captions, splitters, strips)"));
            items.Add(new DesignerActionPropertyItem(nameof(UseThemeColors), "Use Theme Colors", "Appearance",
                "Paint docking chrome with the active Beep theme palette"));
            items.Add(new DesignerActionPropertyItem(nameof(Theme), "Theme", "Appearance",
                "Active Beep theme name used by docking chrome"));

            return items;
        }

        // ── Appearance (smart-tag editable properties) ───────────────────────────
        // Mutations are routed through the change service so they persist into
        // *.Designer.cs and re-render the live design surface.

        public BeepControlStyle Style
        {
            get => _manager.Style;
            set => BeepDockingDesignerWiring.SetProperty(_manager, nameof(BeepDockingManager.Style), value, AsServiceProvider);
        }

        public bool UseThemeColors
        {
            get => _manager.UseThemeColors;
            set => BeepDockingDesignerWiring.SetProperty(_manager, nameof(BeepDockingManager.UseThemeColors), value, AsServiceProvider);
        }

        public string Theme
        {
            get => _manager.Theme;
            set => BeepDockingDesignerWiring.SetProperty(_manager, nameof(BeepDockingManager.Theme), value, AsServiceProvider);
        }

        public void AddPanelLeft()   => AddPanel(DockPosition.Left);
        public void AddPanelRight()  => AddPanel(DockPosition.Right);
        public void AddPanelTop()    => AddPanel(DockPosition.Top);
        public void AddPanelBottom() => AddPanel(DockPosition.Bottom);
        public void AddPanelFill()   => AddPanel(DockPosition.Fill);

        public void MoveSelectedLeft()   => MoveSelectedPanel(DockPosition.Left);
        public void MoveSelectedRight()  => MoveSelectedPanel(DockPosition.Right);
        public void MoveSelectedTop()    => MoveSelectedPanel(DockPosition.Top);
        public void MoveSelectedBottom() => MoveSelectedPanel(DockPosition.Bottom);
        public void MoveSelectedFill()   => MoveSelectedPanel(DockPosition.Fill);
        public void MoveSelectedEarlier() => MoveSelectedPanelInStack(-1);
        public void MoveSelectedLater() => MoveSelectedPanelInStack(1);

        public void StackSelectedWithPrevious()
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            DockPanel target = BeepDockingDesignerWiring.GetPreviousPanel(panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(panel, target, AsServiceProvider);
        }

        public void StackSelectedWithNext()
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            DockPanel target = BeepDockingDesignerWiring.GetNextPanel(panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(panel, target, AsServiceProvider);
        }

        public void RefreshLayout() =>
            BeepDockingDesignerWiring.RefreshHostLayout(_manager, AsServiceProvider);

        private void AddPanel(DockPosition position)
        {
            try
            {
                BeepDockingDesignerWiring.AddPanel(_manager, position, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerActionList] Error adding panel at {position}: {ex.Message}");
            }
        }

        private void MoveSelectedPanel(DockPosition position)
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            if (panel != null)
                BeepDockingDesignerWiring.MovePanel(panel, position, AsServiceProvider);
        }

        private void MoveSelectedPanelInStack(int delta)
        {
            DockPanel panel = BeepDockingDesignerWiring.GetSelectedPanel(AsServiceProvider);
            if (panel != null)
                BeepDockingDesignerWiring.MovePanelInStack(panel, delta, AsServiceProvider);
        }

        public void ValidatePanels()
        {
            try
            {
                var panels = BeepDockingDesignerWiring.GetPanelsFor(_manager, AsServiceProvider);
                int issues = 0;

                foreach (DockPanel p in panels)
                {
                    if (string.IsNullOrWhiteSpace(p.Key))
                    {
                        Debug.WriteLine($"[BeepDockingManagerActionList] Warning: Panel has no Key");
                        issues++;
                    }
                }

                Debug.WriteLine(issues == 0
                    ? $"[BeepDockingManagerActionList] {panels.Count} panel(s) OK"
                    : $"[BeepDockingManagerActionList] {issues} issue(s) found in {panels.Count} panel(s)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockingManagerActionList] Validation error: {ex.Message}");
            }
        }

        public void AttachHostForm() => _designer.AttachHostForm();

        // IServiceProvider passthrough used by BeepDockingDesignerWiring
        private IServiceProvider AsServiceProvider => new ServiceProviderAdapter(base.GetService);
    }

    file sealed class ServiceProviderAdapter : IServiceProvider
    {
        private readonly Func<Type, object> _getter;
        internal ServiceProviderAdapter(Func<Type, object> getter) => _getter = getter;
        public object GetService(Type serviceType) => _getter(serviceType);
    }
}
