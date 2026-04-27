using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public class BeepStepperBreadCrumbDesigner : ControlDesigner
    {
        private DesignerActionListCollection? _actionLists;
        private DesignerVerbCollection? _verbs;
        private IComponentChangeService? _changeService;

        private BeepStepperBreadCrumb? Stepper => Component as BeepStepperBreadCrumb;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepStepperBreadCrumbActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Add Step", (_, _) => AddStep()),
                new DesignerVerb("Remove Last Step", (_, _) => RemoveLastStep())
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

        public void AddStep()
        {
            if (Stepper == null) return;
            PropertyDescriptor? listItemsProp = TypeDescriptor.GetProperties(Stepper)["ListItems"];
            if (listItemsProp?.GetValue(Stepper) is BindingList<SimpleItem> listItems)
            {
                int newIndex = listItems.Count + 1;
                var newItem = new SimpleItem { ID = newIndex, Name = $"Step {newIndex}", Text = $"Step {newIndex}", IsChecked = false };
                listItems.Add(newItem);
            }
        }

        public void RemoveLastStep()
        {
            if (Stepper == null) return;
            PropertyDescriptor? listItemsProp = TypeDescriptor.GetProperties(Stepper)["ListItems"];
            if (listItemsProp?.GetValue(Stepper) is BindingList<SimpleItem> listItems && listItems.Count > 0)
            {
                listItems.RemoveAt(listItems.Count - 1);
            }
        }
    }

    public class BeepStepperBreadCrumbActionList : DesignerActionList
    {
        private readonly BeepStepperBreadCrumbDesigner _designer;

        public BeepStepperBreadCrumbActionList(BeepStepperBreadCrumbDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        protected BeepStepperBreadCrumb? Stepper => Component as BeepStepperBreadCrumb;

        [Category("Appearance")]
        [Description("Layout direction")]
        public Orientation Orientation
        {
            get => _designer.GetProperty<Orientation>(nameof(BeepStepperBreadCrumb.Orientation));
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.Orientation), value);
        }

        [Category("Appearance")]
        [Description("Chevron direction")]
        public ChevronDirection Direction
        {
            get => _designer.GetProperty<ChevronDirection>(nameof(BeepStepperBreadCrumb.Direction));
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.Direction), value);
        }

        [Category("Appearance")]
        [Description("Minimum touch target width for step buttons")]
        public int MinTouchTargetWidth
        {
            get => _designer.GetProperty<int>(nameof(BeepStepperBreadCrumb.MinTouchTargetWidth));
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.MinTouchTargetWidth), value);
        }

        [Category("Appearance")]
        [Description("Controls when step text labels are visible")]
        public TabLabelVisibility StepLabelVisibility
        {
            get => _designer.GetProperty<TabLabelVisibility>(nameof(BeepStepperBreadCrumb.StepLabelVisibility));
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.StepLabelVisibility), value);
        }

        [Category("Appearance")]
        [Description("Default image path for checked steps")]
        public string CheckImage
        {
            get => _designer.GetProperty<string>(nameof(BeepStepperBreadCrumb.CheckImage)) ?? string.Empty;
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.CheckImage), value);
        }

        [Category("Behavior")]
        [Description("Selected step index")]
        public int SelectedIndex
        {
            get => _designer.GetProperty<int>(nameof(BeepStepperBreadCrumb.SelectedIndex));
            set => _designer.SetProperty(nameof(BeepStepperBreadCrumb.SelectedIndex), value);
        }

        public void AddStep() => _designer.AddStep();
        public void RemoveLastStep() => _designer.RemoveLastStep();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Orientation), "Orientation", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(Direction), "Direction", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(MinTouchTargetWidth), "Min Touch Target Width", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(StepLabelVisibility), "Step Label Visibility", "Appearance"));
            items.Add(new DesignerActionPropertyItem(nameof(CheckImage), "Check Image", "Appearance"));

            items.Add(new DesignerActionHeaderItem("Steps"));
            items.Add(new DesignerActionPropertyItem(nameof(SelectedIndex), "Selected Index", "Steps"));
            items.Add(new DesignerActionMethodItem(this, nameof(AddStep), "Add Step", "Steps", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RemoveLastStep), "Remove Last Step", "Steps", true));

            return items;
        }
    }
}
