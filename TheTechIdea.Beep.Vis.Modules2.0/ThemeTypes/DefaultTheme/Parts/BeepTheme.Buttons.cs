using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class DefaultTheme
    {
        // Button Colors and Styles
        public TypographyStyle ButtonFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ButtonHoverFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Regular);
        public TypographyStyle ButtonSelectedFont { get; set; } = ThemeUtils.ConvertFontToTypographyStyle("Segoe UI", 10F, FontStyle.Bold);

        // Primary (filled) button tokens (kept for compatibility)
        public Color ButtonPrimaryBackColor { get; set; } = Color.FromArgb(33, 150, 243); // Accent blue
        public Color ButtonPrimaryForeColor { get; set; } = Color.White;
        public Color ButtonPrimaryBorderColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonPrimaryHoverBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonPrimaryHoverForeColor { get; set; } = Color.White;
        public Color ButtonPrimaryPressedBackColor { get; set; } = Color.FromArgb(21, 101, 192);

        // Outline / default button tokens (outline style)
        public Color ButtonOutlineBackColor { get; set; } = Color.FromArgb(241, 246, 252);                // Light neutral, distinct from page white
        public Color ButtonOutlineForeColor { get; set; } = Color.FromArgb(33, 150, 243);                 // Accent blue text for outline
        public Color ButtonOutlineBorderColor { get; set; } = Color.FromArgb(33, 150, 243);
        public Color ButtonOutlineHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253);          // Light blue hover
        public Color ButtonOutlineHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243);          // Accent blue
        public Color ButtonOutlineHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        // Legacy tokens (kept for backward compatibility)
        // Map legacy tokens to represent an outline-by-default button style with clear pressed/selected states
        public Color ButtonBackColor { get; set; } = Color.FromArgb(241, 246, 252);                      // Neutral default
        public Color ButtonForeColor { get; set; } = Color.FromArgb(33, 150, 243);                       // Accent blue text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(227, 242, 253);          // Light blue hover (outline)
        // Use accent blue on hover for outline buttons; for filled/primary controls, controls should use Primary tokens
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(33, 150, 243);          // Accent blue
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(33, 150, 243);

        // Pressed/Selected states favor filled primary appearance
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(21, 101, 192);          // Pressed state: darker blue
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(21, 101, 192);

        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(25, 118, 210);       // Darker blue for selected
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(25, 118, 210);
        public Color ButtonSelectedForeColor { get; set; } = Color.White;

        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(21, 101, 192);    // Even darker on hover
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.White;
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(21, 101, 192);

        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(244, 67, 54);            // Red error
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(211, 47, 47);
    }
}
