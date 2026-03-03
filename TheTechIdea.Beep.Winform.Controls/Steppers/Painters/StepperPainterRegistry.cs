using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Steppers.Painters
{
    public static class StepperPainterRegistry
    {
        private static readonly Dictionary<string, IStepperPainter> _painters = new(StringComparer.OrdinalIgnoreCase)
        {
            ["NoOp"] = new NoOpStepperPainter(),
            ["CircularNode"] = new CircularNodeStepperPainter(),
            ["ChevronBreadcrumb"] = new ChevronBreadcrumbStepperPainter(),
            ["CompactInline"] = new CompactInlineStepperPainter(),
            ["ProgressBar"] = new ProgressBarStepperPainter(),
            ["SegmentedTab"] = new SegmentedTabStepperPainter(),
            ["Dots"] = new DotsStepperPainter(),
            ["SquareDashed"] = new SquareDashedStepperPainter(),
            ["IconTimeline"] = new IconTimelineStepperPainter(),
            ["VerticalTimeline"] = new VerticalTimelineStepperPainter(),
            ["GradientMaterial"] = new GradientMaterialStepperPainter(),
            ["BadgeStatus"] = new BadgeStatusStepperPainter(),
            ["AlternatingTimeline"] = new AlternatingTimelineStepperPainter()
        };

        public static IStepperPainter GetPainter(string painterName)
        {
            if (string.IsNullOrWhiteSpace(painterName))
            {
                return _painters["NoOp"];
            }

            return _painters.TryGetValue(painterName, out var painter)
                ? painter
                : _painters["NoOp"];
        }

        public static void Register(IStepperPainter painter)
        {
            if (painter == null || string.IsNullOrWhiteSpace(painter.Name))
            {
                return;
            }

            _painters[painter.Name] = painter;
        }

        public static IReadOnlyCollection<string> GetPainterNames()
        {
            return _painters.Keys;
        }
    }
}
