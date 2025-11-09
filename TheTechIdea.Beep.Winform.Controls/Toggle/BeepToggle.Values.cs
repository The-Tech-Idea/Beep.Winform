using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Toggle
{
    /// <summary>
    /// Value handling partial class for BeepToggle
    /// Supports different value types: Boolean, String, Numeric
    /// </summary>
    public partial class BeepToggle
    {
        #region Value Type

        /// <summary>
        /// Type of value the toggle represents
        /// </summary>
        public enum ToggleValueType
        {
            /// <summary>Boolean: true/false</summary>
            Boolean,
            /// <summary>String: custom strings for ON/OFF</summary>
            String,
            /// <summary>Numeric: numbers for ON/OFF</summary>
            Numeric,
            /// <summary>YesNo: "Yes"/"No"</summary>
            YesNo,
            /// <summary>OnOff: "On"/"Off"</summary>
            OnOff,
            /// <summary>EnabledDisabled: "Enabled"/"Disabled"</summary>
            EnabledDisabled,
            /// <summary>ActiveInactive: "Active"/"Inactive"</summary>
            ActiveInactive
        }

        private ToggleValueType _valueType = ToggleValueType.Boolean;
        private object? _onValue = true;
        private object? _offValue = false;
        private object? _currentValue = false;

        /// <summary>
        /// Gets or sets the type of value the toggle represents
        /// </summary>
        [Category("Beep")]
        [Description("Type of value the toggle represents")]
        [DefaultValue(ToggleValueType.Boolean)]
        public ToggleValueType ValueType
        {
            get => _valueType;
            set
            {
                if (_valueType != value)
                {
                    _valueType = value;
                    UpdateDefaultValues();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value when toggle is ON
        /// </summary>
        [Category("Beep")]
        [Description("Value when toggle is ON")]
        [DefaultValue(true)]
        public object? OnValue
        {
            get => _onValue;
            set
            {
                _onValue = value;
                if (_isOn)
                    _currentValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the value when toggle is OFF
        /// </summary>
        [Category("Beep")]
        [Description("Value when toggle is OFF")]
        [DefaultValue(false)]
        public object? OffValue
        {
            get => _offValue;
            set
            {
                _offValue = value;
                if (!_isOn)
                    _currentValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the current value of the toggle
        /// </summary>
        [Category("Beep")]
        [Description("Current value of the toggle")]
        [Browsable(true)]
        public object? Value
        {
            get => _currentValue;
            set
            {
                if (value == null)
                {
                    IsOn = false;
                    return;
                }

                // Determine if value matches ON or OFF
                bool newIsOn = IsValueOn(value);
                
                if (newIsOn != _isOn)
                {
                    _currentValue = value;
                    IsOn = newIsOn;
                }
                else
                {
                    _currentValue = value;
                }
            }
        }

        #endregion

        #region Value Helpers

        /// <summary>
        /// Update default values based on ValueType
        /// </summary>
        private void UpdateDefaultValues()
        {
            switch (_valueType)
            {
                case ToggleValueType.Boolean:
                    _onValue = true;
                    _offValue = false;
                    _onText = "ON";
                    _offText = "OFF";
                    break;

                case ToggleValueType.YesNo:
                    _onValue = "Yes";
                    _offValue = "No";
                    _onText = "Yes";
                    _offText = "No";
                    break;

                case ToggleValueType.OnOff:
                    _onValue = "On";
                    _offValue = "Off";
                    _onText = "On";
                    _offText = "Off";
                    break;

                case ToggleValueType.EnabledDisabled:
                    _onValue = "Enabled";
                    _offValue = "Disabled";
                    _onText = "Enabled";
                    _offText = "Disabled";
                    break;

                case ToggleValueType.ActiveInactive:
                    _onValue = "Active";
                    _offValue = "Inactive";
                    _onText = "Active";
                    _offText = "Inactive";
                    break;

                case ToggleValueType.Numeric:
                    _onValue = 1;
                    _offValue = 0;
                    _onText = "1";
                    _offText = "0";
                    break;

                case ToggleValueType.String:
                    // Keep custom values
                    if (_onValue == null) _onValue = "ON";
                    if (_offValue == null) _offValue = "OFF";
                    break;
            }

            // Update current value based on IsOn state
            _currentValue = _isOn ? _onValue : _offValue;
        }

        /// <summary>
        /// Determine if a value represents ON state
        /// </summary>
        private bool IsValueOn(object value)
        {
            if (value == null)
                return false;

            // Check direct equality with OnValue
            if (_onValue != null && value.Equals(_onValue))
                return true;

            // Check direct equality with OffValue
            if (_offValue != null && value.Equals(_offValue))
                return false;

            // Type-specific comparisons
            switch (_valueType)
            {
                case ToggleValueType.Boolean:
                    if (value is bool boolVal)
                        return boolVal;
                    if (bool.TryParse(value.ToString(), out bool parsedBool))
                        return parsedBool;
                    break;

                case ToggleValueType.Numeric:
                    if (value is int intVal)
                        return intVal != 0;
                    if (value is double doubleVal)
                        return Math.Abs(doubleVal) > 0.0001;
                    if (value is float floatVal)
                        return Math.Abs(floatVal) > 0.0001f;
                    if (int.TryParse(value.ToString(), out int parsedInt))
                        return parsedInt != 0;
                    break;

                case ToggleValueType.String:
                case ToggleValueType.YesNo:
                case ToggleValueType.OnOff:
                case ToggleValueType.EnabledDisabled:
                case ToggleValueType.ActiveInactive:
                    string strValue = value.ToString()?.ToLowerInvariant() ?? "";
                    
                    // Common truthy values
                    if (strValue == "true" || strValue == "yes" || strValue == "on" || 
                        strValue == "enabled" || strValue == "active" || strValue == "1")
                        return true;
                    
                    // Common falsy values
                    if (strValue == "false" || strValue == "no" || strValue == "off" || 
                        strValue == "disabled" || strValue == "inactive" || strValue == "0")
                        return false;

                    // Check against OnValue string
                    if (_onValue != null && strValue == _onValue.ToString()?.ToLowerInvariant())
                        return true;

                    break;
            }

            // Default to false if cannot determine
            return false;
        }

        /// <summary>
        /// Update current value when IsOn changes
        /// </summary>
        private void UpdateValueFromIsOn()
        {
            _currentValue = _isOn ? _onValue : _offValue;
        }

        /// <summary>
        /// Get display value as string
        /// </summary>
        public string GetDisplayValue()
        {
            return _currentValue?.ToString() ?? "";
        }

        /// <summary>
        /// Get value as specific type
        /// </summary>
        public T? GetValue<T>()
        {
            if (_currentValue == null)
                return default;

            try
            {
                return (T)Convert.ChangeType(_currentValue, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Set value from string
        /// </summary>
        public void SetValueFromString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                IsOn = false;
                return;
            }

            // Try to match ON value
            if (_onValue?.ToString()?.Equals(value, StringComparison.OrdinalIgnoreCase) == true)
            {
                Value = _onValue;
                return;
            }

            // Try to match OFF value
            if (_offValue?.ToString()?.Equals(value, StringComparison.OrdinalIgnoreCase) == true)
            {
                Value = _offValue;
                return;
            }

            // Fall back to IsValueOn logic
            Value = value;
        }

        #endregion

        #region Data Binding Support

        /// <summary>
        /// Event raised when Value changes
        /// </summary>
        [Category("Beep")]
        [Description("Raised when the Value property changes")]
        public event EventHandler? ValueChanged;

        /// <summary>
        /// Raise ValueChanged event
        /// </summary>
        protected new virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        #endregion
    }
}
