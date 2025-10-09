using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    /// <summary>
    /// Interface for context menu painters
    /// Each painter is responsible for drawing all visual elements
    /// </summary>
    public interface IContextMenuPainter
    {
        /// <summary>
        /// Draws the background of the context menu
        /// </summary>
        void DrawBackground(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme);
        
        /// <summary>
        /// Draws all menu items
        /// </summary>
        void DrawItems(Graphics g, BeepContextMenu owner, IList<SimpleItem> items, 
            SimpleItem selectedItem, SimpleItem hoveredItem, IBeepTheme theme);
        
        /// <summary>
        /// Draws the border of the context menu
        /// </summary>
        void DrawBorder(Graphics g, BeepContextMenu owner, Rectangle bounds, IBeepTheme theme);
        
        /// <summary>
        /// Gets the preferred height for menu items
        /// </summary>
        int GetPreferredItemHeight();
    }
}
