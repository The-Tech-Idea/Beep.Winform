using TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Models;

namespace TheTechIdea.Beep.Winform.Controls.Buttons.BeepAdvancedButton.Painters
{
    /// <summary>
    /// Interface for all button painters
    /// </summary>
    public interface IAdvancedButtonPainter
    {
        /// <summary>
        /// Paint the button with the given context
        /// </summary>
        void Paint(AdvancedButtonPaintContext context);
    }
}
