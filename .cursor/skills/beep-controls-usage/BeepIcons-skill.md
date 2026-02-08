# BeepIcons (Svgs) Skill

## Overview
Static class providing 200+ embedded SVG icon paths organized by category.

## Namespace
```csharp
using TheTechIdea.Beep.Icons; // Svgs class
```

---

## Usage Pattern
```csharp
// Svgs provides resource paths, not actual images
string iconPath = Svgs.Save;

// Use with BeepImage control
beepImage.ImagePath = Svgs.Settings;

// Use with ImageLoader
var svg = SvgDocument.Open<SvgDocument>(
    Svgs.ResourceAssembly.GetManifestResourceStream(Svgs.Search)
);
Image bitmap = svg.Draw(24, 24);
```

---

## Icon Categories

### General Actions & UI (60+)
| Icon | Path |
|------|------|
| Add, Remove, Edit, Delete | `Svgs.Add`, `Svgs.Remove`, `Svgs.Edit`, `Svgs.Trash` |
| Save, SaveAll, Undo, Refresh | `Svgs.Save`, `Svgs.SaveAll`, `Svgs.Undo`, `Svgs.Refresh` |
| Search, Filter, Sort | `Svgs.Search`, `Svgs.Filter`, `Svgs.Sort` |
| Check, Close, Cancel | `Svgs.Check`, `Svgs.Close`, `Svgs.Cancel` |
| Copy, Print, Export, Share | `Svgs.Copy`, `Svgs.Print`, `Svgs.Export`, `Svgs.Share` |
| Settings, Home, Menu | `Svgs.Settings`, `Svgs.Home`, `Svgs.Menu` |
| Plus, Minus, Power | `Svgs.Plus`, `Svgs.Minus`, `Svgs.Power` |

### Navigation & Arrows
| Icon | Path |
|------|------|
| Angles | `Svgs.AngleSmallUp/Down/Left/Right` |
| Double Angles | `Svgs.AngleDoubleSmallUp/Down/Left/Right` |
| Arrows | `Svgs.LeftArrow`, `Svgs.RightArrow` |
| Page Nav | `Svgs.FirstPage`, `Svgs.LastPage`, `Svgs.FirstRecord`, `Svgs.LastRecord` |

### Information & Status
| Icon | Path |
|------|------|
| Error | `Svgs.Error` |
| Information | `Svgs.Information` |
| Question | `Svgs.Question` |
| Notice | `Svgs.Notice` |
| Loading | `Svgs.Loading` |

### INFO Icons (Subfolder)
`Svgs.InfoAlarm`, `Svgs.InfoAlert`, `Svgs.InfoHelp`, `Svgs.InfoWarning`, `Svgs.InfoImportant`, `Svgs.InfoLike`, `Svgs.InfoDislike`, `Svgs.InfoHeart`

### Database & Data Sources (40+)
| Database | Path |
|----------|------|
| SQL Server | `Svgs.SqlServer`, `Svgs.MicrosoftSqlServer` |
| MySQL | `Svgs.MySql`, `Svgs.MySqlDatabase` |
| PostgreSQL | `Svgs.Postgre` |
| Oracle | `Svgs.Oracle`, `Svgs.OracleLogo` |
| SQLite | `Svgs.Sqlite` |
| MongoDB | `Svgs.MongoDb` |
| Redis | `Svgs.Redis` |
| Firebase | `Svgs.Firebase` |
| Snowflake | `Svgs.Snowflake` |
| DuckDB | `Svgs.DuckDb` |
| Cassandra | `Svgs.Cassandra` |

### File Types
`Svgs.Csv`, `Svgs.Json`, `Svgs.Xls`, `Svgs.File`

### NAV Subfolder (UI Navigation)
`Svgs.NavBackArrow`, `Svgs.NavChevron`, `Svgs.NavDoubleChevron`, `Svgs.NavDashboard`, `Svgs.NavUser`, `Svgs.NavSearch`, `Svgs.NavTrash`, `Svgs.NavPrinter`

---

## Helper Methods

```csharp
// Get all icon paths as dictionary
Dictionary<string, string> all = Svgs.GetAllPaths();

// Check if resource exists
bool exists = Svgs.ResourceExists(Svgs.Save);

// Get all available SVG resources
string[] resources = Svgs.GetAvailableResources();
```

---

## Extension Methods for BeepImage
```csharp
beepImage.SetSvgPath(Svgs.Settings);
beepImage.SetSearchIcon();
beepImage.SetCloseIcon();
beepImage.SetEditIcon();
beepImage.SetUserIcon();
beepImage.SetEmailIcon();
beepImage.SetCalendarIcon();
beepImage.SetSettingsIcon();
beepImage.SetVisibilityIcon();
beepImage.SetHideIcon();
```

---

## Usage Examples

### Set Button Icon
```csharp
beepButton.ImagePath = Svgs.Save;
beepButton.ShowImage = true;
```

### Build Icon Menu
```csharp
var icons = Svgs.GetAllPaths();
foreach (var kvp in icons)
{
    menuItem.Items.Add(new MenuItem { Text = kvp.Key, ImagePath = kvp.Value });
}
```

### Render SVG to Specific Size
```csharp
using var stream = Svgs.ResourceAssembly.GetManifestResourceStream(Svgs.Home);
var doc = SvgDocument.Open<SvgDocument>(stream);
Bitmap icon = doc.Draw(32, 32);
```

## Related
- `ImageLoader` - Load images from paths
- `BeepImage` - Image display control
