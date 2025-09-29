namespace TheTechIdea.Beep.Winform.Controls.ProgressBars
{
    public enum ProgressPainterKind
    {
        Linear = 0,         // default linear bar
        StepperCircles = 1, // steps with circles and labels
        ChevronSteps = 2,   // chevron arrows per step
        DotsLoader = 3,     // row of dots filling/active
        Segmented = 4,      // segmented blocks along line
        Ring = 5,           // circular line
        DottedRing = 6,     // circular dotted line
        LinearBadge = 7,    // linear with floating percentage badge
        LinearTrackerIcon = 8, // linear with moving tracker icon/marker
        ArrowStripe = 9,    // herringbone/arrow stripe bar
        RadialSegmented = 10, // donut with separated segments
        RingCenterImage = 11, // ring with center image/icon
        ArrowHeadAnimated = 12 // thin bar with animated arrow head
    }
}
