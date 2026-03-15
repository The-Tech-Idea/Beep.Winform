# Phase 4 — Cleanup & Polish: Dispose, Sync, Keyboard Redirect, Theme

**Priority**: Low  
**Status**: Not Started  
**Depends on**: Phase 1 (search text box must exist)

## Problem

After Phases 1–3 add the live `BeepTextBox` search and fix layout/scrolling, several loose ends remain:

1. **Dispose**: `_searchTextBox` must be unsubscribed and disposed alongside the existing scrollbar cleanup in `Dispose(bool)`.
2. **Bidirectional SearchText sync**: The `SearchText` property setter and the `_searchTextBox.TextChanged` handler can infinitely recurse if not guarded.
3. **Keyboard type-ahead redirects to wrong target**: `OnKeyPress()` currently appends chars directly to `SearchText` when `ShowSearch == true`. After Phase 1 the live `BeepTextBox` handles input — the old code path should redirect focus to `_searchTextBox` instead of duplicating logic.
4. **Backspace handler also duplicates**: `OnKeyDown()` manually trims `SearchText` via `Substring`. This should also redirect to `_searchTextBox`.
5. **Theme propagation**: When `ApplyTheme()` is called, the search text box needs its theme updated too.
6. **Tab stop order**: `_searchTextBox.TabStop` should be `false` to keep keyboard nav on the list. The search box should only receive focus when clicked or when a printable key is pressed with `ShowSearch == true`.

---

## Steps

### Step 4.1 — Dispose _searchTextBox properly

**File**: `BeepListBox.Core.cs`  
**Location**: `Dispose(bool disposing)` method (around line 480)

Add search text box cleanup alongside existing scrollbar disposal:

```csharp
// In Dispose(bool disposing), after scrollbar cleanup:
if (_searchTextBox != null)
{
    _searchTextBox.TextChanged -= SearchTextBox_TextChanged;
    if (Controls.Contains(_searchTextBox)) Controls.Remove(_searchTextBox);
    _searchTextBox.Dispose();
    _searchTextBox = null;
}
```

**Why**: Prevents event handler leaks and orphaned child controls.

---

### Step 4.2 — Guard bidirectional SearchText sync

**File**: `BeepListBox.Properties.cs`  
**Location**: `SearchText` property setter

Add a re-entrancy guard to avoid infinite loop:

```csharp
// New field in Core.cs:
private bool _suppressSearchSync;
```

Update `SearchText` setter:
```csharp
public string SearchText
{
    get => _searchText;
    set
    {
        if (_searchText == value) return;
        _searchText = value ?? string.Empty;

        if (!_suppressSearchSync && _searchTextBox != null && _searchTextBox.Text != _searchText)
        {
            _suppressSearchSync = true;
            try { _searchTextBox.Text = _searchText; }
            finally { _suppressSearchSync = false; }
        }

        ApplyFilter();
        Invalidate();
    }
}
```

Update `SearchTextBox_TextChanged` handler (from Phase 1):
```csharp
private void SearchTextBox_TextChanged(object sender, EventArgs e)
{
    if (_suppressSearchSync) return;

    _suppressSearchSync = true;
    try { SearchText = _searchTextBox?.Text ?? string.Empty; }
    finally { _suppressSearchSync = false; }
}
```

**Why**: Without the guard, setting `SearchText` updates `_searchTextBox.Text`, which fires `TextChanged`, which sets `SearchText` again — infinite loop.

---

### Step 4.3 — Redirect keyboard type-ahead to _searchTextBox

**File**: `BeepListBox.Keyboard.cs`  
**Location**: `OnKeyPress()` method, search-mode branch (around line 225–240)

Change from:
```csharp
if (_showSearch && _listBoxPainter != null && _listBoxPainter.SupportsSearch())
{
    SearchText = (SearchText ?? string.Empty) + e.KeyChar;
    var filtered = _helper?.GetVisibleItems();
    if (filtered != null && filtered.Count > 0)
    {
        FocusedIndex = 0;
        if (SelectionFollowsFocus && SelectionMode == SelectionModeEnum.Single)
        {
            SelectedItem = filtered[0];
        }
    }
    e.Handled = true;
    return;
}
```

