using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormsHostDesigner : ParentControlDesigner
    {
        private IComponentChangeService? _changeService;
        private DesignerActionListCollection? _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        internal void SetProperty(string propertyName, object? value)
        {
            if (Component == null)
            {
                return;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return;
            }

            object? currentValue = property.GetValue(Component);
            if (ReferenceEquals(currentValue, value) || Equals(currentValue, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, currentValue, value);
        }

        internal T? GetProperty<T>(string propertyName)
        {
            if (Component == null)
            {
                return default;
            }

            PropertyDescriptor? property = TypeDescriptor.GetProperties(Component)[propertyName];
            if (property == null)
            {
                return default;
            }

            object? value = property.GetValue(Component);
            return value is T typedValue ? typedValue : default;
        }

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepFormsHostActionList(this)
            };
    }

    public sealed class BeepFormsHostActionList : DesignerActionList
    {
        private readonly BeepFormsHostDesigner _designer;

        public BeepFormsHostActionList(BeepFormsHostDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public string FormName
        {
            get => _designer.GetProperty<string>(nameof(BeepForms.FormName)) ?? string.Empty;
            set => _designer.SetProperty(nameof(BeepForms.FormName), value);
        }

        public bool AutoCreateBlocksFromDefinition
        {
            get => _designer.GetProperty<bool>(nameof(BeepForms.AutoCreateBlocksFromDefinition));
            set => _designer.SetProperty(nameof(BeepForms.AutoCreateBlocksFromDefinition), value);
        }

        public BeepFormsDefinition? Definition
        {
            get => _designer.GetProperty<BeepFormsDefinition>(nameof(BeepForms.Definition));
            set => _designer.SetProperty(nameof(BeepForms.Definition), value);
        }

        public void CreateStarterDefinition()
        {
            BeepFormsDefinition definition = Definition?.Clone() ?? new BeepFormsDefinition();
            string resolvedFormName = string.IsNullOrWhiteSpace(FormName) ? "BeepForm" : FormName;

            definition.FormName = resolvedFormName;
            if (string.IsNullOrWhiteSpace(definition.Title))
            {
                definition.Title = resolvedFormName;
            }

            if (definition.Blocks.Count == 0)
            {
                definition.Blocks.Add(new BeepBlockDefinition
                {
                    BlockName = "MainBlock",
                    Caption = "Main Block",
                    PresentationMode = BeepBlockPresentationMode.Record,
                    Fields =
                    {
                        new BeepFieldDefinition
                        {
                            FieldName = "Name",
                            Label = "Name",
                            EditorKey = "text",
                            ControlType = BeepFieldControlTypeRegistry.ResolveDefaultControlType("text"),
                            BindingProperty = BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(nameof(BeepTextBox), "text"),
                            Order = 0
                        }
                    }
                });
            }

            Definition = definition;
        }

        public void RebuildBlocksNow()
        {
            if (_designer.Component is BeepForms forms)
            {
                forms.RebuildBlocksFromDefinition();
            }
        }

        public void RestoreDefinitionDefaults()
        {
            AutoCreateBlocksFromDefinition = true;
            if (Definition == null)
            {
                CreateStarterDefinition();
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Definition"));
            items.Add(new DesignerActionPropertyItem(nameof(FormName), "Form Name", "Definition", "Logical form name used for FormsManager coordination."));
            items.Add(new DesignerActionPropertyItem(nameof(AutoCreateBlocksFromDefinition), "Auto Create Blocks", "Definition", "Automatically materialize block controls from the assigned definition."));
            items.Add(new DesignerActionPropertyItem(nameof(Definition), "Definition", "Definition", "Edit the form definition, block set, and field composition."));

            items.Add(new DesignerActionHeaderItem("Quick Actions"));
            items.Add(new DesignerActionMethodItem(this, nameof(CreateStarterDefinition), "Create Starter Definition", "Quick Actions", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RebuildBlocksNow), "Rebuild Blocks Now", "Quick Actions", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RestoreDefinitionDefaults), "Restore Definition Defaults", "Quick Actions", true));

            return items;
        }
    }
}