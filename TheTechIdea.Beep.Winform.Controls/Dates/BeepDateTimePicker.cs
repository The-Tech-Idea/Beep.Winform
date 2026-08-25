using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateTimePicker : BaseControl
    {
        protected override Size DefaultSize => BeepLayoutMetrics.FieldDateTime;

        // Implementation is split across partial classes:
        // - BeepDateTimePicker.Core.cs:       fields, events, constructor, painter/hit-handler selection
        // - BeepDateTimePicker.Properties.cs: public properties
        // - BeepDateTimePicker.Events.cs:     mouse/keyboard event routing
        // - BeepDateTimePicker.Methods.cs:    public methods and ApplyTheme
        // - BeepDateTimePicker.Drawing.cs:    DrawContent/Draw and painter delegation
        // Rendering is delegated to IDateTimePickerPainter (one painter per DatePickerMode);
        // hit testing is delegated to IDateTimePickerHitHandler.
    }
}
