using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsToolbarDesigner : ParentControlDesigner
    {
        private IComponentChangeService? _changeService;
        private DesignerActionListCollection? _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        internal void SetProperty(string propertyName, object value)
        {
            if (Component == null)
            {
                return;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return;
            }

            object? currentValue = property.GetValue(Component);
            if (Equals(currentValue, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        internal T? GetProperty<T>(string propertyName)
        {
            if (Component == null)
            {
                return default;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return default;
            }

            object? value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepFormsToolbarActionList(this)
            };
    }

    public sealed class BeepFormsToolbarActionList : DesignerActionList
    {
        private readonly BeepFormsToolbarDesigner _designer;

        public BeepFormsToolbarActionList(BeepFormsToolbarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public BeepFormsShellToolbarButtons ShellToolbarButtons
        {
            get => _designer.GetProperty<BeepFormsShellToolbarButtons>(nameof(BeepFormsToolbar.ShellToolbarButtons));
            set => _designer.SetProperty(nameof(BeepFormsToolbar.ShellToolbarButtons), value);
        }

        public BeepFormsSavepointToolbarActions SavepointToolbarActions
        {
            get => _designer.GetProperty<BeepFormsSavepointToolbarActions>(nameof(BeepFormsToolbar.SavepointToolbarActions));
            set => _designer.SetProperty(nameof(BeepFormsToolbar.SavepointToolbarActions), value);
        }

        public BeepFormsAlertToolbarPresets AlertToolbarPresets
        {
            get => _designer.GetProperty<BeepFormsAlertToolbarPresets>(nameof(BeepFormsToolbar.AlertToolbarPresets));
            set => _designer.SetProperty(nameof(BeepFormsToolbar.AlertToolbarPresets), value);
        }

        public BeepFormsShellToolbarOrder ShellToolbarOrder
        {
            get => _designer.GetProperty<BeepFormsShellToolbarOrder>(nameof(BeepFormsToolbar.ShellToolbarOrder));
            set => _designer.SetProperty(nameof(BeepFormsToolbar.ShellToolbarOrder), value);
        }

        public FlowDirection ShellToolbarFlowDirection
        {
            get => _designer.GetProperty<FlowDirection>(nameof(BeepFormsToolbar.ShellToolbarFlowDirection));
            set => _designer.SetProperty(nameof(BeepFormsToolbar.ShellToolbarFlowDirection), value);
        }

        public string SavepointToolbarCaption
        {
            get => _designer.GetProperty<string>(nameof(BeepFormsToolbar.SavepointToolbarCaption)) ?? "Savepoints";
            set => _designer.SetProperty(nameof(BeepFormsToolbar.SavepointToolbarCaption), value);
        }

        public string AlertToolbarCaption
        {
            get => _designer.GetProperty<string>(nameof(BeepFormsToolbar.AlertToolbarCaption)) ?? "Alerts";
            set => _designer.SetProperty(nameof(BeepFormsToolbar.AlertToolbarCaption), value);
        }

        public void ShowAllShellToolbarButtons()
        {
            ShellToolbarButtons = BeepFormsShellToolbarButtons.All;
        }

        public void ShowSavepointButtonOnly()
        {
            ShellToolbarButtons = BeepFormsShellToolbarButtons.Savepoints;
        }

        public void ShowAlertButtonOnly()
        {
            ShellToolbarButtons = BeepFormsShellToolbarButtons.Alerts;
        }

        public void HideAllShellToolbarButtons()
        {
            ShellToolbarButtons = BeepFormsShellToolbarButtons.None;
        }

        public void PlaceSavepointsFirst()
        {
            ShellToolbarOrder = BeepFormsShellToolbarOrder.SavepointsFirst;
        }

        public void PlaceAlertsFirst()
        {
            ShellToolbarOrder = BeepFormsShellToolbarOrder.AlertsFirst;
        }

        public void RestoreDefaultShellComposition()
        {
            ShellToolbarButtons = BeepFormsShellToolbarButtons.All;
            SavepointToolbarActions = BeepFormsSavepointToolbarActions.All;
            AlertToolbarPresets = BeepFormsAlertToolbarPresets.All;
            ShellToolbarOrder = BeepFormsShellToolbarOrder.SavepointsFirst;
            ShellToolbarFlowDirection = FlowDirection.LeftToRight;
            SavepointToolbarCaption = "Savepoints";
            AlertToolbarCaption = "Alerts";
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Shell Toolbar"));
            items.Add(new DesignerActionPropertyItem(nameof(ShellToolbarButtons), "Toolbar Buttons", "Shell Toolbar", "Select which top-level shell toolbar buttons are shown."));
            items.Add(new DesignerActionPropertyItem(nameof(SavepointToolbarActions), "Savepoint Actions", "Shell Toolbar", "Select which savepoint actions are offered in the toolbar popup."));
            items.Add(new DesignerActionPropertyItem(nameof(AlertToolbarPresets), "Alert Presets", "Shell Toolbar", "Select which alert presets are offered in the toolbar popup."));
            items.Add(new DesignerActionPropertyItem(nameof(ShellToolbarOrder), "Button Order", "Shell Toolbar", "Select whether savepoints or alerts appear first in the shell toolbar."));
            items.Add(new DesignerActionPropertyItem(nameof(ShellToolbarFlowDirection), "Flow Direction", "Shell Toolbar", "Select the layout direction for the shell toolbar button row."));
            items.Add(new DesignerActionPropertyItem(nameof(SavepointToolbarCaption), "Savepoint Caption", "Shell Toolbar", "Customize the text shown on the savepoint shell toolbar button."));
            items.Add(new DesignerActionPropertyItem(nameof(AlertToolbarCaption), "Alert Caption", "Shell Toolbar", "Customize the text shown on the alert shell toolbar button."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllShellToolbarButtons), "Show All Toolbar Buttons", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowSavepointButtonOnly), "Savepoints Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAlertButtonOnly), "Alerts Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(HideAllShellToolbarButtons), "Hide Toolbar Buttons", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(PlaceSavepointsFirst), "Place Savepoints First", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(PlaceAlertsFirst), "Place Alerts First", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultShellComposition), "Restore Default Composition", "Quick Presets", true));

            return items;
        }
    }
}