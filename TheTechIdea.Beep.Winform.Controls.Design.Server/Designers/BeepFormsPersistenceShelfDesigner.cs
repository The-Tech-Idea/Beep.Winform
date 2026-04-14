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
    public sealed class BeepFormsPersistenceShelfDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsPersistenceShelfActionList(this)
            };
        }
    }

    public sealed class BeepFormsPersistenceShelfActionList : DesignerActionList
    {
        private readonly BeepFormsPersistenceShelfDesigner _designer;

        public BeepFormsPersistenceShelfActionList(BeepFormsPersistenceShelfDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public bool AutoBindFormsHost
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsPersistenceShelf.AutoBindFormsHost));
            set => _designer.SetProperty(nameof(BeepFormsPersistenceShelf.AutoBindFormsHost), value);
        }

        public BeepFormsPersistenceShelfButtons PersistenceButtons
        {
            get => _designer.GetProperty<BeepFormsPersistenceShelfButtons>(nameof(BeepFormsPersistenceShelf.PersistenceButtons));
            set => _designer.SetProperty(nameof(BeepFormsPersistenceShelf.PersistenceButtons), value);
        }

        public FlowDirection PersistenceFlowDirection
        {
            get => _designer.GetProperty<FlowDirection>(nameof(BeepFormsPersistenceShelf.PersistenceFlowDirection));
            set => _designer.SetProperty(nameof(BeepFormsPersistenceShelf.PersistenceFlowDirection), value);
        }

        public void ShowAllPersistenceActions()
        {
            PersistenceButtons = BeepFormsPersistenceShelfButtons.All;
        }

        public void ShowCommitOnly()
        {
            PersistenceButtons = BeepFormsPersistenceShelfButtons.Commit;
        }

        public void ShowRollbackOnly()
        {
            PersistenceButtons = BeepFormsPersistenceShelfButtons.Rollback;
        }

        public void FlowLeftToRight()
        {
            PersistenceFlowDirection = FlowDirection.LeftToRight;
        }

        public void FlowRightToLeft()
        {
            PersistenceFlowDirection = FlowDirection.RightToLeft;
        }

        public void RestoreDefaultLayout()
        {
            AutoBindFormsHost = true;
            PersistenceButtons = BeepFormsPersistenceShelfButtons.All;
            PersistenceFlowDirection = FlowDirection.LeftToRight;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Persistence Shelf"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoBindFormsHost), "Auto Bind Forms Host", "Persistence Shelf", "Automatically resolve a nearby BeepForms host when FormsHost is not set explicitly."));
            items.Add(new DesignerActionPropertyItem(nameof(PersistenceButtons), "Persistence Buttons", "Persistence Shelf", "Select which persistence buttons are shown."));
            items.Add(new DesignerActionPropertyItem(nameof(PersistenceFlowDirection), "Flow Direction", "Persistence Shelf", "Select the layout direction for the persistence shelf button row."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllPersistenceActions), "Show All Persistence Actions", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowCommitOnly), "Commit Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowRollbackOnly), "Rollback Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowLeftToRight), "Flow Left To Right", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowRightToLeft), "Flow Right To Left", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultLayout), "Restore Default Layout", "Quick Presets", true));

            return items;
        }
    }
}