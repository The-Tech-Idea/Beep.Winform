using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsHeaderDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsHeaderActionList(this)
            };
        }
    }

    public sealed class BeepFormsHeaderActionList : DesignerActionList
    {
        private readonly BeepFormsHeaderDesigner _designer;

        public BeepFormsHeaderActionList(BeepFormsHeaderDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public bool AutoBindFormsHost
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsHeader.AutoBindFormsHost));
            set => _designer.SetProperty(nameof(BeepFormsHeader.AutoBindFormsHost), value);
        }

        public bool ShowActiveBlock
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsHeader.ShowActiveBlock));
            set => _designer.SetProperty(nameof(BeepFormsHeader.ShowActiveBlock), value);
        }

        public bool ShowStateSummary
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsHeader.ShowStateSummary));
            set => _designer.SetProperty(nameof(BeepFormsHeader.ShowStateSummary), value);
        }

        public void ShowFullContext()
        {
            ShowActiveBlock = true;
            ShowStateSummary = true;
        }

        public void ShowTitleOnly()
        {
            ShowActiveBlock = false;
            ShowStateSummary = false;
        }

        public void ShowBlockContextOnly()
        {
            ShowActiveBlock = true;
            ShowStateSummary = false;
        }

        public void ShowStateSummaryOnly()
        {
            ShowActiveBlock = false;
            ShowStateSummary = true;
        }

        public void RestoreDefaultHeaderLayout()
        {
            AutoBindFormsHost = true;
            ShowActiveBlock = true;
            ShowStateSummary = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Header"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoBindFormsHost), "Auto Bind Forms Host", "Header", "Automatically resolve a nearby BeepForms host when FormsHost is not set explicitly."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowActiveBlock), "Show Active Block", "Header", "Show the active block name in the header context line."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowStateSummary), "Show State Summary", "Header", "Show query mode and dirty-state summary in the header context line."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowFullContext), "Show Full Context", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowTitleOnly), "Title Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowBlockContextOnly), "Block Context Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowStateSummaryOnly), "State Summary Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultHeaderLayout), "Restore Default Layout", "Quick Presets", true));

            return items;
        }
    }
}