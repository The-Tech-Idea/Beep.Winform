using System.ComponentModel;
using System.ComponentModel.Design;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.ActionLists
{
    /// <summary>
    /// Smart-tag action list for a DockPanel component.
    /// Exposes the most-used docking properties as quick-action links
    /// and lets the developer change dock position or detach the panel.
    /// </summary>
    internal sealed class DockPanelActionList : DesignerActionList
    {
        private readonly DockPanel _panel;
        private readonly DockPanelDesigner _designer;

        public DockPanelActionList(IComponent component, DockPanelDesigner designer)
            : base(component)
        {
            _panel = (DockPanel)component;
            _designer = designer;
        }

        // --- editable properties surfaced as smart-tag property items ---

        [Category("Docking")]
        [Description("Display title shown in the panel tab")]
        public string Title
        {
            get => _panel.Title;
            set => _designer.SetPanelProperty(nameof(DockPanel.Title), value);
        }

        [Category("Docking")]
        [Description("The edge this panel docks to")]
        public DockPosition DockPosition
        {
            get => _panel.DockPosition;
            set => _designer.SetPanelProperty(nameof(DockPanel.DockPosition), value);
        }

        [Category("Docking")]
        [Description("Whether the user can close this panel")]
        public bool CanClose
        {
            get => _panel.CanClose;
            set => _designer.SetPanelProperty(nameof(DockPanel.CanClose), value);
        }

        [Category("Docking")]
        [Description("Whether this panel can float as a separate window")]
        public bool CanFloat
        {
            get => _panel.CanFloat;
            set => _designer.SetPanelProperty(nameof(DockPanel.CanFloat), value);
        }

        [Category("Docking")]
        [Description("Whether this panel can be auto-hidden to a side tab strip")]
        public bool CanAutoHide
        {
            get => _panel.CanAutoHide;
            set => _designer.SetPanelProperty(nameof(DockPanel.CanAutoHide), value);
        }

        [Category("Docking")]
        [Description("Preferred width when docked Left or Right")]
        public int PreferredWidth
        {
            get => _panel.PreferredWidth;
            set => _designer.SetPanelProperty(nameof(DockPanel.PreferredWidth), value);
        }

        [Category("Docking")]
        [Description("Preferred height when docked Top or Bottom")]
        public int PreferredHeight
        {
            get => _panel.PreferredHeight;
            set => _designer.SetPanelProperty(nameof(DockPanel.PreferredHeight), value);
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Docking"));
            items.Add(new DesignerActionPropertyItem(nameof(Title),       "Title",           "Docking", "Tab label"));
            items.Add(new DesignerActionPropertyItem(nameof(DockPosition), "Dock Position",   "Docking", "Edge to dock to"));
            items.Add(new DesignerActionPropertyItem(nameof(PreferredWidth),  "Width",   "Docking", "Width when docked Left/Right"));
            items.Add(new DesignerActionPropertyItem(nameof(PreferredHeight), "Height",  "Docking", "Height when docked Top/Bottom"));

            items.Add(new DesignerActionHeaderItem("Behaviour"));
            items.Add(new DesignerActionPropertyItem(nameof(CanClose),    "Can Close",       "Behaviour", "Show close button"));
            items.Add(new DesignerActionPropertyItem(nameof(CanFloat),    "Can Float",       "Behaviour", "Allow tear-off floating"));
            items.Add(new DesignerActionPropertyItem(nameof(CanAutoHide), "Can Auto-Hide",   "Behaviour", "Allow auto-hide to tab strip"));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, nameof(DockLeft),   "Dock Left",   "Actions", "Set position to Left",   true));
            items.Add(new DesignerActionMethodItem(this, nameof(DockRight),  "Dock Right",  "Actions", "Set position to Right",  true));
            items.Add(new DesignerActionMethodItem(this, nameof(DockTop),    "Dock Top",    "Actions", "Set position to Top",    true));
            items.Add(new DesignerActionMethodItem(this, nameof(DockBottom), "Dock Bottom", "Actions", "Set position to Bottom", true));
            items.Add(new DesignerActionMethodItem(this, nameof(DockFill),   "Dock Fill",   "Actions", "Set position to Fill",   true));
            items.Add(new DesignerActionMethodItem(this, nameof(StackWithPrevious), "Stack with Previous", "Actions", "Stack this panel with the previous panel", true));
            items.Add(new DesignerActionMethodItem(this, nameof(StackWithNext),     "Stack with Next",     "Actions", "Stack this panel with the next panel", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveEarlier),       "Move Earlier",        "Actions", "Move this panel earlier in its stack", true));
            items.Add(new DesignerActionMethodItem(this, nameof(MoveLater),         "Move Later",          "Actions", "Move this panel later in its stack", true));

            return items;
        }

        public void DockLeft()   => _designer.MovePanel(DockPosition.Left);
        public void DockRight()  => _designer.MovePanel(DockPosition.Right);
        public void DockTop()    => _designer.MovePanel(DockPosition.Top);
        public void DockBottom() => _designer.MovePanel(DockPosition.Bottom);
        public void DockFill()   => _designer.MovePanel(DockPosition.Fill);
        public void StackWithPrevious() => _designer.StackWithPreviousPanel();
        public void StackWithNext() => _designer.StackWithNextPanel();
        public void MoveEarlier() => _designer.MoveEarlierInStack();
        public void MoveLater() => _designer.MoveLaterInStack();
    }
}
