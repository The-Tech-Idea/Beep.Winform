---
name: beep-controls-usage
description: Guide for using Beep.Winform controls instead of standard WinForms controls. Provides control mapping, usage examples, migration patterns, and best practices for replacing generic Windows Forms controls with Beep controls that offer Material Design, theming, and enhanced functionality.
---

# Beep.Winform Controls Usage Guide

Complete guide for using Beep.Winform controls instead of standard Windows Forms controls. Beep controls provide Material Design styling, theme support, DPI scaling, and enhanced functionality while maintaining compatibility with WinForms.

## Complete Control Catalog

### Button Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepButton` | `TheTechIdea.Beep.Winform.Controls.Buttons` | Primary button with 16 styles, icons, ripple effects |
| `BeepChevronButton` | `TheTechIdea.Beep.Winform.Controls.Buttons` | Chevron/arrow button for navigation |
| `BeepCircularButton` | `TheTechIdea.Beep.Winform.Controls.Buttons` | Circular/FAB-style button |
| `BeepExtendedButton` | `TheTechIdea.Beep.Winform.Controls` | Extended button with additional features |
| `BeepButtonPopList` | `TheTechIdea.Beep.Winform.Controls` | Button with dropdown popup list |

### Text Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepLabel` | `TheTechIdea.Beep.Winform.Controls` | Label with image support, subheader, multiline |
| `BeepTextBox` | `TheTechIdea.Beep.Winform.Controls.TextFields` | Material Design textbox with floating labels |

### Selection Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepComboBox` | `TheTechIdea.Beep.Winform.Controls.ComboBoxes` | Material Design dropdown with search |
| `BeepDropDownCheckBoxSelect` | `TheTechIdea.Beep.Winform.Controls.ComboBoxes` | Multi-select dropdown with checkboxes |
| `BeepSelect` | `TheTechIdea.Beep.Winform.Controls` | Advanced selection control |
| `BeepListBox` | `TheTechIdea.Beep.Winform.Controls.ListBoxs` | Material Design listbox with virtual scrolling |

### CheckBox/Toggle Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepCheckBox` | `TheTechIdea.Beep.Winform.Controls.CheckBoxes` | Generic checkbox with custom styling |
| `BeepToggle` | `TheTechIdea.Beep.Winform.Controls.Toggle` | Material Design toggle switch |
| `BeepSwitch` | `TheTechIdea.Beep.Winform.Controls.Switchs` | Animated switch control |
| `BeepRadioGroup` | `TheTechIdea.Beep.Winform.Controls.RadioGroup` | Grouped radio buttons |
| `BeepHierarchicalRadioGroup` | `TheTechIdea.Beep.Winform.Controls.RadioGroup` | Hierarchical radio button groups |

### Container Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepCard` | `TheTechIdea.Beep.Winform.Controls.Cards` | Material Design card with elevation |
| `BeepTabs` | `TheTechIdea.Beep.Winform.Controls.Tabs` | Tabbed container control |
| `BeepDialogBox` | `TheTechIdea.Beep.Winform.Controls` | Material Design dialog box |

### Navigation Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepSideBar` | `TheTechIdea.Beep.Winform.Controls.SideBar` | Side navigation with accordion |
| `BeepSideMenu` | `TheTechIdea.Beep.Winform.Controls` | Simple side menu |
| `BeepNavBar` | `TheTechIdea.Beep.Winform.Controls.NavBars` | Top navigation bar |
| `BeepMenuBar` | `TheTechIdea.Beep.Winform.Controls.Menus` | Application menu bar |
| `BeepBreadcrump` | `TheTechIdea.Beep.Winform.Controls.BreadCrumbs` | Breadcrumb navigation |
| `BeepWebHeaderAppBar` | `TheTechIdea.Beep.Winform.Controls.AppBars` | Web-style header app bar |
| `BeepFlyoutMenu` | `TheTechIdea.Beep.Winform.Controls` | Flyout menu panel |
| `BeepDropDownMenu` | `TheTechIdea.Beep.Winform.Controls` | Dropdown menu |

