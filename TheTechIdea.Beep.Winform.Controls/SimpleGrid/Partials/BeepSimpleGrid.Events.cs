using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing all event declarations for BeepSimpleGrid
    /// Organizes all custom and standard events in one location
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Data Events

        /// <summary>
        /// Event raised when data source changes
        /// </summary>
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Event raised when data is loaded
        /// </summary>
        public event EventHandler DataLoaded;

        #endregion

        #region Cell Events

        /// <summary>
        /// Event raised when a cell is clicked
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellClick;

        /// <summary>
        /// Event raised when a cell is double-clicked
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellDoubleClick;

        /// <summary>
        /// Event raised when mouse enters a cell
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellMouseEnter;

        /// <summary>
        /// Event raised when mouse leaves a cell
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellMouseLeave;

        /// <summary>
        /// Event raised when mouse button is pressed on a cell
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellMouseDown;

        /// <summary>
        /// Event raised when mouse button is released on a cell
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellMouseUp;

        /// <summary>
        /// Event raised before cell value is updated
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellPreUpdateCellValue;

        /// <summary>
        /// Event raised for custom cell drawing
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellCustomCellDraw;

        /// <summary>
        /// Event raised when a cell value changes
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellValueChanged;

        /// <summary>
        /// Event raised when a cell value is changing
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellValueChanging;

        /// <summary>
        /// Event raised when a cell is validating
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellValidating;

        /// <summary>
        /// Event raised when a cell has been validated
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellValidated;

        /// <summary>
        /// Event raised when a cell is formatting
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellFormatting;

        /// <summary>
        /// Event raised when a cell has been formatted
        /// </summary>
        public event EventHandler<BeepCellEventArgs> CellFormatted;

        #endregion

        #region Row Events

        /// <summary>
        /// Event raised when current row changes
        /// </summary>
        public event EventHandler<BeepRowSelectedEventArgs> CurrentRowChanged;

        #endregion

        #region Cell Selection Events

        /// <summary>
        /// Event raised when current cell changes
        /// </summary>
        public event EventHandler<BeepCellSelectedEventArgs> CurrentCellChanged;

        #endregion

        #region Filtering and Sorting Events

        /// <summary>
        /// Event raised when a column filter icon is clicked
        /// </summary>
        public event EventHandler<ColumnFilterEventArgs> ColumnFilterClicked;

        /// <summary>
        /// Event raised when a column sort icon is clicked
        /// </summary>
        public event EventHandler<ColumnSortEventArgs> ColumnSortClicked;

        #endregion

        #region Editor Events

        /// <summary>
        /// Event raised when editor is closed
        /// </summary>
        public event EventHandler EditorClosed;

        #endregion

        #region Navigation Events

        /// <summary>
        /// Event raised when printer is called
        /// </summary>
        public event EventHandler CallPrinter;

        /// <summary>
        /// Event raised when sending/sharing message
        /// </summary>
        public event EventHandler SendMessage;

        /// <summary>
        /// Event raised when search/filter is shown
        /// </summary>
        public event EventHandler ShowSearch;

        /// <summary>
        /// Event raised when a new record is created
        /// </summary>
        public event EventHandler NewRecordCreated;

        /// <summary>
        /// Event raised when save is called
        /// </summary>
        public event EventHandler SaveCalled;

        /// <summary>
        /// Event raised when delete is called
        /// </summary>
        public event EventHandler DeleteCalled;

        /// <summary>
        /// Event raised when edit is called
        /// </summary>
        public event EventHandler EditCalled;

        #endregion
    }
}
