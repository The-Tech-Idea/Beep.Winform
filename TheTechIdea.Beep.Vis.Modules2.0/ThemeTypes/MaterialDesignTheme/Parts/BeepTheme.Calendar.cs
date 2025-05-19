using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MaterialDesignTheme
    {
        // Calendar Fonts & Colors
<<<<<<< HEAD
        public Font CalendarTitleFont { get; set; } = new Font("Roboto", 16f, FontStyle.Bold);
        public Color CalendarTitleForColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900

        public Font DaysHeaderFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Color CalendarDaysHeaderForColor { get; set; } = Color.FromArgb(97, 97, 97); // Grey 600

        public Font SelectedDateFont { get; set; } = new Font("Roboto", 14f, FontStyle.Bold);
        public Color CalendarSelectedDateBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Blue 500
        public Color CalendarSelectedDateForColor { get; set; } = Color.White;

        public Font CalendarSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
        public Font CalendarUnSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color CalendarBackColor { get; set; } = Color.White;
        public Color CalendarForeColor { get; set; } = Color.FromArgb(33, 33, 33); // Dark Grey 900

        public Color CalendarTodayForeColor { get; set; } = Color.FromArgb(244, 67, 54); // Red 500
        public Color CalendarBorderColor { get; set; } = Color.FromArgb(224, 224, 224); // Grey 300

        public Color CalendarHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253); // Light Blue 50
        public Color CalendarHoverForeColor { get; set; } = Color.FromArgb(21, 101, 192); // Blue 800

        public Font HeaderFont { get; set; } = new Font("Roboto", 14f, FontStyle.Regular);
        public Font MonthFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font YearFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font DaysFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);
        public Font DaysSelectedFont { get; set; } = new Font("Roboto", 12f, FontStyle.Bold);
        public Font DateFont { get; set; } = new Font("Roboto", 12f, FontStyle.Regular);

        public Color CalendarFooterColor { get; set; } = Color.FromArgb(158, 158, 158); // Grey 500
        public Font FooterFont { get; set; } = new Font("Roboto", 10f, FontStyle.Italic);
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
