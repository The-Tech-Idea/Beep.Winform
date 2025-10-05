using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Interface for menu bar painters following the widget painter pattern
    /// </summary>
    public interface IMenuBarPainter : IDisposable
    {
        /// <summary>
        /// Initialize the painter with owner control and theme
        /// </summary>
        /// <param name="owner">The BeepMenuBar control that owns this painter</param>
        /// <param name="theme">Current theme to apply</param>
        void Initialize(BaseControl owner, IBeepTheme theme);

        /// <summary>
        /// Adjust layout and calculate rectangles based on the drawing area and context
        /// </summary>
        /// <param name="drawingRect">Available drawing rectangle</param>
        /// <param name="ctx">Menu bar context with data and settings</param>
        /// <returns>Updated context with calculated layout rectangles</returns>
        MenuBarContext AdjustLayout(Rectangle drawingRect, MenuBarContext ctx);

        /// <summary>
        /// Draw the background elements of the menu bar
        /// </summary>
        /// <param name="g">Graphics context for drawing</param>
        /// <param name="ctx">Menu bar context with layout and data</param>
        void DrawBackground(Graphics g, MenuBarContext ctx);

        /// <summary>
        /// Draw the main content (menu items, text, icons)
        /// </summary>
        /// <param name="g">Graphics context for drawing</param>
        /// <param name="ctx">Menu bar context with layout and data</param>
        void DrawContent(Graphics g, MenuBarContext ctx);

        /// <summary>
        /// Draw foreground accents (hover effects, selection indicators, etc.)
        /// </summary>
        /// <param name="g">Graphics context for drawing</param>
        /// <param name="ctx">Menu bar context with layout and data</param>
        void DrawForegroundAccents(Graphics g, MenuBarContext ctx);

        /// <summary>
        /// Update hit areas for interaction handling
        /// </summary>
        /// <param name="owner">The BeepMenuBar control</param>
        /// <param name="ctx">Menu bar context with layout data</param>
        /// <param name="notifyAreaHit">Callback for hit area notifications</param>
        void UpdateHitAreas(BaseControl owner, MenuBarContext ctx, Action<string, Rectangle> notifyAreaHit);

        /// <summary>
        /// Calculate preferred size for the menu bar based on content
        /// </summary>
        /// <param name="ctx">Menu bar context with data</param>
        /// <param name="proposedSize">Proposed size constraints</param>
        /// <returns>Calculated preferred size</returns>
        Size CalculatePreferredSize(MenuBarContext ctx, Size proposedSize);

        /// <summary>
        /// Handle theme changes
        /// </summary>
        /// <param name="theme">New theme to apply</param>
        void ApplyTheme(IBeepTheme theme);
    }
}