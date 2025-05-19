using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GalaxyTheme
    {
        // Calendar Fonts & Colors
<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(230, 230, 250); // Soft white
        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(180, 180, 255); // Light blue-purple
        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(70, 20, 120); // Deep purple
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;
        public Font CalendarSelectedFont { get; set; } = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Color CalendarBackColor { get; set; } = Color.FromArgb(20, 20, 40); // Dark space blue
        public Color CalendarForeColor { get; set; } = Color.FromArgb(200, 200, 230); // Light lavender
        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(100, 180, 255); // Bright blue
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(72, 61, 139); // Dark slate blue
        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(50, 50, 80); // Medium space blue
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(240, 240, 255); // Bright white
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font MonthFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Bold);
        public Font YearFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Color CalendarFooterColor { get; set; } = Color.FromArgb(30, 30, 60); // Deep space blue
        public Font FooterFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Regular);
=======
        public TypographyStyle CalendarTitleFont { get; set; }
        public Color CalendarTitleForColor { get; set; }
        public TypographyStyle DaysHeaderFont { get; set; }
        public Color CalendarDaysHeaderForColor { get; set; }
        public TypographyStyle SelectedDateFont { get; set; }
        public Color CalendarSelectedDateBackColor { get; set; }
        public Color CalendarSelectedDateForColor { get; set; }
        public TypographyStyle CalendarSelectedFont { get; set; }
        public TypographyStyle CalendarUnSelectedFont { get; set; }
        public Color CalendarBackColor { get; set; }
        public Color CalendarForeColor { get; set; }
        public Color CalendarTodayForeColor { get; set; }
        public Color CalendarBorderColor { get; set; }
        public Color CalendarHoverBackColor { get; set; }
        public Color CalendarHoverForeColor { get; set; }
        public TypographyStyle HeaderFont { get; set; }
        public TypographyStyle MonthFont { get; set; }
        public TypographyStyle YearFont { get; set; }
        public TypographyStyle DaysFont { get; set; }
        public TypographyStyle DaysSelectedFont { get; set; }
        public TypographyStyle DateFont { get; set; }
        public Color CalendarFooterColor { get; set; }
        public TypographyStyle FooterFont { get; set; }
>>>>>>> 00d68a6e1277c6b19c9d032a5dafd4d4e082d634
    }
}