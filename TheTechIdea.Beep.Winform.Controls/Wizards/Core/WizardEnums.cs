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
        Minimal
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
}
