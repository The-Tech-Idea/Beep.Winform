using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Services;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    public sealed class BeepFormsDefinitionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            BeepFormsDefinition editable = value is BeepFormsDefinition definition
                ? definition.Clone()
                : CreateDefaultDefinition(context);

            using var dialog = new DefinitionObjectEditorForm<BeepFormsDefinition>(
                "Edit BeepForms Definition",
                editable,
                item => item.Clone());

            return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
        }

        private static BeepFormsDefinition CreateDefaultDefinition(ITypeDescriptorContext? context)
        {
            var definition = new BeepFormsDefinition();
            if (context?.Instance is BeepForms forms)
            {
                definition.FormName = string.IsNullOrWhiteSpace(forms.FormName) ? forms.Name : forms.FormName;
                definition.Title = definition.FormName;
            }

            return definition;
        }
    }

    public sealed class BeepBlockDefinitionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            BeepBlockDefinition editable = value is BeepBlockDefinition definition
                ? definition.Clone()
                : CreateDefaultDefinition(context);

            using var dialog = new DefinitionObjectEditorForm<BeepBlockDefinition>(
                "Edit BeepBlock Definition",
                editable,
                item => item.Clone());

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return value;
            }

            bool hadExplicitFieldDefinitions = value is BeepBlockDefinition existingDefinition
                && BeepBlockFieldDefinitionStateHelper.HasExplicitFieldDefinitions(existingDefinition);
            BeepBlockDefinition result = dialog.Result;
            BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(result, hadExplicitFieldDefinitions);
            return result;
        }

        private static BeepBlockDefinition CreateDefaultDefinition(ITypeDescriptorContext? context)
        {
            var definition = new BeepBlockDefinition();
            if (context?.Instance is BeepBlock block)
            {
                definition.BlockName = string.IsNullOrWhiteSpace(block.BlockName) ? block.Name : block.BlockName;
                definition.Caption = definition.BlockName;
            }

            return definition;
        }
    }

    public sealed class BeepBlockEntityDefinitionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            BeepBlockEntityDefinition editable = value is BeepBlockEntityDefinition definition
                ? definition.Clone()
                : CreateDefaultDefinition(context);

            using var dialog = new DefinitionObjectEditorForm<BeepBlockEntityDefinition>(
                "Edit Block Entity Snapshot",
                editable,
                item => item.Clone());

            return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
        }

        private static BeepBlockEntityDefinition CreateDefaultDefinition(ITypeDescriptorContext? context)
        {
            var definition = new BeepBlockEntityDefinition();
            if (context?.Instance is BeepBlock block)
            {
                definition.EntityName = string.IsNullOrWhiteSpace(block.BlockName) ? block.Name : block.BlockName;
                definition.Caption = definition.EntityName;
            }

            return definition;
        }
    }

    public sealed class BeepBlockNavigationDefinitionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            BeepBlockNavigationDefinition editable = value is BeepBlockNavigationDefinition definition
                ? definition.Clone()
                : new BeepBlockNavigationDefinition();

            using var dialog = new DefinitionObjectEditorForm<BeepBlockNavigationDefinition>(
                "Edit Block Navigation Settings",
                editable,
                item => item.Clone());

            return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
        }
    }

    public sealed class BeepBlockDefinitionCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            int nextIndex = value is IEnumerable<BeepBlockDefinition> existing ? existing.Count() + 1 : 1;
            var dialog = new DefinitionCollectionEditorForm<BeepBlockDefinition>(
                "Edit Block Definitions",
                IntegratedDefinitionEditorHelpers.ExtractItems<BeepBlockDefinition>(value, item => item.Clone()),
                () => IntegratedDefinitionFactories.CreateDefaultBlockDefinition(context, nextIndex++),
                item => item.Clone());

            using (dialog)
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
            }
        }
    }

    public sealed class BeepFieldDefinitionCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            int nextIndex = value is IEnumerable<BeepFieldDefinition> existing ? existing.Count() + 1 : 1;
            var dialog = new DefinitionCollectionEditorForm<BeepFieldDefinition>(
                "Edit Field Definitions",
                IntegratedDefinitionEditorHelpers.ExtractItems<BeepFieldDefinition>(value, item => item.Clone()),
                () => IntegratedDefinitionFactories.CreateDefaultFieldDefinition(nextIndex++),
                item => item.Clone());

            using (dialog)
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return value;
                }

                List<BeepFieldDefinition> result = dialog.Result;
                if (context?.Instance is BeepBlockDefinition ownerDefinition)
                {
                    var definitionDraft = ownerDefinition.Clone();
                    definitionDraft.Fields = result.Select(item => item.Clone()).ToList();
                    BeepBlockFieldDefinitionStateHelper.UpdateExplicitFieldState(definitionDraft, treatEmptyAsExplicit: true);
                    ownerDefinition.Metadata = definitionDraft.Metadata;
                }

                return result;
            }
        }
    }

    public sealed class BeepBlockEntityFieldDefinitionCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            int nextIndex = value is IEnumerable<BeepBlockEntityFieldDefinition> existing ? existing.Count() + 1 : 1;
            var dialog = new DefinitionCollectionEditorForm<BeepBlockEntityFieldDefinition>(
                "Edit Entity Field Snapshot",
                IntegratedDefinitionEditorHelpers.ExtractItems<BeepBlockEntityFieldDefinition>(value, item => item.Clone()),
                () => new BeepBlockEntityFieldDefinition
                {
                    FieldName = "Field" + nextIndex,
                    Label = "Field " + nextIndex++,
                    Order = nextIndex - 2
                },
                item => item.Clone());

            using (dialog)
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
            }
        }
    }

    public sealed class BeepFieldOptionDefinitionCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            int nextIndex = value is IEnumerable<BeepFieldOptionDefinition> existing ? existing.Count() + 1 : 1;
            var dialog = new DefinitionCollectionEditorForm<BeepFieldOptionDefinition>(
                "Edit Field Options",
                IntegratedDefinitionEditorHelpers.ExtractItems<BeepFieldOptionDefinition>(value, item => item.Clone()),
                () => IntegratedDefinitionFactories.CreateDefaultFieldOption(nextIndex++),
                item => item.Clone());

            using (dialog)
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
            }
        }
    }

    public sealed class BeepFormsBlockNameTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            var blockNames = ResolveAvailableBlockNames(context)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new StandardValuesCollection(blockNames);
        }

        private static IEnumerable<string> ResolveAvailableBlockNames(ITypeDescriptorContext? context)
        {
            if (context?.Instance is not BeepBlock block)
            {
                return Array.Empty<string>();
            }

            var formsHost = IntegratedDesignTimeResolver.FindFormsHost(block);
            if (formsHost?.Definition?.Blocks == null)
            {
                return Array.Empty<string>();
            }

            return formsHost.Definition.Blocks
                .Where(item => !string.IsNullOrWhiteSpace(item?.BlockName))
                .Select(item => item!.BlockName.Trim())
                .ToList();
        }
    }

    public sealed class BeepFieldEditorKeyTypeConverter : StringConverter
    {
        private static readonly string[] KnownEditorKeys = CreateKnownEditorKeys();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
            => new StandardValuesCollection(KnownEditorKeys);

        private static string[] CreateKnownEditorKeys()
        {
            var registry = new BeepBlockPresenterRegistry();
            registry.RegisterDefaults();

            var keys = new HashSet<string>(registry.Presenters.Select(presenter => presenter.Key), StringComparer.OrdinalIgnoreCase)
            {
                "lov",
                "option"
            };

            return keys
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }

    public sealed class BeepFieldControlTypeTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
            => new StandardValuesCollection(BeepFieldControlTypeRegistry.GetKnownControlTypes().ToArray());
    }

    public sealed class BeepFieldBindingPropertyTypeConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
            => new StandardValuesCollection(BeepFieldControlTypeRegistry.GetKnownBindingProperties(ResolveControlTypeName(context)).ToArray());

        private static string ResolveControlTypeName(ITypeDescriptorContext? context)
        {
            if (context?.Instance == null)
            {
                return string.Empty;
            }

            IEnumerable<object?> instances = context.Instance is Array array
                ? array.Cast<object?>()
                : new[] { context.Instance };

            foreach (var instance in instances)
            {
                if (instance == null)
                {
                    continue;
                }

                var type = instance.GetType();
                string? controlType = type.GetProperty("ControlType")?.GetValue(instance) as string;
                if (!string.IsNullOrWhiteSpace(controlType))
                {
                    return controlType;
                }

                string? editorKey = type.GetProperty("EditorKey")?.GetValue(instance) as string;
                if (!string.IsNullOrWhiteSpace(editorKey))
                {
                    return BeepFieldControlTypeRegistry.ResolveDefaultControlType(editorKey);
                }
            }

            return string.Empty;
        }
    }

    public sealed class BeepStringDictionaryEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            var editable = value is IDictionary<string, string> existing
                ? new Dictionary<string, string>(existing, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string title = context?.PropertyDescriptor?.DisplayName switch
            {
                { Length: > 0 } displayName => "Edit " + displayName,
                _ => "Edit Metadata"
            };

            using var dialog = new StringDictionaryEditorForm(title, editable);
            return dialog.ShowDialog() == DialogResult.OK ? dialog.Result : value;
        }
    }

    public sealed class BeepFormsDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepFormsDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Form Definition" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepBlockDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepBlockDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Block Definition" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepBlockEntityDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepBlockEntityDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Entity Snapshot" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepBlockEntityFieldDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepBlockEntityFieldDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Entity Field" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepBlockNavigationDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepBlockNavigationDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Navigation Settings" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepBlockNavigationCommandDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepBlockNavigationCommandDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Command Settings" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepFieldDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepFieldDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Field Definition" : base.ConvertTo(context, culture, value, destinationType);
    }

    public sealed class BeepFieldOptionDefinitionTypeConverter : ExpandableObjectConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(typeof(BeepFieldOptionDefinition), attributes);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            => destinationType == typeof(string) ? value?.ToString() ?? "Option" : base.ConvertTo(context, culture, value, destinationType);
    }

    internal sealed class DefinitionObjectEditorForm<T> : Form
        where T : class
    {
        private readonly PropertyGrid _propertyGrid;
        public T Result { get; }

        public DefinitionObjectEditorForm(string title, T value, Func<T, T> cloneItem)
        {
            Result = cloneItem(value);
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(760, 560);

            _propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                SelectedObject = Result,
                ToolbarVisible = false,
                HelpVisible = true
            };

            Controls.Add(_propertyGrid);
            Controls.Add(CreateFooter());
        }

        private Control CreateFooter()
        {
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8, 8, 8, 8)
            };

            var okButton = new Button { Text = "OK", Width = 96, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancel", Width = 96, DialogResult = DialogResult.Cancel };
            footer.Controls.Add(okButton);
            footer.Controls.Add(cancelButton);
            AcceptButton = okButton;
            CancelButton = cancelButton;
            return footer;
        }
    }

    internal sealed class StringDictionaryEditorForm : Form
    {
        private readonly DataGridView _grid;
        private Dictionary<string, string> _result;

        public Dictionary<string, string> Result => new(_result, StringComparer.OrdinalIgnoreCase);

        public StringDictionaryEditorForm(string title, IDictionary<string, string> values)
        {
            _result = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);

            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(760, 460);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KeyColumn",
                HeaderText = "Key",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 40f
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ValueColumn",
                HeaderText = "Value",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 60f
            });

            foreach (var item in values.OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase))
            {
                _grid.Rows.Add(item.Key, item.Value);
            }

            Controls.Add(_grid);
            Controls.Add(CreateToolbar());
            Controls.Add(CreateFooter());
        }

        private Control CreateToolbar()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(8, 6, 8, 6)
            };

            panel.Controls.Add(CreateActionButton("Add", (_, _) => _grid.Rows.Add(string.Empty, string.Empty)));
            panel.Controls.Add(CreateActionButton("Remove", (_, _) => RemoveSelectedRow()));
            return panel;
        }

        private Control CreateFooter()
        {
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8, 8, 8, 8)
            };

            var okButton = new Button { Text = "OK", Width = 96 };
            okButton.Click += (_, _) => AcceptChanges();

            var cancelButton = new Button { Text = "Cancel", Width = 96, DialogResult = DialogResult.Cancel };
            footer.Controls.Add(okButton);
            footer.Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
            return footer;
        }

        private static Button CreateActionButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Width = 72,
                Height = 26,
                Margin = new Padding(0, 0, 8, 0)
            };
            button.Click += clickHandler;
            return button;
        }

        private void RemoveSelectedRow()
        {
            if (_grid.CurrentRow == null || _grid.CurrentRow.IsNewRow)
            {
                return;
            }

            _grid.Rows.Remove(_grid.CurrentRow);
        }

        private void AcceptChanges()
        {
            if (!TryBuildResult(out var result, out var message))
            {
                MessageBox.Show(this, message, "Metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _result = result;
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool TryBuildResult(out Dictionary<string, string> result, out string message)
        {
            result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            message = string.Empty;

            for (int rowIndex = 0; rowIndex < _grid.Rows.Count; rowIndex++)
            {
                DataGridViewRow row = _grid.Rows[rowIndex];
                if (row.IsNewRow)
                {
                    continue;
                }

                string key = Convert.ToString(row.Cells[0].Value)?.Trim() ?? string.Empty;
                string currentValue = Convert.ToString(row.Cells[1].Value) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(key))
                {
                    if (string.IsNullOrWhiteSpace(currentValue))
                    {
                        continue;
                    }

                    message = "Metadata keys cannot be empty.";
                    _grid.CurrentCell = row.Cells[0];
                    return false;
                }

                if (result.ContainsKey(key))
                {
                    message = "Metadata keys must be unique.";
                    _grid.CurrentCell = row.Cells[0];
                    return false;
                }

                result[key] = currentValue;
            }

            return true;
        }
    }

    internal sealed class DefinitionCollectionEditorForm<T> : Form
        where T : class
    {
        private readonly List<T> _items;
        private readonly Func<T> _itemFactory;
        private readonly Func<T, T> _cloneItem;
        private readonly ListBox _itemsList;
        private readonly PropertyGrid _propertyGrid;

        public List<T> Result
        {
            get
            {
                NormalizeOrderedItems();
                return _items.Select(_cloneItem).ToList();
            }
        }

        public DefinitionCollectionEditorForm(
            string title,
            IEnumerable<T> items,
            Func<T> itemFactory,
            Func<T, T> cloneItem)
        {
            _itemFactory = itemFactory;
            _cloneItem = cloneItem;
            _items = items.Select(cloneItem).ToList();

            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(920, 560);

            _itemsList = new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };
            _itemsList.SelectedIndexChanged += (_, _) => _propertyGrid.SelectedObject = GetSelectedItem();

            _propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                ToolbarVisible = false,
                HelpVisible = true
            };
            _propertyGrid.PropertyValueChanged += (_, _) => RefreshItems(_itemsList.SelectedIndex);

            Controls.Add(CreateBody());
            Controls.Add(CreateFooter());

            RefreshItems(0);
        }

        private Control CreateBody()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 260,
                FixedPanel = FixedPanel.Panel1
            };

            split.Panel1.Controls.Add(_itemsList);
            split.Panel1.Controls.Add(CreateListToolbar());
            split.Panel2.Controls.Add(_propertyGrid);

            return split;
        }

        private Control CreateListToolbar()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(8, 6, 8, 6)
            };

            panel.Controls.Add(CreateActionButton("Add", (_, _) => AddItem()));
            panel.Controls.Add(CreateActionButton("Remove", (_, _) => RemoveSelectedItem()));
            panel.Controls.Add(CreateActionButton("Up", (_, _) => MoveSelectedItem(-1)));
            panel.Controls.Add(CreateActionButton("Down", (_, _) => MoveSelectedItem(1)));
            return panel;
        }

        private Control CreateFooter()
        {
            var footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8, 8, 8, 8)
            };

            var okButton = new Button { Text = "OK", Width = 96, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancel", Width = 96, DialogResult = DialogResult.Cancel };
            footer.Controls.Add(okButton);
            footer.Controls.Add(cancelButton);
            AcceptButton = okButton;
            CancelButton = cancelButton;
            return footer;
        }

        private static Button CreateActionButton(string text, EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = text,
                Width = 72,
                Height = 26,
                Margin = new Padding(0, 0, 8, 0)
            };
            button.Click += clickHandler;
            return button;
        }

        private void AddItem()
        {
            _items.Add(_itemFactory());
            NormalizeOrderedItems();
            RefreshItems(_items.Count - 1);
        }

        private void RemoveSelectedItem()
        {
            int index = _itemsList.SelectedIndex;
            if (index < 0 || index >= _items.Count)
            {
                return;
            }

            _items.RemoveAt(index);
            NormalizeOrderedItems();
            RefreshItems(Math.Min(index, _items.Count - 1));
        }

        private void MoveSelectedItem(int offset)
        {
            int index = _itemsList.SelectedIndex;
            int targetIndex = index + offset;
            if (index < 0 || targetIndex < 0 || targetIndex >= _items.Count)
            {
                return;
            }

            (_items[index], _items[targetIndex]) = (_items[targetIndex], _items[index]);
            NormalizeOrderedItems();
            RefreshItems(targetIndex);
        }

        private void NormalizeOrderedItems()
        {
            IntegratedDefinitionEditorHelpers.NormalizeItemOrder(_items);
        }

        private void RefreshItems(int selectedIndex)
        {
            _itemsList.BeginUpdate();
            _itemsList.Items.Clear();
            foreach (T item in _items)
            {
                _itemsList.Items.Add(item);
            }
            _itemsList.EndUpdate();

            if (_items.Count == 0)
            {
                _propertyGrid.SelectedObject = null;
                return;
            }

            int safeIndex = Math.Max(0, Math.Min(selectedIndex, _items.Count - 1));
            _itemsList.SelectedIndex = safeIndex;
            _propertyGrid.SelectedObject = _items[safeIndex];
        }

        private T? GetSelectedItem()
        {
            return _itemsList.SelectedIndex >= 0 && _itemsList.SelectedIndex < _items.Count
                ? _items[_itemsList.SelectedIndex]
                : null;
        }
    }

    internal static class IntegratedDefinitionEditorHelpers
    {
        public static List<T> ExtractItems<T>(object? value, Func<T, T> cloneItem)
            where T : class
        {
            return value is IEnumerable<T> items
                ? items.Select(cloneItem).ToList()
                : new List<T>();
        }

        public static void NormalizeItemOrder<T>(IList<T> items)
            where T : class
        {
            var orderProperty = typeof(T).GetProperty("Order");
            if (orderProperty == null || !orderProperty.CanWrite || orderProperty.PropertyType != typeof(int))
            {
                return;
            }

            for (int index = 0; index < items.Count; index++)
            {
                orderProperty.SetValue(items[index], index);
            }
        }
    }

    internal static class IntegratedDefinitionFactories
    {
        public static BeepBlockDefinition CreateDefaultBlockDefinition(ITypeDescriptorContext? context, int nextIndex = 1)
        {
            string blockName = context?.Instance is BeepForms forms && !string.IsNullOrWhiteSpace(forms.FormName)
                ? forms.FormName + nextIndex + "Block"
                : (nextIndex <= 1 ? "MainBlock" : "MainBlock" + nextIndex);

            return new BeepBlockDefinition
            {
                BlockName = blockName,
                Caption = blockName,
                PresentationMode = BeepBlockPresentationMode.Record,
                Fields = new List<BeepFieldDefinition>
                {
                    new()
                    {
                        FieldName = "Name",
                        Label = "Name",
                        EditorKey = "text",
                        ControlType = BeepFieldControlTypeRegistry.ResolveDefaultControlType("text"),
                        BindingProperty = BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(nameof(BeepTextBox), "text"),
                        Order = 0
                    }
                }
            };
        }

        public static BeepFieldDefinition CreateDefaultFieldDefinition(int nextIndex)
        {
            string editorKey = "text";
            string controlType = BeepFieldControlTypeRegistry.ResolveDefaultControlType(editorKey);

            return new BeepFieldDefinition
            {
                FieldName = "Field" + nextIndex,
                Label = "Field " + nextIndex,
                EditorKey = editorKey,
                ControlType = controlType,
                BindingProperty = BeepFieldControlTypeRegistry.ResolveDefaultBindingProperty(controlType, editorKey),
                Order = nextIndex - 1
            };
        }

        public static BeepFieldOptionDefinition CreateDefaultFieldOption(int nextIndex)
        {
            return new BeepFieldOptionDefinition
            {
                Text = "Option " + nextIndex,
                Name = "Option" + nextIndex,
                Value = nextIndex
            };
        }
    }

    internal static class IntegratedDesignTimeResolver
    {
        public static BeepForms? FindFormsHost(Control? source)
        {
            Control? current = source?.Parent;
            while (current != null)
            {
                var resolved = FindDescendantFormsHost(current, source);
                if (resolved != null)
                {
                    return resolved;
                }

                current = current.Parent;
            }

            if (source?.Site?.Container != null)
            {
                foreach (IComponent component in source.Site.Container.Components)
                {
                    if (component is BeepForms formsHost)
                    {
                        return formsHost;
                    }
                }
            }

            return null;
        }

        private static BeepForms? FindDescendantFormsHost(Control parent, Control? exclude)
        {
            var stack = new Stack<Control>();
            stack.Push(parent);

            while (stack.Count > 0)
            {
                Control current = stack.Pop();
                if (!ReferenceEquals(current, exclude) && current is BeepForms formsHost)
                {
                    return formsHost;
                }

                for (int index = current.Controls.Count - 1; index >= 0; index--)
                {
                    stack.Push(current.Controls[index]);
                }
            }

            return null;
        }
    }

}