using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepControlWidgetDesigner : BaseWidgetDesigner
    {
        public BeepControlWidget? ControlWidget => Component as BeepControlWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepControlWidgetActionList(this));
            return lists;
        }
    }

    public class BeepControlWidgetActionList : DesignerActionList
    {
        private readonly BeepControlWidgetDesigner _designer;

        public BeepControlWidgetActionList(BeepControlWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Control")]
        [Description("Visual style of the control widget")]
        public ControlWidgetStyle Style
        {
            get => _designer.GetProperty<ControlWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Control")]
        [Description("Title/label for the control")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsToggleSwitch() { Style = ControlWidgetStyle.ToggleSwitch; }
        public void ConfigureAsSlider() { Style = ControlWidgetStyle.Slider; }
        public void ConfigureAsDatePicker() { Style = ControlWidgetStyle.DatePicker; }
        public void ConfigureAsSearchBox() { Style = ControlWidgetStyle.SearchBox; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsToggleSwitch", "Toggle Switch", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSlider", "Slider", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDatePicker", "Date Picker", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSearchBox", "Search Box", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
