using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsDesignModeSafe) return;
            Focus();
            _keyboardFocusVisible = false;

            // Hit-test through the per-view painter. The legacy
            // CalendarRenderer / CalendarRenderContext pipeline is gone.
            var hit = ResolveInteractionTarget(e.Location);
            if (hit == null || !hit.HasTarget) return;

            // The interaction target result drives the existing
            // BeepCalendar interaction pipeline via the public hit-test
            // method consumers expect. We do not need to manually call
            // UpdateViewButtonStates here — the painted toolbar's
            // active-state is recomputed in PaintToolbar via IsViewActive.
        }
    }
}