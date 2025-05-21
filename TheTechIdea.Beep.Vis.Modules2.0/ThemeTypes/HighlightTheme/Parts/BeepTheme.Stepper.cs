using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighlightTheme
    {
        // Stepper Fonts & Colors
//<<<<<<< HEAD
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.LightSteelBlue;
        public Color StepperForeColor { get; set; } = Color.Black;
        public Color StepperBorderColor { get; set; } = Color.SteelBlue;
        public Color StepperItemForeColor { get; set; } = Color.Black;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.White;
        public Color StepperItemHoverBackColor { get; set; } = Color.RoyalBlue;
        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.MediumBlue;
        public Color StepperItemBorderColor { get; set; } = Color.LightSteelBlue;
        public Color StepperItemHoverBorderColor { get; set; } = Color.RoyalBlue;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.DodgerBlue;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.MediumBlue;
    }
}
