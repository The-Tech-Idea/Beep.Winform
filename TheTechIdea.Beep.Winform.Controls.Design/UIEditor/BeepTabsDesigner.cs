using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private Control _beepTabs;
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            //_beepTabs = component as BeepTabs;

            // Enable drag-and-drop
            if (_beepTabs != null)
            {
                EnableDragDrop(true);
            }
        }
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (_beepTabs == null)
                return;

            // Get the selected tab
            var selectedTab = _beepTabs.SelectedTab;
            if (selectedTab == null)
            {
                MessageBox.Show("No tab selected to drop the control.");
                return;
            }

            if (e.Data.GetDataPresent(typeof(ToolboxItem)))
            {
                var toolboxItem = (ToolboxItem)e.Data.GetData(typeof(ToolboxItem));
                var host = (IDesignerHost)GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    var transaction = host.CreateTransaction("Add Control to Tab");
                    try
                    {
                        // Create the control
                        var control = (Control)toolboxItem.CreateComponents(host)[0];

                        // Add the control to the selected tab
                        selectedTab.Controls.Add(control);

                        // Position the control
                        Point dropPoint = selectedTab.PointToClient(new Point(e.X, e.Y));
                        control.Location = dropPoint;

                        Console.WriteLine($"Control added to {selectedTab.TitleText} at {control.Location}.");
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
