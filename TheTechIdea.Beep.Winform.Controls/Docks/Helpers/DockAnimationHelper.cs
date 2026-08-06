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
                if (state.IsDisabled)
                {
                    state.TargetScale = 1.0f;
                    state.IsHovered = false;
                    continue;
                }

                if (state.IsPressed)
                {
                    state.TargetScale = Math.Max(0.85f, config.PressedScale);
                    state.IsHovered = false;
                }
                else if (i == hoveredIndex)
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
        /// Advances every item's animation by <paramref name="deltaSeconds"/>, easing along the curve
        /// the config's <see cref="DockAnimationStyle"/> selects.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This used to take an <c>animationSpeed</c> fraction and call
        /// <c>Lerp(current, target, speed)</c> - an exponential approach with no notion of time or
        /// progress. That is why 341 lines of easing functions sat unreferenced next door and why all
        /// nine <see cref="DockAnimationStyle"/> values produced the same motion: there was no `t` to
        /// give a curve. Wiring the easing in was never "call the helper"; it needed each item to have
        /// a start value, a target, a duration and a clock.
        /// </para>
        /// <para>
        /// The exponential approach also never actually arrived, so the 60 FPS timer never idled.
        /// A timed animation ends.
        /// </para>
        /// </remarks>
        public static bool UpdateAnimations(List<DockItemState> itemStates, DockConfig config, float deltaSeconds)
        {
            bool needsRedraw = false;
            float duration = Math.Max(0.016f, config.AnimationDuration);
            var curve = DockEasingHelper.GetEasingFunction(config.AnimationStyle);

            // None means none. It used to fall through GetEasingFunction's `_ =>` to EaseOutCubic, so
            // the one value whose whole purpose is "do not animate" animated exactly like Scale.
            if (config.AnimationStyle == DockAnimationStyle.None)
            {
                foreach (var state in itemStates)
                {
                    float targetOpacityNow = state.IsDisabled ? 0.45f : (state.IsHovered || state.IsSelected ? 1.0f : 0.9f);
                    if (Math.Abs(state.CurrentScale - state.TargetScale) > 0.0001f ||
                        Math.Abs(state.CurrentOpacity - targetOpacityNow) > 0.0001f)
                    {
                        needsRedraw = true;
                    }

                    state.CurrentScale = state.TargetScale;
                    state.AnimationFromScale = state.TargetScale;
                    state.AnimationToScale = state.TargetScale;
                    state.AnimationElapsed = 0f;
                    state.CurrentOpacity = targetOpacityNow;
                }

                return needsRedraw;
            }

            foreach (var state in itemStates)
            {
                // A new target restarts the animation from wherever the item currently is, so an
                // interrupted hover eases on from its current size rather than snapping back.
                if (Math.Abs(state.AnimationToScale - state.TargetScale) > 0.0001f)
                {
                    state.AnimationFromScale = state.CurrentScale;
                    state.AnimationToScale = state.TargetScale;
                    state.AnimationElapsed = 0f;
                }

                if (Math.Abs(state.CurrentScale - state.TargetScale) > 0.0001f)
                {
                    state.AnimationElapsed += deltaSeconds;
                    float t = Math.Min(1f, state.AnimationElapsed / duration);

                    if (t >= 1f)
                    {
                        state.CurrentScale = state.TargetScale;
                    }
                    else
                    {
                        state.CurrentScale = state.AnimationFromScale +
                            (state.TargetScale - state.AnimationFromScale) * curve(t);
                        needsRedraw = true;
                    }

                    needsRedraw = true;
                }

                // Rotate and Pulse name effects, not easing shapes, so they need something driven
                // beyond CurrentScale. Both were selectable and did nothing: Rotate mapped to the
                // same curve as Scale while CurrentRotation was written once, to zero, and read by
                // no painter; Pulse mapped to the same curve as Fade with nothing pulsing.
                if (config.AnimationStyle == DockAnimationStyle.Rotate)
                {
                    float targetRotation = state.IsHovered || state.IsSelected ? 12f : 0f;
                    if (Math.Abs(state.CurrentRotation - targetRotation) > 0.01f)
                    {
                        float step = Math.Min(1f, deltaSeconds / duration);
                        state.CurrentRotation += (targetRotation - state.CurrentRotation) * step;
                        if (Math.Abs(state.CurrentRotation - targetRotation) <= 0.01f)
                            state.CurrentRotation = targetRotation;
                        needsRedraw = true;
                    }
                }
                else if (state.CurrentRotation != 0f)
                {
                    state.CurrentRotation = 0f;
                    needsRedraw = true;
                }

                if (config.AnimationStyle == DockAnimationStyle.Pulse &&
                    (state.IsHovered || state.IsSelected || state.IsRunning))
                {
                    // A continuous breath around the item's target scale, rather than a one-shot ease.
                    state.PulsePhase = (state.PulsePhase + deltaSeconds / Math.Max(0.05f, duration * 4f)) % 1f;
                    float breath = (float)Math.Sin(state.PulsePhase * 2 * Math.PI) * 0.04f;
                    state.CurrentScale = state.TargetScale * (1f + breath);
                    needsRedraw = true;
                }
                else if (state.PulsePhase != 0f)
                {
                    state.PulsePhase = 0f;
                }

                // Opacity is a plain fade, not a style choice - easing it would make a disabled item
                // bounce, which is nobody's intent.
                float targetOpacity = state.IsDisabled ? 0.45f : (state.IsHovered || state.IsSelected ? 1.0f : 0.9f);
                if (Math.Abs(targetOpacity - state.CurrentOpacity) > 0.001f)
                {
                    float step = deltaSeconds / duration;
                    state.CurrentOpacity += (targetOpacity - state.CurrentOpacity) * Math.Min(1f, step);
                    if (Math.Abs(targetOpacity - state.CurrentOpacity) <= 0.001f)
                        state.CurrentOpacity = targetOpacity;
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
         /// <summary>
        /// Ease in out for balanced animations
        /// </summary>
         /// <summary>
        /// Elastic bounce effect
        /// </summary>
     }
}
