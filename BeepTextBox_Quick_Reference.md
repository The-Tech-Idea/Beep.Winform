# BeepTextBox Quick Reference

## Partial Class Structure

### 📁 File Organization

```
BeepTextBox/
├── BeepTextBox.cs              ← Main entry point (declare here)
├── BeepTextBox.Core.cs         ← Fields, constructor, initialization
├── BeepTextBox.Properties.cs   ← All properties
├── BeepTextBox.Events.cs       ← Event handlers
├── BeepTextBox.Input.cs        ← Keyboard & text operations
├── BeepTextBox.Methods.cs      ← Helper methods
├── BeepTextBox.Drawing.cs      ← Custom rendering
└── BeepTextBox.Theme.cs        ← Theme support
```

## 🔍 Where to Find Things

### Adding a New Property?
→ **BeepTextBox.Properties.cs**

### Adding a New Method?
→ **BeepTextBox.Methods.cs**

### Modifying Event Handling?
→ **BeepTextBox.Events.cs**

### Changing Keyboard Behavior?
→ **BeepTextBox.Input.cs**

### Customizing Drawing?
→ **BeepTextBox.Drawing.cs**

### Adjusting Theme Colors?
→ **BeepTextBox.Theme.cs**

### Adding New Fields or Initializing?
→ **BeepTextBox.Core.cs**

## 🎯 Common Tasks

### Add a New Property
1. Open `BeepTextBox.Properties.cs`
2. Find the appropriate region (e.g., `#region "Properties - Behavior"`)
3. Add your property with attributes:
```csharp
private bool _myNewProperty = false;

[Browsable(true)]
[Category("Behavior")]
[DefaultValue(false)]
[Description("My new property description.")]
public bool MyNewProperty
{
    get => _myNewProperty;
    set
    {
        _myNewProperty = value;
        Invalidate();
    }
}
```

### Add a New Event
1. Open `BeepTextBox.Core.cs` to declare the event:
```csharp
public event EventHandler MyNewEvent;
```

2. Open `BeepTextBox.Events.cs` to handle it:
```csharp
private void OnMyNewEvent()
{
    MyNewEvent?.Invoke(this, EventArgs.Empty);
}
```

### Add Custom Drawing
1. Open `BeepTextBox.Drawing.cs`
2. Add your drawing method:
```csharp
private void DrawMyCustomElement(Graphics g)
{
    // Your drawing code
}
```

3. Call it from `DrawContent`:
```csharp
protected override void DrawContent(Graphics g)
{
    base.DrawContent(g);
    DrawMyCustomElement(g);
    // ... other drawing
}
```

### Add a New Keyboard Shortcut
1. Open `BeepTextBox.Input.cs`
2. Modify `OnKeyDown` method:
```csharp
protected override void OnKeyDown(KeyEventArgs e)
{
    base.OnKeyDown(e);
    
    if (e.Control && e.KeyCode == Keys.YourKey)
    {
        // Handle your shortcut
        e.Handled = true;
        return;
    }
    // ... rest of method
}
```

## 🛠️ Helper Integration

### Using the Helper
All partial classes have access to `_helper`:

```csharp
// In any partial class
_helper?.Caret?.MoveCaret(1, false);
_helper?.Scrolling?.ScrollToCaret(position);
_helper?.Validation?.ValidateData(out message);
_helper?.UndoRedo?.Undo();
_helper?.AutoComplete?.ShowCompletions();
```

### Helper Components
- `_helper.Performance` - Performance optimization
- `_helper.AutoComplete` - AutoComplete functionality
- `_helper.UndoRedo` - Undo/Redo operations
- `_helper.Validation` - Input validation
- `_helper.Caret` - Caret management
- `_helper.Scrolling` - Scroll operations
- `_helper.Drawing` - Drawing helpers
- `_helper.AdvancedEditing` - Advanced editing features

## 📋 Property Categories

### Core Text
- `Text`, `PlaceholderText`, `TextFont`

### Modern Features
- `EnableSmartFeatures`, `EnableFocusAnimation`, `EnableTypingIndicator`
- `MaxLength`, `ShowCharacterCount`, `EnableSpellCheck`

### Appearance
- `TextFont`, `PlaceholderTextColor`, `FocusBorderColor`, `BorderWidth`

### Behavior
- `Multiline`, `ReadOnly`, `AcceptsReturn`, `AcceptsTab`
- `TextAlignment`, `WordWrap`
- `UseSystemPasswordChar`, `PasswordChar`

### Validation
- `MaskFormat`, `CustomMask`, `DateFormat`, `TimeFormat`
- `OnlyDigits`, `OnlyCharacters`

### AutoComplete
- `AutoCompleteMode`, `AutoCompleteSource`, `AutoCompleteCustomSource`

### Scrolling
- `ShowVerticalScrollBar`, `ShowHorizontalScrollBar`, `ScrollBars`

### Selection
- `SelectionStart`, `SelectionLength`, `SelectedText`, `SelectionBackColor`

### Image
- `ImagePath`, `MaxImageSize`, `TextImageRelation`, `ImageAlign`
- `ApplyThemeOnImage`, `ImageVisible`

### Line Numbers
- `ShowLineNumbers`, `LineNumberMarginWidth`
- `LineNumberForeColor`, `LineNumberBackColor`, `LineNumberFont`

## 🎨 Theme Integration

### Applying Themes
Themes are handled in `BeepTextBox.Theme.cs`:

```csharp
public override void ApplyTheme()
{
    if (_currentTheme == null) return;
    base.ApplyTheme();
    
    // Apply theme colors
    BackColor = _currentTheme.TextBoxBackColor;
    ForeColor = _currentTheme.TextBoxForeColor;
    BorderColor = _currentTheme.BorderColor;
    // ... etc
}
```

## 🐛 Debugging Tips

### Check Errors
```csharp
get_errors // Use in VS Code
```

### Add Breakpoints
- **Input issues**: `BeepTextBox.Input.cs` → `OnKeyDown`, `OnKeyPress`
- **Display issues**: `BeepTextBox.Drawing.cs` → `DrawContent`
- **Property issues**: `BeepTextBox.Properties.cs` → property setter
- **Event issues**: `BeepTextBox.Events.cs` → specific event handler

### Trace Helper Calls
```csharp
System.Diagnostics.Debug.WriteLine($"Caret Position: {_helper?.Caret?.CaretPosition}");
```

## ✅ Best Practices

1. **Keep partial classes focused** - Don't mix concerns
2. **Use regions** - Organize code within partial classes
3. **Add XML comments** - Document public members
4. **Use proper attributes** - `[Browsable]`, `[Category]`, `[Description]`
5. **Invalidate when needed** - Call `Invalidate()` after visual changes
6. **Test in Designer** - Ensure properties work at design-time
7. **Follow naming conventions** - Private fields with `_`, public properties in PascalCase

## 📚 Related Files

- `BeepSimpleTextBoxHelper.cs` - Shared helper system
- `IBeepTextBox` interface in helper file
- `BeepSimpleTextBox.cs` - Similar control implementation
- Theme files in `Vis.Modules`

---

**Last Updated:** October 6, 2025
