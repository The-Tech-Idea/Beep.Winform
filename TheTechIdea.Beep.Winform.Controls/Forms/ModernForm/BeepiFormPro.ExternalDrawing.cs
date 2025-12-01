using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    /// <summary>
    /// External drawing support for BeepiFormPro.
    /// Allows child controls to draw on the form's surface (e.g., labels, badges, helper text).
    /// </summary>
    public partial class BeepiFormPro : IExternalDrawingProvider
    {
        private ControlExternalDrawingHelper _externalDrawing;

        #region IExternalDrawingProvider Implementation

        /// <summary>
        /// Adds an external drawing handler for a child control.
        /// The handler will be called by the form when painting, allowing the child to draw on the form's surface.
        /// </summary>
        public void AddChildExternalDrawing(Control child, DrawExternalHandler handler, DrawingLayer layer = DrawingLayer.AfterAll)
        {
            EnsureExternalDrawingHelper();
            _externalDrawing.AddChildExternalDrawing(child, handler, layer);
        }

        /// <summary>
        /// Sets whether a child's external drawing should be redrawn.
        /// </summary>
        public void SetChildExternalDrawingRedraw(Control child, bool redraw)
        {
            EnsureExternalDrawingHelper();
            _externalDrawing.SetChildExternalDrawingRedraw(child, redraw);
        }

        /// <summary>
        /// Clears all external drawing handlers for a specific child control.
        /// </summary>
        public void ClearChildExternalDrawing(Control child)
        {
            if (_externalDrawing != null)
            {
                _externalDrawing.ClearChildExternalDrawing(child);
            }
        }

        /// <summary>
        /// Clears all external drawing handlers for all child controls.
        /// </summary>
        public void ClearAllChildExternalDrawing()
        {
            if (_externalDrawing != null)
            {
                _externalDrawing.ClearAllChildExternalDrawing();
            }
        }

        #endregion

        #region Helper Methods

        private void EnsureExternalDrawingHelper()
        {
            if (_externalDrawing == null)
            {
                _externalDrawing = new ControlExternalDrawingHelper(this);
            }
        }

        /// <summary>
        /// Performs external drawing for a specific layer during form painting.
        /// This should be called from the form's OnPaint method.
        /// </summary>
        internal void PerformExternalDrawing(Graphics g, DrawingLayer layer)
        {
            if (_externalDrawing != null)
            {
                _externalDrawing.PerformExternalDrawing(g, layer);
            }
        }

        #endregion
    }
}

