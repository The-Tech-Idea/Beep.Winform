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
    public sealed class BeepFormsCommandBarDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsCommandBarActionList(this)
            };
        }
    }

    public sealed class BeepFormsCommandBarActionList : DesignerActionList
    {
        private readonly BeepFormsCommandBarDesigner _designer;

        public BeepFormsCommandBarActionList(BeepFormsCommandBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public bool AutoBindFormsHost
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsCommandBar.AutoBindFormsHost));
            set => _designer.SetProperty(nameof(BeepFormsCommandBar.AutoBindFormsHost), value);
        }

        public BeepFormsCommandBarButtons CommandButtons
        {
            get => _designer.GetProperty<BeepFormsCommandBarButtons>(nameof(BeepFormsCommandBar.CommandButtons));
            set => _designer.SetProperty(nameof(BeepFormsCommandBar.CommandButtons), value);
        }

        public FlowDirection CommandFlowDirection
        {
            get => _designer.GetProperty<FlowDirection>(nameof(BeepFormsCommandBar.CommandFlowDirection));
            set => _designer.SetProperty(nameof(BeepFormsCommandBar.CommandFlowDirection), value);
        }

        public void ShowAllCommands()
        {
            CommandButtons = BeepFormsCommandBarButtons.All;
        }

        public void ShowBlockSelectorOnly()
        {
            CommandButtons = BeepFormsCommandBarButtons.BlockSelector;
        }

        public void ShowSyncOnly()
        {
            CommandButtons = BeepFormsCommandBarButtons.Sync;
        }

        public void FlowLeftToRight()
        {
            CommandFlowDirection = FlowDirection.LeftToRight;
        }

        public void FlowRightToLeft()
        {
            CommandFlowDirection = FlowDirection.RightToLeft;
        }

        public void RestoreDefaultLayout()
        {
            AutoBindFormsHost = true;
            CommandButtons = BeepFormsCommandBarButtons.All;
            CommandFlowDirection = FlowDirection.LeftToRight;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Command Bar"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoBindFormsHost), "Auto Bind Forms Host", "Command Bar", "Automatically resolve a nearby BeepForms host when FormsHost is not set explicitly."));
            items.Add(new DesignerActionPropertyItem(nameof(CommandButtons), "Command Buttons", "Command Bar", "Select which top-level form command buttons are shown."));
            items.Add(new DesignerActionPropertyItem(nameof(CommandFlowDirection), "Flow Direction", "Command Bar", "Select the layout direction for the form command button row."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllCommands), "Show All Commands", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowBlockSelectorOnly), "Block Selector Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowSyncOnly), "Sync Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowLeftToRight), "Flow Left To Right", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowRightToLeft), "Flow Right To Left", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultLayout), "Restore Default Layout", "Quick Presets", true));

            return items;
        }
    }
}