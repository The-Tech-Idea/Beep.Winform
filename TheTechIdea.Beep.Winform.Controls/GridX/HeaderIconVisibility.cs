namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Controls how sort/filter header icons are displayed.
    /// </summary>
    public enum HeaderIconVisibility
    {
        /// <summary>
        /// Always show the icon.
        /// </summary>
        Always = 0,

        /// <summary>
        /// Show the icon on header hover (active sorted/filtered columns stay visible).
        /// </summary>
        HoverOnly = 1,

        /// <summary>
        /// Never show the icon.
        /// </summary>
        Hidden = 2
    }
}
