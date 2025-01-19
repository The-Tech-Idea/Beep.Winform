using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace TheTechIdea.Beep.Winform.Controls.Design
{

    public class TabControlWithoutTabsDesigner : ParentControlDesigner
    {
        private TabControlWithoutTabs tabControl;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            tabControl = component as TabControlWithoutTabs;

            if (tabControl != null)
            {
                tabControl.SelectedIndexChanged += OnSelectedIndexChanged;
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl == null || tabControl.SelectedTab == null)
                return;

            // Activate the selected TabPage
            ActivateDesignSurface(tabControl.SelectedTab);
            NotifyChange();
        }

        private void ActivateDesignSurface(Control activeControl)
        {
            var selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            if (selectionService != null)
            {
                // Set the selected TabPage as the active design-time component
                selectionService.SetSelectedComponents(new[] { activeControl }, SelectionTypes.Replace);
            }
        }

        private void NotifyChange()
        {
            var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (changeService != null)
            {
                // Notify that the TabControl has been updated
                changeService.OnComponentChanged(tabControl, null, null, null);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && tabControl != null)
            {
                tabControl.SelectedIndexChanged -= OnSelectedIndexChanged;
            }

            base.Dispose(disposing);
        }
    }
}
