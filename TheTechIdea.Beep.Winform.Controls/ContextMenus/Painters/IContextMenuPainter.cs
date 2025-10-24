using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// Interface for context menu painters
    /// Each painter is responsible for drawing all visual elements with a specific FormStyle
    /// </summary>
    public interface IContextMenuPainter
    {
        /// <summary>
        /// Gets the FormStyle this painter implements
        /// </summary>
        FormStyle Style { get; }
        
        /// <summary>
        /// Gets the metrics for this painter Style
        /// </summary>
        /// <param name="theme">Optional theme to apply</param>
        /// <param name="useThemeColors">Whether to use theme colors</param>
        /// <returns>ContextMenuMetrics configured for this Style</returns>
        ContextMenuMetrics GetMetrics(IBeepTheme theme = null, bool useThemeColors = false);
        
        /// <summary>
        /// Draws the background of the context menu
        /// </summary>
        void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme);
        
        /// <summary>
        /// Draws all menu items
        /// </summary>
        void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, 
            ContextMenuMetrics metrics, IBeepTheme theme);
        
        /// <summary>
        /// Draws the border of the context menu
        /// </summary>
        void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, 
            ContextMenuMetrics metrics, IBeepTheme theme);
        
        /// <summary>
        /// Gets the preferred height for menu items (legacy support)
        /// </summary>
        int GetPreferredItemHeight();
    }
}
