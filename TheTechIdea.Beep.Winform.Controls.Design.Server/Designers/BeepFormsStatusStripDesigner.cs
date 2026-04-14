using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsStatusStripDesigner : BaseBeepParentControlDesigner
    {
        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            return new DesignerActionListCollection
            {
                new BeepFormsStatusStripActionList(this)
            };
        }
    }

    public sealed class BeepFormsStatusStripActionList : DesignerActionList
    {
        private readonly BeepFormsStatusStripDesigner _designer;

        public BeepFormsStatusStripActionList(BeepFormsStatusStripDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public bool AutoBindFormsHost
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.AutoBindFormsHost));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.AutoBindFormsHost), value);
        }

        public bool ShowStatusLine
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.ShowStatusLine));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.ShowStatusLine), value);
        }

        public bool ShowMessageLine
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.ShowMessageLine));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.ShowMessageLine), value);
        }

        public bool ShowCoordinationLine
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.ShowCoordinationLine));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.ShowCoordinationLine), value);
        }

        public bool ShowSavepointLine
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.ShowSavepointLine));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.ShowSavepointLine), value);
        }

        public bool ShowAlertLine
        {
            get => _designer.GetProperty<bool>(nameof(BeepFormsStatusStrip.ShowAlertLine));
            set => _designer.SetProperty(nameof(BeepFormsStatusStrip.ShowAlertLine), value);
        }

        public void ShowAllLines()
        {
            ShowStatusLine = true;
            ShowMessageLine = true;
            ShowCoordinationLine = true;
            ShowSavepointLine = true;
            ShowAlertLine = true;
        }

        public void ShowWorkflowOnly()
        {
            ShowStatusLine = true;
            ShowMessageLine = true;
            ShowCoordinationLine = true;
            ShowSavepointLine = false;
            ShowAlertLine = false;
        }

        public void ShowStatusOnly()
        {
            ShowStatusLine = true;
            ShowMessageLine = false;
            ShowCoordinationLine = false;
            ShowSavepointLine = false;
            ShowAlertLine = false;
        }

        public void ShowWorkflowAndSavepoints()
        {
            ShowStatusLine = true;
            ShowMessageLine = true;
            ShowCoordinationLine = true;
            ShowSavepointLine = true;
            ShowAlertLine = false;
        }

        public void RestoreDefaultLayout()
        {
            AutoBindFormsHost = true;
            ShowAllLines();
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Status Strip"));
            items.Add(new DesignerActionPropertyItem(nameof(AutoBindFormsHost), "Auto Bind Forms Host", "Status Strip", "Automatically resolve a nearby BeepForms host when FormsHost is not set explicitly."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowStatusLine), "Show Status Line", "Status Strip", "Show the primary status line."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowMessageLine), "Show Message Line", "Status Strip", "Show the shared current-message line."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowCoordinationLine), "Show Coordination Line", "Status Strip", "Show the coordination summary line."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowSavepointLine), "Show Savepoint Line", "Status Strip", "Show the savepoint summary line."));
            items.Add(new DesignerActionPropertyItem(nameof(ShowAlertLine), "Show Alert Line", "Status Strip", "Show the alert summary line."));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowAllLines), "Show All Lines", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowWorkflowOnly), "Workflow Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowWorkflowAndSavepoints), "Workflow + Savepoints", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ShowStatusOnly), "Status Only", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefaultLayout), "Restore Default Layout", "Quick Presets", true));

            return items;
        }
    }
}