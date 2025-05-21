using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class HighContrastTheme
    {
        // Stepper Fonts & Colors
//<<<<<<< HEAD
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);
        public Color StepperBackColor { get; set; } = Color.Black;
        public Color StepperForeColor { get; set; } = Color.White;
        public Color StepperBorderColor { get; set; } = Color.White;
        public Color StepperItemForeColor { get; set; } = Color.White;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Italic);
        public Color StepperItemHoverForeColor { get; set; } = Color.Yellow;
        public Color StepperItemHoverBackColor { get; set; } = Color.DimGray;
        public Color StepperItemSelectedForeColor { get; set; } = Color.Black;
        public Color StepperItemSelectedBackColor { get; set; } = Color.Yellow;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.White;
        public Color StepperItemBorderColor { get; set; } = Color.Gray;
        public Color StepperItemHoverBorderColor { get; set; } = Color.LightGray;
        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.Black;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.Lime;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.White;
    }
}
