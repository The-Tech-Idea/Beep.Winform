namespace TheTechIdea.Beep.Winform.Controls.Models
{
    /// <summary>
    /// Controls how much vertical space is reserved under the input for helper text and/or character counter.
    /// </summary>
    public enum SupportingSpaceMode
    {
        Off = 0,                 // No space unless helper text actually present
        WhenHasTextOrCounter = 1, // Reserve when helper text exists or counter is enabled
        Always = 2               // Always reserve space regardless of content
    }
}
