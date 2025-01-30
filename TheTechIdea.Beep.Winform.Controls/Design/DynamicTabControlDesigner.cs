using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class DynamicTabControlDesigner : ParentControlDesigner
    {
        private BeepDynamicTabControl _dynamicTabControl;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Ensure the control is a BeepDynamicTabControl
            _dynamicTabControl = component as BeepDynamicTabControl ?? throw new InvalidOperationException("Control must be of type BeepDynamicTabControl.");

            // Retrieve design-time services
            _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
            _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (_dynamicTabControl != null)
            {
                EnableDragAndDropForMainControl();    // Enable drag-and-drop for the main control
                EnableDragAndDropForContentPanel();  // Enable drag-and-drop for the ContentPanel
            }
        }

        /// <summary>
        /// Enables drag-and-drop functionality for the main BeepDynamicTabControl.
        /// </summary>
        private void EnableDragAndDropForMainControl()
        {
            _dynamicTabControl.AllowDrop = true;

            _dynamicTabControl.DragEnter += MainControl_DragEnter;
            _dynamicTabControl.DragDrop += MainControl_DragDrop;
        }

        private void MainControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IComponent)) || e.Data.GetDataPresent("System.Windows.Forms.Design.ToolboxItem"))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MainControl_DragDrop(object sender, DragEventArgs e)
        {
            if (_dynamicTabControl.ContentPanel != null)
            {
                RouteToSelectedPanel(e);
            }
        }

        /// <summary>
        /// Enables drag-and-drop functionality for the ContentPanel of BeepDynamicTabControl.
        /// </summary>
        private void EnableDragAndDropForContentPanel()
        {
            if (_dynamicTabControl.ContentPanel == null)
                return;

            _dynamicTabControl.ContentPanel.AllowDrop = true;

            _dynamicTabControl.ContentPanel.DragEnter += ContentPanel_DragEnter;
            _dynamicTabControl.ContentPanel.DragDrop += ContentPanel_DragDrop;
        }

        private void ContentPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IComponent)) || e.Data.GetDataPresent("System.Windows.Forms.Design.ToolboxItem"))
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
            RouteToSelectedPanel(e);
        }

        /// <summary>
        /// Routes the dragged control to the currently selected panel.
        /// </summary>
        private void RouteToSelectedPanel(DragEventArgs e)
        {
            var selectedPanel = GetSelectedPanel();
            if (selectedPanel == null)
            {
                Console.WriteLine("No selected panel to drop the control.");
                return;
            }

            if (e.Data.GetData("System.Windows.Forms.Design.ToolboxItem") is ToolboxItem toolboxItem)
            {
                // Create a control from ToolboxItem
                var control = toolboxItem.CreateComponents(_designerHost).OfType<Control>().FirstOrDefault();
                if (control != null)
                {
                    HandleDropToPanel(selectedPanel, control, e);
                }
            }
            else if (e.Data.GetData(typeof(IComponent)) is Control draggedControl)
            {
                // Handle existing control being dragged
                HandleDropToPanel(selectedPanel, draggedControl, e);
            }
        }

        /// <summary>
        /// Retrieves the currently selected panel in the dynamic tab control.
        /// </summary>
        private Panel GetSelectedPanel()
        {
            if (_dynamicTabControl?.SelectedTab != null)
            {
                foreach (Control control in _dynamicTabControl.ContentPanel.Controls)
                {
                    if (control is Panel panel && panel.Visible)
                    {
                        return panel;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a control to a panel and registers it with the designer.
        /// </summary>
        private void HandleDropToPanel(Panel panel, Control control, DragEventArgs e)
        {
            if (panel == null || control == null)
                throw new ArgumentNullException();

            // Add the control to the panel
            panel.Controls.Add(control);

            // Notify the designer about the addition
            _changeService?.OnComponentChanging(panel, null);
            _changeService?.OnComponentChanged(panel, null, null, null);

            // Register the control with the designer
            if (_designerHost?.Container != null)
            {
                bool exists = _designerHost.Container.Components.Cast<IComponent>().Any(c => c == control);
                if (!exists)
                {
                    _designerHost.Container.Add(control, control.Name);
                }
            }

            // Position the control within the panel
            var dropPoint = panel.PointToClient(new System.Drawing.Point(e.X, e.Y));
            control.Location = dropPoint;

            // Ensure the control is visible and brought to the front
            control.BringToFront();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _dynamicTabControl != null)
            {
                // Remove drag-and-drop event handlers
                _dynamicTabControl.DragEnter -= MainControl_DragEnter;
                _dynamicTabControl.DragDrop -= MainControl_DragDrop;

                if (_dynamicTabControl.ContentPanel != null)
                {
                    _dynamicTabControl.ContentPanel.DragEnter -= ContentPanel_DragEnter;
                    _dynamicTabControl.ContentPanel.DragDrop -= ContentPanel_DragDrop;
                }
            }

            base.Dispose(disposing);
        }
    }
}
