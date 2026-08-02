using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Keyboard routing, and the actions the keyboard handler invokes.
    /// </summary>
    /// <remarks>
    /// Moved out of <c>BeepFilter.cs</c> alongside the inline editor. These members previously sat
    /// under two regions named <c>Phase 1: …</c> - names that recorded the increment which added the
    /// code rather than what it does, which is no help to anyone reading it afterwards.
    /// </remarks>
    public partial class BeepFilter
    {
        #region Keyboard routing

        /// <summary>
        /// Processes keyboard commands (Ctrl+F, Ctrl+N, etc.)
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (KeyboardShortcutsEnabled && _keyboardHandler != null)
            {
                var e = new KeyEventArgs(keyData);
                if (_keyboardHandler.ProcessKeyPress(e))
                {
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Actions invoked by the keyboard handler

        /// <summary>
        /// Focuses the quick search field (Ctrl+F)
        /// </summary>
        internal void FocusQuickSearch()
        {
            // Find quick search hit area and raise event
            OnSearchFocusRequested(ClientRectangle);
            Invalidate();
        }

        /// <summary>
        /// Shows command palette (Ctrl+K)
        /// </summary>
        internal void ShowCommandPalette()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Command Palette", "Ctrl+K", "beepfilter-command-palette-pending");
        }

        /// <summary>
        /// Adds a new blank filter - wrapper for Ctrl+N shortcut
        /// </summary>
        internal void AddNewFilterViaKeyboard()
        {
            // Call existing private method
            AddNewFilter();
        }

        /// <summary>
        /// Undoes the last change (Ctrl+Z)
        /// </summary>
        internal void UndoLastChange()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Undo", "Ctrl+Z", "beepfilter-undo-pending");
        }

        /// <summary>
        /// Redoes the last undone change (Ctrl+Y)
        /// </summary>
        internal void RedoLastChange()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Redo", "Ctrl+Y", "beepfilter-redo-pending");
        }

        /// <summary>
        /// Saves the current filter as a view (Ctrl+S)
        /// </summary>
        internal void SaveCurrentView()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Save View", "Ctrl+S", "beepfilter-save-view-pending");
        }

        /// <summary>
        /// Opens saved filter views (Ctrl+O)
        /// </summary>
        internal void OpenSavedView()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Open View", "Ctrl+O", "beepfilter-open-view-pending");
        }

        /// <summary>
        /// Selects all filters (Ctrl+A)
        /// </summary>
        internal void SelectAllFilters()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending("Select All", "Ctrl+A", "beepfilter-select-all-pending");
        }

        /// <summary>
        /// Duplicates a filter (Ctrl+D)
        /// </summary>
        internal void DuplicateFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                var original = _activeFilter.Criteria[index];
                var duplicate = new FilterCriteria
                {
                    ColumnName = original.ColumnName,
                    Operator = original.Operator,
                    Value = original.Value,
                    Value2 = original.Value2,
                    CaseSensitive = original.CaseSensitive
                };

                _activeFilter.Criteria.Insert(index + 1, duplicate);
                _filterCount = _activeFilter.Criteria.Count;

                RecalculateLayout();
                Invalidate();
                OnFilterAdded();
            }
        }

        /// <summary>
        /// Shows advanced filter dialog (Ctrl+Shift+F)
        /// </summary>
        internal void ShowAdvancedFilterDialog()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending(
                "Advanced Filter",
                "Ctrl+Shift+F",
                "beepfilter-advanced-pending");
        }

        /// <summary>
        /// Clears all filters - wrapper for Ctrl+Shift+C shortcut
        /// </summary>
        internal void ClearAllFiltersViaKeyboard()
        {
            // Call existing private method
            ClearAllFilters();
        }

        /// <summary>
        /// Deletes selected filters (Ctrl+Shift+D)
        /// </summary>
        internal void DeleteSelectedFilters()
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending(
                "Delete Selected",
                "Ctrl+Shift+D",
                "beepfilter-delete-selected-pending");
        }

        /// <summary>
        /// Exports filters (Ctrl+Shift+E)
        /// </summary>
        internal void ExportFilters()
        {
            var dialogManager = CreateDialogManager();
            var path = dialogManager.SaveFileWithConfirm(
                filter: "JSON files (*.json)|*.json|All files (*.*)|*.*",
                defaultFileName: "filters.json",
                title: "Export Filters");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                var snapshot = _activeFilter?.Clone() ?? new FilterConfiguration();
                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);

                var sourceName = Path.GetFileName(path);
                dialogManager.NotifyExportSuccess(sourceName, snapshot.Criteria?.Count ?? 0);
            }
            catch (Exception ex)
            {
                dialogManager.NotifyExportFailure(Path.GetFileName(path), ex.Message);
            }
        }

        /// <summary>
        /// Imports filters (Ctrl+Shift+I)
        /// </summary>
        internal void ImportFilters()
        {
            var dialogManager = CreateDialogManager();
            var path = dialogManager.OpenFile(
                filter: "JSON files (*.json)|*.json|All files (*.*)|*.*",
                title: "Import Filters");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(path);
                var imported = JsonSerializer.Deserialize<FilterConfiguration>(json);
                if (imported == null)
                {
                    dialogManager.NotifyImportFailure(Path.GetFileName(path), "Invalid filter file format.");
                    return;
                }

                imported.Criteria ??= new List<FilterCriteria>();
                imported.ModifiedDate = DateTime.Now;

                _activeFilter = imported;
                _filterCount = _activeFilter.Criteria.Count;
                OnFilterChanged();
                RecalculateLayout();
                Invalidate();

                dialogManager.NotifyImportSuccess(Path.GetFileName(path), _filterCount);
            }
            catch (Exception ex)
            {
                dialogManager.NotifyImportFailure(Path.GetFileName(path), ex.Message);
            }
        }

        private BeepDialogManager CreateDialogManager()
        {
            var manager = BeepDialogManager.Instance;
            manager.SetHostForm(FindForm());
            return manager;
        }

        /// <summary>
        /// Moves a filter up or down (Alt+Up/Down)
        /// </summary>
        internal void MoveFilter(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < _activeFilter.Criteria.Count &&
                toIndex >= 0 && toIndex < _activeFilter.Criteria.Count)
            {
                var item = _activeFilter.Criteria[fromIndex];
                _activeFilter.Criteria.RemoveAt(fromIndex);
                _activeFilter.Criteria.Insert(toIndex, item);

                RecalculateLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Activates a saved view by index (Alt+1-9)
        /// </summary>
        internal void ActivateSavedView(int viewIndex)
        {
            var dialogManager = CreateDialogManager();
            dialogManager.NotifyFeaturePending(
                $"Activate View {viewIndex + 1}",
                $"Alt+{viewIndex + 1}",
                $"beepfilter-saved-view-pending::{viewIndex}");
        }

        /// <summary>
        /// Applies the current filters - wrapper for Enter key
        /// </summary>
        internal void ApplyFiltersViaKeyboard()
        {
            // Call existing private method
            ApplyFilters();
        }

        /// <summary>
        /// Removes a filter by index (Delete)
        /// </summary>
        internal void RemoveFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                _activeFilter.Criteria.RemoveAt(index);
                _filterCount = _activeFilter.Criteria.Count;

                RecalculateLayout();
                Invalidate();
                OnFilterRemoved(index);
            }
        }

        /// <summary>
        /// Closes the filter UI (Escape when no filters)
        /// </summary>
        internal void CloseFilterUI()
        {
            if (CollapsesWhenInactive)
            {
                IsExpanded = false;
            }
        }

        /// <summary>
        /// True when this display mode shows only the header until the filter is engaged.
        /// </summary>
        /// <remarks>
        /// <see cref="FilterDisplayMode.Collapsible"/> and <see cref="FilterDisplayMode.OnHover"/>
        /// share one collapse behaviour and differ only in what expands them - a click or the
        /// pointer. OnHover previously had no implementation at all: it was declared, exposed as a
        /// browsable property, and never compared anywhere, so selecting it produced an always-
        /// visible filter indistinguishable from the default.
        /// </remarks>
        internal bool CollapsesWhenInactive
            => _displayMode == FilterDisplayMode.Collapsible
               || _displayMode == FilterDisplayMode.OnHover;


        /// <summary>
        /// Edits a filter by index (F2)
        /// </summary>
        internal void EditFilter(int index)
        {
            if (index >= 0 && index < _activeFilter.Criteria.Count)
            {
                // Raise event to show edit UI for this filter
                OnValueInputRequested(index, ClientRectangle);
            }
        }

        /// <summary>
        /// Shows keyboard shortcuts help (F1)
        /// </summary>
        internal void ShowKeyboardShortcutsHelp()
        {
            if (_keyboardHandler != null)
            {
                string help = _keyboardHandler.GetShortcutsHelp();
                var dialogManager = CreateDialogManager();
                dialogManager.Info("BeepFilter Keyboard Shortcuts", help);
            }
        }

        #endregion
    }
}
