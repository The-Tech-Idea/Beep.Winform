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
 
        /// <summary>
        /// OnPaintBackground is called BEFORE child controls paint.
        /// This is where we paint form decorations (borders, caption, background effects).
        /// This ensures controls paint ON TOP of our decorations, allowing proper interaction.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {// Always force repaint in design mode when properties change
           // Always fill the background first
           // using (var bgBrush = new SolidBrush(this.BackColor))
           //// {
          //      e.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
          //  }
            // CRITICAL: When using custom skinning (DrawCustomWindowBorder = true), 
            // we handle WM_ERASEBKGND ourselves and do NOT call base.OnPaintBackground.
            // This prevents interference with child control rendering (black boxes on labels, etc.)
            // 
            // When NOT using custom skinning, call base to get standard form painting.
            //if (!DrawCustomWindowBorder)
            //{
                base.OnPaintBackground(e);
            //}

            // Now lay our custom chrome on top while preserving the original graphics state so we don't leak
            // quality settings to child controls.
            var state = e.Graphics.Save();
            try
            {
               
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
