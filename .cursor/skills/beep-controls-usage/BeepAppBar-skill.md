# BeepAppBar Skill

## Overview
App bars for top/bottom navigation with multiple styles including web header style.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls.AppBars;
```

## Available Classes
- `BeepWebHeaderAppBar` - Web-style header with navigation
- Various app bar painters for different styles

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `WebHeaderStyle` | `WebHeaderStyle` | Visual style |
| `Title` | `string` | App bar title |
| `ShowBackButton` | `bool` | Show navigation back |

## Usage
```csharp
var appBar = new BeepWebHeaderAppBar
{
    Title = "My App",
    WebHeaderStyle = WebHeaderStyle.Modern
};
```
