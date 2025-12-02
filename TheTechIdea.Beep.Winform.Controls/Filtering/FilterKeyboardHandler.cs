using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// Handles keyboard shortcuts and navigation for BeepFilter
    /// Provides professional keyboard-driven interaction patterns
    /// </summary>
    public class FilterKeyboardHandler
    {
        private readonly BeepFilter _filter;
        private int _focusedFilterIndex = -1;
        
        /// <summary>
        /// Gets or sets the index of the currently focused filter
        /// </summary>
        public int FocusedFilterIndex
        {
            get => _focusedFilterIndex;
            set
            {
                if (value >= -1 && value < (_filter?.ActiveFilter?.Criteria?.Count ?? 0))
                {
                    _focusedFilterIndex = value;
                    _filter?.Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Initializes the keyboard handler
        /// </summary>
        public FilterKeyboardHandler(BeepFilter filter)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }
        
        /// <summary>
        /// Processes keyboard input for filter operations
        /// </summary>
        public bool ProcessKeyPress(KeyEventArgs e)
        {
            // Ctrl+ shortcuts
            if (e.Control && !e.Shift && !e.Alt)
            {
                return ProcessControlShortcuts(e.KeyCode);
            }
            
            // Ctrl+Shift+ shortcuts
            if (e.Control && e.Shift && !e.Alt)
            {
                return ProcessControlShiftShortcuts(e.KeyCode);
            }
            
            // Alt+ shortcuts
            if (e.Alt && !e.Control && !e.Shift)
            {
                return ProcessAltShortcuts(e.KeyCode);
            }
            
            // Direct keys (no modifiers)
            if (!e.Control && !e.Shift && !e.Alt)
            {
                return ProcessDirectKeys(e.KeyCode);
            }
            
            return false;
        }
        
        /// <summary>
        /// Processes Ctrl+ shortcuts
        /// </summary>
        private bool ProcessControlShortcuts(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F:
                    // Ctrl+F: Focus quick search
                    _filter.FocusQuickSearch();
                    return true;
                
                case Keys.K:
                    // Ctrl+K: Show command palette (VS Code style)
                    _filter.ShowCommandPalette();
                    return true;
                
                case Keys.N:
                    // Ctrl+N: Add new filter
                    _filter.AddNewFilterViaKeyboard();
                    return true;
                
                case Keys.Z:
                    // Ctrl+Z: Undo
                    _filter.UndoLastChange();
                    return true;
                
                case Keys.Y:
                    // Ctrl+Y: Redo
                    _filter.RedoLastChange();
                    return true;
                
                case Keys.S:
                    // Ctrl+S: Save current filter view
                    _filter.SaveCurrentView();
                    return true;
                
                case Keys.O:
                    // Ctrl+O: Open saved filter view
                    _filter.OpenSavedView();
                    return true;
                
                case Keys.A:
                    // Ctrl+A: Select all filters (for bulk operations)
                    _filter.SelectAllFilters();
                    return true;
                
                case Keys.D:
                    // Ctrl+D: Duplicate focused filter
                    if (_focusedFilterIndex >= 0)
                    {
                        _filter.DuplicateFilter(_focusedFilterIndex);
                        return true;
                    }
                    break;
            }
            
            return false;
        }
        
        /// <summary>
        /// Processes Ctrl+Shift+ shortcuts
        /// </summary>
        private bool ProcessControlShiftShortcuts(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F:
                    // Ctrl+Shift+F: Advanced filter dialog
                    _filter.ShowAdvancedFilterDialog();
                    return true;
                
                case Keys.Z:
                    // Ctrl+Shift+Z: Redo (alternative to Ctrl+Y)
                    _filter.RedoLastChange();
                    return true;
                
                case Keys.C:
                    // Ctrl+Shift+C: Clear all filters
                    _filter.ClearAllFiltersViaKeyboard();
                    return true;
                
                case Keys.D:
                    // Ctrl+Shift+D: Delete selected filters
                    _filter.DeleteSelectedFilters();
                    return true;
                
                case Keys.E:
                    // Ctrl+Shift+E: Export filters
                    _filter.ExportFilters();
                    return true;
                
                case Keys.I:
                    // Ctrl+Shift+I: Import filters
                    _filter.ImportFilters();
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Processes Alt+ shortcuts
        /// </summary>
        private bool ProcessAltShortcuts(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Up:
                    // Alt+Up: Move focused filter up
                    if (_focusedFilterIndex > 0)
                    {
                        _filter.MoveFilter(_focusedFilterIndex, _focusedFilterIndex - 1);
                        _focusedFilterIndex--;
                        return true;
                    }
                    break;
                
                case Keys.Down:
                    // Alt+Down: Move focused filter down
                    if (_focusedFilterIndex >= 0 && _focusedFilterIndex < _filter.ActiveFilter.Criteria.Count - 1)
                    {
                        _filter.MoveFilter(_focusedFilterIndex, _focusedFilterIndex + 1);
                        _focusedFilterIndex++;
                        return true;
                    }
                    break;
                
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    // Alt+1-9: Quick switch to saved view
                    int viewIndex = (int)keyCode - (int)Keys.D1;
                    _filter.ActivateSavedView(viewIndex);
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Processes direct key presses (no modifiers)
        /// </summary>
        private bool ProcessDirectKeys(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Escape:
                    // Escape: Clear filters or close dialog
                    if (_filter.ActiveFilter.Criteria.Count > 0)
                    {
                        _filter.ClearAllFiltersViaKeyboard();
                    }
                    else
                    {
                        _filter.CloseFilterUI();
                    }
                    return true;
                
                case Keys.Enter:
                    // Enter: Apply filters
                    _filter.ApplyFiltersViaKeyboard();
                    return true;
                
                case Keys.Delete:
                    // Delete: Remove focused filter
                    if (_focusedFilterIndex >= 0)
                    {
                        _filter.RemoveFilter(_focusedFilterIndex);
                        return true;
                    }
                    break;
                
                case Keys.Tab:
                    // Tab: Navigate to next filter
                    NavigateToNextFilter();
                    return true;
                
                case Keys.F1:
                    // F1: Show help/shortcuts
                    _filter.ShowKeyboardShortcutsHelp();
                    return true;
                
                case Keys.F2:
                    // F2: Edit focused filter
                    if (_focusedFilterIndex >= 0)
                    {
                        _filter.EditFilter(_focusedFilterIndex);
                        return true;
                    }
                    break;
            }
            
            return false;
        }
        
        /// <summary>
        /// Navigates to the next filter in the list
        /// </summary>
        private void NavigateToNextFilter()
        {
            if (_filter?.ActiveFilter?.Criteria == null) return;
            
            int count = _filter.ActiveFilter.Criteria.Count;
            if (count == 0) return;
            
            _focusedFilterIndex = (_focusedFilterIndex + 1) % count;
            _filter.Invalidate();
        }
        
        /// <summary>
        /// Navigates to the previous filter in the list
        /// </summary>
        private void NavigateToPreviousFilter()
        {
            if (_filter?.ActiveFilter?.Criteria == null) return;
            
            int count = _filter.ActiveFilter.Criteria.Count;
            if (count == 0) return;
            
            _focusedFilterIndex--;
            if (_focusedFilterIndex < 0)
                _focusedFilterIndex = count - 1;
            
            _filter.Invalidate();
        }
        
        /// <summary>
        /// Gets a description of all available shortcuts
        /// </summary>
        public string GetShortcutsHelp()
        {
            return @"
BeepFilter Keyboard Shortcuts:

FILTER OPERATIONS:
  Ctrl+F          Quick search / filter
  Ctrl+Shift+F    Advanced filter dialog
  Ctrl+N          Add new filter
  Enter           Apply filters
  Escape          Clear filters / Close

NAVIGATION:
  Tab             Next filter
  Shift+Tab       Previous filter
  Delete          Remove focused filter
  F2              Edit focused filter

EDITING:
  Ctrl+Z          Undo
  Ctrl+Y          Redo
  Ctrl+Shift+Z    Redo (alternative)
  Ctrl+D          Duplicate filter
  Alt+Up          Move filter up
  Alt+Down        Move filter down

VIEWS & TEMPLATES:
  Ctrl+S          Save filter view
  Ctrl+O          Open saved view
  Alt+1-9         Quick switch to saved view

BULK OPERATIONS:
  Ctrl+A          Select all filters
  Ctrl+Shift+D    Delete selected filters
  Ctrl+Shift+C    Clear all filters

IMPORT/EXPORT:
  Ctrl+Shift+E    Export filters
  Ctrl+Shift+I    Import filters

HELP:
  F1              Show this help
";
        }
    }
}

