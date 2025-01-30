using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class DynamicTabControlDesigner : ParentControlDesigner
    {
        private BeepDynamicTabControl _dynamicTabControl;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Cast the associated component to your custom control
            _dynamicTabControl = component as BeepDynamicTabControl
                ?? throw new InvalidOperationException("Control must be a BeepDynamicTabControl.");

            // Retrieve design-time services
            _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            // Subscribe to ComponentChanged event
            if (_changeService != null)
            {
                _changeService.ComponentChanged += OnDesignerComponentChanged;
            }

            // Enable drag-and-drop at design time
            _dynamicTabControl.AllowDrop = true;
            _dynamicTabControl.DragEnter += MainControl_DragEnter;
            _dynamicTabControl.DragDrop += MainControl_DragDrop;

            // Enable design mode for existing panels
            foreach (Control child in _dynamicTabControl.ContentPanel.Controls)
            {
                if (child is Panel panel)
                {
                    // Enable each panel to be a design surface
                    EnableDesignMode(panel, panel.Name);
                }
            }

            // Subscribe to Tabs collection changes to enable design mode for new panels
            _dynamicTabControl.Tabs.ListChanged += Tabs_ListChanged;
        }

        /// <summary>
        /// Handles dynamic addition of new tabs by enabling design mode for their panels.
        /// </summary>
        private void Tabs_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                var newTab = _dynamicTabControl.Tabs[e.NewIndex];
                if (_dynamicTabControl.Panels.TryGetValue(newTab.GuidId, out var panel))
                {
                    EnableDesignMode(panel, panel.Name);
                    Debug.WriteLine($"Designer: Enabled design mode for panel {panel.Name}");
                }
            }
        }

        /// <summary>
        /// Event handler for when a component property changes.
        /// Specifically handles changes to the SelectedTab property.
        /// </summary>
        private void OnDesignerComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            // Check if the changed component is our control and the property is "SelectedTab"
            if (e.Component == _dynamicTabControl && e.Member?.Name == nameof(BeepDynamicTabControl.SelectedTab))
            {
                // Invoke UpdateSelectedTab to refresh the visible panel
                _dynamicTabControl.UpdateSelectedTab();

                // Optionally, refresh the designer view
                var designerActionUIService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));
                designerActionUIService?.Refresh(_dynamicTabControl);

                Debug.WriteLine($"Designer: SelectedTab changed to {_dynamicTabControl.SelectedTab?.Name}");
            }
        }

        /// <summary>
        /// Handles the DragEnter event at the main control level.
        /// Determines if the data being dragged is acceptable.
        /// </summary>
        private void MainControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IComponent)) ||
                e.Data.GetDataPresent("System.Windows.Forms.Design.ToolboxItem"))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles the DragDrop event at the main control level.
        /// Adds the dropped control to the selected panel.
        /// </summary>
        private void MainControl_DragDrop(object sender, DragEventArgs e)
        {
            var selectedPanel = GetSelectedPanel();
            if (selectedPanel == null) return;

            if (e.Data.GetData("System.Windows.Forms.Design.ToolboxItem") is ToolboxItem toolboxItem)
            {
                var newControl = toolboxItem.CreateComponents(_designerHost).OfType<Control>().FirstOrDefault();
                if (newControl != null)
                {
                    HandleDropToPanel(selectedPanel, newControl, e);
                }
            }
            else if (e.Data.GetData(typeof(IComponent)) is Control existingControl)
            {
                HandleDropToPanel(selectedPanel, existingControl, e);
            }
        }

        /// <summary>
        /// Retrieves the currently selected panel based on the SelectedTab property.
        /// </summary>
        private Panel GetSelectedPanel()
        {
            if (_dynamicTabControl?.SelectedTab != null &&
                _dynamicTabControl.ContentPanel != null)
            {
                if (_dynamicTabControl.Panels.TryGetValue(_dynamicTabControl.SelectedTab.GuidId, out var selectedPanel))
                {
                    return selectedPanel;
                }
            }
            return null;
        }

        /// <summary>
        /// Handles adding the dropped control to the specified panel.
        /// </summary>
        private void HandleDropToPanel(Panel panel, Control control, DragEventArgs e)
        {
            if (panel == null || control == null) return;

            panel.Controls.Add(control);

            // Notify the designer about the addition
            _changeService?.OnComponentChanging(panel, null);
            _changeService?.OnComponentChanged(panel, null, null, null);

            // Register the new control with the design container if it's new
            if (_designerHost?.Container != null)
            {
                bool exists = _designerHost.Container.Components
                    .Cast<IComponent>()
                    .Any(c => c == control);
                if (!exists)
                {
                    _designerHost.Container.Add(control, control.Name);
                }
            }

            // Position the dropped control
            var dropPoint = panel.PointToClient(new Point(e.X, e.Y));
            control.Location = dropPoint;
            control.BringToFront();

            Debug.WriteLine($"Designer: Control {control.Name} added to {panel.Name} at location {control.Location}");
        }

        /// <summary>
        /// Overrides the Dispose method to clean up event handlers.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from ComponentChanged event
                if (_changeService != null)
                {
                    _changeService.ComponentChanged -= OnDesignerComponentChanged;
                }

                // Unhook drag-and-drop events
                if (_dynamicTabControl != null)
                {
                    _dynamicTabControl.DragEnter -= MainControl_DragEnter;
                    _dynamicTabControl.DragDrop -= MainControl_DragDrop;

                    // Unhook ContentPanel drag events if necessary
                    _dynamicTabControl.ContentPanel.DragEnter -= ContentPanel_DragEnter;
                    _dynamicTabControl.ContentPanel.DragDrop -= ContentPanel_DragDrop;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Optional: Handle drag events on ContentPanel if desired.
        /// Enables drag-and-drop within individual panels.
        /// </summary>
        private void ContentPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Control)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ContentPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (_dynamicTabControl == null) return;

            var selectedPanel = GetSelectedPanel();
            if (selectedPanel == null) return;

            if (e.Data.GetData(typeof(Control)) is Control existingControl)
            {
                HandleDropToPanel(selectedPanel, existingControl, e);
            }
        }
    }
}
