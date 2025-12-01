using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Interface for login view painters
    /// </summary>
    public interface ILoginPainter
    {
        /// <summary>
        /// Paints the login control for a specific view type
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="path">Graphics path for the control bounds</param>
        /// <param name="viewType">The login view type to paint</param>
        /// <param name="controlStyle">The BeepControlStyle to use</param>
        /// <param name="theme">The theme to apply</param>
        /// <param name="useThemeColors">Whether to use theme colors</param>
        /// <param name="metrics">Layout metrics for positioning controls</param>
        /// <param name="styleConfig">Style configuration</param>
        void Paint(
            Graphics g,
            GraphicsPath path,
            LoginViewType viewType,
            BeepControlStyle controlStyle,
            IBeepTheme theme,
            bool useThemeColors,
            LoginLayoutMetrics metrics,
            LoginStyleConfig styleConfig);

        /// <summary>
        /// Calculates layout metrics for the view type
        /// </summary>
        /// <param name="bounds">The bounds of the control</param>
        /// <param name="viewType">The login view type</param>
        /// <param name="styleConfig">Style configuration</param>
        /// <returns>Calculated layout metrics</returns>
        LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds,
            LoginViewType viewType,
            LoginStyleConfig styleConfig);
    }
}

