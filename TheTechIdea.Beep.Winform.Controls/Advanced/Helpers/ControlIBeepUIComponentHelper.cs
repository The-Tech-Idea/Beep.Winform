using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls.Advanced.Helpers
{
    internal static class ControlIBeepUIComponentHelper
    {
        public static void SetBinding(Control target, string controlProperty, object dataSource, string dataSourceProperty)
        {
            if (target == null || dataSource == null || string.IsNullOrEmpty(controlProperty) || string.IsNullOrEmpty(dataSourceProperty))
                return;

            var existing = target.DataBindings[controlProperty];
            if (existing != null) target.DataBindings.Remove(existing);

            var binding = new Binding(controlProperty, dataSource, dataSourceProperty)
            {
                DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged
            };
            target.DataBindings.Add(binding);
        }

        public static AppFilter ToFilter(string field, object value)
        {
            return new AppFilter
            {
                FieldName = field,
                FilterValue = value?.ToString(),
                Operator = "=",
                valueType = "string"
            };
        }
    }
}
