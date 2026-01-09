using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    /// <summary>
    /// Type converter for DashboardMetric to enable expandable properties in designer
    /// </summary>
    public class DashboardMetricTypeConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
        {
            return TypeDescriptor.GetProperties(typeof(DashboardMetric), attributes);
        }
    }
}
