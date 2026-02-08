# BeepLabel Skill

## Overview
`BeepLabel` is an enhanced label with subheader text, image support, multi-line, word wrap, auto-ellipsis, and theme integration.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Main label text |
| `SubHeaderText` | `string` | Secondary text below main |
| `TextFont` | `Font` | Main text font |
| `SubHeaderFont` | `Font` | Subheader font (auto-smaller) |
| `SubHeaderForeColor` | `Color` | Subheader text color |
| `TextAlign` | `ContentAlignment` | Text alignment |
| `HeaderSubheaderSpacing` | `int` | Space between header/sub |
| `ImagePath` | `string` | Image file (SVG/PNG/JPG) |
| `MaxImageSize` | `Size` | Max image dimensions |
| `ImageAlign` | `ContentAlignment` | Image alignment |
| `TextImageRelation` | `TextImageRelation` | Image/text position |
| `ApplyThemeOnImage` | `bool` | Apply theme to SVG |
| `Multiline` | `bool` | Allow multiple lines |
| `WordWrap` | `bool` | Enable word wrapping |
| `AutoEllipsis` | `bool` | Show ... when truncated |
| `AutoSize` | `bool` | Resize to content |
| `HideText` | `bool` | Hide text (show image only) |

## Usage Examples

### Basic Label
```csharp
var label = new BeepLabel
{
    Text = "Hello World",
    TextAlign = ContentAlignment.MiddleCenter,
    UseThemeColors = true
};
```

### With Subheader
```csharp
var label = new BeepLabel
{
    Text = "John Smith",
    SubHeaderText = "Software Engineer",
    HeaderSubheaderSpacing = 4
};
```

### With Image
```csharp
var label = new BeepLabel
{
    Text = "Settings",
    ImagePath = "settings.svg",
    TextImageRelation = TextImageRelation.ImageBeforeText,
    ApplyThemeOnImage = true
};
```

### Multi-line with Word Wrap
```csharp
var label = new BeepLabel
{
    Text = "This is a long description that should wrap to multiple lines",
    Multiline = true,
    WordWrap = true,
    AutoSize = true
};
```

### Truncated with Ellipsis
```csharp
var label = new BeepLabel
{
    Text = "Very long text that will be truncated",
    AutoEllipsis = true,
    Width = 100
};
```

## Methods
| Method | Description |
|--------|-------------|
| `Measure(Size)` | Get preferred size |
| `ApplyThemeToSvg()` | Apply theme to SVG |

## Theme Colors
| Property | Theme Source |
|----------|-------------|
| `BackColor` | LabelBackColor |
| `ForeColor` | LabelForeColor |
| `HoverBackColor` | LabelHoverBackColor |
| `SelectedBackColor` | LabelSelectedBackColor |
| `BorderColor` | LabelBorderColor |
