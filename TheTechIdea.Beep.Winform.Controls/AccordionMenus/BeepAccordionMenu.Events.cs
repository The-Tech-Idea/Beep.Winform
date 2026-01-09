using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.AccordionMenus
{
    public partial class BeepAccordionMenu
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                if (!isInitialized)
                {
                    InitializeMenu();
                    InitializeMenuItemState();
                    Invalidate();
                    isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BeepAccordionMenu: Error in OnHandleCreated: {ex.Message}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (isInitialized)
            {
                Invalidate();
            }
        }

        // Override mouse events to handle hover and selection states
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Check if the mouse is over any of our hit areas
            HitTestWithMouse();

            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Let the hit test mechanism handle clicks
            if (HitTestWithMouse())
            {
                // A hit area was clicked. The action will be executed via HitTestWithMouse

                // Check if it was the toggle button that was clicked
                if (HitTestControl != null && HitTestControl.Name == "ToggleButton")
                {
                    // Handle toggle button click
                    isCollapsed = !isCollapsed;
                    StartAccordionAnimation();
                    ToggleClicked?.Invoke(this, new BeepMouseEventArgs("ToggleClicked", isCollapsed));
                }
                // Check if it was a header item or expand/collapse icon
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("HeaderItem_"))
                {
                    // Get the item from the hit test
                    if (HitTestControl.HitAction != null)
                    {
                        HitTestControl.HitAction.Invoke();
                    }
                }
                // Check if it was a child item
                else if (HitTestControl != null && HitTestControl.Name.StartsWith("ChildItem_"))
                {
                    // Get the item from the hit test
                    if (HitTestControl.HitAction != null)
                    {
                        HitTestControl.HitAction.Invoke();
                    }
                }

                Invalidate();
            }
        }
    }
}
