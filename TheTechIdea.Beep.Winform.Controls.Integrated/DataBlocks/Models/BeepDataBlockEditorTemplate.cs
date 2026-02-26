using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepDataBlockEditorTemplate
    {
        public string TemplateId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string EditorTypeFullName { get; set; } = string.Empty;
        public string SupportedFieldCategoriesCsv { get; set; } = string.Empty;
        public string SettingsJson { get; set; } = "{}";
        public string ValidationJson { get; set; } = "{}";
        public int Version { get; set; } = 1;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [JsonIgnore]
        public Type? EditorType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EditorTypeFullName))
                {
                    return null;
                }

                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(EditorTypeFullName, false, true))
                    .FirstOrDefault(t => t != null);
            }
            set => EditorTypeFullName = value?.FullName ?? string.Empty;
        }

        public bool SupportsCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(SupportedFieldCategoriesCsv))
            {
                return true;
            }

            var supported = SupportedFieldCategoriesCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return supported.Contains(categoryName);
        }
    }

    public sealed class BeepDataBlockFieldTemplateAssignment
    {
        public string FieldName { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string EditorTypeOverrideFullName { get; set; } = string.Empty;
        public string InlineSettingsJson { get; set; } = "{}";
    }

    public sealed class BeepDataBlockTemplatePayload
    {
        public int SchemaVersion { get; set; } = 1;
        public List<BeepDataBlockEditorTemplate> Templates { get; set; } = new();
        public List<BeepDataBlockFieldTemplateAssignment> FieldAssignments { get; set; } = new();
    }
}
