using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private TabControlWithoutHeader _tabControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            System.Diagnostics.Debug.WriteLine("BeepTabsDesigner: Initialize called.");

            if (component is BeepTabs beepTabs)
            {
                // Access the embedded TabControlWithoutHeader
                _tabControl = beepTabs.Controls.OfType<TabControlWithoutHeader>().FirstOrDefault();

                if (_tabControl == null)
                {
                    System.Diagnostics.Debug.WriteLine("Embedded TabControlWithoutHeader not found.");
                    return;
                }

                // Enable drag-and-drop on the internal tab control.
                _tabControl.AllowDrop = true;
                // Attach drag-and-drop event handlers.
                _tabControl.DragEnter += BeepTabs_DragEnter;
                _tabControl.DragOver += BeepTabs_DragOver;
                _tabControl.DragDrop += BeepTabs_DragDrop;
            }
        }

        private void BeepTabs_DragEnter(object sender, DragEventArgs e)
        {
            if (IsToolboxItem(e.Data))
            {
                System.Diagnostics.Debug.WriteLine("DragEnter: ToolboxItem found");
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DragEnter: No ToolboxItem found");
                e.Effect = DragDropEffects.None;
            }
        }

        private void BeepTabs_DragOver(object sender, DragEventArgs e)
        {
            if (IsToolboxItem(e.Data))
            {
                System.Diagnostics.Debug.WriteLine("DragOver: ToolboxItem found");
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DragOver: No ToolboxItem found");
                e.Effect = DragDropEffects.None;
            }
        }

        private void BeepTabs_DragDrop(object sender, DragEventArgs e)
        {
            if (!IsToolboxItem(e.Data))
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("DragDrop: ToolboxItem detected");
            IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (designerHost == null)
            {
                MessageBox.Show("Designer host service not found.");
                return;
            }

            ToolboxItem toolboxItem = GetToolboxItem(e.Data);
            if (toolboxItem == null)
            {
                MessageBox.Show("Toolbox item not found.");
                return;
            }

            var components = toolboxItem.CreateComponents(designerHost);
            if (components == null || !components.Any())
            {
                MessageBox.Show("No components created from the toolbox item.");
                return;
            }

            Control control = components.OfType<Control>().FirstOrDefault();
            if (control == null)
            {
                MessageBox.Show("The dragged item is not a valid Control.");
                return;
            }

            if (_tabControl.SelectedTab != null)
            {
                // Add the control to the selected tab.
                _tabControl.SelectedTab.Controls.Add(control);
                // Convert the drop coordinates to the selected tab's client coordinates.
                Point dropPoint = _tabControl.SelectedTab.PointToClient(new Point(e.X, e.Y));
                control.Location = dropPoint;
            }
        }

        // Determines whether the dragged data represents a ToolboxItem.
        public bool IsToolboxItem(IDataObject data)
        {
            return data.GetDataPresent(typeof(ToolboxItem)) || data.GetDataPresent("CF_TOOLBOXITEM");
        }

        // Retrieves the ToolboxItem from the dragged data.
        public ToolboxItem GetToolboxItem(IDataObject data)
        {
            if (data.GetDataPresent(typeof(ToolboxItem)))
            {
                System.Diagnostics.Debug.WriteLine("Getting ToolboxItem from data.");
                return data.GetData(typeof(ToolboxItem)) as ToolboxItem;
            }
            else if (data.GetDataPresent("CF_TOOLBOXITEM"))
            {
                System.Diagnostics.Debug.WriteLine("Getting ToolboxItem from data (CF_TOOLBOXITEM).");
                return data.GetData("CF_TOOLBOXITEM") as ToolboxItem;
            }
            System.Diagnostics.Debug.WriteLine("No ToolboxItem found in data.");
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _tabControl != null)
            {
                _tabControl.DragEnter -= BeepTabs_DragEnter;
                _tabControl.DragOver -= BeepTabs_DragOver;
                _tabControl.DragDrop -= BeepTabs_DragDrop;
            }
            base.Dispose(disposing);
        }
    }
}
