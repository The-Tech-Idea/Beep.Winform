using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class GlassmorphismTheme
    {
        // Stepper Fonts & Colors
//<<<<<<< HEAD
        public Font StepperTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Bold);
        public Font StepperUnSelectedFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.White;
        public Color StepperForeColor { get; set; } = Color.Black;
        public Color StepperBorderColor { get; set; } = Color.LightGray;

        public Color StepperItemForeColor { get; set; } = Color.Black;
        public Font StepperItemFont { get; set; } = new Font("Segoe UI", 10f, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Segoe UI", 9f, FontStyle.Italic);

        public Color StepperItemHoverForeColor { get; set; } = Color.Black;
        public Color StepperItemHoverBackColor { get; set; } = Color.LightBlue;

        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.DeepSkyBlue;
        public Color StepperItemSelectedBorderColor { get; set; } = Color.SteelBlue;

        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(200, 210, 220);
        public Color StepperItemHoverBorderColor { get; set; } = Color.CornflowerBlue;

        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.White;
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.MediumSeaGreen;
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.SeaGreen;
    }
}
