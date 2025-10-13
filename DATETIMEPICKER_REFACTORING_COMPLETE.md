# DateTimePicker Architecture - Mode-Based with BeepTheme Styling

## Final Architecture Decision

After initial confusion mixing functional modes with visual themes, we've implemented a clean separation:

**ONE ENUM: `DatePickerMode`** 
- Defines WHAT the picker does (Single, Range, Appointment, etc.)

**Visual Styling: `BeepTheme`**
- Defines HOW it looks (colors, fonts, borders, etc.)
- Uses existing Beep framework theming system

## Benefits

1. **Single Responsibility**: Each painter focuses on ONE functional mode
2. **Theme Consistency**: All Beep controls share visual styling through BeepTheme
3. **Flexibility**: Any mode can be styled by any BeepTheme
4. **Simplicity**: One property to set the mode, theme handled automatically
5. **Extensibility**: Easy to add new modes without creating theme variations

## DatePickerMode Enum

```csharp
public enum DatePickerMode
{
    Single,             // Standard single date selection calendar
    SingleWithTime,     // Single date with time picker section
    Range,              // Date range picker with start/end selection
    RangeWithTime,      // Date range with time selection
    Multiple,           // Multiple date selection with checkboxes
    Appointment,        // Calendar with time slot list for scheduling
    Timeline,           // Date range with visual timeline representation
    Quarterly,          // Quarterly range selector with Q1-Q4 shortcuts
    Compact,            // Compact dropdown with minimal chrome
    ModernCard,         // Modern card with quick date buttons (Today, Tomorrow, etc.)
    DualCalendar,       // Side-by-side month view for range selection
    WeekView,           // Week-based calendar view
    MonthView,          // Month picker view
    YearView            // Year picker view
}
```

## Usage

```csharp
// Simple and clean
myPicker.Mode = DatePickerMode.Single;           // Standard date picker
myPicker.Mode = DatePickerMode.Range;            // Range selection
myPicker.Mode = DatePickerMode.Appointment;      // With time slots

// Visual styling comes from BeepTheme automatically
myPicker.ApplyTheme(myCustomTheme);              // Change visual appearance
```

## Painter Architecture

### Interface
```csharp
public interface IDateTimePickerPainter
{
    DatePickerMode Mode { get; }  // Which mode this painter implements
    
    // All painting methods use BeepTheme for colors/fonts/styling
    void PaintCalendar(...);
    void PaintDayCell(...);
    // etc.
}
```

### Mode-Specific Painters

Each painter implements ONE functional mode with distinct layout/behavior:

1. **SingleDateTimePickerPainter** - Standard calendar, single date selection
2. **SingleWithTimeDateTimePickerPainter** - Calendar + time picker section
3. **RangeDateTimePickerPainter** - Start/end date selection with range highlighting
4. **RangeWithTimeDateTimePickerPainter** - Range + time selection
5. **MultipleDateTimePickerPainter** - Multiple dates with checkboxes
6. **AppointmentDateTimePickerPainter** - Calendar + time slot list (9:00 AM, 9:30 AM, etc.)
7. **TimelineDateTimePickerPainter** - Visual timeline with range
8. **QuarterlyDateTimePickerPainter** - Q1-Q4 shortcuts with range
9. **CompactDateTimePickerPainter** - Minimal chrome, compact layout
10. **ModernCardDateTimePickerPainter** - Card with Today/Tomorrow/+7 days buttons
11. **DualCalendarDateTimePickerPainter** - Side-by-side months for range
12. **WeekViewDateTimePickerPainter** - Week-based selection
13. **MonthViewDateTimePickerPainter** - Month picker (Jan, Feb, Mar...)
14. **YearViewDateTimePickerPainter** - Year picker (2023, 2024, 2025...)

### BeepTheme Usage in Painters

All painters use BeepTheme properties for visual styling:

```csharp
var bgColor = _theme?.BackgroundColor ?? Color.White;
var accentColor = _theme?.AccentColor ?? Color.Blue;
var textColor = _theme?.TextColor ?? Color.Black;
var font = _theme?.RegularFont ?? new Font("Segoe UI", 10f);
var boldFont = _theme?.BoldFont ?? new Font("Segoe UI", 10f, FontStyle.Bold);
var hoverColor = _theme?.HoverBackColor ?? Color.LightGray;
var borderColor = _theme?.BorderColor ?? Color.Gray;
```

