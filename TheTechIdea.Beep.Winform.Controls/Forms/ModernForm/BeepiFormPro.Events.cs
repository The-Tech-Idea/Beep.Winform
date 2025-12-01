using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
    public partial class BeepiFormPro
    {
 
        /// <summary>
        /// OnPaintBackground is called BEFORE child controls paint.
        /// This is where we paint form decorations (borders, caption, background effects).
        /// This ensures controls paint ON TOP of our decorations, allowing proper interaction.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // CRITICAL: Skip painting if form has invalid dimensions
            // This prevents ArgumentException during form initialization
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }
            
            // CRITICAL: In design mode, behave EXACTLY like a normal WinForm for maximum compatibility
            // This ensures the designer can properly track and repaint the form
            if (InDesignMode)
            {
                // Call base to let Windows paint the background normally
                base.OnPaintBackground(e);
                
                // Then paint our custom style ON TOP (if we have a painter)
                if (ActivePainter != null)
                {
                    var state = e.Graphics.Save();
                    try
                    {
                        SetupGraphicsQuality(e.Graphics);
                        
                        // CRITICAL: Don't clear hit areas on every paint!
                        // Hit areas should only be recalculated when layout changes (resize, property changes, etc.)
                        // Clearing them here breaks mouse interaction because paint can happen between mouse-down and mouse-up
                        
                        // Paint using the active form painter
                        ActivePainter.PaintBackground(e.Graphics, this);
                        
                        if (ShowCaptionBar)
                        {
                            ActivePainter.PaintCaption(e.Graphics, this, CurrentLayout.CaptionRect);
                        }
                        
                        ActivePainter.PaintBorders(e.Graphics, this);
                        PaintRegions(e.Graphics);
                    }
                    finally
                    {
                        e.Graphics.Restore(state);
                    }
                }
                return;
            }
            
            // RUNTIME: Full custom painting (no base call)
            // Only call base if we don't have a custom painter (fallback for safety)
            if (ActivePainter == null)
            {
                base.OnPaintBackground(e);
                return;
            }

            // Custom painting: preserve graphics state to avoid leaking quality settings to child controls
            var runtimeState = e.Graphics.Save();
            try
            {
                SetupGraphicsQuality(e.Graphics);

                // Use cached layout - only recalculates when necessary (size/style/property changes)
                // This dramatically improves performance vs. recalculating on every paint
                EnsureLayoutCalculated();
                // Apply backdrop effects (Acrylic, Mica, etc.)
                if (BackdropEffect != BackdropEffect.None)
                {
                    ApplyBackdropEffect(e.Graphics);
                }

                // Paint using the active form painter
                if (ActivePainter.SupportsAnimations && EnableAnimations)
                {
                    ActivePainter.PaintWithEffects(e.Graphics, this, ClientRectangle);
                }
                else
                {
                    // Paint background (this fills the entire form with the custom style)
                    ActivePainter.PaintBackground(e.Graphics, this);

                    // Paint caption bar if visible
                    if (ShowCaptionBar)
                    {
                        ActivePainter.PaintCaption(e.Graphics, this, CurrentLayout.CaptionRect);
                    }

                    // Paint borders
                    ActivePainter.PaintBorders(e.Graphics, this);
                }

                // Paint any custom regions
                PaintRegions(e.Graphics);
            }
            finally
            {
                e.Graphics.Restore(runtimeState);
            }
        }

        /// <summary>
        /// OnPaint is called AFTER child controls have painted.
        /// Use this only for overlays that should appear ON TOP of controls.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Perform external drawing for child controls (labels, badges, helper text)
            if (_externalDrawing != null)
            {
                // Draw before content (labels, helper text)
                PerformExternalDrawing(e.Graphics, DrawingLayer.BeforeContent);
                // Draw after all (badges)
                PerformExternalDrawing(e.Graphics, DrawingLayer.AfterAll);
            }
        }
    }
}
