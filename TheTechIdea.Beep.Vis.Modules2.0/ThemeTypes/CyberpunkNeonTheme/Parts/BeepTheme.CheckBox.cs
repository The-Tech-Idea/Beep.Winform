using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // CheckBox properties

        public Color CheckBoxBackColor { get; set; } = Color.FromArgb(24, 24, 48);           // Cyberpunk Black
        public Color CheckBoxForeColor { get; set; } = Color.FromArgb(0, 255, 255);          // Neon Cyan
        public Color CheckBoxBorderColor { get; set; } = Color.FromArgb(255, 0, 255);        // Neon Magenta

        public Color CheckBoxCheckedBackColor { get; set; } = Color.FromArgb(0, 102, 255);   // Neon Blue
        public Color CheckBoxCheckedForeColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow
        public Color CheckBoxCheckedBorderColor { get; set; } = Color.FromArgb(0, 255, 128); // Neon Green

        public Color CheckBoxHoverBackColor { get; set; } = Color.FromArgb(255, 0, 255);     // Neon Magenta
        public Color CheckBoxHoverForeColor { get; set; } = Color.FromArgb(0, 255, 255);     // Neon Cyan
        public Color CheckBoxHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);   // Neon Yellow

        public Font CheckBoxFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Regular);
        public Font CheckBoxCheckedFont { get; set; } = new Font("Consolas", 10.5f, FontStyle.Bold);
    }
}
