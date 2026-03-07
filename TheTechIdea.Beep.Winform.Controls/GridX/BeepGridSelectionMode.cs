namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Determines how rows and cells can be selected in <see cref="BeepGridPro"/>.
    /// </summary>
    public enum BeepGridSelectionMode
    {
        /// <summary>Clicking a cell selects only that cell.</summary>
        CellSelect,

        /// <summary>Clicking any cell in a row selects the entire row.</summary>
        FullRowSelect,

        /// <summary>Clicking a column header selects the entire column.</summary>
        FullColumnSelect
    }
}
