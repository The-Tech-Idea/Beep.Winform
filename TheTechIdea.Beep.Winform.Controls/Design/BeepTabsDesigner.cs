using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private TabControlWithoutHeader _tabControl;
        public override void Initialize(IComponent component)
        {
            System.Diagnostics.Debug.WriteLine("BeepTabsDesigner: Initialize called.");
            base.Initialize(component);

            if (component is BeepTabs beepTabs)
            {
                // Access the embedded TabControlWithoutHeader
                _tabControl = beepTabs.Controls.OfType<TabControlWithoutHeader>().FirstOrDefault();

                if (_tabControl == null)
                {
                    System.Diagnostics.Debug.WriteLine("Embedded TabControlWithoutHeader not found.");
                    return;
                }

                // Attach drag-and-drop handlers
                _tabControl.AllowDrop = true;
                _tabControl.DragEnter += BeepTabs_DragEnter;
                _tabControl.DragOver += BeepTabs_DragOver;
                _tabControl.DragDrop += BeepTabs_DragDrop;
            }
        }
        private void BeepTabs_DragEnter(object sender, DragEventArgs e)
        {
            if (IsToolboxItem(e.Data))
            {
                Console.WriteLine("DragEnter: ToolboxItem found");
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                Console.WriteLine("DragEnter: No ToolboxItem found");
                e.Effect = DragDropEffects.None;
            }
        }
        private void BeepTabs_DragOver(object sender, DragEventArgs e)
        {
            if (IsToolboxItem(e.Data))
            {
                Console.WriteLine("DragOver: ToolboxItem found");
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                Console.WriteLine("DragOver: No ToolboxItem found");
                e.Effect = DragDropEffects.None;
            }
        }
        private void BeepTabs_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("DragDrop: ToolboxItem found");
            if (IsToolboxItem(e.Data))
            {
                Console.WriteLine("DragDrop: ToolboxItem found");
                var designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (designerHost == null)
                {
                    MessageBox.Show("Designer host service not found.");
                    return;
                }

                var toolboxItem = GetToolboxItem(e.Data);
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

                var control = components.OfType<Control>().FirstOrDefault();
                if (control == null)
                {
                    MessageBox.Show("The dragged item is not a valid Control.");
                    return;
                }

                if (_tabControl.SelectedTab != null)
                {
                    _tabControl.SelectedTab.Controls.Add(control);

                    var dropPoint = _tabControl.SelectedTab.PointToClient(new Point(e.X, e.Y));
                    control.Location = dropPoint;
                }
            }
        }
        // Check if the dragged data is a toolbox item
        public bool IsToolboxItem(IDataObject data)
        {
            // Check for the ToolboxItem format
            return data.GetDataPresent(typeof(ToolboxItem)) || data.GetDataPresent("CF_TOOLBOXITEM");
        }
        // Get the toolbox item from the dragged data
        public ToolboxItem GetToolboxItem(IDataObject data)
        {
            if (data.GetDataPresent(typeof(ToolboxItem)))
            {
                Console.WriteLine("Getting ToolboxItem from data");
                return data.GetData(typeof(ToolboxItem)) as ToolboxItem;
            }
            else if (data.GetDataPresent("CF_TOOLBOXITEM"))
            {
                Console.WriteLine("Getting ToolboxItem from data (CF_TOOLBOXITEM)");
                return data.GetData("CF_TOOLBOXITEM") as ToolboxItem;
            }
            Console.WriteLine("No ToolboxItem found in data");
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _tabControl != null)
            {
                // Detach DragDrop event handlers
                _tabControl.DragEnter -= BeepTabs_DragEnter;
                _tabControl.DragOver -= BeepTabs_DragOver;
                _tabControl.DragDrop -= BeepTabs_DragDrop;
            }

            base.Dispose(disposing);
        }
    }
}
