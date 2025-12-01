using System;
using TheTechIdea.Beep.Winform.Controls.Ratings;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Factory for creating rating painter instances based on RatingStyle
    /// </summary>
    public static class RatingPainterFactory
    {
        /// <summary>
        /// Create a rating painter based on the specified style
        /// </summary>
        /// <param name="style">The rating style</param>
        /// <returns>An instance of the appropriate painter</returns>
        public static IRatingPainter CreatePainter(RatingStyle style)
        {
            return style switch
            {
                RatingStyle.ClassicStar => new ClassicStarPainter(),
                RatingStyle.ModernStar => new ModernStarPainter(),
                RatingStyle.Heart => new HeartRatingPainter(),
                RatingStyle.Thumb => new ThumbRatingPainter(),
                RatingStyle.Circle => new CircleRatingPainter(),
                RatingStyle.Emoji => new EmojiRatingPainter(),
                RatingStyle.Bar => new BarRatingPainter(),
                RatingStyle.GradientStar => new GradientStarPainter(),
                RatingStyle.Minimal => new MinimalRatingPainter(),
                _ => new ClassicStarPainter()
            };
        }

        /// <summary>
        /// Get the default painter style
        /// </summary>
        public static RatingStyle DefaultStyle => RatingStyle.ClassicStar;
    }
}

