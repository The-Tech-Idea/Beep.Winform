using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public override void ApplyTheme()
        {
            // Follow the BaseControl pattern (BaseControl.Properties.cs:382):
            //   Theme = BeepStyling.GetThemeStyle(_controlstyle);
            // which resolves the default theme from this control's
            // ControlStyle via
            //   BeepStyling.GetFormStyle(_controlstyle)
            //   -> BeepThemesManager.GetThemeNameForFormStyle(FormStyle)
            //   -> BeepThemesManager.GetTheme(name)
            // The base ApplyTheme() then projects the theme's ForeColor /
            // BorderColor / etc onto this control.
            if (_currentTheme == null)
            {
                string themeName = BeepStyling.GetThemeStyle(ControlStyle);
                if (!string.IsNullOrEmpty(themeName))
                {
                    Theme = themeName;
                }
            }

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
