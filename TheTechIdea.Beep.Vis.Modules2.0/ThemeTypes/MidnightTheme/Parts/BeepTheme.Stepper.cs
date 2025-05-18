using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class MidnightTheme
    {
        // Stepper Fonts & Colors
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.FromArgb(30, 30, 40);
        public Color StepperForeColor { get; set; } = Color.LightGray;
        public Color StepperBorderColor { get; set; } = Color.DimGray;
        public Color StepperItemForeColor { get; set; } = Color.LightGray;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 11f, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(60, 60, 80);
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(70, 130, 180); // SteelBlue
        public Color StepperItemSelectedBorderColor { get; set; } = Color.CornflowerBlue;
        public Color StepperItemBorderColor { get; set; } = Color.DimGray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightSteelBlue;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.SteelBlue;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.CornflowerBlue;
    }
}
