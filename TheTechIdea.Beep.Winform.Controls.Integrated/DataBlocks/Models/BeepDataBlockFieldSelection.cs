using System;
using System.ComponentModel;
using System.Linq;
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

        [Category("Rendering")]
        [DisplayName("Control Type")]
        [Description("Beep control used when ViewMode is RecordControls.")]
        [TypeConverter(typeof(BeepUIComponentTypeConverter))]
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
