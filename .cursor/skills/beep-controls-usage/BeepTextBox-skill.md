# BeepTextBox Skill

## Overview
`BeepTextBox` is a comprehensive text input with Material Design, multiline support, validation, masking, auto-complete, spell check, and advanced editing features.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.TextFields;
```

## Key Properties

### Core Properties
| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Text content |
| `PlaceholderText` | `string` | Placeholder when empty |
| `TextFont` | `Font` | Text font |
| `TextAlignment` | `HorizontalAlignment` | Left, Center, Right |
| `MaxLength` | `int` | Max characters (0 = unlimited) |
| `ShowCharacterCount` | `bool` | Display character counter |

### Multiline Properties
| Property | Type | Description |
|----------|------|-------------|
| `Multiline` | `bool` | Enable multiline |
| `AcceptsReturn` | `bool` | Enter creates new line |
| `AcceptsTab` | `bool` | Tab inserts character |
| `WordWrap` | `bool` | Word wrapping |
| `ScrollBars` | `ScrollBars` | None, Vertical, Horizontal, Both |
| `ShowLineNumbers` | `bool` | Display line numbers |

### Password Properties
| Property | Type | Description |
|----------|------|-------------|
| `UseSystemPasswordChar` | `bool` | Use system password char |
| `PasswordChar` | `char` | Custom password character |

### Validation & Masking
| Property | Type | Description |
|----------|------|-------------|
| `MaskFormat` | `TextBoxMaskFormat` | Mask type |
| `CustomMask` | `string` | Custom mask pattern |
| `DateFormat` | `string` | Date mask format |
| `TimeFormat` | `string` | Time mask format |
| `OnlyDigits` | `bool` | Numbers only |
| `OnlyCharacters` | `bool` | Letters only |

### Auto-Complete
| Property | Type | Description |
|----------|------|-------------|
| `AutoCompleteMode` | `AutoCompleteMode` | None, Suggest, Append, SuggestAppend |
| `AutoCompleteSource` | `AutoCompleteSource` | Source type |
| `AutoCompleteCustomSource` | `AutoCompleteStringCollection` | Custom suggestions |

### Modern Features
| Property | Type | Description |
|----------|------|-------------|
| `EnableSmartFeatures` | `bool` | Enable modern features |
| `EnableFocusAnimation` | `bool` | Focus border animation |
| `EnableTypingIndicator` | `bool` | Visual typing indicator |
| `EnableSpellCheck` | `bool` | Enable spell checking |
| `ReadOnly` | `bool` | Prevent editing |

### Selection & Caret
| Property | Type | Description |
|----------|------|-------------|
| `SelectionStart` | `int` | Selection start position |
| `SelectionLength` | `int` | Selection length |
| `SelectedText` | `string` | Currently selected text |
| `SelectionBackColor` | `Color` | Selection highlight color |

### Image Support
| Property | Type | Description |
|----------|------|-------------|
| `ImagePath` | `string` | Icon image path |
| `ImageVisible` | `bool` | Show/hide image |
| `ImageAlign` | `ContentAlignment` | Image position |
| `MaxImageSize` | `Size` | Image dimensions |
| `ApplyThemeOnImage` | `bool` | Theme tinting for SVG |

## Usage Examples

### Basic TextBox
```csharp
var txt = new BeepTextBox
{
    PlaceholderText = "Enter name...",
    UseThemeColors = true
};
```

### Password Field
```csharp
var pwd = new BeepTextBox
{
    PlaceholderText = "Password",
    UseSystemPasswordChar = true
    // Or use: PasswordChar = '●'
};
```

### Multiline Editor
```csharp
var editor = new BeepTextBox
{
    Multiline = true,
    AcceptsReturn = true,
    AcceptsTab = true,
    WordWrap = true,
    ScrollBars = ScrollBars.Both,
    ShowLineNumbers = true
};
```

### Masked Input (Phone)
```csharp
var phone = new BeepTextBox
{
    MaskFormat = TextBoxMaskFormat.Phone,
    PlaceholderText = "(555) 123-4567"
};
```

### Numeric Input
```csharp
var amount = new BeepTextBox
{
    OnlyDigits = true,
    PlaceholderText = "Enter amount",
    MaxLength = 10,
    ShowCharacterCount = true
};
```

### With Auto-Complete
```csharp
var search = new BeepTextBox
{
    AutoCompleteMode = AutoCompleteMode.SuggestAppend,
    AutoCompleteSource = AutoCompleteSource.CustomSource
};
search.AutoCompleteCustomSource.AddRange(new[] { "Apple", "Banana", "Cherry" });
```

### With Icon
```csharp
var search = new BeepTextBox
{
    PlaceholderText = "Search...",
    ImagePath = "GFX/icons/search.svg",
    ImageVisible = true,
    ApplyThemeOnImage = true
};
```

## Events
| Event | Description |
|-------|-------------|
| `TextChanged` | Text content changed |
| `Enter` | Control received focus |
| `Leave` | Control lost focus |
| `KeyDown` | Key pressed |
| `KeyUp` | Key released |

## Related Controls
- `BeepLabel` - Text display
- `BeepComboBox` - Dropdown selection
