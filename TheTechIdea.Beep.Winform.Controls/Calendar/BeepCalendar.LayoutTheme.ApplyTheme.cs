using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;
            using var _ = DeferVisualUpdate();
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;

            if (UseThemeFont)
            {
                ApplyThemeTypography();
            }

            Invalidate();
        }
    }
}