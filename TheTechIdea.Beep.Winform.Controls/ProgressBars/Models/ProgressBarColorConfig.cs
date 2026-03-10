using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ProgressBarColorConfig
    {
        [Category("Colors")]
        public Color BackgroundColor { get; set; } = Color.FromArgb(240, 240, 240);

        [Category("Colors")]
        public Color ProgressColor { get; set; } = Color.FromArgb(52, 152, 219);

        [Category("Colors")]
        public Color TextColor { get; set; } = Color.White;

        [Category("Colors")]
        public Color BorderColor { get; set; } = Color.FromArgb(30, 0, 0, 0);

        [Category("Colors")]
        public Color SecondaryProgressColor { get; set; } = Color.FromArgb(50, 100, 100, 100);

        [Category("Colors")]
        public Color SuccessColor { get; set; } = Color.FromArgb(34, 197, 94);

        [Category("Colors")]
        public Color WarningColor { get; set; } = Color.FromArgb(245, 158, 11);

        [Category("Colors")]
        public Color ErrorColor { get; set; } = Color.FromArgb(239, 68, 68);
    }
}
