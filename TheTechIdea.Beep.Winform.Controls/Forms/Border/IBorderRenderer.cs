using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Forms.Border
{
    /// <summary>
    /// Strategy interface for painting form borders.
    /// Like IListBoxPainter - receives owner to access all properties and helpers.
    /// </summary>
    internal interface IBorderRenderer
    {
        /// <summary>
        /// Initialize the renderer with owner and theme
        /// </summary>
        void Initialize(IBeepModernFormHost owner, IBeepTheme theme);
        
        /// <summary>
        /// Update the current theme
        /// </summary>
        void UpdateTheme(IBeepTheme theme);
        
        /// <summary>
        /// Paints the border.
        /// Like IListBoxPainter.Paint() - receives owner to access all properties/helpers.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="owner">Owner form (access to all properties, helpers, layout)</param>
        /// <param name="borderPath">The path defining border area</param>
        void Paint(Graphics g, IBeepModernFormHost owner, GraphicsPath borderPath);
    }
}
