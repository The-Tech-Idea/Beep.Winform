---
name: beep-controls-usage
description: Guide for using Beep.Winform controls instead of standard WinForms controls. Provides control mapping, usage examples, migration patterns, and best practices for replacing generic Windows Forms controls with Beep controls that offer Material Design, theming, and enhanced functionality.
---

# Beep.Winform Controls Usage Guide

Complete guide for using Beep.Winform controls instead of standard Windows Forms controls. Beep controls provide Material Design styling, theme support, DPI scaling, and enhanced functionality while maintaining compatibility with WinForms.

## Why Use Beep Controls?

### Advantages Over Standard WinForms Controls

1. **Material Design Support** - 16 built-in design styles (Material3, iOS15, Fluent2, etc.)
2. **Theme Integration** - Centralized theme management with `BeepThemesManager`
3. **DPI Awareness** - Automatic scaling for high-DPI displays
4. **Enhanced Functionality** - Additional features beyond standard controls
5. **Consistent Styling** - Unified look and feel across your application
6. **Modern UI** - Contemporary design patterns and animations
7. **Better Performance** - Optimized rendering with double buffering

## Control Mapping Reference

### Text Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `Label` | `BeepLabel` | `TheTechIdea.Beep.Winform.Controls` | Image support, subheader text, multiline, theme colors |
| `TextBox` | `BeepTextBox` | `TheTechIdea.Beep.Winform.Controls` | Material Design styles, floating labels, validation, icons |
| `RichTextBox` | `BeepTextBox` (with RichText support) | `TheTechIdea.Beep.Winform.Controls` | Enhanced formatting, theme-aware |

### Button Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `Button` | `BeepButton` | `TheTechIdea.Beep.Winform.Controls.Buttons` | 16 styles, icons, animations, ripple effects |
| `CheckBox` | `BeepCheckBox<T>` | `TheTechIdea.Beep.Winform.Controls.CheckBoxes` | Generic support, custom styling, theme integration |
| `RadioButton` | `BeepRadioGroup` | `TheTechIdea.Beep.Winform.Controls.RadioGroup` | Grouped radio buttons, modern styling |
| `ToggleButton` | `BeepToggle` | `TheTechIdea.Beep.Winform.Controls.Toggle` | Material Design toggle switches |

### Selection Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `ComboBox` | `BeepComboBox` | `TheTechIdea.Beep.Winform.Controls.ComboBoxes` | Material Design dropdown, search, icons |
| `ListBox` | `BeepListBox` | `TheTechIdea.Beep.Winform.Controls.ListBoxs` | Virtual scrolling, multi-select, custom rendering |
| `CheckedListBox` | `BeepListBox` (with checkboxes) | `TheTechIdea.Beep.Winform.Controls.ListBoxs` | Enhanced selection, theme support |

### Date/Time Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `DateTimePicker` | `BeepDatePicker` | `TheTechIdea.Beep.Winform.Controls` | Material Design calendar, multiple formats |
| `MonthCalendar` | `BeepCalendar` | `TheTechIdea.Beep.Winform.Controls.Calendar` | Modern calendar UI, range selection |

### Data Display Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `DataGridView` | `BeepSimpleGrid` | `TheTechIdea.Beep.Winform.Controls` | Virtual scrolling, filtering, Material Design |
| `TreeView` | `BeepTree` | `TheTechIdea.Beep.Winform.Controls.Trees` | Modern tree UI, icons, animations |
| `ListView` | `BeepListBox` (with list view mode) | `TheTechIdea.Beep.Winform.Controls.ListBoxs` | Custom item rendering, grouping |

### Container Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `Panel` | `BeepCard` | `TheTechIdea.Beep.Winform.Controls.Cards` | Material Design cards, shadows, rounded corners |
| `GroupBox` | `BeepCard` (with header) | `TheTechIdea.Beep.Winform.Controls.Cards` | Modern grouping, theme support |
| `TabControl` | `BeepTabs` | `TheTechIdea.Beep.Winform.Controls.Tabs` | Material Design tabs, animations |
| `MenuStrip` | `BeepMenuBar` | `TheTechIdea.Beep.Winform.Controls.Menus` | Modern menu bar, icons, theme support |
| `ToolStrip` | `BeepToolBar` | `TheTechIdea.Beep.Winform.Controls` | Material Design toolbar |

### Navigation Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `MenuStrip` | `BeepSideMenu` | `TheTechIdea.Beep.Winform.Controls` | Side navigation menu, Material Design |
| `ToolStripMenuItem` | `BeepMenuItem` | `TheTechIdea.Beep.Winform.Controls.Menus` | Modern menu items, icons |
| `ContextMenuStrip` | `BeepContextMenu` | `TheTechIdea.Beep.Winform.Controls.ContextMenus` | Material Design context menus |

