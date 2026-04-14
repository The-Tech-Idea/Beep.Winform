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

        [Category("Control")]
        [Description("Enable or disable the widget")]
        public bool IsEnabled
        {
            get => _designer.GetProperty<bool>("IsEnabled");
            set => _designer.SetProperty("IsEnabled", value);
        }

        [Category("Control")]
        [Description("Show or hide the control label")]
        public bool ShowLabel
        {
            get => _designer.GetProperty<bool>("ShowLabel");
            set => _designer.SetProperty("ShowLabel", value);
        }

        [Category("Control")]
        [Description("Selected option for dropdown and option-based styles")]
        public string SelectedOption
        {
            get => _designer.GetProperty<string>("SelectedOption") ?? string.Empty;
            set => _designer.SetProperty("SelectedOption", value);
        }

        [Category("Control")]
        [Description("Search text used by search-style controls")]
        public string SearchText
        {
            get => _designer.GetProperty<string>("SearchText") ?? string.Empty;
            set => _designer.SetProperty("SearchText", value);
        }

        public void ConfigureAsToggleSwitch()
        {
            Style = ControlWidgetStyle.ToggleSwitch;
            ShowLabel = true;
            IsEnabled = true;
        }

        public void ConfigureAsSlider()
        {
            Style = ControlWidgetStyle.Slider;
            ShowLabel = true;
            IsEnabled = true;
        }

        public void ConfigureAsDatePicker()
        {
            Style = ControlWidgetStyle.DatePicker;
            ShowLabel = true;
            IsEnabled = true;
        }

        public void ConfigureAsSearchBox()
        {
            Style = ControlWidgetStyle.SearchBox;
            ShowLabel = false;
            SearchText = "Search";
            IsEnabled = true;
        }

        public void ConfigureAsDropdownFilter()
        {
            Style = ControlWidgetStyle.DropdownFilter;
            ShowLabel = true;
            SelectedOption = "Option 1";
            IsEnabled = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsToggleSwitch", "Toggle Switch", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSlider", "Slider", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDatePicker", "Date Picker", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsSearchBox", "Search Box", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsDropdownFilter", "Dropdown Filter", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("IsEnabled", "Is Enabled", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowLabel", "Show Label", "Properties"));
            items.Add(new DesignerActionPropertyItem("SelectedOption", "Selected Option", "Properties"));
            items.Add(new DesignerActionPropertyItem("SearchText", "Search Text", "Properties"));
            return items;
        }
    }
}
