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
            var hoveredItemName = _hoveredIndex >= 0 && _hoveredIndex < _items.Count
                ? _items[_hoveredIndex].Name
                : null;
            DockAnimationHelper.ApplySpringEffect(_itemStates, hoveredItemName, _config);
            bool needsRedraw = DockAnimationHelper.UpdateAnimations(_itemStates, _config.AnimationSpeed);

            if (needsRedraw)
            {
                UpdateItemBounds();
                Invalidate();
            }
        }
        #endregion
    }
}