### Data Display Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepSimpleGrid` | `TheTechIdea.Beep.Winform.Controls` | High-performance data grid |
| `BeepTree` | `TheTechIdea.Beep.Winform.Controls.Trees` | Tree view with icons, animations |
| `BeepChart` | `TheTechIdea.Beep.Winform.Controls.Charts` | Charting control |

### Progress/Status Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepProgressBar` | `TheTechIdea.Beep.Winform.Controls.ProgressBars` | Linear/circular progress |
| `BeepStarRating` | `TheTechIdea.Beep.Winform.Controls.Ratings` | Star rating control |
| `BeepStepperBar` | `TheTechIdea.Beep.Winform.Controls.Steppers` | Step indicator control |
| `BeepStepperBreadCrumb` | `TheTechIdea.Beep.Winform.Controls.Steppers` | Breadcrumb-style stepper |

### Date/Time Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepDatePicker` | `TheTechIdea.Beep.Winform.Controls` | Material Design date picker |
| `BeepTimePicker` | `TheTechIdea.Beep.Winform.Controls` | Time selection control |
| `BeepCalendar` | `TheTechIdea.Beep.Winform.Controls.Calendar` | Full calendar control |
| `BeepDatePickerView` | `TheTechIdea.Beep.Winform.Controls` | Date picker view control |

### Notification Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepNotification` | `TheTechIdea.Beep.Winform.Controls.Notifications` | Individual notification |
| `BeepNotificationManager` | `TheTechIdea.Beep.Winform.Controls.Notifications` | Notification system manager |
| `BeepNotificationGroup` | `TheTechIdea.Beep.Winform.Controls.Notifications` | Grouped notifications |
| `BeepNotificationHistory` | `TheTechIdea.Beep.Winform.Controls.Notifications` | Notification history viewer |

### Chip/Tag Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepMultiChipGroup` | `TheTechIdea.Beep.Winform.Controls.Chips` | Multi-select chips/tags |

### Image Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepImage` | `TheTechIdea.Beep.Winform.Controls.Images` | SVG-aware image control |
| `BeepShape` | `TheTechIdea.Beep.Winform.Controls` | Geometric shape control |

### Other Controls
| Control | Namespace | Description |
|---------|-----------|-------------|
| `BeepScrollBar` | `TheTechIdea.Beep.Winform.Controls` | Material Design scrollbar |
| `BeepScrollList` | `TheTechIdea.Beep.Winform.Controls` | Scrollable list control |
| `BeepBindingNavigator` | `TheTechIdea.Beep.Winform.Controls` | Data binding navigator |
| `BeepFunctionsPanel` | `TheTechIdea.Beep.Winform.Controls` | Quick functions panel |
| `BeepCompanyProfile` | `TheTechIdea.Beep.Winform.Controls` | Company profile display |
| `BeepDualPercentageControl` | `TheTechIdea.Beep.Winform.Controls` | Dual percentage display |
| `BeepTaskListItem` | `TheTechIdea.Beep.Winform.Controls` | Task list item |
| `BeepFieldFilter` | `TheTechIdea.Beep.Winform.Controls` | Field filter control |

---

## Control Mapping Reference

### Standard WinForms → Beep Controls