### Progress Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `ProgressBar` | `BeepProgressBar` | `TheTechIdea.Beep.Winform.Controls.ProgressBars` | Material Design progress, circular variants |
| `TrackBar` | `BeepSlider` | `TheTechIdea.Beep.Winform.Controls` | Material Design slider, value display |

### Other Controls

| Standard WinForms Control | Beep Control | Namespace | Key Features |
|---------------------------|--------------|-----------|--------------|
| `PictureBox` | `BeepImage` | `TheTechIdea.Beep.Winform.Controls.Images` | SVG support, theme-aware images, scaling |
| `ToolTip` | `BeepToolTip` | `TheTechIdea.Beep.Winform.Controls.ToolTips` | Material Design tooltips, rich content |
| `ScrollBar` | `BeepScrollBar` | `TheTechIdea.Beep.Winform.Controls` | Material Design scrollbars, smooth scrolling |

## Basic Usage Patterns

### 1. Replacing Label with BeepLabel

**Before (Standard WinForms):**
```csharp
Label lblTitle = new Label();
lblTitle.Text = "Welcome";
lblTitle.ForeColor = Color.Black;
lblTitle.Font = new Font("Segoe UI", 12);
this.Controls.Add(lblTitle);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls;

BeepLabel lblTitle = new BeepLabel();
lblTitle.Text = "Welcome";
lblTitle.UseThemeColors = true; // Automatically uses theme colors
lblTitle.TextFont = BeepFontManager.GetFont("Segoe UI", 12);
lblTitle.SubHeaderText = "Subtitle text"; // Additional feature
this.Controls.Add(lblTitle);
```

### 2. Replacing TextBox with BeepTextBox

**Before (Standard WinForms):**
```csharp
TextBox txtName = new TextBox();
txtName.Location = new Point(10, 10);
txtName.Size = new Size(200, 20);
txtName.Text = "Enter name";
this.Controls.Add(txtName);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls;

BeepTextBox txtName = new BeepTextBox();
txtName.Location = new Point(10, 10);
txtName.Size = new Size(200, 40); // Material Design needs more height
txtName.PlaceholderText = "Enter name";
txtName.UseThemeColors = true;
txtName.Style = BeepTextBoxStyle.Material3; // Choose from 16 styles
this.Controls.Add(txtName);
```

### 3. Replacing Button with BeepButton

**Before (Standard WinForms):**
```csharp
Button btnSave = new Button();
btnSave.Text = "Save";
btnSave.BackColor = Color.Blue;
btnSave.ForeColor = Color.White;
btnSave.Click += BtnSave_Click;
this.Controls.Add(btnSave);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls.Buttons;

BeepButton btnSave = new BeepButton();
btnSave.Text = "Save";
btnSave.UseThemeColors = true; // Uses theme accent color
btnSave.Style = BeepButtonStyle.Material3;
btnSave.IconPath = "path/to/save-icon.svg"; // SVG icon support
btnSave.Click += BtnSave_Click;
this.Controls.Add(btnSave);
```

### 4. Replacing CheckBox with BeepCheckBox

**Before (Standard WinForms):**
```csharp
CheckBox chkAgree = new CheckBox();
chkAgree.Text = "I agree to terms";
chkAgree.Checked = false;
this.Controls.Add(chkAgree);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;

BeepCheckBox<bool> chkAgree = new BeepCheckBox<bool>();
chkAgree.Text = "I agree to terms";
chkAgree.Checked = false;
chkAgree.UseThemeColors = true;
chkAgree.Style = BeepCheckBoxStyle.Material3;
this.Controls.Add(chkAgree);
```

### 5. Replacing ComboBox with BeepComboBox

**Before (Standard WinForms):**
```csharp
ComboBox cmbCountry = new ComboBox();
cmbCountry.Items.AddRange(new[] { "USA", "UK", "Canada" });
cmbCountry.DropDownStyle = ComboBoxStyle.DropDownList;
this.Controls.Add(cmbCountry);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

BeepComboBox cmbCountry = new BeepComboBox();
cmbCountry.Items.AddRange(new[] { "USA", "UK", "Canada" });
cmbCountry.UseThemeColors = true;
cmbCountry.Style = BeepComboBoxStyle.Material3;
cmbCountry.PlaceholderText = "Select country";
this.Controls.Add(cmbCountry);
```

### 6. Replacing Panel with BeepCard

