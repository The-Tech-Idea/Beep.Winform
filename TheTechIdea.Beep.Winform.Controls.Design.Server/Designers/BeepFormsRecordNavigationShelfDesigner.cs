using System;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsRecordNavigationShelfDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsRecordNavigationShelfActionList(this)
            };
        }
    }

    public sealed class BeepFormsRecordNavigationShelfActionList : DesignerActionList
    {
        private readonly BeepFormsRecordNavigationShelfDesigner _designer;

        public BeepFormsRecordNavigationShelfActionList(BeepFormsRecordNavigationShelfDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public BeepFormsRecordNavigationShelfButtons NavigationButtons
        {
            get => _designer.GetProperty<BeepFormsRecordNavigationShelfButtons>(nameof(BeepFormsRecordNavigationShelf.NavigationButtons));
            set => _designer.SetProperty(nameof(BeepFormsRecordNavigationShelf.NavigationButtons), value);
        }

        public FlowDirection NavigationFlowDirection
        {
            get => _designer.GetProperty<FlowDirection>(nameof(BeepFormsRecordNavigationShelf.NavigationFlowDirection));
            set => _designer.SetProperty(nameof(BeepFormsRecordNavigationShelf.NavigationFlowDirection), value);
        }

        public void ShowAllNavigationButtons()
        {
            NavigationButtons = BeepFormsRecordNavigationShelfButtons.All;
        }

        public void ShowCoreNavigationButtons()
        {
            NavigationButtons = BeepFormsRecordNavigationShelfButtons.Core;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Navigation Shelf"));
            items.Add(new DesignerActionPropertyItem(nameof(NavigationButtons), "Navigation Buttons", "Navigation Shelf",
                "Select which record navigation buttons are shown."));
            items.Add(new DesignerActionPropertyItem(nameof(NavigationFlowDirection), "Flow Direction", "Navigation Shelf",
                "Controls the flow direction used by the navigation button row."));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllNavigationButtons), "Show All Buttons", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowCoreNavigationButtons), "Core Buttons Only", "Presets", true));

            return items;
        }
    }
}
