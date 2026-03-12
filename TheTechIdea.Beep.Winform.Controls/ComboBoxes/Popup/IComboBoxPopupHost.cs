using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    // ──────────────────────────────────────────────────────────────────────────
    // Event argument types
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Raised when the user commits a single row selection in the popup
    /// (click, Enter, or touch tap on a selectable row).
    /// </summary>
    internal sealed class ComboBoxRowCommittedEventArgs : EventArgs
    {
        /// <summary>The row that was committed.</summary>
        public ComboBoxPopupRowModel Row { get; }

        /// <summary>Whether the caller should close the popup after committing.</summary>
        public bool ClosePopup { get; }

        public ComboBoxRowCommittedEventArgs(ComboBoxPopupRowModel row, bool closePopup = true)
        {
            Row        = row;
            ClosePopup = closePopup;
        }
    }

    /// <summary>
    /// Raised when the popup closes (for any reason: commit, Escape, outside click).
    /// </summary>
    internal sealed class ComboBoxPopupClosedEventArgs : EventArgs
    {
        /// <summary>
        /// True when the close action should be treated as a commit
        /// (row was selected or Apply was pressed).
        /// False for Escape, outside-click dismissal, or Cancel.
        /// </summary>
        public bool Committed { get; }

        public ComboBoxPopupClosedEventArgs(bool committed)
        {
            Committed = committed;
        }
    }

    /// <summary>
    /// Raised when the user types into the search box inside the popup.
    /// </summary>
    internal sealed class ComboBoxSearchChangedEventArgs : EventArgs
    {
        public string SearchText { get; }

        public ComboBoxSearchChangedEventArgs(string searchText)
        {
            SearchText = searchText ?? string.Empty;
        }
    }

    /// <summary>
    /// Raised when keyboard navigation moves focus to a different row.
    /// </summary>
    internal sealed class ComboBoxKeyboardFocusChangedEventArgs : EventArgs
    {
        /// <summary>0-based index into <c>ComboBoxPopupModel.FilteredRows</c>.</summary>
        public int FocusIndex { get; }

        public ComboBoxKeyboardFocusChangedEventArgs(int focusIndex)
        {
            FocusIndex = focusIndex;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Host interface
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Abstraction for the popup window that hosts a combo box dropdown.
    /// <para>
    /// Both <see cref="BeepComboBox"/> and <see cref="BeepDropDownCheckBoxSelect"/>
    /// drive the same interface, so they can share one popup architecture regardless
    /// of which concrete form class is used underneath.
    /// </para>
    /// <para>
    /// Concrete implementations (a single shared host, or distinct per-variant forms)
    /// should be placed in <c>ComboBoxes/Popup/</c>.  The interface purposely contains
    /// no WinForms-specific surface so that future non-WinForms hosting remains possible.
    /// </para>
    /// </summary>
    internal interface IComboBoxPopupHost : IDisposable
    {
        // ── State ──────────────────────────────────────────────────────────
        /// <summary>True while the popup window is visible.</summary>
        bool IsVisible { get; }

        // ── Lifecycle ─────────────────────────────────────────────────────
        /// <summary>
        /// Shows the popup below (or above when flipped) the trigger control.
        /// </summary>
        /// <param name="owner">The combo box control that owns this popup.</param>
        /// <param name="model">Initial content model.</param>
        /// <param name="triggerBounds">
        /// Screen bounds of the trigger control, used for placement and width inference.
        /// </param>
        void ShowPopup(Control owner, ComboBoxPopupModel model, Rectangle triggerBounds);

        /// <summary>
        /// Closes the popup.
        /// </summary>
        /// <param name="commit">
        /// Pass <c>true</c> when the close should be treated as a commit
        /// (raises <see cref="PopupClosed"/> with <c>Committed = true</c>).
        /// </param>
        void ClosePopup(bool commit);

        // ── Model refresh ─────────────────────────────────────────────────
        /// <summary>
        /// Replaces the content model while the popup is still open (e.g. after
        /// the item list is updated asynchronously or after search filtering).
        /// No-op when the popup is not visible.
        /// </summary>
        void UpdateModel(ComboBoxPopupModel model);

        // ── Incremental updates ───────────────────────────────────────────
        /// <summary>
        /// Sets the search box text inside the popup without rebuilding the full model.
        /// Raises <see cref="SearchTextChanged"/> if the text differs.
        /// </summary>
        void UpdateSearchText(string text);

        /// <summary>
        /// Moves keyboard focus to the row at <paramref name="index"/> in the
        /// filtered rows list.  Clamps silently to valid bounds.
        /// </summary>
        void SetKeyboardFocusIndex(int index);

        // ── Events ─────────────────────────────────────────────────────────
        /// <summary>
        /// Raised when the user commits a row (click, Enter, external accept action).
        /// The handler should update <see cref="BeepComboBox.SelectedItem"/> /
        /// <see cref="BeepComboBox.SelectedItems"/> then call <see cref="ClosePopup"/>
        /// if <c>ClosePopup == true</c> in the args.
        /// </summary>
        event EventHandler<ComboBoxRowCommittedEventArgs> RowCommitted;

        /// <summary>
        /// Raised when the popup closes for any reason. Inspect
        /// <c>ComboBoxPopupClosedEventArgs.Committed</c> to decide whether to apply
        /// or discard pending state (relevant for apply/cancel multi-select flows).
        /// </summary>
        event EventHandler<ComboBoxPopupClosedEventArgs> PopupClosed;

        /// <summary>Raised when the user types into the popup's search box.</summary>
        event EventHandler<ComboBoxSearchChangedEventArgs> SearchTextChanged;

        /// <summary>
        /// Raised when keyboard navigation moves focus to a different row.
        /// The owner can use this to preview the focused item in the field
        /// without committing it yet.
        /// </summary>
        event EventHandler<ComboBoxKeyboardFocusChangedEventArgs> KeyboardFocusChanged;
    }
}