**Before (Standard WinForms):**
```csharp
Panel pnlContainer = new Panel();
pnlContainer.BackColor = Color.White;
pnlContainer.BorderStyle = BorderStyle.FixedSingle;
this.Controls.Add(pnlContainer);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls.Cards;

BeepCard pnlContainer = new BeepCard();
pnlContainer.UseThemeColors = true;
pnlContainer.Style = BeepCardStyle.Material3;
pnlContainer.Elevation = 2; // Material Design shadow
pnlContainer.CornerRadius = 8; // Rounded corners
this.Controls.Add(pnlContainer);
```

### 7. Replacing DataGridView with BeepSimpleGrid

**Before (Standard WinForms):**
```csharp
DataGridView dgvData = new DataGridView();
dgvData.DataSource = dataTable;
dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
this.Controls.Add(dgvData);
```

**After (Beep Control):**
```csharp
using TheTechIdea.Beep.Winform.Controls;

BeepSimpleGrid dgvData = new BeepSimpleGrid();
dgvData.DataSource = dataTable;
dgvData.UseThemeColors = true;
dgvData.Style = BeepGridStyle.Material3;
dgvData.EnableVirtualScrolling = true; // Better performance
this.Controls.Add(dgvData);
```

## Theme Integration

### Setting Up Themes

```csharp
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

// Get theme manager
var themeManager = BeepThemesManager.Instance;

// Apply theme to entire form
this.CurrentTheme = themeManager.GetTheme("Material3Dark");

// Apply theme to all Beep controls
ApplyThemeToControls(this);
```

### Applying Theme to All Controls

```csharp
private void ApplyThemeToControls(Control parent)
{
    foreach (Control control in parent.Controls)
    {
        if (control is BaseControl beepControl)
        {
            beepControl.CurrentTheme = this.CurrentTheme;
            beepControl.ApplyTheme();
        }
        
        // Recursively apply to child controls
        if (control.HasChildren)
        {
            ApplyThemeToControls(control);
        }
    }
}
```

### Using Theme Colors vs Custom Colors

```csharp
// Use theme colors (recommended)
beepButton.UseThemeColors = true;
// Automatically uses CurrentTheme.AccentColor, BackColor, ForeColor

// Use custom colors
beepButton.UseThemeColors = false;
beepButton.AccentColor = Color.FromArgb(103, 80, 164); // Material3 Primary
beepButton.BackColor = Color.White;
beepButton.ForeColor = Color.Black;
```

## Common Properties

### BaseControl Properties (Available on All Beep Controls)

```csharp
// Theme properties
bool UseThemeColors { get; set; }           // Use theme colors
IBeepTheme CurrentTheme { get; set; }        // Current theme instance
Color AccentColor { get; set; }             // Custom accent color

// Style properties
ControlStyle Style { get; set; }            // Visual style (Material3, iOS15, etc.)

// DPI scaling (automatic)
float ScaleFactor { get; }                  // Current DPI scale factor
Rectangle DrawingRect { get; }               // DPI-scaled drawing rectangle

// Layout properties
Padding MaterialPadding { get; set; }       // Material Design padding
int CornerRadius { get; set; }              // Rounded corners radius
```

### Control-Specific Properties

**BeepLabel:**
```csharp
string SubHeaderText { get; set; }          // Subtitle text
Font TextFont { get; set; }                  // Text font
bool Multiline { get; set; }                 // Multiline support
string ImagePath { get; set; }               // Image/SVG path
TextImageRelation TextImageRelation { get; set; } // Image position
```

**BeepTextBox:**
```csharp
string PlaceholderText { get; set; }        // Placeholder text
bool ShowFloatingLabel { get; set; }         // Material Design floating label
string IconPath { get; set; }                // Leading icon
string TrailingIconPath { get; set; }        // Trailing icon
BeepTextBoxType TextBoxType { get; set; }   // Text, Password, Number, etc.
```

**BeepButton:**
```csharp
string IconPath { get; set; }                // Button icon (SVG)
BeepButtonVariant Variant { get; set; }     // Contained, Outlined, Text
bool EnableRipple { get; set; }              // Ripple animation
int Elevation { get; set; }                  // Shadow elevation
```

## Migration Checklist

When migrating from standard WinForms controls to Beep controls:

- [ ] **Add NuGet Reference** - Ensure `TheTechIdea.Beep.Winform.Controls` is referenced
- [ ] **Add Using Statements** - Add appropriate namespaces
- [ ] **Replace Control Types** - Change control class names
- [ ] **Update Properties** - Map standard properties to Beep equivalents
- [ ] **Set UseThemeColors** - Enable theme support (recommended)
- [ ] **Choose Style** - Select appropriate Material Design style
- [ ] **Update Event Handlers** - Most events remain compatible
- [ ] **Adjust Sizing** - Material Design controls may need more space
- [ ] **Test DPI Scaling** - Verify appearance at different DPI settings
- [ ] **Apply Theme** - Set up theme management for consistent styling

