using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private float GetDensityScale()
        {
            return _densityMode == CalendarDensityMode.Compact ? 0.85f : 1.0f;
        }

        private int ScaleMetric(int baseValue)
        {
            float dpiScale = BeepThemesManager.DpiScaleX > 0f ? BeepThemesManager.DpiScaleX : 1f;
            return Math.Max(1, (int)Math.Round(baseValue * GetDensityScale() * dpiScale));
        }

        private void MoveFocusedDate(int deltaDays)
        {
            _focusedDate = _focusedDate.AddDays(deltaDays);
        }

    }
}