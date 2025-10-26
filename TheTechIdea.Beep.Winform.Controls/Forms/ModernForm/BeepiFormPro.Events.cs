using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
        // Debounce timer used to defer expensive layout work until the user stops
    //    // actively resizing. Interval is conservative to balance responsiveness
    //    // and avoiding repeated heavy work.
    //    private Timer _resizeDebounceTimer;

    //    /// <summary>
    //    /// Override OnHandleCreated to apply backdrop effects when window handle is created.
    //    /// </summary>
    //    protected override void OnHandleCreated(EventArgs e)
    //    {
    //        base.OnHandleCreated(e);
    //        // Update DPI scale AFTER handle is created and form is initialized
    //        // This ensures we don't interfere with AutoScale initialization


    //        // Apply backdrop effects
    //        ApplyBackdrop();
    //        // Ensure window region and layout are correct once handle exists.
    //        // Recalculate layout in both design-time and runtime so the designer
    //        // shows the same layout as the running app. Updating the actual
    //        // window region (SetWindowRgn/SetWindowPos) is runtime-only and
    //        // skipped while in the designer to avoid host-window side-effects.
    //        RecalculateLayoutAndHitAreas();

    //        UpdateWindowRegion();
    //        // Inform the windowing system that the frame changed so WM_NCCALCSIZE
    //        // will be sent and non-client metrics are updated immediately.
    //        if (IsHandleCreated)
    //        {
    //            try { SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED); }
    //            catch { }
    //        }

    //        // Create debounce timer (only once) to avoid expensive recalculation during continuous resizing
    //        if (_resizeDebounceTimer == null)
    //        {
    //            _resizeDebounceTimer = new Timer();
    //            _resizeDebounceTimer.Interval = 200; // ms
    //            _resizeDebounceTimer.Tick += ResizeDebounceTimer_Tick;
    //        }
    //        //  ApplyAcrylicEffectIfNeeded();
    //        //  ApplyMicaBackdropIfNeeded();
    //    }
    //    protected override void OnResize(EventArgs e)
    //    {
    //        base.OnResize(e);
    //        // When resizing we must recalc layout, update the shaped window region
    //        // and force both client and non-client to repaint immediately so the
    //        // custom chrome stays in sync with the resize.
    //        // Always perform light repaint and schedule heavy recalculation so the
    //        // designer preview updates while the user resizes; only perform the
    //        // window-region and heavy win32 work when running (not in designer).
    //        //
    //        // Light repaint while user is actively resizing. Heavy work (layout
    //        // calc + region update) is deferred until resizing stops to avoid
    //        // repeated expensive operations that also risk runtime NREs.

    //        // Light repaint while user is actively resizing. Heavy work (layout
    //        // calc + region update) is deferred until resizing stops to avoid
    //        // repeated expensive operations that also risk designer/runtime NREs.
    //        if (IsHandleCreated)
    //        {
    //            try
    //            {
    //                Invalidate(true);
    //                Update();
    //                RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_UPDATENOW);
    //            }
    //            catch { }
    //        }

    //        // Restart debounce timer; when it fires we will perform the heavy work.
    //        try
    //        {
    //            if (_resizeDebounceTimer != null)
    //            {
    //                _resizeDebounceTimer.Stop();
    //                _resizeDebounceTimer.Start();
    //            }
    //            else
    //            {
    //                // Fallback: if timer not available, do the work immediately
    //                RecalculateLayoutAndHitAreas();
    //                if (!InDesignModeSafe) UpdateWindowRegion();
    //            }
    //        }
    //        catch { }
    //    }
    


    //private void ResizeDebounceTimer_Tick(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            _resizeDebounceTimer?.Stop();
    //            if (InDesignModeSafe) return;

    //            RecalculateLayoutAndHitAreas();
    //            UpdateWindowRegion();

    //            if (IsHandleCreated)
    //            {
    //                try
    //                {
    //                    Invalidate(true);
    //                    Update();
    //                    RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_UPDATENOW);
    //                    try { SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED); } catch { }
    //                }
    //                catch { }
    //            }
    //        }
    //        catch { }
    //    }
        /// <summary>
        /// OnPaintBackground is called BEFORE child controls paint.
        /// This is where we paint form decorations (borders, caption, background effects).
        /// This ensures controls paint ON TOP of our decorations, allowing proper interaction.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {// Always force repaint in design mode when properties change
          
            // CRITICAL: When using custom skinning (DrawCustomWindowBorder = true), 
            // we handle WM_ERASEBKGND ourselves and do NOT call base.OnPaintBackground.
            // This prevents interference with child control rendering (black boxes on labels, etc.)
            // 
            // When NOT using custom skinning, call base to get standard form painting.
            if (!DrawCustomWindowBorder)
            {
                base.OnPaintBackground(e);
            }

            // Now lay our custom chrome on top while preserving the original graphics state so we don't leak
            // quality settings to child controls.
            var state = e.Graphics.Save();
            try
            {
                // Always start with an opaque base in the client area when custom skinning is enabled
                // to avoid any blending artifacts with GDI-based child controls (e.g., Label, TextBox).
                // CRITICAL: Use the painter's shape (rounded corners, etc.) not a plain rectangle
                //if (DrawCustomWindowBorder && ActivePainter != null)
                //{
                //    using var bg = new SolidBrush(this.BackColor);

                //    // Get the painter's corner radius to match the form's actual shape
                //    var radius = ActivePainter.GetCornerRadius(this);

                //    // Fill using the same rounded shape as the border to avoid rectangular corners showing
                //    using var shapePath = CreateRoundedRectanglePath(this.ClientRectangle, radius);
                //    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //    e.Graphics.FillPath(bg, shapePath);
                //}

                SetupGraphicsQuality(e.Graphics);

                if (ActivePainter != null)
                {
                    _hits?.Clear();
                    ActivePainter.CalculateLayoutAndHitAreas(this);

                    if (ShowCaptionBar && CurrentLayout.CaptionRect.Width > 0 && CurrentLayout.CaptionRect.Height > 0)
                    {
                        _hits?.RegisterHitArea("caption", CurrentLayout.CaptionRect, HitAreaType.Caption);
                    }

                    if (BackdropEffect != BackdropEffect.None)
                    {
                        ApplyBackdropEffect(e.Graphics);
                    }

                    if (ActivePainter.SupportsAnimations && EnableAnimations)
                    {
                        ActivePainter.PaintWithEffects(e.Graphics, this, ClientRectangle);
                    }
                    else
                    {
                        ActivePainter.PaintBackground(e.Graphics, this);

                        if (ShowCaptionBar)
                        {
                            ActivePainter.PaintCaption(e.Graphics, this, CurrentLayout.CaptionRect);
                        }

                        ActivePainter.PaintBorders(e.Graphics, this);
                    }
                }

                PaintRegions(e.Graphics);
            }
            finally
            {
                e.Graphics.Restore(state);
            }
        }

        /// <summary>
        /// OnPaint is called AFTER child controls have painted.
        /// Use this only for overlays that should appear ON TOP of controls.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