## Files Modified

1. **enums.cs**
   - Added comprehensive `DatePickerMode` enum (14 modes)
   - Removed `DatePickerTheme` enum (not needed)
   - Removed deprecated `DatePickerTimePickerStyle` enum

2. **IDateTimePickerPainter.cs**
   - Changed `Style` property to `Mode` property
   - Type changed from `DatePickerTimePickerStyle` to `DatePickerMode`

3. **BeepDateTimePicker.Core.cs**
   - Single field: `_mode` (no theme field needed)
   - Removed all deprecated/backward compatibility code
   - Clean `InitializePainter()` and `UpdatePainter()` methods

4. **BeepDateTimePicker.Properties.cs**
   - Single property: `Mode` of type `DatePickerMode`
   - Removed `Theme` property
   - Removed deprecated `DateTimeStyle` property

5. **DateTimePickerPainterFactory.cs**
   - Single method: `CreatePainter(DatePickerMode, owner, theme)`
   - Maps each mode to its specific painter
   - Removed all deprecated overloads

6. **DateTimePickerModels.cs**
   - Cleaned up duplicate enum definitions
   - Removed deprecated code

## Painters Status

### Implemented:
- ✅ SingleDateTimePickerPainter - Complete with BeepTheme styling

### To Create (13 remaining):
- ⏳ SingleWithTimeDateTimePickerPainter
- ⏳ RangeDateTimePickerPainter
- ⏳ RangeWithTimeDateTimePickerPainter
- ⏳ MultipleDateTimePickerPainter
- ⏳ AppointmentDateTimePickerPainter
- ⏳ TimelineDateTimePickerPainter
- ⏳ QuarterlyDateTimePickerPainter
- ⏳ CompactDateTimePickerPainter
- ⏳ ModernCardDateTimePickerPainter
- ⏳ DualCalendarDateTimePickerPainter
- ⏳ WeekViewDateTimePickerPainter
- ⏳ MonthViewDateTimePickerPainter
- ⏳ YearViewDateTimePickerPainter

### Old Theme-Based Painters (To Delete):
These were created before architecture change and mix themes with functionality:
- Material3DateTimePickerPainter
- Fluent2DateTimePickerPainter
- AntDesignDateTimePickerPainter
- BootstrapDateTimePickerPainter
- ClassicDateTimePickerPainter
- iOS15DateTimePickerPainter
- GlassmorphismDateTimePickerPainter
- NeumorphismDateTimePickerPainter
- DarkGlowDateTimePickerPainter
- ChakraUIDateTimePickerPainter

## Mode Descriptions

### Single
Standard calendar view for selecting one date. Clean, simple, intuitive.

### SingleWithTime
Calendar on left/top, time picker on right/bottom. Select both date and time.

### Range
Two-date selection (start/end). Highlights dates between selected range. Common for hotel bookings, flight dates.

### RangeWithTime
Range selection + time selection for both start and end. For appointments spanning multiple days with specific times.

### Multiple
Select multiple individual dates (with checkboxes or multi-click). For recurring events or batch date selection.

### Appointment
Calendar + scrollable time slot list (hourly/half-hourly). For scheduling appointments, meetings. Shows available time slots.

### Timeline
Visual timeline bar showing date range with drag handles. Intuitive for selecting periods. Shows context of months/years.

### Quarterly
Quick selection for Q1 (Jan-Mar), Q2 (Apr-Jun), Q3 (Jul-Sep), Q4 (Oct-Dec). For business/financial date ranges.

### Compact
Minimal UI - smaller calendar, less padding, dropdown time. For constrained spaces or embedded forms.

### ModernCard
Calendar in a card with quick action buttons: "Today", "Tomorrow", "+7 days", "Next Month". Fast selection for common dates.

### DualCalendar
Two calendars side-by-side showing consecutive months. Better UX for range selection - see start/end simultaneously.

### WeekView
Calendar highlighting full weeks. Select week as a unit (Mon-Sun or custom). For weekly planning.

