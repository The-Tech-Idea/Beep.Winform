using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Strategy contract for rendering a BaseControl. Painters own layout and drawing for the control skin.
    /// Implementations may reuse existing helpers (ControlPaintHelper, BaseControlMaterialHelper) during transition.
    /// </summary>
    internal interface IBaseControlPainter
    {
        /// <summary>
        /// Update any cached rectangles or helper layout based on the owner state/size.
        /// Painters must also update DrawingRect/BorderRect/ContentRect.
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

        /// <summary>
        /// Painter-driven preferred size calculation. Return Size.Empty to use the owner's fallback.
        /// </summary>
        Size GetPreferredSize(Base.BaseControl owner, Size proposedSize);

        /// <summary>
        /// The rectangle available for main content drawing, after padding/border/shadow and labels/supporting text.
        /// </summary>
        Rectangle DrawingRect { get; }

        /// <summary>
        /// The rectangle used for outer border drawing in this painter.
        /// </summary>
        Rectangle BorderRect { get; }

        /// <summary>
        /// The content rectangle adjusted for icons so text does not overlap icons.
        /// </summary>
        Rectangle ContentRect { get; }
    }
}
