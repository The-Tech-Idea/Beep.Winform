using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void ApplyThemeTypography()
        {
            HeaderFont = ResolveFont(_currentTheme?.CalendarTitleFont, HeaderFont);
            DaysHeaderFont = ResolveFont(_currentTheme?.DaysHeaderFont, DaysHeaderFont);
            DayFont = ResolveFont(_currentTheme?.DateFont, DayFont);
            EventFont = ResolveFont(_currentTheme?.CalendarSelectedFont, EventFont);
            TimeFont = ResolveFont(_currentTheme?.CalendarUnSelectedFont, TimeFont);
        }

        private static Font ResolveFont(TypographyStyle style, Font fallback)
        {
            return style != null ? BeepThemesManager.ToFont(style) : fallback;
        }

    }
}