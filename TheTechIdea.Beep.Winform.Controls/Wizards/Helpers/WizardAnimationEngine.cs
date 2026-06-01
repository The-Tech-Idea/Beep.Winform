using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Wizards.Helpers
{
    /// <summary>
    /// Slide-transition animation engine.
    /// Used by WizardHelpers.AnimateStepTransition for bitmap-based animation.
    /// </summary>
    internal static class WizardAnimationEngine
    {
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - (float)Math.Pow(-2f * t + 2f, 3) / 2f;
        }
    }
}
