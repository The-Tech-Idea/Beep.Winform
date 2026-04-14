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

        [Category("Form")]
        [Description("Subtitle of the form widget")]
        public string Subtitle
        {
            get => _designer.GetProperty<string>("Subtitle") ?? string.Empty;
            set => _designer.SetProperty("Subtitle", value);
        }

        [Category("Form")]
        [Description("Description text shown with the form")]
        public string Description
        {
            get => _designer.GetProperty<string>("Description") ?? string.Empty;
            set => _designer.SetProperty("Description", value);
        }

        [Category("Form")]
        [Description("Show validation indicators")]
        public bool ShowValidation
        {
            get => _designer.GetProperty<bool>("ShowValidation");
            set => _designer.SetProperty("ShowValidation", value);
        }

        [Category("Form")]
        [Description("Show required-field indicators")]
        public bool ShowRequired
        {
            get => _designer.GetProperty<bool>("ShowRequired");
            set => _designer.SetProperty("ShowRequired", value);
        }

        [Category("Form")]
        [Description("Render the form in read-only mode")]
        public bool IsReadOnly
        {
            get => _designer.GetProperty<bool>("IsReadOnly");
            set => _designer.SetProperty("IsReadOnly", value);
        }

        [Category("Form")]
        [Description("Show step-progress UI")]
        public bool ShowProgress
        {
            get => _designer.GetProperty<bool>("ShowProgress");
            set => _designer.SetProperty("ShowProgress", value);
        }

        [Category("Form")]
        [Description("Current step for step-based forms")]
        public int CurrentStep
        {
            get => _designer.GetProperty<int>("CurrentStep");
            set => _designer.SetProperty("CurrentStep", value);
        }

        [Category("Form")]
        [Description("Total number of steps")]
        public int TotalSteps
        {
            get => _designer.GetProperty<int>("TotalSteps");
            set => _designer.SetProperty("TotalSteps", value);
        }

        [Category("Form")]
        [Description("Overall layout mode")]
        public FormLayout Layout
        {
            get => _designer.GetProperty<FormLayout>("Layout");
            set => _designer.SetProperty("Layout", value);
        }

        public void ConfigureAsContactForm()
        {
            Style = FormWidgetStyle.FieldGroup;
            Layout = FormLayout.Vertical;
            ShowValidation = true;
            ShowRequired = true;
            IsReadOnly = false;
            ShowProgress = false;
        }

        public void ConfigureAsLoginForm()
        {
            Style = FormWidgetStyle.InputCard;
            Layout = FormLayout.Vertical;
            ShowValidation = true;
            ShowRequired = true;
            IsReadOnly = false;
            ShowProgress = false;
        }

        public void ConfigureAsValidationPanel()
        {
            Style = FormWidgetStyle.ValidationPanel;
            Layout = FormLayout.Vertical;
            ShowValidation = true;
            ShowRequired = true;
            IsReadOnly = false;
        }

        public void ConfigureAsStepForm()
        {
            Style = FormWidgetStyle.FormStep;
            Layout = FormLayout.Vertical;
            ShowProgress = true;
            CurrentStep = 1;
            TotalSteps = 3;
            ShowValidation = true;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("Style Presets"));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsContactForm", "Contact Form", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsLoginForm", "Login Form", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsValidationPanel", "Validation Panel", "Style Presets", true));
            items.Add(new DesignerActionMethodItem(this, "ConfigureAsStepForm", "Step Form", "Style Presets", true));
            items.Add(new DesignerActionHeaderItem("Properties"));
            items.Add(new DesignerActionPropertyItem("Style", "Style", "Properties"));
            items.Add(new DesignerActionPropertyItem("Title", "Title", "Properties"));
            items.Add(new DesignerActionPropertyItem("Subtitle", "Subtitle", "Properties"));
            items.Add(new DesignerActionPropertyItem("Description", "Description", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowValidation", "Show Validation", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowRequired", "Show Required", "Properties"));
            items.Add(new DesignerActionPropertyItem("IsReadOnly", "Is Read Only", "Properties"));
            items.Add(new DesignerActionPropertyItem("ShowProgress", "Show Progress", "Properties"));
            items.Add(new DesignerActionPropertyItem("CurrentStep", "Current Step", "Properties"));
            items.Add(new DesignerActionPropertyItem("TotalSteps", "Total Steps", "Properties"));
            items.Add(new DesignerActionPropertyItem("Layout", "Layout", "Properties"));
            return items;
        }
    }
}
