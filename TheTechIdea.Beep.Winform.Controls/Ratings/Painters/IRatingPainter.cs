using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Ratings.Painters
{
    /// <summary>
    /// Interface for rating painters that handle visual rendering of rating controls
    /// </summary>
    public interface IRatingPainter
    {
        /// <summary>
        /// Name of the painter style
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Paint the rating control using the provided context
        /// </summary>
        /// <param name="context">Context containing all rating information and drawing parameters</param>
        void Paint(RatingPainterContext context);

        /// <summary>
        /// Calculate the preferred size for the rating control
        /// </summary>
        /// <param name="context">Context containing rating information</param>
        /// <returns>Preferred size</returns>
        Size CalculateSize(RatingPainterContext context);

        /// <summary>
        /// Get the hit test rectangle for a specific rating index
        /// </summary>
        /// <param name="context">Context containing rating information</param>
        /// <param name="index">Index of the rating item (0-based)</param>
        /// <returns>Hit test rectangle</returns>
        Rectangle GetHitTestRect(RatingPainterContext context, int index);

        /// <summary>
        /// Clean up resources
        /// </summary>
        void Dispose();
    }
}

