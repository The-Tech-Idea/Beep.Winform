namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing clipboard operations for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Clipboard Operations

        /// <summary>
        /// Copy selected rows to clipboard in tab-delimited format (Excel compatible)
        /// </summary>
        /// <param name="includeHeaders">Include column headers in the copied data</param>
        /// <param name="visibleColumnsOnly">Copy only visible columns (exclude hidden columns)</param>
        public void CopyToClipboard(bool includeHeaders = true, bool visibleColumnsOnly = true)
        {
            Clipboard.CopyToClipboard(includeHeaders, visibleColumnsOnly);
        }

        /// <summary>
        /// Cut selected rows to clipboard (copy and mark for deletion after paste)
        /// </summary>
        /// <param name="includeHeaders">Include column headers in the copied data</param>
        /// <param name="visibleColumnsOnly">Copy only visible columns (exclude hidden columns)</param>
        public void CutToClipboard(bool includeHeaders = true, bool visibleColumnsOnly = true)
        {
            Clipboard.CutToClipboard(includeHeaders, visibleColumnsOnly);
        }

        /// <summary>
        /// Paste data from clipboard into grid starting at current selection
        /// </summary>
        /// <param name="pasteOption">Specify what to paste (All, ValuesOnly, or FormattingOnly)</param>
        public void PasteFromClipboard(Helpers.PasteOptions pasteOption = Helpers.PasteOptions.All)
        {
            Clipboard.PasteFromClipboard(pasteOption);
        }

        /// <summary>
        /// Copy only the currently selected cell value to clipboard
        /// </summary>
        public void CopyCellToClipboard()
        {
            Clipboard.CopyCellToClipboard();
        }

        #endregion
    }
}
