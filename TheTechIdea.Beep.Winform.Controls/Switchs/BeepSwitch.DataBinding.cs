using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - IBeepUIComponent implementation
    /// Data binding and value management
    /// </summary>
    public partial class BeepSwitch
    {
        #region IBeepUIComponent Implementation

        /// <summary>
        /// Gets the old value before the last change
        /// </summary>
        public object Oldvalue { get; private set; }

        /// <summary>
        /// Sets the value of the switch (expects bool)
        /// </summary>
        public new void SetValue(object value)
        {
            if (value == null) return;
            
            Oldvalue = _checked;
            
            if (value is bool boolValue)
            {
                Checked = boolValue;
            }
            else if (value is string strValue)
            {
                // Support string values: "true", "1", "on", "yes"
                Checked = strValue.Equals("true", System.StringComparison.OrdinalIgnoreCase) ||
                          strValue.Equals("1") ||
                          strValue.Equals("on", System.StringComparison.OrdinalIgnoreCase) ||
                          strValue.Equals("yes", System.StringComparison.OrdinalIgnoreCase);
            }
            else if (value is int intValue)
            {
                // Support int values: 1 = true, 0 = false
                Checked = intValue != 0;
            }
        }

        /// <summary>
        /// Gets the current value as object
        /// </summary>
        public new object GetValue()
        {
            return Checked;
        }

        /// <summary>
        /// Clears the value (sets to false)
        /// </summary>
        public new void ClearValue()
        {
            Oldvalue = _checked;
            Checked = false;
        }

        /// <summary>
        /// Returns true if the switch has a filterable value
        /// </summary>
        public new bool HasFilterValue()
        {
            // Switch always has a value (true or false)
            return true;
        }

        /// <summary>
        /// Converts the current value to a filter
        /// </summary>
        public new AppFilter ToFilter()
        {
            return new AppFilter
            {
               FieldName = BoundProperty ?? "Checked",
                FilterValue = Checked.ToString(),
                Operator = "="
            };
        }

        /// <summary>
        /// Refreshes data binding from DataContext
        /// </summary>
        public new void RefreshBinding()
        {
            if (DataContext != null && !string.IsNullOrEmpty(DataSourceProperty))
            {
                var propInfo = DataContext.GetType().GetProperty(DataSourceProperty);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(DataContext);
                    SetValue(value);
                }
            }
        }

        #endregion

        #region Position Properties (IBeepUIComponent)

        // Note: Left, Top, Width, Height are already implemented in Control base class
        // These are exposed through IBeepUIComponent interface

        #endregion
    }
}

