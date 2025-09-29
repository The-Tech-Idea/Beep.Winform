using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Strategy contract for rendering a BaseControl. Painters own layout and drawing for the control skin.
    /// Implementations may reuse existing helpers (ControlPaintHelper, BaseControlMaterialHelper).
    /// </summary>
    internal interface IBaseControlPainter
    {
        /// <summary>
        /// Update any cached rectangles or helper layout based on the owner state/size.
        /// </summary>
        void UpdateLayout(Base.BaseControl owner);

        /// <summary>
        /// Perform drawing for the owner. Painters should fully render background, state layers, borders, etc.
        /// </summary>
        void Paint(System.Drawing.Graphics g, Base.BaseControl owner);

        /// <summary>
        /// Register hit areas that the painter defines (icons, buttons, etc.).
        /// The register callback maps a name to a rectangle in owner coordinates and an optional click action.
        /// </summary>
        void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register);
    }
}
