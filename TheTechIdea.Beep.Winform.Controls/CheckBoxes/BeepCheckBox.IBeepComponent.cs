using System;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        #region IBeepComponent Implementation
        public override string BoundProperty { get; set; } = "State";

        public override void SetValue(object value)
        {
            if (value != null)
            {
                CurrentValue = (T)value;
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
            return CurrentValue != null;
        }

        public override AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = State.ToString(),
                Operator = "="
            };
        }
        #endregion IBeepComponent Implementation
    }
}
