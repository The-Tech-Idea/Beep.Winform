using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepDataBlockFieldSelection
    {
        public string FieldName { get; set; } = string.Empty;
        public bool IncludeInView { get; set; } = true;

        [Browsable(false)]
        public string ControlTypeFullName { get; set; } = string.Empty;

        [Category("Templates")]
        [DisplayName("Template Id")]
        [Description("Optional template id (for reusable editor presets).")]
        public string TemplateId { get; set; } = string.Empty;

        [Browsable(false)]
        public string EditorTypeOverrideFullName { get; set; } = string.Empty;

        [Category("Templates")]
        [DisplayName("Editor Type Override")]
        [Description("Optional direct editor override. Used when no Template Id editor is applied.")]
        [TypeConverter(typeof(BeepUIComponentTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [JsonIgnore]
        public Type? EditorTypeOverride
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EditorTypeOverrideFullName))
                {
                    return null;
                }

                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(EditorTypeOverrideFullName, false, true))
                    .FirstOrDefault(t => t != null);
            }
            set => EditorTypeOverrideFullName = value?.FullName ?? string.Empty;
        }

        [Category("Templates")]
        [DisplayName("Inline Settings (JSON)")]
        [Description("Optional JSON settings that override selected template settings for this field.")]
        public string InlineSettingsJson { get; set; } = string.Empty;

        [Category("Rendering")]
        [DisplayName("Control Type")]
        [Description("Beep control used when ViewMode is RecordControls.")]
        [TypeConverter(typeof(BeepUIComponentTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [JsonIgnore]
        public Type? ControlType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ControlTypeFullName))
                {
                    return null;
                }

                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(a => a.GetType(ControlTypeFullName, false, true))
                    .FirstOrDefault(t => t != null);
            }
            set => ControlTypeFullName = value?.FullName ?? string.Empty;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(FieldName))
            {
                return base.ToString() ?? nameof(BeepDataBlockFieldSelection);
            }

            return $"{FieldName} ({(IncludeInView ? "Shown" : "Hidden")})";
        }
    }

    public enum DataBlockViewMode
    {
        RecordControls = 0,
        BeepGridPro = 1
    }
}
