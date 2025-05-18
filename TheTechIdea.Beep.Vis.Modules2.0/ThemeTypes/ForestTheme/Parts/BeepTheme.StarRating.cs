using System.Drawing;

namespace TheTechIdea.Beep.Vis.Modules
{
    public partial class ForestTheme
    {
        // Star Rating Fonts & Colors
        public Color StarRatingForeColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green
        public Color StarRatingBackColor { get; set; } = Color.FromArgb(240, 255, 240); // Honeydew light greenish
        public Color StarRatingBorderColor { get; set; } = Color.FromArgb(0, 100, 0); // Dark Green border
        public Color StarRatingFillColor { get; set; } = Color.FromArgb(46, 139, 87); // Medium Sea Green fill
        public Color StarRatingHoverForeColor { get; set; } = Color.FromArgb(60, 179, 113); // Medium Spring Green hover
        public Color StarRatingHoverBackColor { get; set; } = Color.FromArgb(224, 255, 255); // Light Cyan hover background
        public Color StarRatingHoverBorderColor { get; set; } = Color.FromArgb(34, 139, 34); // Forest Green hover border
        public Color StarRatingSelectedForeColor { get; set; } = Color.White;
        public Color StarRatingSelectedBackColor { get; set; } = Color.FromArgb(0, 128, 0); // Green selected background
        public Color StarRatingSelectedBorderColor { get; set; } = Color.FromArgb(0, 100, 0); // Dark Green selected border

        public Font StarTitleFont { get; set; } = new Font("Segoe UI", 14f, FontStyle.Bold);
        public Font StarSubTitleFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Italic);
        public Font StarSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Bold);
        public Font StarUnSelectedFont { get; set; } = new Font("Segoe UI", 12f, FontStyle.Regular);

        public Color StarTitleForeColor { get; set; } = Color.DarkGreen;
        public Color StarTitleBackColor { get; set; } = Color.Transparent;
    }
}
