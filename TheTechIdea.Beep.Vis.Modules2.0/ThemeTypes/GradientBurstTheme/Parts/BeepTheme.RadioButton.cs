using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GradientBurstTheme
    {
        // RadioButton properties
//<<<<<<< HEAD
        public Color RadioButtonBackColor { get; set; } = Color.White;
        public Color RadioButtonForeColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color RadioButtonBorderColor { get; set; } = Color.FromArgb(160, 160, 160);
        public Color RadioButtonCheckedBackColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonCheckedForeColor { get; set; } = Color.White;
        public Color RadioButtonCheckedBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Color RadioButtonHoverBackColor { get; set; } = Color.FromArgb(235, 245, 255);
        public Color RadioButtonHoverForeColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonHoverBorderColor { get; set; } = Color.FromArgb(0, 84, 153);
        public Font RadioButtonFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font RadioButtonCheckedFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Bold);
        public Color RadioButtonSelectedForeColor { get; set; } = Color.FromArgb(0, 120, 212);
        public Color RadioButtonSelectedBackColor { get; set; } = Color.FromArgb(220, 235, 255);
    }
}
