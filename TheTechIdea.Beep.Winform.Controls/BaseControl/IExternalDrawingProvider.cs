using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// Interface for controls and forms that support external drawing from child controls.
    /// Allows child controls to draw on the parent's surface (e.g., labels, badges, helper text).
    /// </summary>
    public interface IExternalDrawingProvider
    {
        /// <summary>
        /// Adds an external drawing handler for a child control.
        /// The handler will be called by the parent when painting, allowing the child to draw on the parent's surface.
        /// </summary>
        /// <param name="child">The child control that wants to draw on the parent</param>
        /// <param name="handler">The drawing handler that matches DrawExternalHandler signature: void Handler(Graphics parentGraphics, Rectangle childBounds)</param>
        /// <param name="layer">The drawing layer (BeforeContent or AfterAll)</param>
        void AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll);

        /// <summary>
        /// Sets whether a child's external drawing should be redrawn.
        /// </summary>
        /// <param name="child">The child control</param>
        /// <param name="redraw">Whether to redraw the external drawing</param>
        void SetChildExternalDrawingRedraw(Control child, bool redraw);

        /// <summary>
        /// Clears all external drawing handlers for a specific child control.
        /// </summary>
        /// <param name="child">The child control</param>
        void ClearChildExternalDrawing(Control child);

        /// <summary>
        /// Clears all external drawing handlers for all child controls.
        /// </summary>
        void ClearAllChildExternalDrawing();
    }
}