Change to:
```csharp
if (_showSearch && _searchTextBox != null)
{
    // Redirect focus to the live search text box
    if (!_searchTextBox.Focused)
    {
        _searchTextBox.Focus();
    }
    // Let the text box handle the character naturally — don't set SearchText manually
    // The TextChanged event will sync back via SearchTextBox_TextChanged
    e.Handled = true;
    return;
}
```

**Why**: After Phase 1, the `BeepTextBox` is the canonical search input. Duplicating character accumulation creates desync between what the text box shows and what `SearchText` contains.

**Note**: The `SendKeys` approach is fragile. A better pattern is to focus the textbox and let the WinForms input pipeline deliver the keystroke. If the character is lost, append it to `_searchTextBox.Text` manually as a fallback.

---

### Step 4.4 — Redirect Backspace to _searchTextBox

**File**: `BeepListBox.Keyboard.cs`  
**Location**: `OnKeyDown()`, `Keys.Back` case (around line 200–210)

Change from:
```csharp
case Keys.Back:
    if (_showSearch && _listBoxPainter != null && _listBoxPainter.SupportsSearch())
    {
        if (!string.IsNullOrEmpty(SearchText))
        {
            SearchText = SearchText.Length > 1 ? SearchText.Substring(0, SearchText.Length - 1) : string.Empty;
            var filtered = _helper?.GetVisibleItems();
            if (filtered != null && filtered.Count > 0)
            {
                FocusedIndex = 0;
            }
        }
        e.Handled = e.SuppressKeyPress = true;
    }
    break;
```

Change to:
```csharp
case Keys.Back:
    if (_showSearch && _searchTextBox != null)
    {
        if (!_searchTextBox.Focused)
        {
            _searchTextBox.Focus();
        }
        // Let the text box handle backspace natively
        e.Handled = e.SuppressKeyPress = true;
    }
    break;
```

**Why**: Same reasoning as Step 4.3 — the text box should own its own editing behavior. Manual substring manipulation is fragile and doesn't handle cursor position.

---

### Step 4.5 — Propagate theme to _searchTextBox

**File**: `BeepListBox.Core.cs` or wherever `ApplyTheme()` is defined  
**Location**: End of `ApplyTheme()` method

Add:
```csharp
// In ApplyTheme(), after existing theme application:
if (_searchTextBox != null)
{
    _searchTextBox.Theme = Theme;
    _searchTextBox.ApplyThemeOnImage = false;
    _searchTextBox.ApplyTheme();
}
```

**Why**: Without this, the search text box retains its default theme when the list box theme changes at runtime.

---

### Step 4.6 — Set correct TabStop behavior

**File**: `BeepListBox.Core.cs`  
**Location**: `InitializeSearchTextBox()` (from Phase 1)

Ensure these properties are set during construction:
```csharp
_searchTextBox.TabStop = false;    // Tab should skip the search box (nav stays on list)
_searchTextBox.IsChild = true;     // Prevents the text box from drawing its own border
```

**Why**: The list box itself is the tab stop. The search box should only gain focus via mouse click or keyboard redirect (Step 4.3).

---

## Verification

| Scenario | Expected Behavior |
|----------|-------------------|
| Create list, set ShowSearch, dispose | No event handler leaks, no orphaned controls |
| Set `SearchText = "abc"` programmatically | Text box shows "abc", no infinite loop |
| `_searchTextBox.Text` changed by user input | `SearchText` syncs, filter applied, no loop |
| Press letter key with list focused | Focus moves to search box, char appears |
| Press Backspace with list focused | Focus moves to search box, last char deleted |
| Change Theme at runtime | Search box updates to match new theme |
| Tab through form | List box gets focus, search box is skipped |
| Click search box directly | Search box gains focus, typing works normally |

---

## Acceptance Criteria

- [ ] `_searchTextBox` is properly disposed in `Dispose(bool)`
- [ ] No infinite recursion between `SearchText` setter and `TextChanged` handler
- [ ] Printable key presses redirect focus to search text box when `ShowSearch == true`
- [ ] Backspace redirects to search text box instead of manual string trimming
- [ ] Theme changes propagate to search text box
- [ ] `_searchTextBox.TabStop` is false
- [ ] All existing keyboard shortcuts (Arrow, Home, End, PgUp/PgDn, Space, Enter, Esc, Ctrl+A, Ctrl+C, Delete, F2) still work correctly
