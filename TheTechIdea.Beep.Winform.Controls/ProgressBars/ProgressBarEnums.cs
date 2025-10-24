namespace TheTechIdea.Beep.Winform.Controls.ProgressBars
{
    // Visual text display modes for BeepProgressBar
    public enum ProgressBarDisplayMode
    {
        NoText,
        Percentage,
        CurrProgress,
        CustomText,
        TextAndPercentage,
        TextAndCurrProgress,
        TaskProgress,     // "5/12 tasks"
        CenterPercentage, // center-only percentage text (for rings/chevrons)
        LoadingText,      // shows a generic "Loading..." text
        StepLabels,       // shows step labels text when provided
        ValueOverMax      // shows "value/max"
    }

    // Legacy fill Style used only by the legacy linear painter
    public enum ProgressBarStyle
    {
        Flat,
        Gradient,
        Striped,
        Animated,
        Segmented
    }

    // Convenience sizing presets
    public enum ProgressBarSize
    {
        Thin,
        Small,
        Medium,
        Large,
        ExtraLarge
    }
}