### MonthView
Grid of months (Jan, Feb, Mar...). Select entire month. For monthly reports or subscriptions.

### YearView
List or grid of years. Select a year. For birth year, graduation year, etc.

## Next Steps

1. ✅ Clean up architecture - COMPLETE
2. ⏳ Create remaining 13 mode-based painters
3. ⏳ Delete old theme-based painters
4. ⏳ Test all modes with different BeepThemes
5. ⏳ Implement time picker functionality
6. ⏳ Implement quick buttons
7. ⏳ Document each mode with screenshots

## Migration from Old Code

Any old code using theme-based painters should switch to mode-based:

```csharp
// OLD (before refactor - doesn't exist anymore)
picker.DateTimeStyle = DatePickerTimePickerStyle.Material3;

// NEW (clean architecture)
picker.Mode = DatePickerMode.Single;
// Visual styling comes from BeepTheme automatically
```

## Problem Identified

The original `DatePickerTimePickerStyle` enum was mixing two completely different concepts:

1. **Functional Types** (what the picker DOES):
   - `Default`, `ModernCard`, `RangeSelector`, `AppointmentPicker`, etc.
   
2. **Visual Themes** (how it LOOKS):
   - `Material3`, `iOS15`, `Fluent2`, `AntDesign`, `Bootstrap`, etc.

This design made it impossible to:
- Use a Material3 theme with Range selection
- Apply iOS15 styling to an Appointment picker
- Mix and match functionality with visual appearance

## Solution Implemented

### New Enum Structure

**`DatePickerMode` - Functional Behavior**
```csharp
public enum DatePickerMode
{
    Single,             // Standard single date selection
    SingleWithTime,     // Single date with time picker
    Range,              // Date range picker with start/end
    RangeWithTime,      // Date range with time selection
    Multiple,           // Multiple date selection
    Appointment,        // Calendar with time slot list (for scheduling)
    Timeline,           // Date range with visual timeline representation
    Quarterly,          // Quarterly range selector
    Compact             // Compact dropdown style
}
```

**`DatePickerTheme` - Visual Styling**
```csharp
public enum DatePickerTheme
{
    Default,            // Standard theme following current BeepTheme
    Material3,          // Google Material Design 3 style
    iOS15,              // Apple iOS 15+ style
    Fluent2,            // Microsoft Fluent Design 2
    AntDesign,          // Ant Design system
    Bootstrap,          // Bootstrap framework style
    ChakraUI,           // Chakra UI design
    TailwindCard,       // Tailwind CSS card style
    Glassmorphism,      // Glass morphism effect
    Neumorphism,        // Soft UI neumorphic design
    DarkGlow,           // Dark theme with neon accents
    NotionMinimal,      // Notion-inspired minimal
    MinimalClean,       // Ultra-clean minimal design
    Classic             // Classic WinForms style
}
```

### New BeepDateTimePicker Properties

```csharp
[Category("Appearance")]
[Description("Functional mode of the date/time picker")]
public DatePickerMode PickerMode { get; set; }  // What it does

[Category("Appearance")]
[Description("Visual theme/skin of the date/time picker")]
public DatePickerTheme Theme { get; set; }      // How it looks
```

### Usage Example

```csharp
// OLD WAY (limited, confusing):
datePicker.DateTimeStyle = DatePickerTimePickerStyle.Material3;  // Can only be Material3 calendar

// NEW WAY (flexible, clear):
datePicker.PickerMode = DatePickerMode.Range;           // Range selector functionality
datePicker.Theme = DatePickerTheme.Material3;           // Material3 visual style

// Mix and match as needed:
datePicker.PickerMode = DatePickerMode.Appointment;     // Appointment scheduler
datePicker.Theme = DatePickerTheme.iOS15;               // With iOS15 look

datePicker.PickerMode = DatePickerMode.Quarterly;       // Quarterly range
datePicker.Theme = DatePickerTheme.Glassmorphism;       // With glass effect
```

## Backward Compatibility

The old `DatePickerTimePickerStyle` enum and `DateTimeStyle` property are **marked as deprecated** but still functional:

```csharp
[Obsolete("Use PickerMode and Theme properties instead")]
public DatePickerTimePickerStyle DateTimeStyle { get; set; }
```

