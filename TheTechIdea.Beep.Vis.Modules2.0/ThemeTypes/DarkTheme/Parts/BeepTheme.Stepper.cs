using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DarkTheme
    {
        // Stepper Fonts & Colors
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color StepperForeColor { get; set; } = Color.WhiteSmoke;
        public Color StepperBorderColor { get; set; } = Color.DimGray;
        public Color StepperItemForeColor { get; set; } = Color.LightGray;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(0, 122, 204); // Blue accent
        public Color StepperItemSelectedBorderColor { get; set; } = Color.DeepSkyBlue;
        public Color StepperItemBorderColor { get; set; } = Color.Gray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightBlue;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.DeepSkyBlue;
    }
}
