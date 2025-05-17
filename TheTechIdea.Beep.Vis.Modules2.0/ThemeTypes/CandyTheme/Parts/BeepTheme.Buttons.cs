using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class CandyTheme
    {
        // Button Colors and Styles

        // Fun, bold font for a candy look
        public Font ButtonFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);
        public Font ButtonHoverFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold | FontStyle.Underline); // Slightly playful on hover
        public Font ButtonSelectedFont { get; set; } = new Font("Comic Sans MS", 10.5f, FontStyle.Bold);

        // Normal state: mint green button, white text, soft blue border
        public Color ButtonBackColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint
        public Color ButtonForeColor { get; set; } = Color.FromArgb(85, 85, 85); // Gray text
        public Color ButtonBorderColor { get; set; } = Color.FromArgb(153, 204, 255); // Pastel Blue

        // Hover: pastel blue background, pink border, pink text
        public Color ButtonHoverBackColor { get; set; } = Color.FromArgb(210, 235, 255); // Baby Blue
        public Color ButtonHoverForeColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink
        public Color ButtonHoverBorderColor { get; set; } = Color.FromArgb(255, 182, 193); // Light Pink

        // Pressed: bubblegum pink background, white text, mint border
        public Color ButtonPressedBackColor { get; set; } = Color.FromArgb(255, 182, 193); // Bubblegum Pink
        public Color ButtonPressedForeColor { get; set; } = Color.White;
        public Color ButtonPressedBorderColor { get; set; } = Color.FromArgb(204, 255, 240); // Mint

        // Selected: lemon yellow, candy pink border, navy text
        public Color ButtonSelectedBackColor { get; set; } = Color.FromArgb(255, 253, 194); // Lemon
        public Color ButtonSelectedForeColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy
        public Color ButtonSelectedBorderColor { get; set; } = Color.FromArgb(240, 100, 180); // Candy Pink

        // Selected + hover: deeper pastel yellow, navy text, blue border
        public Color ButtonSelectedHoverBackColor { get; set; } = Color.FromArgb(255, 236, 139); // Deeper Lemon
        public Color ButtonSelectedHoverForeColor { get; set; } = Color.FromArgb(44, 62, 80); // Navy
        public Color ButtonSelectedHoverBorderColor { get; set; } = Color.FromArgb(54, 162, 235); // Soft Blue

        // Error: lively strawberry red with white, pastel yellow border
        public Color ButtonErrorBackColor { get; set; } = Color.FromArgb(255, 99, 132); // Candy Red
        public Color ButtonErrorForeColor { get; set; } = Color.White;
        public Color ButtonErrorBorderColor { get; set; } = Color.FromArgb(255, 223, 93); // Candy Lemon
    }
}
