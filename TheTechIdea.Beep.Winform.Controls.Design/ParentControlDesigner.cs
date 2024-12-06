using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Editors
{
    public class BeepTabsDesigner : ParentControlDesigner
    {
        private ISelectionService _selectionService;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            if (component is BeepTabs beepTabs)
            {
                EnableDesignMode(beepTabs.HeaderPanel, "HeaderPanel");

                if (beepTabs._headerPanel != null)
                {
                    foreach (Control control in beepTabs._headerPanel.Controls)
                    {
                        if (control is BeepButton button)
                        {
                            EnableDesignMode(button, button.Name ?? $"Button_{button.Text}");
                            button.Click += (sender, e) => SelectTabAtDesignTime(beepTabs, button);
                        }
                    }

                    beepTabs._headerPanel.ControlAdded += (sender, e) =>
                    {
                        if (e.Control is BeepButton button)
                        {
                            EnableDesignMode(button, button.Name ?? $"Button_{button.Text}");
                            button.Click += (sender, e) => SelectTabAtDesignTime(beepTabs, button);
                        }
                    };
                }
            }

            _selectionService = (ISelectionService)GetService(typeof(ISelectionService));
        }

        private void SelectTabAtDesignTime(BeepTabs beepTabs, BeepButton button)
        {
            if (button.Tag is SimpleItem item)
            {
                beepTabs.SelectTab(item.ReferenceID);

                // Highlight the selected tab in the designer
                _selectionService?.SetSelectedComponents(new[] { beepTabs }, SelectionTypes.Primary);
            }
        }
    }

}
