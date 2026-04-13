using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Editor;
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
            get => _designer.GetProperty<string>(nameof(BeepBlock.BlockName)) ?? string.Empty;
            set => _designer.SetProperty(nameof(BeepBlock.BlockName), value);
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

        public void CreateStarterDefinition()
        {
            BeepBlockDefinition definition = Definition?.Clone() ?? new BeepBlockDefinition();
            string resolvedBlockName = string.IsNullOrWhiteSpace(BlockName) ? "MainBlock" : BlockName;

            definition.BlockName = resolvedBlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption) ? resolvedBlockName : definition.Caption;
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

            BeepBlockDefinition definition = Definition?.Clone() ?? new BeepBlockDefinition();
            string resolvedBlockName = string.IsNullOrWhiteSpace(BlockName) ? "MainBlock" : BlockName;

            definition.BlockName = string.IsNullOrWhiteSpace(definition.BlockName) ? resolvedBlockName : definition.BlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption) ? resolvedBlockName : definition.Caption;
            definition.Entity = entity;
            if (definition.Fields.Count == 0)
            {
                definition.Fields = entity.CreateFieldDefinitions();
            }

            Definition = definition;
        }

        public void RebuildFieldsFromEntity()
        {
            BeepBlockDefinition definition = Definition?.Clone() ?? new BeepBlockDefinition();
            string resolvedBlockName = string.IsNullOrWhiteSpace(BlockName) ? "MainBlock" : BlockName;

            definition.BlockName = string.IsNullOrWhiteSpace(definition.BlockName) ? resolvedBlockName : definition.BlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption) ? resolvedBlockName : definition.Caption;

            BeepBlockEntityDefinition entity = (Entity ?? definition.Entity ?? new BeepBlockEntityDefinition()).Clone();
            definition.Entity = entity;
            definition.Fields = entity.CreateFieldDefinitions();
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
            Definition = definition;
        }

        public void EditFieldDefaultPolicy()
        {
            using var dialog = new BeepFieldControlTypePolicyEditorForm();
            dialog.ShowDialog();
        }

        private void SetPresentationMode(BeepBlockPresentationMode presentationMode)
        {
            BeepBlockDefinition definition = CreateWorkingDefinition();

            definition.PresentationMode = presentationMode;
            Definition = definition;
        }

        private BeepBlockDefinition CreateWorkingDefinition()
        {
            BeepBlockDefinition definition = Definition?.Clone() ?? new BeepBlockDefinition();
            string resolvedBlockName = string.IsNullOrWhiteSpace(BlockName) ? "MainBlock" : BlockName;

            definition.BlockName = string.IsNullOrWhiteSpace(definition.BlockName) ? resolvedBlockName : definition.BlockName;
            definition.Caption = string.IsNullOrWhiteSpace(definition.Caption) ? resolvedBlockName : definition.Caption;
            definition.Entity ??= new BeepBlockEntityDefinition();
            return definition;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Block"));
            items.Add(new DesignerActionPropertyItem(nameof(BlockName), "Block Name", "Block", "Logical block name used by the BeepForms host."));
            items.Add(new DesignerActionPropertyItem(nameof(Definition), "Definition", "Block", "Edit the block definition, presentation mode, navigation, and UI field composition."));
            items.Add(new DesignerActionMethodItem(this, nameof(OpenSetupWizard), "Open Setup Wizard...", "Block", true));
            items.Add(new DesignerActionMethodItem(this, nameof(EditFieldProperties), "Edit Field Properties...", "Block", true));
            items.Add(new DesignerActionMethodItem(this, nameof(EditFieldDefaultPolicy), "Edit Field Default Policy...", "Block", true));

            items.Add(new DesignerActionHeaderItem("Entity"));
            items.Add(new DesignerActionPropertyItem(nameof(Entity), "Entity Snapshot", "Entity", "Edit the typed entity, field, and structure metadata stored by the block UI."));
            items.Add(new DesignerActionMethodItem(this, nameof(CaptureEntityFromManager), "Capture Entity From Manager", "Entity", true));
            items.Add(new DesignerActionMethodItem(this, nameof(RebuildFieldsFromEntity), "Rebuild Fields From Entity", "Entity", true));

            items.Add(new DesignerActionHeaderItem("Quick Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(CreateStarterDefinition), "Create Starter Definition", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseRecordMode), "Use Record Mode", "Quick Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseGridMode), "Use Grid Mode", "Quick Presets", true));

            return items;
        }
    }
}