## Best Practices

### 1. Always Use Theme Colors

```csharp
// ✅ GOOD - Uses theme colors
beepButton.UseThemeColors = true;

// ❌ BAD - Hardcoded colors ignore themes
beepButton.BackColor = Color.Blue;
beepButton.UseThemeColors = false;
```

### 2. Set Style Consistently

```csharp
// Set style once at form level
private void InitializeBeepControls()
{
    var style = BeepButtonStyle.Material3; // Choose one style
    
    btnSave.Style = style;
    btnCancel.Style = style;
    btnDelete.Style = style;
}
```

### 3. Use SVG Icons

```csharp
// ✅ GOOD - SVG icons scale perfectly
beepButton.IconPath = "GFX/icons/save.svg";

// ❌ BAD - Bitmap icons don't scale well
beepButton.IconPath = "icons/save.png";
```

### 4. Leverage DPI Scaling

```csharp
// Beep controls automatically handle DPI scaling
// Use DrawingRect instead of ClientRectangle for layout calculations
Rectangle contentRect = DrawingRect; // Already scaled
```

### 5. Apply Theme at Form Level

```csharp
public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        
        // Set theme once
        this.CurrentTheme = BeepThemesManager.Instance.GetTheme("Material3Dark");
        
        // Apply to all controls
        ApplyThemeToControls(this);
    }
}
```

## Common Issues and Solutions

### Issue: Control Not Appearing

**Solution:** Ensure `UseThemeColors = true` and `CurrentTheme` is set:
```csharp
beepControl.UseThemeColors = true;
beepControl.CurrentTheme = BeepThemesManager.Instance.GetTheme("Material3");
```

### Issue: Colors Not Matching Theme

**Solution:** Check `UseThemeColors` property:
```csharp
// Make sure UseThemeColors is true
if (!beepControl.UseThemeColors)
{
    beepControl.UseThemeColors = true;
    beepControl.ApplyTheme();
}
```

### Issue: Control Too Small/Large

**Solution:** Material Design controls need more space. Adjust size:
```csharp
// Material Design controls typically need more height
beepTextBox.Height = 40; // Instead of 20
beepButton.Height = 36;  // Instead of 23
```

### Issue: Icons Not Showing

**Solution:** Use SVG paths and ensure `ImagePath` is set correctly:
```csharp
// Use relative or absolute paths
beepButton.IconPath = @"GFX\icons\save.svg";
// Or use embedded resource paths
beepButton.IconPath = "TheTechIdea.Beep.Winform.Controls.GFX.icons.save.svg";
```

## Quick Reference: Control Equivalents

### Text Input
- `Label` → `BeepLabel`
- `TextBox` → `BeepTextBox`
- `RichTextBox` → `BeepTextBox` (with RichText)

### Buttons
- `Button` → `BeepButton`
- `CheckBox` → `BeepCheckBox<T>`
- `RadioButton` → `BeepRadioGroup`
- `LinkLabel` → `BeepButton` (with Text variant)

### Selection
- `ComboBox` → `BeepComboBox`
- `ListBox` → `BeepListBox`
- `CheckedListBox` → `BeepListBox` (with checkboxes)

### Containers
- `Panel` → `BeepCard`
- `GroupBox` → `BeepCard` (with header)
- `TabControl` → `BeepTabs`
- `MenuStrip` → `BeepMenuBar`

### Data Display
- `DataGridView` → `BeepSimpleGrid`
- `TreeView` → `BeepTree`
- `ListView` → `BeepListBox` (list mode)

### Other
- `PictureBox` → `BeepImage`
- `ProgressBar` → `BeepProgressBar`
- `ToolTip` → `BeepToolTip`
- `DateTimePicker` → `BeepDatePicker`

## Additional Resources

- **Control Creation Guide**: `.cursor/skills/beep-winform/SKILL.md`
- **BaseControl Documentation**: `TheTechIdea.Beep.Winform.Controls/BaseControl/Readme.md`
- **Theme Management**: `TheTechIdea.Beep.Winform.Controls/ThemeManagement/Readme.md`
- **Styling Guide**: `TheTechIdea.Beep.Winform.Controls/Styling/Readme.md`
- **Control Instructions**: `.github/instructions/CreateUpdateBeepControl.instructions.md`

## Summary

Beep.Winform controls provide a modern, theme-aware alternative to standard WinForms controls with:

- ✅ Material Design support (16 styles)
- ✅ Centralized theme management
- ✅ Automatic DPI scaling
- ✅ Enhanced functionality
- ✅ Consistent styling
- ✅ Better performance

**Always prefer Beep controls over standard WinForms controls** for new development and when migrating existing applications.
