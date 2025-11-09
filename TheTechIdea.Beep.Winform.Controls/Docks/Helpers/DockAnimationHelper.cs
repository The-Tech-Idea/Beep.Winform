using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Helpers
{
    /// <summary>
    /// Helper for dock animation calculations
    /// Implements spring physics for smooth magnification effect
    /// </summary>
    public static class DockAnimationHelper
    {
        /// <summary>
        /// Apply spring magnification effect based on hover
        /// </summary>
        public static void ApplySpringEffect(
            List<DockItemState> itemStates,
            string hoveredItemName,
            DockConfig config)
        {
            int hoveredIndex = -1;

            if (!string.IsNullOrEmpty(hoveredItemName))
            {
                hoveredIndex = itemStates.FindIndex(s => s.Item.Name == hoveredItemName);
            }

            for (int i = 0; i < itemStates.Count; i++)
            {
                var state = itemStates[i];

                if (i == hoveredIndex)
                {
                    // Maximum scale for hovered item
                    state.TargetScale = config.MaxScale;
                    state.IsHovered = true;
                }
                else if (state.IsSelected)
                {
                    // Selected item slightly larger
                    state.TargetScale = config.SelectedScale;
                    state.IsHovered = false;
                }
                else if (hoveredIndex >= 0)
                {
                    // Apply distance-based scaling to neighbors
                    int distance = Math.Abs(i - hoveredIndex);
                    float neighborScale = CalculateNeighborScale(distance, config.MaxScale);
                    state.TargetScale = neighborScale;
                    state.IsHovered = false;
                }
                else
                {
                    // No hover - return to normal
                    state.TargetScale = 1.0f;
                    state.IsHovered = false;
                }
            }
        }

        /// <summary>
        /// Calculate scale for neighboring items based on distance
        /// </summary>
        private static float CalculateNeighborScale(int distance, float maxScale)
        {
            // macOS-style falloff: adjacent items scale down gradually
            return distance switch
            {
                0 => maxScale,
                1 => 1.0f + (maxScale - 1.0f) * 0.6f, // 60% of extra scale
                2 => 1.0f + (maxScale - 1.0f) * 0.3f, // 30% of extra scale
                _ => 1.0f // No scaling for items further away
            };
        }

        /// <summary>
        /// Smoothly interpolate current values toward target values
        /// </summary>
        public static bool UpdateAnimations(List<DockItemState> itemStates, float animationSpeed)
        {
            bool needsRedraw = false;

            foreach (var state in itemStates)
            {
                // Scale animation
                if (Math.Abs(state.TargetScale - state.CurrentScale) > 0.001f)
                {
                    state.CurrentScale = Lerp(state.CurrentScale, state.TargetScale, animationSpeed);
                    needsRedraw = true;
                }

                // Opacity animation
                float targetOpacity = state.IsHovered || state.IsSelected ? 1.0f : 0.9f;
                if (Math.Abs(targetOpacity - state.CurrentOpacity) > 0.001f)
                {
                    state.CurrentOpacity = Lerp(state.CurrentOpacity, targetOpacity, animationSpeed * 0.5f);
                    needsRedraw = true;
                }
            }

            return needsRedraw;
        }

        /// <summary>
        /// Linear interpolation
        /// </summary>
        private static float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }

        /// <summary>
        /// Ease out cubic for smoother animations
        /// </summary>
        public static float EaseOutCubic(float t)
        {
            return 1 - (float)Math.Pow(1 - t, 3);
        }

        /// <summary>
        /// Ease in out for balanced animations
        /// </summary>
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4 * t * t * t
                : 1 - (float)Math.Pow(-2 * t + 2, 3) / 2;
        }

        /// <summary>
        /// Elastic bounce effect
        /// </summary>
        public static float EaseOutElastic(float t)
        {
            const float c4 = (2 * (float)Math.PI) / 3;

            return t == 0 ? 0
                : t == 1 ? 1
                : (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 10 - 0.75f) * c4) + 1;
        }
    }
}
