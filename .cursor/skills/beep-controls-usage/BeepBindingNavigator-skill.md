# BeepBindingNavigator Skill

## Overview
Data navigation control for record browsing (First, Previous, Next, Last) with CRUD operations.

## Namespace
```csharp
using TheTechIdea.Beep.Winform.Controls;
```

## Key Properties
| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Bound data source |
| `CurrentPosition` | `int` | Current record index |
| `ShowAddNew` | `bool` | Show add new button |
| `ShowDelete` | `bool` | Show delete button |
| `ShowSave` | `bool` | Show save button |

## Usage
```csharp
var nav = new BeepBindingNavigator();
nav.DataSource = myBindingList;
nav.PositionChanged += (s, e) => LoadRecord(nav.CurrentPosition);
```
