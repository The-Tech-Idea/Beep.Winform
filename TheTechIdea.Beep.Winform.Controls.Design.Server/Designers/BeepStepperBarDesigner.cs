using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepStepperBarDesigner : ControlDesigner
    {
        private DesignerActionListCollection? _actionLists;
        private DesignerVerbCollection? _verbs;
        private IComponentChangeService? _changeService;

        private BeepStepperBar? Stepper => Component as BeepStepperBar;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepStepperBarActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Next Step", (_, _) => NextStep()),
                new DesignerVerb("Previous Step", (_, _) => PreviousStep()),
                new DesignerVerb("Configure Task Workflow", (_, _) => ConfigureTaskWorkflow()),
                new DesignerVerb("Configure Order Tracking", (_, _) => ConfigureOrderTracking())
            };

        public T? GetProperty<T>(string propertyName)
        {
            if (Component == null) return default;
            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null) return default;
            object? value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        public void SetProperty(string propertyName, object value)
        {
            if (Component == null) return;
            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null || property.IsReadOnly) return;
            object? currentValue = property.GetValue(Component);
            if (Equals(currentValue, value)) return;
            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        public void NextStep() => Stepper?.NextStep();
        public void PreviousStep() => Stepper?.PreviousStep();
        public void ConfigureTaskWorkflow() => Stepper?.ConfigureForTaskWorkflow();
        public void ConfigureOrderTracking() => Stepper?.ConfigureForOrderTracking();
    }

    public class BeepStepperBarActionList : DesignerActionList
    {
        private readonly BeepStepperBarDesigner _designer;

        public BeepStepperBarActionList(BeepStepperBarDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepStepperBar? Stepper => Component as BeepStepperBar;

        [Category("Appearance")]
        [Description("Layout direction")]
        public Orientation Orientation
        {
            get => _designer.GetProperty<Orientation>(nameof(BeepStepperBar.Orientation));
            set => _designer.SetProperty(nameof(BeepStepperBar.Orientation), value);
        }

        [Category("Appearance")]
        [Description("Visual style of step indicators")]
        public StepDisplayMode DisplayMode
        {
            get => _designer.GetProperty<StepDisplayMode>(nameof(BeepStepperBar.DisplayMode));
            set => _designer.SetProperty(nameof(BeepStepperBar.DisplayMode), value);
        }

        [Category("Appearance")]
        [Description("Minimum touch target width for step buttons")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>(nameof(BeepStepperBar.MinTouchTargetWidth));
            set => _designer.SetProperty(nameof(BeepStepperBar.MinTouchTargetWidth), value);
        }

        [Category("Appearance")]
        [Description("Controls when step text labels are visible")]
        public TabLabelVisibility StepLabelVisibility
        {
            get => _designer.GetProperty<TabLabelVisibility>(nameof(BeepStepperBar.StepLabelVisibility));
            set => _designer.SetProperty(nameof(BeepStepperBar.StepLabelVisibility), value);
        }

        [Category("Business Logic")]
        [Description("Number of steps")]
        public int StepCount
        {
            get => _designer.GetProperty<int>(nameof(BeepStepperBar.StepCount));
            set => _designer.SetProperty(nameof(BeepStepperBar.StepCount), value);
        }

        [Category("Business Logic")]
        [Description("Currently active step")]
        public int CurrentStep
        {
            get => _designer.GetProperty<int>(nameof(BeepStepperBar.CurrentStep));
            set => _designer.SetProperty(nameof(BeepStepperBar.CurrentStep), value);
        }

        [Category("Behavior")]
        [Description("Show connector lines between steps")]
        public bool ShowConnectorLines
        {
            get => _designer.GetProperty<bool>(nameof(BeepStepperBar.ShowConnectorLines));
            set => _designer.SetProperty(nameof(BeepStepperBar.ShowConnectorLines), value);
        }

        [Category("Behavior")]
        [Description("Allow step navigation")]
        public bool AllowStepNavigation
        {
            get => _designer.GetProperty<bool>(nameof(BeepStepperBar.AllowStepNavigation));
            set => _designer.SetProperty(nameof(BeepStepperBar.AllowStepNavigation), value);
        }

        public void NextStep() => _designer.NextStep();
        public void PreviousStep() => _designer.PreviousStep();
        public void ConfigureTaskWorkflow() => _designer.ConfigureTaskWorkflow();
        public void ConfigureOrderTracking() => _designer.ConfigureOrderTracking();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Orientation), "Orientation", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(DisplayMode), "Display Mode", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(MinTouchTargetWidth), "Min Touch Target Width", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(StepLabelVisibility), "Step Label Visibility", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Steps"));
            items.Add(new DesignerActionPropertyItem(nameof(StepCount), "Step Count", "Steps"));
            items.Add(new DesignerActionPropertyItem(nameof(CurrentStep), "Current Step", "Steps"));

            items.Add(new DesignerActionHeaderItem("Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowConnectorLines), "Show Connector Lines", "Behavior"));
            items.Add(new DesignerActionPropertyItem(nameof(AllowStepNavigation), "Allow Step Navigation", "Behavior"));

            items.Add(new DesignerActionHeaderItem("Navigation"));
            items.Add(new DesignerActionMethodItem(this, nameof(NextStep), "Next Step", "Navigation", true));
            items.Add(new DesignerActionMethodItem(this, nameof(PreviousStep), "Previous Step", "Navigation", true));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureTaskWorkflow), "Task Workflow", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ConfigureOrderTracking), "Order Tracking", "Presets", true));

            return items;
        }
    }
}
