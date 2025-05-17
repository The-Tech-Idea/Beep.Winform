using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CyberpunkNeonTheme
    {
        // Stepper Fonts & Colors

        public Font StepperTitleFont { get; set; } = new Font("Consolas", 14f, FontStyle.Bold);
        public Font StepperSelectedFont { get; set; } = new Font("Consolas", 12f, FontStyle.Bold | FontStyle.Italic);
        public Font StepperUnSelectedFont { get; set; } = new Font("Consolas", 12f, FontStyle.Regular);

        public Color StepperBackColor { get; set; } = Color.FromArgb(18, 18, 32);                // Dark cyberpunk panel
        public Color StepperForeColor { get; set; } = Color.FromArgb(0, 255, 255);               // Neon cyan default text
        public Color StepperBorderColor { get; set; } = Color.FromArgb(255, 0, 255);             // Neon magenta border

        public Color StepperItemForeColor { get; set; } = Color.FromArgb(0, 255, 255);
        public Font StepperItemFont { get; set; } = new Font("Consolas", 12f, FontStyle.Regular);
        public Font StepperSubTitleFont { get; set; } = new Font("Consolas", 10f, FontStyle.Italic);

        public Color StepperItemHoverForeColor { get; set; } = Color.FromArgb(255, 255, 0);     // Neon yellow hover text
        public Color StepperItemHoverBackColor { get; set; } = Color.FromArgb(0, 255, 128);     // Neon green hover bg

        public Color StepperItemSelectedForeColor { get; set; } = Color.White;
        public Color StepperItemSelectedBackColor { get; set; } = Color.FromArgb(255, 0, 255);   // Neon magenta selected bg
        public Color StepperItemSelectedBorderColor { get; set; } = Color.FromArgb(0, 255, 128); // Neon green selected border

        public Color StepperItemBorderColor { get; set; } = Color.FromArgb(0, 255, 255);         // Neon cyan border
        public Color StepperItemHoverBorderColor { get; set; } = Color.FromArgb(255, 255, 0);    // Neon yellow hover border

        public Color StepperItemCheckedBoxForeColor { get; set; } = Color.FromArgb(255, 0, 255); // Neon magenta checkbox fore
        public Color StepperItemCheckedBoxBackColor { get; set; } = Color.FromArgb(18, 18, 32);  // Dark bg checkbox back
        public Color StepperItemCheckedBoxBorderColor { get; set; } = Color.FromArgb(0, 255, 255);// Neon cyan checkbox border
    }
}
