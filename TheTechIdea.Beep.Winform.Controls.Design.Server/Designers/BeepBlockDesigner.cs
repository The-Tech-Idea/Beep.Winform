using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Editors;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepBlockDesigner : ParentControlDesigner
    {
        private IComponentChangeService? _changeService;
        private DesignerActionListCollection? _actionLists;
        private IDesignTimeServiceLease? _serviceLease;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            try
            {
                _serviceLease = DesignTimeBeepServiceManager.AcquireForBlockDesigner();
            }
            catch
            {
                _serviceLease?.Dispose();
                _serviceLease = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _actionLists = null;
                _serviceLease?.Dispose();
                _serviceLease = null;
            }

            base.Dispose(disposing);
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

        internal IDMEEditor? GetEditor()
            => _serviceLease?.BeepService?.DMEEditor;

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new BeepBlockActionList(this)
            };
    }

    public sealed class BeepBlockActionList : DesignerActionList
    {
        private readonly BeepBlockDesigner _designer;

        public BeepBlockActionList(BeepBlockDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        public string BlockName
        {
            get
            {
                BeepBlockDefinition? definition = Definition;
                if (!string.IsNullOrWhiteSpace(definition?.BlockName))
                {
                    return definition.BlockName;
                }

                return GetComponentBlockName();
            }
            set
            {
                string resolvedBlockName = value?.Trim() ?? string.Empty;
                BeepBlockDefinition definition = CreateWorkingDefinition();
                string previousBlockName = string.IsNullOrWhiteSpace(definition.BlockName)
                    ? GetComponentBlockName()
                    : definition.BlockName;

                definition.BlockName = resolvedBlockName;
                if (string.IsNullOrWhiteSpace(definition.ManagerBlockName) ||
                    string.Equals(definition.ManagerBlockName, previousBlockName, StringComparison.OrdinalIgnoreCase))
                {
                    definition.ManagerBlockName = resolvedBlockName;
                }

                if (string.IsNullOrWhiteSpace(definition.Caption) ||
                    string.Equals(definition.Caption, previousBlockName, StringComparison.OrdinalIgnoreCase))
                {
                    definition.Caption = resolvedBlockName;
                }

                _designer.SetProperty(nameof(BeepBlock.BlockName), resolvedBlockName);
                Definition = definition;
            }
        }

        public BeepBlockDefinition? Definition
        {
            get => _designer.GetProperty<BeepBlockDefinition>(nameof(BeepBlock.Definition));
            set => _designer.SetProperty(nameof(BeepBlock.Definition), value);
        }

        public BeepBlockEntityDefinition? Entity
        {
            get => _designer.GetProperty<BeepBlockEntityDefinition>(nameof(BeepBlock.Entity));
            set => _designer.SetProperty(nameof(BeepBlock.Entity), value);
        }

        public string Caption
        {
            get => CreateWorkingDefinition().Caption;
            set
            {
                BeepBlockDefinition definition = CreateWorkingDefinition();
                definition.Caption = value?.Trim() ?? string.Empty;
                Definition = definition;
            }
        }

        public string ManagerBlockName
        {
            get
            {
                BeepBlockDefinition definition = CreateWorkingDefinition();
                return string.IsNullOrWhiteSpace(definition.ManagerBlockName)
                    ? definition.BlockName
                    : definition.ManagerBlockName;
            }
            set
            {
                BeepBlockDefinition definition = CreateWorkingDefinition();
                definition.ManagerBlockName = string.IsNullOrWhiteSpace(value)
                    ? definition.BlockName
                    : value.Trim();
                Definition = definition;
            }
        }

        public BeepBlockPresentationMode PresentationMode
        {
            get => CreateWorkingDefinition().PresentationMode;
            set => SetPresentationMode(value);
        }

        public BeepBlockNavigationDefinition Navigation
        {
            get => CreateWorkingDefinition().Navigation?.Clone() ?? new BeepBlockNavigationDefinition();
            set
            {
                BeepBlockDefinition definition = CreateWorkingDefinition();
                definition.Navigation = value?.Clone() ?? new BeepBlockNavigationDefinition();
                Definition = definition;
            }
        }

        public BeepBlockFieldControlsLayoutMode FieldControlsLayoutMode
        {
            get => BeepBlockFieldControlsLayoutModeHelper.Resolve(CreateWorkingDefinition());
            set
            {
                BeepBlockDefinition definition = CreateWorkingDefinition();
                if (definition.PresentationMode != BeepBlockPresentationMode.DesignerGenerated)
                {
                    BeepBlockFieldControlsLayoutModeHelper.Clear(definition);
                    Definition = definition;
                    return;
                }

                BeepBlockFieldControlsLayoutModeHelper.Apply(definition, value);
                Definition = definition;
            }
        }

        public void CreateStarterDefinition()
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();
            string resolvedBlockName = definition.BlockName;
            definition.Entity ??= new BeepBlockEntityDefinition();
            definition.Entity.EntityName = string.IsNullOrWhiteSpace(definition.Entity.EntityName) ? resolvedBlockName : definition.Entity.EntityName;
            definition.Entity.Caption = string.IsNullOrWhiteSpace(definition.Entity.Caption) ? definition.Caption : definition.Entity.Caption;
            if (definition.Entity.Fields.Count == 0)
            {
                definition.Entity.Fields.Add(new BeepBlockEntityFieldDefinition
                {
                    FieldName = "Name",
                    Label = "Name",
                    Order = 0
                });
            }

            if (definition.Fields.Count == 0)
            {
                definition.Fields = definition.Entity.CreateFieldDefinitions();
            }

            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definition);

            Definition = definition;
        }

        public void CaptureEntityFromManager()
        {
            if (Component is not BeepBlock block)
            {
                return;
            }

            BeepBlockEntityDefinition? entity = block.CreateEntitySnapshotFromManager();
            if (entity == null)
            {
                return;
            }

            BeepBlockDefinition definition = CreateWorkingDefinition();
            definition.Entity = entity;
            if (definition.Fields.Count == 0)
            {
                definition.Fields = entity.CreateFieldDefinitions();
            }

            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definition);

            Definition = definition;
        }

        public void RebuildFieldsFromEntity()
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();
            BeepBlockEntityDefinition entity = (Entity ?? definition.Entity ?? new BeepBlockEntityDefinition()).Clone();
            definition.Entity = entity;
            definition.Fields = entity.CreateFieldDefinitions();
            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definition);
            Definition = definition;
        }

        public void UseRecordMode()
        {
            SetPresentationMode(BeepBlockPresentationMode.Record);
        }

        public void UseGridMode()
        {
            SetPresentationMode(BeepBlockPresentationMode.Grid);
        }

        public void UseDesignerGeneratedMode()
        {
            SetPresentationMode(BeepBlockPresentationMode.DesignerGenerated);
        }

        public void UseStackedVerticalLayout()
        {
            ApplyDesignerGeneratedLayout(BeepBlockFieldControlsLayoutMode.StackedVertical);
        }

        public void UseLabelFieldPairsLayout()
        {
            ApplyDesignerGeneratedLayout(BeepBlockFieldControlsLayoutMode.LabelFieldPairs);
        }

        public void UseGridLayout()
        {
            ApplyDesignerGeneratedLayout(BeepBlockFieldControlsLayoutMode.GridLayout);
        }

        public void OpenSetupWizard()
        {
            if (Component is not BeepBlock block)
            {
                return;
            }

            using var wizard = new BeepBlockSetupWizardForm(block, _designer.GetEditor());
            if (wizard.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Definition = wizard.BuildUpdatedDefinition();
        }

        public void EditFieldProperties()
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();
            using var editorForm = new BeepBlockFieldEditorForm(definition.Fields, definition.Entity);
            if (editorForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            definition.Fields = editorForm.BuildFieldDefinitions();
            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definition, treatEmptyAsExplicit: true);
            Definition = definition;
        }

        public void EditFieldDefaultPolicy()
        {
            using var dialog = new BeepFieldControlTypePolicyEditorForm();
            dialog.ShowDialog();
        }

        public void EditNavigationSettings()
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();
            using var dialog = new DefinitionObjectEditorForm<BeepBlockNavigationDefinition>(
                "Edit Block Navigation Settings",
                definition.Navigation?.Clone() ?? new BeepBlockNavigationDefinition(),
                item => item.Clone());

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            definition.Navigation = dialog.Result?.Clone() ?? new BeepBlockNavigationDefinition();
            Definition = definition;
        }

        private void ApplyDesignerGeneratedLayout(BeepBlockFieldControlsLayoutMode layoutMode)
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();
            definition.PresentationMode = BeepBlockPresentationMode.DesignerGenerated;
            BeepBlockFieldControlsLayoutModeHelper.Apply(definition, layoutMode);
            Definition = definition;
        }

        private void SetPresentationMode(BeepBlockPresentationMode presentationMode)
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();

            definition.PresentationMode = presentationMode;
            if (presentationMode == BeepBlockPresentationMode.DesignerGenerated)
            {
                BeepBlockFieldControlsLayoutModeHelper.Apply(
                    definition,
                    BeepBlockFieldControlsLayoutModeHelper.Resolve(definition));
            }
            else
            {
                BeepBlockFieldControlsLayoutModeHelper.Clear(definition);
            }

            Definition = definition;
        }

        private BeepBlockDefinition CreateWorkingDefinition()
        {
            BeepBlockDefinition definition = Definition?.Clone() ?? new BeepBlockDefinition();
            string componentBlockName = GetComponentBlockName();
            string resolvedBlockName = string.IsNullOrWhiteSpace(definition.BlockName)
                ? (string.IsNullOrWhiteSpace(componentBlockName) ? "MainBlock" : componentBlockName)
                : definition.BlockName;

            definition.BlockName = resolvedBlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption) ? resolvedBlockName : definition.Caption;
            definition.ManagerBlockName = string.IsNullOrWhiteSpace(definition.ManagerBlockName) ? definition.BlockName : definition.ManagerBlockName;
            definition.Entity ??= new BeepBlockEntityDefinition();
            definition.Navigation ??= new BeepBlockNavigationDefinition();
            return definition;
        }

        private string GetComponentBlockName()
            => _designer.GetProperty<string>(nameof(BeepBlock.BlockName)) ?? string.Empty;

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();
            bool isDesignerGenerated = CreateWorkingDefinition().PresentationMode == BeepBlockPresentationMode.DesignerGenerated;

            items.Add(new DesignerActionHeaderItem("Block"));
            items.Add(new DesignerActionPropertyItem(nameof(BlockName), "Block Name", "Block", "Logical block name used by the BeepForms host."));
            items.Add(new DesignerActionPropertyItem(nameof(Caption), "Caption", "Block", "Header caption shown by the block surface."));
            items.Add(new DesignerActionPropertyItem(nameof(ManagerBlockName), "Manager Block Name", "Block", "Runtime block name used when the visual block points at a different FormsManager registration."));
            items.Add(new DesignerActionPropertyItem(nameof(PresentationMode), "Presentation Mode", "Block", "Choose Record, Grid, or DesignerGenerated presentation for this block."));
            items.Add(new DesignerActionPropertyItem(nameof(Navigation), "Navigation Settings", "Block", "Edit typed navigation overrides without opening the full definition object."));
            items.Add(new DesignerActionMethodItem(this, nameof(EditNavigationSettings), "Edit Navigation Settings...", "Block", true));
            items.Add(new DesignerActionPropertyItem(nameof(Definition), "Definition", "Block", "Edit the block definition, presentation mode, navigation, and UI field composition."));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenSetupWizard), "Open Setup Wizard...", "Block", true));
            items.Add(new DesignerActionMethodItem(this, nameof(EditFieldProperties), "Edit Field Properties...", "Block", true));
            items.Add(new DesignerActionMethodItem(this, nameof(EditFieldDefaultPolicy), "Edit Field Default Policy...", "Block", true));

            items.Add(new DesignerActionHeaderItem("Entity"));
            items.Add(new DesignerActionPropertyItem(nameof(Entity), "Entity Snapshot", "Entity", "Edit the typed entity, field, and structure metadata stored by the block UI."));
            items.Add(new DesignerActionMethodItem(this, nameof(CaptureEntityFromManager), "Capture Entity From Manager", "Entity", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RebuildFieldsFromEntity), "Rebuild Fields From Entity", "Entity", true));

            items.Add(new DesignerActionHeaderItem("Designer Generated"));
            if (isDesignerGenerated)
            {
                items.Add(new DesignerActionPropertyItem(nameof(FieldControlsLayoutMode), "Field Controls Layout", "Designer Generated", "Persist the generated field-controls layout metadata used by DesignerGenerated blocks."));
            }
            items.Add(new DesignerActionMethodItem(this, nameof(UseDesignerGeneratedMode), "Use Designer Generated Mode", "Designer Generated", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseStackedVerticalLayout), "Stacked Vertical Layout", "Designer Generated", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseLabelFieldPairsLayout), "Label Field Pairs Layout", "Designer Generated", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseGridLayout), "Grid Layout", "Designer Generated", true));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(CreateStarterDefinition), "Create Starter Definition", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseRecordMode), "Use Record Mode", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseGridMode), "Use Grid Mode", "Quick Presets", true));

            return items;
        }
    }
}