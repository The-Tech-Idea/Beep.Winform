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
            System.Diagnostics.Debug.WriteLine($"BeepGridPro.OnMouseDown at {e.Location}, TopFilterRect={Layout.TopFilterRect}, ShowTopFilterPanel={ShowTopFilterPanel}");
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
            System.Diagnostics.Debug.WriteLine($"handledByScrollbar={handledByScrollbar}, Input is null: {Input == null}, Input type: {Input?.GetType().FullName}");
            if (!handledByScrollbar)
            {
                System.Diagnostics.Debug.WriteLine($"About to call Input.HandleMouseDown, method exists: {Input.GetType().GetMethod("HandleMouseDown") != null}");
                Input.HandleMouseDown(e);
                System.Diagnostics.Debug.WriteLine("Input.HandleMouseDown returned");
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
