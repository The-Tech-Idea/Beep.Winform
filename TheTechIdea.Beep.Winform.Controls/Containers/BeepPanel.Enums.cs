using System;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Defines the shape style for BeepPanel
    /// </summary>
    public enum PanelShape
    {
        /// <summary>
        /// Standard rectangular panel
        /// </summary>
        Rectangle,
        
        /// <summary>
        /// Rounded rectangle panel
        /// </summary>
        RoundedRectangle,
        
        /// <summary>
        /// Elliptical/circular panel
        /// </summary>
        Ellipse,
        
        /// <summary>
        /// Custom shape defined by a GraphicsPath
        /// </summary>
        Custom
    }
    
    /// <summary>
    /// Defines the title display style for BeepPanel
    /// </summary>
    public enum PanelTitleStyle
    {
        /// <summary>
        /// GroupBox style - title text breaks the border line (like WinForms GroupBox)
        /// </summary>
        GroupBox,
        
        /// <summary>
        /// Title displayed above the panel with optional line below
        /// </summary>
        Above,
        
        /// <summary>
        /// Title displayed below the panel
        /// </summary>
        Below,
        
        /// <summary>
        /// Title displayed on the left side
        /// </summary>
        Left,
        
        /// <summary>
        /// Title displayed on the right side
        /// </summary>
        Right,
        
        /// <summary>
        /// Title displayed inside the panel at the top (overlay style)
        /// </summary>
        Overlay
    }
}
