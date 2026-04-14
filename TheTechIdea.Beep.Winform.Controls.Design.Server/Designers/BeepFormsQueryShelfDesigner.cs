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
    public sealed class BeepFormsQueryShelfDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsQueryShelfActionList(this)
            };
        }
    }

    public sealed class BeepFormsQueryShelfActionList : DesignerActionList
    {
        private readonly BeepFormsQueryShelfDesigner _designer;

        public BeepFormsQueryShelfActionList(BeepFormsQueryShelfDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public bool AutoBindFormsHost
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsQueryShelf.AutoBindFormsHost));
            set => _designer.SetProperty(nameof(BeepFormsQueryShelf.AutoBindFormsHost), value);
        }

        public BeepFormsQueryShelfButtons QueryButtons
        {
            get => _designer.GetProperty<BeepFormsQueryShelfButtons>(nameof(BeepFormsQueryShelf.QueryButtons));
            set => _designer.SetProperty(nameof(BeepFormsQueryShelf.QueryButtons), value);
        }

        public FlowDirection QueryFlowDirection
        {
            get => _designer.GetProperty<FlowDirection>(nameof(BeepFormsQueryShelf.QueryFlowDirection));
            set => _designer.SetProperty(nameof(BeepFormsQueryShelf.QueryFlowDirection), value);
        }

        public bool ShowQueryContextCaption
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsQueryShelf.ShowQueryContextCaption));
            set => _designer.SetProperty(nameof(BeepFormsQueryShelf.ShowQueryContextCaption), value);
        }

        public BeepFormsQueryShelfCaptionMode QueryCaptionMode
        {
            get => _designer.GetProperty<BeepFormsQueryShelfCaptionMode>(nameof(BeepFormsQueryShelf.QueryCaptionMode));
            set => _designer.SetProperty(nameof(BeepFormsQueryShelf.QueryCaptionMode), value);
        }

        public void ShowAllQueryActions()
        {
            QueryButtons = BeepFormsQueryShelfButtons.All;
        }

        public void ShowEnterQueryOnly()
        {
            QueryButtons = BeepFormsQueryShelfButtons.EnterQuery;
        }

        public void ShowExecuteQueryOnly()
        {
            QueryButtons = BeepFormsQueryShelfButtons.ExecuteQuery;
        }

        public void ShowTitleOnlyCaption()
        {
            ShowQueryContextCaption = true;
            QueryCaptionMode = BeepFormsQueryShelfCaptionMode.TitleOnly;
        }

        public void ShowTargetOnlyCaption()
        {
            ShowQueryContextCaption = true;
            QueryCaptionMode = BeepFormsQueryShelfCaptionMode.TargetOnly;
        }

        public void ShowTargetPlusModeCaption()
        {
            ShowQueryContextCaption = true;
            QueryCaptionMode = BeepFormsQueryShelfCaptionMode.TargetPlusMode;
        }

        public void HideCaption()
        {
            ShowQueryContextCaption = false;
        }

        public void FlowLeftToRight()
        {
            QueryFlowDirection = FlowDirection.LeftToRight;
        }

        public void FlowRightToLeft()
        {
            QueryFlowDirection = FlowDirection.RightToLeft;
        }

        public void RestoreDefaultLayout()
        {
            AutoBindFormsHost = true;
            QueryButtons = BeepFormsQueryShelfButtons.All;
            QueryFlowDirection = FlowDirection.LeftToRight;
            ShowQueryContextCaption = true;
            QueryCaptionMode = BeepFormsQueryShelfCaptionMode.TargetPlusMode;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Query Shelf"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoBindFormsHost), "Auto Bind Forms Host", "Query Shelf", "Automatically resolve a nearby BeepForms host when FormsHost is not set explicitly."));
            items.Add(new DesignerActionPropertyItem(nameof(QueryButtons), "Query Buttons", "Query Shelf", "Select which query-mode buttons are shown."));
            items.Add(new DesignerActionPropertyItem(nameof(QueryFlowDirection), "Flow Direction", "Query Shelf", "Select the layout direction for the query shelf button row."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowQueryContextCaption), "Show Query Context Caption", "Query Shelf", "Show the caption that identifies the block currently targeted by query actions."));
            items.Add(new DesignerActionPropertyItem(nameof(QueryCaptionMode), "Query Caption Mode", "Query Shelf", "Select whether the caption shows a title only, the current target, or the target plus query-mode state."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllQueryActions), "Show All Query Actions", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowEnterQueryOnly), "Enter Query Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowExecuteQueryOnly), "Execute Query Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowTitleOnlyCaption), "Caption: Title Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowTargetOnlyCaption), "Caption: Target Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowTargetPlusModeCaption), "Caption: Target + Mode", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(HideCaption), "Caption: Hidden", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowLeftToRight), "Flow Left To Right", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FlowRightToLeft), "Flow Right To Left", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultLayout), "Restore Default Layout", "Quick Presets", true));

            return items;
        }
    }
}