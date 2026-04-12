using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models
{
    [TypeConverter("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepBlockEntityDefinitionTypeConverter, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public sealed class BeepBlockEntityDefinition
    {
        [NotifyParentProperty(true)]
        public string ConnectionName { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string EntityName { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string DatasourceEntityName { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string Caption { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string Description { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string DataSourceId { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public bool IsMasterBlock { get; set; }

        [NotifyParentProperty(true)]
        public string MasterBlockName { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string MasterKeyField { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string ForeignKeyField { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepBlockEntityFieldDefinitionCollectionEditor, TheTechIdea.Beep.Winform.Controls.Design.Server", typeof(UITypeEditor))]
        public List<BeepBlockEntityFieldDefinition> Fields { get; set; } = new();

        public List<BeepFieldDefinition> CreateFieldDefinitions()
        {
            if (Fields == null || Fields.Count == 0)
            {
                return new List<BeepFieldDefinition>();
            }

            return Fields
                .Where(field => field != null && !string.IsNullOrWhiteSpace(field.FieldName))
                .OrderBy(field => field!.Order)
                .Select((field, index) => field!.ToFieldDefinition(index))
                .ToList();
        }

        public BeepBlockEntityDefinition Clone()
        {
            var clone = new BeepBlockEntityDefinition
            {
                ConnectionName = ConnectionName,
                EntityName = EntityName,
                DatasourceEntityName = DatasourceEntityName,
                Caption = Caption,
                Description = Description,
                DataSourceId = DataSourceId,
                IsMasterBlock = IsMasterBlock,
                MasterBlockName = MasterBlockName,
                MasterKeyField = MasterKeyField,
                ForeignKeyField = ForeignKeyField
            };

            foreach (var field in Fields)
            {
                if (field != null)
                {
                    clone.Fields.Add(field.Clone());
                }
            }

            return clone;
        }

        public override string ToString()
        {
            string name = string.IsNullOrWhiteSpace(Caption) ? EntityName : Caption;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Entity Snapshot";
            }

            return Fields.Count == 0 ? name : $"{name} ({Fields.Count} fields)";
        }
    }

    [TypeConverter("TheTechIdea.Beep.Winform.Controls.Design.Server.Editors.BeepBlockEntityFieldDefinitionTypeConverter, TheTechIdea.Beep.Winform.Controls.Design.Server")]
    public sealed class BeepBlockEntityFieldDefinition
    {
        [NotifyParentProperty(true)]
        public string FieldName { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string Label { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string Description { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public string DataType { get; set; } = string.Empty;

        [NotifyParentProperty(true)]
        public DbFieldCategory Category { get; set; } = DbFieldCategory.String;

        [NotifyParentProperty(true)]
        public int Order { get; set; }

        [NotifyParentProperty(true)]
        public int Size { get; set; }

        [NotifyParentProperty(true)]
        public short NumericPrecision { get; set; }

        [NotifyParentProperty(true)]
        public short NumericScale { get; set; }

        [NotifyParentProperty(true)]
        public bool IsRequired { get; set; }

        [NotifyParentProperty(true)]
        public bool AllowDBNull { get; set; } = true;

        [NotifyParentProperty(true)]
        public bool IsPrimaryKey { get; set; }

        [NotifyParentProperty(true)]
        public bool IsUnique { get; set; }

        [NotifyParentProperty(true)]
        public bool IsIndexed { get; set; }

        [NotifyParentProperty(true)]
        public bool IsAutoIncrement { get; set; }

        [NotifyParentProperty(true)]
        public bool IsReadOnly { get; set; }

        [NotifyParentProperty(true)]
        public bool IsCheck { get; set; }

        public BeepFieldDefinition ToFieldDefinition(int order)
        {
            return new BeepFieldDefinition
            {
                FieldName = FieldName,
                Label = string.IsNullOrWhiteSpace(Label)
                    ? (string.IsNullOrWhiteSpace(Description) ? FieldName : Description)
                    : Label,
                EditorKey = ResolveEditorKey(),
                Order = order,
                Width = ResolveWidth(),
                IsVisible = true,
                IsReadOnly = IsReadOnly || IsAutoIncrement
            };
        }

        public BeepBlockEntityFieldDefinition Clone()
        {
            return new BeepBlockEntityFieldDefinition
            {
                FieldName = FieldName,
                Label = Label,
                Description = Description,
                DataType = DataType,
                Category = Category,
                Order = Order,
                Size = Size,
                NumericPrecision = NumericPrecision,
                NumericScale = NumericScale,
                IsRequired = IsRequired,
                AllowDBNull = AllowDBNull,
                IsPrimaryKey = IsPrimaryKey,
                IsUnique = IsUnique,
                IsIndexed = IsIndexed,
                IsAutoIncrement = IsAutoIncrement,
                IsReadOnly = IsReadOnly,
                IsCheck = IsCheck
            };
        }

        public override string ToString()
        {
            string name = string.IsNullOrWhiteSpace(Label) ? FieldName : Label;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Field Snapshot";
            }

            return string.IsNullOrWhiteSpace(DataType) ? name : $"{name} ({DataType})";
        }

        private string ResolveEditorKey()
        {
            string typeName = DataType ?? string.Empty;

            if (IsCheck ||
                Category == DbFieldCategory.Boolean ||
                typeName.IndexOf("bool", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                typeName.IndexOf("bit", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "checkbox";
            }

            if (Category == DbFieldCategory.Enum)
            {
                return "combo";
            }

            if (Category == DbFieldCategory.Date ||
                Category == DbFieldCategory.DateTime ||
                typeName.IndexOf("date", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                typeName.IndexOf("time", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "date";
            }

            if (Category == DbFieldCategory.Numeric ||
                Category == DbFieldCategory.Integer ||
                Category == DbFieldCategory.Decimal ||
                Category == DbFieldCategory.Double ||
                Category == DbFieldCategory.Float ||
                Category == DbFieldCategory.Currency ||
                typeName.IndexOf("int", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                typeName.IndexOf("decimal", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                typeName.IndexOf("double", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                typeName.IndexOf("float", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "numeric";
            }

            return "text";
        }

        private int ResolveWidth()
        {
            if (IsCheck || Category == DbFieldCategory.Boolean)
            {
                return 120;
            }

            if (Category == DbFieldCategory.Date || Category == DbFieldCategory.DateTime)
            {
                return 180;
            }

            if (Category == DbFieldCategory.Enum)
            {
                return 220;
            }

            if (Category == DbFieldCategory.Numeric ||
                Category == DbFieldCategory.Integer ||
                Category == DbFieldCategory.Decimal ||
                Category == DbFieldCategory.Double ||
                Category == DbFieldCategory.Float ||
                Category == DbFieldCategory.Currency)
            {
                return 140;
            }

            int raw = Size > 0 ? Size : 160;
            int width = raw <= 24 ? raw * 12 : raw * 2;
            return System.Math.Max(120, System.Math.Min(320, width));
        }
    }
}