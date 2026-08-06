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
        private long _lastAnimationTick;

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Real elapsed time, not "one timer tick". The timer asks for 16ms and Windows delivers
            // whatever it delivers; an eased animation that assumed a fixed step would run at a
            // different speed under load, and would finish early or late rather than on its duration.
            long now = Environment.TickCount64;
            float delta = _lastAnimationTick == 0 ? 0.016f : (now - _lastAnimationTick) / 1000f;
            _lastAnimationTick = now;
            delta = Math.Clamp(delta, 0.001f, 0.25f);

            var hoveredItemName = _hoveredIndex >= 0 && _hoveredIndex < _items.Count
                ? _items[_hoveredIndex].Name
                : null;
            DockAnimationHelper.ApplySpringEffect(_itemStates, hoveredItemName, _config);
            bool needsRedraw = DockAnimationHelper.UpdateAnimations(_itemStates, _config, delta);

            if (needsRedraw)
            {
                UpdateItemBounds();
                Invalidate();
            }
        }
        #endregion
    }
}
