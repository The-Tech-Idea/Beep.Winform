using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing mouse and keyboard input event handlers for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Mouse & Keyboard Event Handlers
        /// <summary>
        /// Handles mouse down events for grid interaction.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // === ADD THESE TWO LINES ===
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            if (e.Button == MouseButtons.Right)
            {
                ShowGridContextMenu(e);
                base.OnMouseDown(e);
                return;
            }
            bool handledByScrollbar = ScrollBars?.HandleMouseDown(e.Location, e.Button) ?? false;
            if (!handledByScrollbar)
            {
                Input.HandleMouseDown(e);
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handles mouse move events for hover effects and dragging.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // === ADD THESE TWO LINES ===
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            if (ScrollBars?.IsDragging ?? false)
            {
                ScrollBars?.HandleMouseMove(e.Location);
            }
            else
            {
                ScrollBars?.HandleMouseMove(e.Location);
                Input.HandleMouseMove(e);
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles mouse up events.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // === ADD THESE TWO LINES ===
            Layout.EnsureCalculated();
            ScrollBars?.UpdateBars();
            bool handledByScrollbar = ScrollBars?.HandleMouseUp(e.Location, e.Button) ?? false;
            if (!handledByScrollbar)
            {
                Input.HandleMouseUp(e);
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Handles mouse wheel events for scrolling.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ContextMenus.ContextMenuManager.IsAnyMenuActive)
            {
                base.OnMouseWheel(e);
                return;
            }
            ScrollBars?.HandleMouseWheel(e);
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Handles keyboard input for navigation and editing.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!ContextMenus.ContextMenuManager.IsAnyMenuActive)
            {
                Input.HandleKeyDown(e);
            }
            base.OnKeyDown(e);
        }
        #endregion
    }
}
