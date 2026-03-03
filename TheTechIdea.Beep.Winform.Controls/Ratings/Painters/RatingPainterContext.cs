using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Ratings;
using TheTechIdea.Beep.Winform.Controls.Ratings.Models;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Context object passed to rating painters containing all necessary information for rendering.
    /// </summary>
    public class RatingPainterContext
    {
        /// <summary>
        /// Owner control for DPI scaling; used with DpiScalingHelper.ScaleValue(value, OwnerControl)
        /// </summary>
        public Control OwnerControl { get; set; }
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

        // ── Sprint 4 additions ────────────────────────────────────────────────────

        /// <summary>Size variant controlling the star/symbol pixel size.</summary>
        public RatingSizeVariant SizeVariant { get; set; } = RatingSizeVariant.MD;

        /// <summary>Layout orientation of symbols.</summary>
        public RatingLayoutMode LayoutMode { get; set; } = RatingLayoutMode.Horizontal;

        /// <summary>Mirror layout for right-to-left locales.</summary>
        public bool IsRightToLeft { get; set; }

        /// <summary>
        /// When true, the filled symbol color transitions from 
        /// <see cref="ColorGradeStart"/> (1 star = red/low) to 
        /// <see cref="ColorGradeEnd"/> (5 stars = green/high).
        /// </summary>
        public bool UseColorGrade { get; set; }

        /// <summary>Color representing the lowest rating in the color grade.</summary>
        public Color ColorGradeStart { get; set; } = Color.FromArgb(244, 67, 54);   // Material Red

        /// <summary>Color representing the highest rating in the color grade.</summary>
        public Color ColorGradeEnd { get; set; } = Color.FromArgb(76, 175, 80);     // Material Green

        /// <summary>
        /// Multi-category ratings (e.g. Speed, Quality).
        /// Populated when the control is in category-rating mode.
        /// </summary>
        public IReadOnlyList<RatingCategory> Categories { get; set; }

        /// <summary>Histogram distribution data for the histogram painter.</summary>
        public RatingHistogramData HistogramData { get; set; }

        /// <summary>
        /// Derives the effective filled-star color from the color grade at the given rating.
        /// Returns <see cref="FilledStarColor"/> when <see cref="UseColorGrade"/> is false.
        /// </summary>
        public Color GetGradedColor(float ratingValue)
        {
            if (!UseColorGrade) return FilledStarColor;

            float t = StarCount <= 1 ? 1f : Math.Max(0f, Math.Min(1f, (ratingValue - 1f) / (StarCount - 1f)));
            return InterpolateColor(ColorGradeStart, ColorGradeEnd, t);
        }

        private static Color InterpolateColor(Color from, Color to, float t)
        {
            return Color.FromArgb(
                (int)(from.A + (to.A - from.A) * t),
                (int)(from.R + (to.R - from.R) * t),
                (int)(from.G + (to.G - from.G) * t),
                (int)(from.B + (to.B - from.B) * t));
        }
    }
}

