using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters
{
    /// <summary>
    /// Optional contract for painters to provide sizing and layout metrics for the caption area.
    /// </summary>
    public interface IFormPainterMetricsProvider
    {
        FormPainterMetrics GetMetrics(BeepiFormPro owner);
    }

    /// <summary>
    /// Interface for form painters that handle custom rendering of BeepiFormPro forms.
    /// Provides methods for painting background, caption bar, and borders.
    /// </summary>
    public interface IFormPainter
    {
        /// <summary>
        /// Paints the background of the form.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        void PaintBackground(Graphics g, BeepiFormPro owner);

        /// <summary>
        /// Paints the caption bar area of the form.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        /// <param name="captionRect">The rectangle defining the caption bar area.</param>
        void PaintCaption(Graphics g, BeepiFormPro owner, Rectangle captionRect);

        /// <summary>
        /// Paints the borders of the form.
        /// </summary>
        /// <param name="g">The graphics context to paint on.</param>
        /// <param name="owner">The form instance being painted.</param>
        void PaintBorders(Graphics g, BeepiFormPro owner);
    }
}