| Standard WinForms Control | Beep Control | Namespace |
|---------------------------|--------------|-----------|
| `Label` | `BeepLabel` | `TheTechIdea.Beep.Winform.Controls` |
| `TextBox` | `BeepTextBox` | `TheTechIdea.Beep.Winform.Controls.TextFields` |
| `Button` | `BeepButton` | `TheTechIdea.Beep.Winform.Controls.Buttons` |
| `CheckBox` | `BeepCheckBox` | `TheTechIdea.Beep.Winform.Controls.CheckBoxes` |
| `RadioButton` | `BeepRadioGroup` | `TheTechIdea.Beep.Winform.Controls.RadioGroup` |
| `ComboBox` | `BeepComboBox` | `TheTechIdea.Beep.Winform.Controls.ComboBoxes` |
| `ListBox` | `BeepListBox` | `TheTechIdea.Beep.Winform.Controls.ListBoxs` |
| `DataGridView` | `BeepSimpleGrid` | `TheTechIdea.Beep.Winform.Controls` |
| `TreeView` | `BeepTree` | `TheTechIdea.Beep.Winform.Controls.Trees` |
| `Panel` | `BeepCard` | `TheTechIdea.Beep.Winform.Controls.Cards` |
| `TabControl` | `BeepTabs` | `TheTechIdea.Beep.Winform.Controls.Tabs` |
| `MenuStrip` | `BeepMenuBar` | `TheTechIdea.Beep.Winform.Controls.Menus` |
| `ProgressBar` | `BeepProgressBar` | `TheTechIdea.Beep.Winform.Controls.ProgressBars` |
| `DateTimePicker` | `BeepDatePicker` | `TheTechIdea.Beep.Winform.Controls` |
| `PictureBox` | `BeepImage` | `TheTechIdea.Beep.Winform.Controls.Images` |
| `ToggleButton` | `BeepToggle` | `TheTechIdea.Beep.Winform.Controls.Toggle` |

---

## Basic Usage Examples

### BeepButton
```csharp
using TheTechIdea.Beep.Winform.Controls.Buttons;

var btn = new BeepButton
{
    Text = "Click Me",
    UseThemeColors = true,
    IconPath = "GFX/icons/check.svg"
};
btn.Click += (s, e) => { /* handle click */ };
```

### BeepTextBox
```csharp
using TheTechIdea.Beep.Winform.Controls.TextFields;

var textBox = new BeepTextBox
{
    PlaceholderText = "Enter name",
    UseThemeColors = true
};
string value = textBox.Text;
```

### BeepComboBox
```csharp
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

var combo = new BeepComboBox
{
    PlaceholderText = "Select option",
    UseThemeColors = true
};
combo.Items.AddRange(new[] { "Option 1", "Option 2" });
```

### BeepCard (Panel replacement)
```csharp
using TheTechIdea.Beep.Winform.Controls.Cards;

var card = new BeepCard
{
    UseThemeColors = true,
    CornerRadius = 8
};
card.Controls.Add(childControl);
```

---

## Theme Integration

```csharp
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

// Apply theme to all controls
var theme = BeepThemesManager.Instance.GetTheme("Material3Dark");
foreach (Control c in this.Controls)
{
    if (c is BaseControl beep)
    {
        beep.CurrentTheme = theme;
        beep.ApplyTheme();
    }
}
```

---

## Individual Control Skills

See individual skill files for detailed usage:
- [BeepButton-skill.md](./BeepButton-skill.md)
- [BeepTextBox-skill.md](./BeepTextBox-skill.md)
- [BeepComboBox-skill.md](./BeepComboBox-skill.md)
- [BeepCheckBox-skill.md](./BeepCheckBox-skill.md)
- [BeepCard-skill.md](./BeepCard-skill.md)
- [BeepListBox-skill.md](./BeepListBox-skill.md)
- [BeepSimpleGrid-skill.md](./BeepSimpleGrid-skill.md)
- [BeepTree-skill.md](./BeepTree-skill.md)
- [BeepProgressBar-skill.md](./BeepProgressBar-skill.md)
- [BeepToggle-skill.md](./BeepToggle-skill.md)
- [BeepSideBar-skill.md](./BeepSideBar-skill.md)
- [BeepTabs-skill.md](./BeepTabs-skill.md)
- [BeepNotification-skill.md](./BeepNotification-skill.md)
- [BeepCalendar-skill.md](./BeepCalendar-skill.md)
- [BeepImage-skill.md](./BeepImage-skill.md)