This allows existing code to continue working while encouraging migration to the new approach.

## Files Modified

1. **enums.cs**
   - Added `DatePickerMode` enum
   - Added `DatePickerTheme` enum
   - Marked `DatePickerTimePickerStyle` as `[Obsolete]`

2. **DateTimePickerModels.cs**
   - Removed duplicate enum definitions
   - Added comments referencing enums.cs

3. **BeepDateTimePicker.Core.cs**
   - Added `_pickerMode` and `_pickerTheme` fields
   - Added new `UpdatePainter()` method
   - Marked `SwitchPainter()` as `[Obsolete]`

4. **BeepDateTimePicker.Properties.cs**
   - Added `PickerMode` property
   - Added `Theme` property
   - Marked `DateTimeStyle` as `[Obsolete]` and `[Browsable(false)]`

5. **DateTimePickerPainterFactory.cs**
   - Added new `CreatePainter(DatePickerTheme)` overload
   - Marked old `CreatePainter(DatePickerTimePickerStyle)` as `[Obsolete]`

## Painters Created (Theme-based)

### Fully Implemented:
1. **Material3DateTimePickerPainter** - Material Design 3
2. **Fluent2DateTimePickerPainter** - Microsoft Fluent Design 2
3. **AntDesignDateTimePickerPainter** - Ant Design system
4. **BootstrapDateTimePickerPainter** - Bootstrap framework
5. **ClassicDateTimePickerPainter** - Classic WinForms
6. **iOS15DateTimePickerPainter** - Apple iOS 15+
7. **GlassmorphismDateTimePickerPainter** - Frosted glass effects
8. **NeumorphismDateTimePickerPainter** - Soft UI neumorphic
9. **DarkGlowDateTimePickerPainter** - Dark theme with neon glow

### Still To Create:
10. ChakraUIDateTimePickerPainter
11. TailwindCardDateTimePickerPainter
12. NotionMinimalDateTimePickerPainter
13. MinimalCleanDateTimePickerPainter

### Mode-Specific Painters (Future):
- ModernCardDateTimePickerPainter (functional type)
- RangeSelectorDateTimePickerPainter (functional type)
- AppointmentPickerDateTimePickerPainter (functional type)
- DualCalendarDateTimePickerPainter (functional type)
- QuarterlyRangeDateTimePickerPainter (functional type)
- CompactDropdownDateTimePickerPainter (functional type)
- TimelineRangeDateTimePickerPainter (functional type)

## Benefits

1. **Separation of Concerns**: Functionality separate from styling
2. **Flexibility**: Any mode can use any theme
3. **Extensibility**: Easy to add new modes or themes independently
4. **Clarity**: Clear naming makes intent obvious
5. **Maintainability**: Logical organization of related concepts
6. **Reusability**: Themes can be shared across different modes

## Migration Guide

### For Developers Using BeepDateTimePicker:

```csharp
// OLD CODE:
myPicker.DateTimeStyle = DatePickerTimePickerStyle.Material3;

// NEW CODE:
myPicker.PickerMode = DatePickerMode.Single;      // or Range, Appointment, etc.
myPicker.Theme = DatePickerTheme.Material3;

// The old property still works but is deprecated
```

### For Painter Developers:

Theme painters focus purely on visual appearance and should support all modes.
Mode-specific painters will handle special layouts (side-by-side calendars, timelines, etc.).

## Future Enhancements

1. Create mode-specific painters that adapt to different themes
2. Allow runtime theme switching without recreating controls
3. Support theme inheritance/customization
4. Add animation transitions between themes
5. Implement theme presets (Light/Dark/Auto)

## Status

- ✅ Enum separation complete
- ✅ Property refactoring complete
- ✅ Factory pattern updated
- ✅ Backward compatibility maintained
- ✅ 9 distinct theme painters implemented
- ⏳ 4 theme painters remaining
- ⏳ Mode-specific painters pending

## Notes

The `IDateTimePickerPainter` interface still uses `DatePickerTimePickerStyle` in the `Style` property for now. This is acceptable as it's just an identifier and maintains compatibility. Future versions may introduce a new interface or update this property.
