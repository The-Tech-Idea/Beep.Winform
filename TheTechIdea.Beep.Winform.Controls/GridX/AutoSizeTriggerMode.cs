namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Controls when automatic sizing is applied.
    /// </summary>
    public enum AutoSizeTriggerMode
    {
        /// <summary>
        /// Auto-size runs only when called explicitly.
        /// </summary>
        Manual = 0,

        /// <summary>
        /// Auto-size runs after data binding/refresh operations.
        /// </summary>
        OnDataBind = 1,

        /// <summary>
        /// Auto-size runs after committed cell edits.
        /// </summary>
        OnEditCommit = 2,

        /// <summary>
        /// Auto-size runs after sort/filter operations.
        /// </summary>
        OnSortFilter = 3,

        /// <summary>
        /// Auto-size runs for any trigger but is debounced.
        /// </summary>
        AlwaysDebounced = 4
    }

    internal enum AutoSizeTriggerSource
    {
        DataBind,
        EditCommit,
        SortFilter
    }
}
