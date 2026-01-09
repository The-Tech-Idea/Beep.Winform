using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Widgets;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepFormWidgetDesigner : BaseWidgetDesigner
    {
        public BeepFormWidget? FormWidget => Component as BeepFormWidget;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
        {
            var lists = new DesignerActionListCollection();
            lists.Add(new BeepFormWidgetActionList(this));
            return lists;
        }
    }

    public class BeepFormWidgetActionList : DesignerActionList
    {
        private readonly BeepFormWidgetDesigner _designer;

        public BeepFormWidgetActionList(BeepFormWidgetDesigner designer) : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Form")]
        [Description("Visual style of the form widget")]
        public FormWidgetStyle Style
        {
            get => _designer.GetProperty<FormWidgetStyle>("Style");
            set => _designer.SetProperty("Style", value);
        }

        [Category("Form")]
        [Description("Title of the form")]
        public string Title
        {
            get => _designer.GetProperty<string>("Title") ?? string.Empty;
            set => _designer.SetProperty("Title", value);
        }

        public void ConfigureAsContactForm() { Style = FormWidgetStyle.FieldGroup; }
        public void ConfigureAsLoginForm() { Style = FormWidgetStyle.InputCard; }
        public void ConfigureAsValidationPanel() { Style = FormWidgetStyle.ValidationPanel; }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsContactForm", "Contact Form", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLoginForm", "Login Form", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsValidationPanel", "Validation Panel", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            return items;
        }
    }
}
