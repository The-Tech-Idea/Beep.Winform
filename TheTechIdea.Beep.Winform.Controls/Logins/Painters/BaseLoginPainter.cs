using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Logins.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Logins.Painters
{
    /// <summary>
    /// Base class for login painters providing common functionality.
    /// All 9 variants share the same Paint() path (background + border
    /// via BeepStyling) — only CalculateMetrics differs per layout.
    /// </summary>
    public abstract class BaseLoginPainter : ILoginPainter
    {
        public virtual void Paint(
            Graphics g,
            GraphicsPath path,
            LoginViewType viewType,
            BeepControlStyle controlStyle,
            IBeepTheme theme,
            bool useThemeColors,
            LoginLayoutMetrics metrics,
            LoginStyleConfig styleConfig)
        {
            BeepStyling.PaintControl(g, path, controlStyle,
                theme, useThemeColors, ControlState.Normal);
        }

        public abstract LoginLayoutMetrics CalculateMetrics(
            Rectangle bounds,
            LoginViewType viewType,
            LoginStyleConfig styleConfig);

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

