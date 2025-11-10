namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Partial class containing navigation and selection methods for BeepGridPro.
    /// </summary>
    public partial class BeepGridPro
    {
        #region Selection and Navigation Methods
        /// <summary>
        /// Selects a specific cell in the grid.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell to select.</param>
        /// <param name="columnIndex">The column index of the cell to select.</param>
        public void SelectCell(int rowIndex, int columnIndex)
        {
            Selection.SelectCell(rowIndex, columnIndex);
            Invalidate();
        }

        /// <summary>
        /// Attaches a binding navigator to the grid for data navigation.
        /// </summary>
        /// <param name="navigator">The binding navigator instance.</param>
        /// <param name="dataSource">The data source to navigate.</param>
        public void AttachNavigator(BeepBindingNavigator navigator, object dataSource)
        {
            Navigator.Attach(navigator, dataSource);
        }

        /// <summary>Navigates to the first record.</summary>
        public void MoveFirst() => Navigator.MoveFirst();

        /// <summary>Navigates to the previous record.</summary>
        public void MovePrevious() => Navigator.MovePrevious();

        /// <summary>Navigates to the next record.</summary>
        public void MoveNext() => Navigator.MoveNext();

        /// <summary>Navigates to the last record.</summary>
        public void MoveLast() => Navigator.MoveLast();

        /// <summary>Inserts a new record.</summary>
        public void InsertNew() => Navigator.InsertNew();

        /// <summary>Deletes the current record.</summary>
        public void DeleteCurrent() => Navigator.DeleteCurrent();

        /// <summary>Saves changes to the data source.</summary>
        public void Save() => Navigator.Save();

        /// <summary>Cancels pending changes.</summary>
        public void Cancel() => Navigator.Cancel();
        #endregion
    }
}
