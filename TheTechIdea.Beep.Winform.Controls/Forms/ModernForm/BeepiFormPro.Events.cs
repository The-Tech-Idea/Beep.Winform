using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
  
        /// <summary>
        /// Override OnHandleCreated to apply backdrop effects when window handle is created.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Update DPI scale AFTER handle is created and form is initialized
            // This ensures we don't interfere with AutoScale initialization

          
            // Apply backdrop effects
            ApplyBackdrop();
          //  ApplyAcrylicEffectIfNeeded();
          //  ApplyMicaBackdropIfNeeded();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
           // RecalculateLayoutAndHitAreas();
          //  Invalidate();
            //// Update window region for custom borders
         //   UpdateWindowRegion();
        }
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
                        _hits.RegisterHitArea("caption", CurrentLayout.CaptionRect, HitAreaType.Caption);
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
