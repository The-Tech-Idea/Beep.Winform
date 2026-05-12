using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        [Browsable(true)] [Category("Appearance")] public Font HeaderFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 16f, FontWeight.Bold, FontStyle.Bold);
        [Browsable(true)] [Category("Appearance")] public Font DayFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 12f, FontWeight.Regular, FontStyle.Regular);
        [Browsable(true)] [Category("Appearance")] public Font EventFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 9f, FontWeight.Regular, FontStyle.Regular);
        [Browsable(true)] [Category("Appearance")] public Font TimeFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 10f, FontWeight.Regular, FontStyle.Regular);
        public Font DaysHeaderFont { get; private set; } = BeepThemesManager.ToFont("Segoe UI", 10f, FontWeight.Medium, FontStyle.Regular);
    }
}