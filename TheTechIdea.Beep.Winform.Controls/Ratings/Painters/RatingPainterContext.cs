using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Context object passed to rating painters containing all necessary information for rendering
    /// </summary>
    public class RatingPainterContext
    {
        // Core rating properties
        public int StarCount { get; set; }
        public int SelectedRating { get; set; }
        public float PreciseRating { get; set; }
        public int HoveredStar { get; set; } = -1;
        public bool AllowHalfStars { get; set; }
        public bool ReadOnly { get; set; }

        // Appearance
        public int StarSize { get; set; }
        public int Spacing { get; set; }
        public Color FilledStarColor { get; set; }
        public Color EmptyStarColor { get; set; }
        public Color HoverStarColor { get; set; }
        public Color StarBorderColor { get; set; }
        public float StarBorderThickness { get; set; }

        // Animation
        public bool EnableAnimations { get; set; }
        public bool UseGlowEffect { get; set; }
        public float[] StarScale { get; set; }
        public float GlowIntensity { get; set; }

        // Labels and text
        public bool ShowLabels { get; set; }
        public string[] RatingLabels { get; set; }
        public Font LabelFont { get; set; }
        public Color LabelColor { get; set; }
        public bool ShowRatingCount { get; set; }
        public int RatingCount { get; set; }
        public bool ShowAverage { get; set; }
        public decimal AverageRating { get; set; }

        // Business
        public string RatingContext { get; set; }

        // Drawing
        public Graphics Graphics { get; set; }
        public Rectangle Bounds { get; set; }

        // Theme
        public IBeepTheme Theme { get; set; }
        public bool UseThemeColors { get; set; }
        public BeepControlStyle ControlStyle { get; set; }

        // Style
        public RatingStyle RatingStyle { get; set; }
    }
}

