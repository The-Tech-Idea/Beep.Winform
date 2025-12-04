# 🚀 QUICK START GUIDE
## Using BeepControls Design-Time Support

**Last Updated**: December 3, 2025

---

## 📋 **TABLE OF CONTENTS**
1. [Overview](#overview)
2. [Accessing Smart Tags](#accessing-smart-tags)
3. [Using the Icon Picker](#using-the-icon-picker)
4. [Using the Style Selector](#using-the-style-selector)
5. [Control-Specific Features](#control-specific-features)
6. [Tips & Tricks](#tips--tricks)

---

## 🎯 **OVERVIEW**

BeepControls now have **enterprise-grade design-time support** with:
- ✅ **14 Designers** for major controls
- ✅ **Icon Picker** - Browse 600+ icons
- ✅ **Style Selector** - Browse 56+ styles
- ✅ **Smart Tags** - Quick configuration
- ✅ **Business Presets** - One-click scenarios

---

## 🏷️ **ACCESSING SMART TAGS**

### **Method 1: Right-Click Menu**
1. Right-click any BeepControl on the design surface
2. Look for "{ControlName} Tasks" menu item
3. Click to expand smart tags panel
4. Configure via properties and actions

### **Method 2: Smart Tag Glyph**
1. Select a BeepControl
2. Look for the small arrow glyph (▶) in the top-right corner
3. Click the glyph
4. Smart tags panel appears

---

## ⭐ **USING THE ICON PICKER**

### **Where to Find:**
Icon Picker is available for these properties:
- `BeepSwitch.OnIconName`
- `BeepSwitch.OffIconName`
- `BeepToggle.IconName`
- `BeepCheckBox.ImagePath` (custom check mark)
- `BeepExtendedButton.ImagePath`
- `BeepExtendedButton.ExtendButtonImagePath`
- Any control with `ImagePath` property

### **How to Use:**

#### **Method 1: Properties Window**
1. Select a control (e.g., `BeepSwitch`)
2. In Properties window, find `OnIconName`
3. Click the dropdown button (▼)
4. **Icon Picker Dialog opens!**

#### **Method 2: Smart Tags**
1. Right-click `BeepSwitch`
2. Click "Select On Icon..." in smart tags
3. **Icon Picker Dialog opens!**

### **Icon Picker Dialog Features:**

```
╔════════════════════════════════════════════════╗
║ Icon Picker - Select from Icon Library        ║
╠════════════════════════════════════════════════╣
║ Tabs: [UI Icons] [General] [DataSources] [★] [Recent]
║ Search: Type to filter (e.g., "check", "arrow")
║ Categories: Auto-organized by type
║ Icon List: Click to select
║ Preview: Large icon preview
║ Details: Path, source, category displayed
║ Buttons: [Cancel] [OK]
╚════════════════════════════════════════════════╝
```

**Tips:**
- Use **Search** to quickly find icons
- **Recent** tab shows your last 20 selections
- **Favorites** tab (⭐) for frequently used icons
- **Double-click** an icon to select immediately

---

## 🎨 **USING THE STYLE SELECTOR**

### **Where to Find:**
Available for ALL BeepControls via:
- Smart Tags → "Select Style..."
- OR: Quick style presets (Material 3, iOS 15, Fluent 2, Minimal)

### **How to Use:**

#### **Method 1: Smart Tags**
1. Right-click any BeepControl
2. Click "{ControlName} Tasks"
3. Under "Visual Style" section, click "Select Style..."
4. **Style Selector Dialog opens!**

#### **Method 2: Quick Presets**
1. Right-click any BeepControl
2. Click "{ControlName} Tasks"
3. Under "Visual Style", click preset name (e.g., "Material 3")
4. **Style applied instantly!**

### **Style Selector Dialog Features:**

```
╔════════════════════════════════════════════════╗
║ Select Control Style                           ║
╠════════════════════════════════════════════════╣
║ Search: Type to filter styles
║ Categories: 10 organized categories
║   - Modern Web
║   - Microsoft
║   - Apple
║   - Linux Desktop
║   - Minimal & Clean
║   - Effects & Glass
║   - Gaming & Neon
║   - Theme Inspired
║   - Fun & Creative
║   - Other
║ Style List: Click to select
║ Preview: Visual preview with border radius
║ Description: Detailed style description
║ Buttons: [Cancel] [OK]
╚════════════════════════════════════════════════╝
```

**Tips:**
- **Browse by Category** for organized browsing
- **Search** for quick access (e.g., "material", "glass", "neon")
- **Preview** shows border radius and representative colors
- **Read Description** to understand style purpose

---

## 🎯 **CONTROL-SPECIFIC FEATURES**

### **1. BeepSwitch** ⭐ (MOST ADVANCED!)

**Smart Tags:**
```
┌─────────────────────────────────┐
│ State & Labels                   │
│  ☑ Checked (On)                 │
│  On Label: [_______]            │
│  Off Label: [_______]           │
│  Orientation: [Horizontal ▼]    │
├─────────────────────────────────┤
│ Icons                            │
│  ▸ Select On Icon...            │
│  ▸ Select Off Icon...           │
│  ▸ Clear Icons                  │
├─────────────────────────────────┤
│ Icon Presets                     │
│  ▸ ✓ Checkmark Icons            │
│  ▸ ⚡ Power Icons                │
│  ▸ ⇄ Toggle Icons               │
│  ▸ 🔒 Lock Icons                │
├─────────────────────────────────┤
│ Style Presets                    │
│  ▸ iOS Style                    │
│  ▸ Material 3                   │
│  ▸ Fluent 2                     │
│  ▸ Minimal                      │
├─────────────────────────────────┤
│ Behavior                         │
│  ☑ Enable Drag to Toggle        │
└─────────────────────────────────┘
```

**Quick Configuration:**
- **Checkmark Icons**: Click once → Check/X icons applied, labels cleared
- **Power Icons**: Click once → Power icon for on/off
- **Lock Icons**: Click once → Lock/Unlock icons
- **iOS Style**: Click once → Complete iOS 15 style applied

---

### **2. BeepNumericUpDown** 💰

**Business Presets:**
- **💰 Currency**: 0.00 - 999,999.99, 2 decimals, 0.01 increment
- **📊 Percentage**: 0-100%, 2 decimals, 0.1 increment
- **🔢 Integer**: 0-1000, 0 decimals, 1 increment
- **📦 Quantity**: 1-9999, 0 decimals, 1 increment

**Use Case:**
```
1. Drag BeepNumericUpDown onto form
2. Right-click → "BeepNumericUpDown Tasks"
3. Click "💰 Currency"
4. Done! Perfect for price fields!
```

---

### **3. BeepDatePicker** 📅

**Business Presets:**
- **📅 Due Date**: Today+, business days, validation
- **📝 Creation Date**: Up to today, read-only
- **🎉 Event Scheduling**: Today+, auto-submit
- **🎂 Birth Date**: Up to today, max 120 years ago
- **🕐 Appointment**: Today to +6 months, business days

**Use Case:**
```
1. Drag BeepDatePicker onto form
2. Right-click → "BeepDatePicker Tasks"
3. Click "📅 Due Date"
4. Done! Perfect for task due dates!
```

---

### **4. BeepTimePicker** 🕐

**Business Presets:**
- **📅 Meeting Time**: 12-hour, business hours, 15-min intervals
- **🕐 Appointment**: 12-hour, 8AM-6PM, 30-min intervals
- **⏰ Shift Time**: 24-hour, 15-min intervals
- **⏳ Deadline**: 12-hour, auto-submit

**Use Case:**
```
1. Drag BeepTimePicker onto form
2. Right-click → "BeepTimePicker Tasks"
3. Click "📅 Meeting Time"
4. Done! Perfect for scheduling!
```

---

### **5. BeepStarRating** ⭐

**Presets:**
- **⭐ 5 Stars**: Standard 5-star rating
- **⭐ 10 Stars**: Extended 10-point scale
- **⭐⭐⭐ 3/5 Rating**: Pre-set to 3 out of 5
- **⭐⭐⭐⭐⭐ 5/5 Rating**: Pre-set to perfect score

---

### **6. BeepCheckBox** ☑️

**Size Presets:**
- **Small (12px)**: Compact forms
- **Medium (16px)**: Standard size
- **Large (20px)**: Touch-friendly
- **X-Large (24px)**: High-DPI displays

**Custom Check Mark:**
- Click "Select Check Mark Icon..."
- Browse 600+ icons
- Use custom SVG as check mark!

---

### **7. BeepBreadcrump** 🍞

**Separator Presets:**
- **/ Slash**: Standard web style
- **> Chevron**: Classic breadcrumb
- **→ Arrow**: Modern direction indicator
- **• Dot**: Minimal style

---

### **8. BeepExtendedButton** 🔘

**Icon Configuration:**
- **Main Button Icon**: Select icon for primary button
- **Extend Button Icon**: Select icon for extend button (dropdown/more)
- **Presets:**
  - ▼ Dropdown (chevron down)
  - ⋮ More Options (menu icon)

---

## 💡 **TIPS & TRICKS**

### **Tip 1: Common Actions Available Everywhere**
Every BeepControl has these smart tags (scroll to bottom):
- Visual Style → Select Style...
- Quick Presets → Material 3, iOS 15, Fluent 2, Minimal
- Theme → Apply Current Theme, Use Theme Colors
- Painting → Use Style Painting

### **Tip 2: Icon Picker Categories**
Icons are auto-categorized:
- **Alerts**: alert-circle, bell, info, check-circle
- **Arrows**: arrow-down, chevron-right, external-link
- **Actions**: check, plus, minus, x
- **People**: user, users, user-plus
- **Files**: file, folder, document, archive
- **Time**: calendar, clock, watch
- **Messages**: mail, message, chat
- **Settings**: settings, tool, wrench
- And more!

### **Tip 3: Search is Your Friend**
Both Icon Picker and Style Selector have powerful search:
- **Icon Picker**: Type "check" → Shows Check, CheckCircle, CheckSquare, etc.
- **Style Selector**: Type "glass" → Shows Glassmorphism, GlassAcrylic

### **Tip 4: Business Presets Save Time**
Instead of configuring 5+ properties manually:
```csharp
// OLD WAY (manual):
datePicker.MinDate = DateTime.Today;
datePicker.AutoAdjustToBusinessDays = true;
datePicker.ShowValidationIcon = true;
datePicker.AllowEmpty = true;
datePicker.DateContext = "Due Date";

// NEW WAY (one click):
// Right-click → "📅 Due Date" → Done!
```

### **Tip 5: Style Presets for Consistency**
Apply same style to multiple controls:
```
1. Select first control
2. Right-click → Tasks → "Material 3"
3. Select second control
4. Right-click → Tasks → "Material 3"
5. Repeat...

Or use the common "Select Style..." to visually choose once!
```

### **Tip 6: Icon Presets for BeepSwitch**
Quick icon configurations:
- **✓ Checkmark Icons**: Perfect for yes/no, enabled/disabled
- **⚡ Power Icons**: Perfect for on/off, power states
- **⇄ Toggle Icons**: Perfect for switch/toggle actions
- **🔒 Lock Icons**: Perfect for locked/unlocked states

---

## 🎨 **COMMON WORKFLOWS**

### **Workflow 1: Create a "Due Date" Field**
```
1. Drag BeepDatePicker onto form
2. Right-click → "BeepDatePicker Tasks"
3. Click "📅 Due Date"
4. Done!

Result:
- MinDate = Today
- AutoAdjustToBusinessDays = true
- ShowValidationIcon = true
- AllowEmpty = true
- DateContext = "Due Date"
```

### **Workflow 2: Create a "Price" Field**
```
1. Drag BeepNumericUpDown onto form
2. Right-click → "BeepNumericUpDown Tasks"
3. Click "💰 Currency"
4. Done!

Result:
- Minimum = 0
- Maximum = 999,999.99
- DecimalPlaces = 2
- Increment = 0.01
```

### **Workflow 3: Style All Controls Consistently**
```
1. Select all controls (Ctrl+Click each)
2. Right-click → "Common BeepControl Tasks"
3. Click "Material 3" (or any style)
4. Done!

All controls now have Material 3 style!
```

### **Workflow 4: Create an Enabled/Disabled Switch**
```
1. Drag BeepSwitch onto form
2. Right-click → "BeepSwitch Tasks"
3. Click "✓ Checkmark Icons"
4. Set OnLabel = "Enabled", OffLabel = "Disabled"
5. Click "Material 3" style preset
6. Done!

Beautiful checkmark switch ready!
```

---

## 🎨 **ICON PICKER CATEGORIES**

### **UI Icons (from SvgsUI - ~400 icons)**
- Alerts & Notifications
- Arrows & Navigation
- Buttons & Controls
- Media & Images
- Files & Folders
- People & Users
- Communication
- Time & Calendar
- Settings & Tools
- And many more!

### **General Icons (from Svgs - ~150 icons)**
- Common UI elements
- System icons
- Application icons

### **DataSource Icons (from SvgsDatasources - ~50 icons)**
- Database icons
- Server icons
- Cloud storage icons
- Data integration icons

---

## 🎨 **STYLE CATEGORIES**

### **Modern Web (9 styles)**
Material3, MaterialYou, iOS15, AntDesign, ChakraUI, TailwindCard, NotionMinimal, VercelClean, StripeDashboard

**Best for**: Modern web-inspired applications, SaaS products

### **Microsoft (6 styles)**
Fluent2, Fluent, Windows11Mica, Metro, Metro2, Office

**Best for**: Windows applications, Office-like apps

### **Apple (2 styles)**
Apple, MacOSBigSur

**Best for**: macOS-style applications, cross-platform with macOS aesthetic

### **Linux Desktop (6 styles)**
Gnome, KDE, Cinnamon, Elementary, Ubuntu, ArcLinux

**Best for**: Linux applications, GTK-style apps

### **Minimal & Clean (6 styles)**
Minimal, Brutalist, NeoBrutalist, NotionMinimal, VercelClean, Paper

**Best for**: Content-focused applications, productivity tools

### **Effects & Glass (4 styles)**
Glassmorphism, GlassAcrylic, Neumorphism, GradientModern

**Best for**: Modern, trendy applications, creative tools

### **Gaming & Neon (5 styles)**
Gaming, Neon, Cyberpunk, DarkGlow, Holographic

**Best for**: Gaming applications, entertainment software

### **Theme Inspired (7 styles)**
Dracula, Nord, Tokyo, OneDark, GruvBox, Solarized, Nordic

**Best for**: Developer tools, code editors, terminal applications

### **Fun & Creative (3 styles)**
Cartoon, ChatBubble, Retro

**Best for**: Children's software, creative applications, retro games

---

## 🎯 **CONTROL-SPECIFIC SMART TAGS**

### **BeepSwitch**
- State & Labels
- Icons (with presets!)
- Style Presets
- Behavior (drag to toggle)

### **BeepNumericUpDown**
- Value Range configuration
- Business Presets (Currency, Percentage, etc.)

### **BeepDatePicker**
- Behavior (AllowEmpty, ReadOnly)
- Business Presets (Due Date, Birth Date, etc.)

### **BeepTimePicker**
- Behavior (MinuteInterval, AllowEmpty)
- Business Presets (Meeting, Appointment, etc.)

### **BeepCheckBox**
- Size configuration
- Size Presets (Small, Medium, Large, X-Large)
- Custom check mark icon

### **BeepStarRating**
- Star count configuration
- Rating presets (3/5, 5/5, etc.)

### **BeepBreadcrump**
- Separator configuration
- Separator Presets (Slash, Chevron, Arrow, Dot)

### **BeepExtendedButton**
- Layout (button widths)
- Icon selection for both buttons
- Extend icon presets (Dropdown, More)

### **BeepChart**
- Appearance (Title)
- Display options (Legend, Grid)

### **BeepCalendar**
- Display options (Week numbers, Today button)

### **BeepListBox & BeepComboBox**
- Behavior (MultiSelect, EnableSearch, IsEditable)

### **BeepMultiChipGroup**
- Behavior (MultiSelect, AllowUserAddition)
- Layout (ChipSpacing)

---

## 💡 **ADVANCED TIPS**

### **Tip 1: Combine Presets**
```
1. Apply business preset (e.g., "💰 Currency")
2. Then apply style preset (e.g., "Material 3")
3. Result: Perfectly configured AND styled control!
```

### **Tip 2: Icon + Style Consistency**
```
For BeepSwitch:
1. Apply "✓ Checkmark Icons"
2. Apply "Material 3" style
3. Set OnLabel/OffLabel to "" (hidden labels)
4. Result: Clean Material 3 checkmark switch!
```

### **Tip 3: Theme First, Then Style**
```
1. Apply theme to form (sets theme for all controls)
2. Select controls individually
3. Use smart tags to apply styles
4. Styles will respect theme colors if UseThemeColors = true
```

### **Tip 4: Quick Multi-Control Styling**
```
1. Select multiple controls (Ctrl+Click)
2. Right-click any selected control
3. "Common BeepControl Tasks" → "Material 3"
4. All selected controls get Material 3 style!
```

---

## 📚 **REFERENCE**

### **All Designers:**
1. BeepButton (existing, enhanced)
2. BeepLabel (existing, enhanced)
3. BeepImage (existing, enhanced)
4. BeepPanel (existing, enhanced)
5. **BeepSwitch** ⭐ (NEW!)
6. **BeepToggle** (NEW!)
7. **BeepCheckBox** (NEW!)
8. **BeepNumericUpDown** (NEW!)
9. **BeepDatePicker** (NEW!)
10. **BeepTimePicker** (NEW!)
11. **BeepListBox** (NEW!)
12. **BeepComboBox** (NEW!)
13. **BeepChart** (NEW!)
14. **BeepStarRating** (NEW!)
15. **BeepCalendar** (NEW!)
16. **BeepMultiChipGroup** (NEW!)
17. **BeepBreadcrump** (NEW!)
18. **BeepExtendedButton** (NEW!)

### **All Editors:**
1. BeepImagePathEditor (existing)
2. **IconPickerEditor** ⭐ (NEW!)
3. **StyleSelectorEditor** ⭐ (NEW!)

### **All Dialogs:**
1. BeepImagePickerDialog (existing)
2. **IconPickerDialog** ⭐ (NEW!)
3. **StyleSelectorDialog** ⭐ (NEW!)

---

## 🚀 **GET STARTED NOW!**

1. **Rebuild your solution** (if not done already)
2. **Open any form in designer**
3. **Drag a BeepSwitch** onto the form
4. **Right-click** → "BeepSwitch Tasks"
5. **Click "✓ Checkmark Icons"**
6. **Click "Material 3"**
7. **Done!** You have a beautiful Material 3 switch with checkmark icons!

---

## 🏆 **RESULT:**

**BeepControls now have the BEST design-time experience in WinForms!**

**Comparable to:**
- ✅ Telerik UI for WinForms
- ✅ DevExpress WinForms
- ✅ Syncfusion WinForms
- ✅ ComponentOne WinForms

**Your controls are ENTERPRISE-READY!** 🎉

---

**Questions? Issues? Check `IMPLEMENTATION_COMPLETE.md` for technical details!**

