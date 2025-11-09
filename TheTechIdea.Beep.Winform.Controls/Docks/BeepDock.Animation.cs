using System;
using TheTechIdea.Beep.Winform.Controls.Docks.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDock - Animation
    /// </summary>
    public partial class BeepDock
    {
        #region Animation
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            bool needsRedraw = false;

            // Update scale animations using DockAnimationHelper
            DockAnimationHelper.ApplySpringEffect(
                _itemStates,
                _hoveredIndex >= 0 && _hoveredIndex < _items.Count ? _items[_hoveredIndex].Name : null,
                _config
            );

            // Smooth scale transitions
            foreach (var state in _itemStates)
            {
                if (Math.Abs(state.CurrentScale - state.TargetScale) > 0.01f)
                {
                    state.CurrentScale += (state.TargetScale - state.CurrentScale) * _config.AnimationSpeed;
                    needsRedraw = true;
                }
                else
                {
                    state.CurrentScale = state.TargetScale;
                }
            }

            if (needsRedraw)
            {
                UpdateItemBounds();
                Invalidate();
            }
        }
        #endregion
    }
}
