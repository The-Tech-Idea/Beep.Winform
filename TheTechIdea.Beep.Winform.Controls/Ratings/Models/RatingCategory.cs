using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Models
{
    /// <summary>
    /// Represents a single category in a multi-criteria rating control
    /// (e.g. "Speed", "Quality", "Support" — each with its own rating value).
    /// </summary>
    public class RatingCategory
    {
        /// <summary>Display label for this category (e.g. "Speed").</summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>Integer rating (1 … StarCount).</summary>
        public int Rating { get; set; }

        /// <summary>
        /// Precise floating-point rating for half-star or continuous display.
        /// Defaults to Rating if not set explicitly.
        /// </summary>
        private float _preciseRating = -1f;
        public float PreciseRating
        {
            get => _preciseRating < 0 ? Rating : _preciseRating;
            set => _preciseRating = value;
        }

        /// <summary>
        /// Optional accent colour for this category row.
        /// If <see cref="Color.Empty"/>, the painter uses the global FilledStarColor.
        /// </summary>
        public Color AccentColor { get; set; } = Color.Empty;

        /// <summary>Whether this category is read-only.</summary>
        public bool ReadOnly { get; set; }

        /// <summary>Optional tooltip or description text.</summary>
        public string Description { get; set; } = string.Empty;

        public RatingCategory() { }

        public RatingCategory(string label, int rating)
        {
            Label  = label;
            Rating = rating;
        }

        public RatingCategory(string label, float preciseRating)
        {
            Label         = label;
            PreciseRating = preciseRating;
            Rating        = (int)System.Math.Round(preciseRating);
        }

        public override string ToString() => $"{Label}: {PreciseRating:F1}";
    }
}
