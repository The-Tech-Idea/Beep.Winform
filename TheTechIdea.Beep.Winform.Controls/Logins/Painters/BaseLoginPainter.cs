using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Base class for login painters providing common functionality
    /// </summary>
    public abstract class BaseLoginPainter : ILoginPainter
    {
        /// <summary>
        /// Paints the login control for a specific view type
        /// </summary>
        public abstract void Paint(
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
        public abstract LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds,
            LoginViewType viewType,
            LoginStyleConfig styleConfig);

        /// <summary>
        /// Helper method to initialize metrics from bounds
        /// </summary>
        protected LoginLayoutMetrics InitializeMetrics(Rectangle bounds, Padding padding)
        {
            return new LoginLayoutMetrics
            {
                ContainerBounds = bounds,
                ContainerWidth = bounds.Width - padding.Left - padding.Right,
                ContainerHeight = bounds.Height - padding.Top - padding.Bottom,
                ContainerPadding = padding,
                CurrentY = padding.Top,
                CurrentX = padding.Left
            };
        }
    }
}

