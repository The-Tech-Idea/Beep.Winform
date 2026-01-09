using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Interface for layout helper classes that build layout templates.
    /// Provides a standardized way to create and configure layout templates.
    /// </summary>
    public interface ILayoutHelper
    {
        /// <summary>
        /// Builds a layout template within the specified parent control.
        /// </summary>
        /// <param name="parent">The parent control container where the layout will be built.</param>
        /// <param name="options">Configuration options for the layout (theme, styling, spacing, etc.).</param>
        /// <returns>The main control representing the built layout template.</returns>
        Control Build(Control parent, LayoutOptions options);
    }
}
