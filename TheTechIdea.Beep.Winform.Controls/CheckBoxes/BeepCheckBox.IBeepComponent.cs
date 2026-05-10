using System;
using System.ComponentModel;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        #region IBeepComponent Implementation
        [DefaultValue(nameof(CurrentValue))]
        public override string BoundProperty { get; set; } = nameof(CurrentValue);

        private object ResolveBoundValue()
        {
            return BoundProperty switch
            {
                nameof(Checked) => Checked,
                nameof(CheckState) => CheckState,
                nameof(State) => State,
                nameof(CurrentValue) => CurrentValue,
                _ => CurrentValue
            };
        }

        public override void SetValue(object value)
        {
            if (value == null)
                return;

            _suppressCurrentValueNotifications = true;
            try
            {
                // Direct assignment when types match
                if (value is T typed)
                {
                    CurrentValue = typed;
                    return;
                }

                // Safe coercion for common cross-type binding cases
                // (e.g., bool column bound to BeepCheckBoxString, or int/byte bound to BeepCheckBoxBool)
                Type targetType = typeof(T);
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
                try
                {
                    object converted = Convert.ChangeType(value, underlyingType,
                        System.Globalization.CultureInfo.InvariantCulture);
                    CurrentValue = (T)converted;
                }
                catch (InvalidCastException) { }
                catch (FormatException) { }
                catch (OverflowException) { }
            }
            finally
            {
                _suppressCurrentValueNotifications = false;
            }
        }

        public override object GetValue()
        {
            return CurrentValue;
        }

        public override void ClearValue()
        {
            CurrentValue = default;
        }

        public override bool HasFilterValue()
        {
            return ResolveBoundValue() != null;
        }

        public override AppFilter ToFilter()
        {
            object boundValue = ResolveBoundValue();
            return new AppFilter
            {
               FieldName = BoundProperty,
                FilterValue = boundValue?.ToString(),
                Operator = "="
            };
        }
        #endregion IBeepComponent Implementation
    }
}
