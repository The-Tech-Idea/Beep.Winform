namespace TheTechIdea.Beep.Winform.Controls.Wizards
{
    /// <summary>
    /// Visual style for wizard forms
    /// </summary>
    public enum WizardStyle
    {
        /// <summary>Horizontal stepper with icons at top</summary>
        HorizontalStepper,
        /// <summary>Vertical stepper with timeline on left side</summary>
        VerticalStepper,
        /// <summary>Minimal clean progress indicator</summary>
        Minimal,
        /// <summary>Card-based step selection with clickable cards on the left</summary>
        Cards
    }

    /// <summary>
    /// Result of wizard completion
    /// </summary>
    public enum WizardResult
    {
        /// <summary>Wizard is still active</summary>
        None,
        /// <summary>Wizard completed successfully</summary>
        Completed,
        /// <summary>Wizard was cancelled by user</summary>
        Cancelled,
        /// <summary>Wizard failed due to error</summary>
        Failed
    }

    /// <summary>
    /// State of a wizard step
    /// </summary>
    public enum StepState
    {
        /// <summary>Step not yet visited</summary>
        Pending,
        /// <summary>Step is currently active</summary>
        Current,
        /// <summary>Step completed successfully</summary>
        Completed,
        /// <summary>Step has validation errors</summary>
        Error,
        /// <summary>Step was skipped (optional step)</summary>
        Skipped
    }

    /// <summary>
    /// Animation transition type for step changes.
    /// </summary>
    public enum TransitionType
    {
        /// <summary>Slide in from the right (forward) or left (backward).</summary>
        Slide,
        /// <summary>Cross-fade between steps.</summary>
        Fade,
        /// <summary>Zoom out old, zoom in new.</summary>
        Zoom,
        /// <summary>Compress old to center, expand new from center (3D flip-like).</summary>
        Flip,
        /// <summary>Instant swap with no animation.</summary>
        None
    }

    /// <summary>
    /// Easing function for transition animations.
    /// </summary>
    public enum TransitionEasing
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseOutQuint,
        EaseOutBack,
        EaseInOutBack,
        EaseOutBounce,
        EaseOutElastic,
        Spring
    }

    /// <summary>
    /// Direction of navigation through wizard steps.
    /// </summary>
    public enum NavigationDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// Wizard navigation mode.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>Sequential step-by-step with back/next buttons.</summary>
        Sequential,
        /// <summary>Click any step title in breadcrumb path to navigate.</summary>
        Breadcrumb
    }
}
