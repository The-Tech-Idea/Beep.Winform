using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.FlyoutMenus;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers.FlyoutMenus
{
    public class BeepFlyoutMenuDesigner : BaseBeepControlDesigner
    {
        private DesignerVerbCollection? _verbs;

        public BeepFlyoutMenu? FlyoutMenu => Component as BeepFlyoutMenu;

        public DesignerVerbCollection CustomVerbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Toggle Menu", OnToggleMenu),
                        new DesignerVerb("Add Sample Items", OnAddSampleItems),
                        new DesignerVerb("Clear Items", OnClearItems)
                    };
                }
                return _verbs;
            }
        }

        public override DesignerVerbCollection Verbs => CustomVerbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepFlyoutMenuActionList(this));
            return lists;
        }

        private void OnToggleMenu(object? sender, EventArgs e) => ToggleMenu();
        private void OnAddSampleItems(object? sender, EventArgs e) => AddSampleItems();
        private void OnClearItems(object? sender, EventArgs e) => ClearItems();

        public void ToggleMenu()
        {
            if (FlyoutMenu == null) return;
            SetProperty("IsOn", !GetProperty<bool>("IsOn"));
        }

        public void AddSampleItems()
        {
            SetProperty("ListItems", new System.ComponentModel.BindingList<TheTechIdea.Beep.Winform.Controls.Models.SimpleItem>
            {
                new() { Text = "Item 1" },
                new() { Text = "Item 2" },
                new() { Text = "Item 3" }
            });
        }

        public void ClearItems()
        {
            SetProperty("ListItems", new System.ComponentModel.BindingList<TheTechIdea.Beep.Winform.Controls.Models.SimpleItem>());
        }
    }

    public class BeepFlyoutMenuActionList : DesignerActionList
    {
        private readonly BeepFlyoutMenuDesigner _designer;

        public BeepFlyoutMenuActionList(BeepFlyoutMenuDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepFlyoutMenu? FlyoutMenu => Component as BeepFlyoutMenu;

        #region Properties

        [Category("Flyout")]
        [Description("Direction the flyout menu expands")]
        public SlideDirection FlyoutDirection
        {
            get => _designer.GetProperty<SlideDirection>("FlyoutDirection");
            set => _designer.SetProperty("FlyoutDirection", value);
        }

        [Category("Flyout")]
        [Description("Controls when the label is displayed")]
        public FlyoutMenuLabelVisibility LabelVisibility
        {
            get => _designer.GetProperty<FlyoutMenuLabelVisibility>("LabelVisibility");
            set => _designer.SetProperty("LabelVisibility", value);
        }

        [Category("Layout")]
        [Description("Position of the label relative to the dropdown button")]
        public LabelPosition LabelPosition
        {
            get => _designer.GetProperty<LabelPosition>("LabelPosition");
            set => _designer.SetProperty("LabelPosition", value);
        }

        [Category("Flyout")]
        [Description("Minimum touch target width in pixels")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>("MinTouchTargetWidth");
            set => _designer.SetProperty("MinTouchTargetWidth", value);
        }

        [Category("Appearance")]
        [Description("Control text (used as label)")]
        public string Text
        {
            get => _designer.GetProperty<string>("Text") ?? "Flyout Menu";
            set => _designer.SetProperty("Text", value);
        }

        #endregion

        #region Actions

        public void ToggleMenu() => _designer.ToggleMenu();
        public void AddSampleItems() => _designer.AddSampleItems();
        public void ClearItems() => _designer.ClearItems();

        public void SetDirectionBottom() => FlyoutDirection = SlideDirection.Bottom;
        public void SetDirectionLeft() => FlyoutDirection = SlideDirection.Left;
        public void SetDirectionRight() => FlyoutDirection = SlideDirection.Right;

        #endregion

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Flyout"));
            items.Add(new DesignerActionPropertyItem("FlyoutDirection", "Direction:", "Flyout"));
            items.Add(new DesignerActionPropertyItem("LabelVisibility", "Label Visibility:", "Flyout"));
            items.Add(new DesignerActionPropertyItem("LabelPosition", "Label Position:", "Flyout"));
            items.Add(new DesignerActionPropertyItem("MinTouchTargetWidth", "Min Touch Width:", "Flyout"));

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem("Text", "Label Text:", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Direction Presets"));
            items.Add(new DesignerActionMethodItem(this, "SetDirectionBottom", "Bottom (Default)", "Direction Presets", true));
            items.Add(new DesignerActionMethodItem(this, "SetDirectionLeft", "Left", "Direction Presets", false));
            items.Add(new DesignerActionMethodItem(this, "SetDirectionRight", "Right", "Direction Presets", false));

            items.Add(new DesignerActionHeaderItem("Actions"));
            items.Add(new DesignerActionMethodItem(this, "ToggleMenu", "Toggle Menu", "Actions", true));
            items.Add(new DesignerActionMethodItem(this, "AddSampleItems", "Add Sample Items", "Actions", false));
            items.Add(new DesignerActionMethodItem(this, "ClearItems", "Clear Items", "Actions", false));

            return items;
        }
    }
}
