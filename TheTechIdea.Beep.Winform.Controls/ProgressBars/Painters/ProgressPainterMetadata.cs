using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class ProgressPainterMetadata
    {
        public bool SupportsHitAreas { get; init; }
        public bool SupportsKeyboard { get; init; }
        public bool SupportsAnimation { get; init; }
        public bool SupportsFocusVisual { get; init; }
        public bool IsInteractive { get; init; }
        public int PreferredMinHeight { get; init; }
    }

    internal static class ProgressPainterRegistry
    {
        private static readonly Dictionary<ProgressPainterKind, ProgressPainterMetadata> _metadata = new()
        {
            [ProgressPainterKind.Linear] = NewMeta(),
            [ProgressPainterKind.Segmented] = NewMeta(),
            [ProgressPainterKind.LinearBadge] = NewMeta(),
            [ProgressPainterKind.LinearTrackerIcon] = NewMeta(),
            [ProgressPainterKind.ArrowStripe] = NewMeta(),
            [ProgressPainterKind.StepperCircles] = NewMeta(hit: true, keyboard: true, focus: true, interactive: true, minHeight: 24),
            [ProgressPainterKind.ChevronSteps] = NewMeta(hit: true, keyboard: true, focus: true, interactive: true, minHeight: 28),
            [ProgressPainterKind.DotsLoader] = NewMeta(animation: true),
            [ProgressPainterKind.ArrowHeadAnimated] = NewMeta(animation: true),
            [ProgressPainterKind.Ring] = NewMeta(hit: true),
            [ProgressPainterKind.DottedRing] = NewMeta(hit: true),
            [ProgressPainterKind.RadialSegmented] = NewMeta(),
            [ProgressPainterKind.RingCenterImage] = NewMeta(hit: true),
        };

        public static ProgressPainterMetadata GetMetadata(ProgressPainterKind kind)
        {
            if (_metadata.TryGetValue(kind, out var metadata))
            {
                return metadata;
            }

            return NewMeta();
        }

        public static int GetPreferredMinimumHeight(BeepProgressBar owner, ProgressPainterKind kind)
        {
            int baseHeight = GetMetadata(kind).PreferredMinHeight;
            if (baseHeight <= 0)
            {
                return 0;
            }

            return ProgressBarDpiHelpers.Scale(owner, baseHeight);
        }

        private static ProgressPainterMetadata NewMeta(
            bool hit = false,
            bool keyboard = false,
            bool animation = false,
            bool focus = false,
            bool interactive = false,
            int minHeight = 0)
        {
            return new ProgressPainterMetadata
            {
                SupportsHitAreas = hit,
                SupportsKeyboard = keyboard,
                SupportsAnimation = animation,
                SupportsFocusVisual = focus,
                IsInteractive = interactive,
                PreferredMinHeight = minHeight
            };
        }
    }
}
