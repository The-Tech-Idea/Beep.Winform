# BeepCalendarWidget Theme Instructions

## Overview
The BeepCalendarWidget ApplyTheme function should properly apply calendar-specific theme properties to ensure consistent styling for scheduling, events, and date selection displays.

## Current ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    BackColor = _currentTheme.BackColor;
    ForeColor = _currentTheme.ForeColor;
    
    InitializePainter();
    Invalidate();
}
```

## Required Theme Properties to Apply

### Calendar-Specific Colors
- `CalendarBackColor` - Background color for calendar
- `CalendarForeColor` - Default text color for calendar
- `CalendarBorderColor` - Border color for calendar
- `CalendarTodayForeColor` - Text color for today's date

### Date Selection Colors
- `CalendarSelectedDateBackColor` - Background for selected dates
- `CalendarSelectedDateForColor` - Text color for selected dates
- `CalendarHoverBackColor` - Background for hovered dates
- `CalendarHoverForeColor` - Text color for hovered dates

### Header and Navigation Colors
- `CalendarTitleForColor` - Color for calendar title (month/year)
- `CalendarDaysHeaderForColor` - Color for day headers (Mon, Tue, etc.)

### Typography Styles
- `CalendarTitleFont` - Font for calendar title
- `MonthFont` - Font for month display
- `YearFont` - Font for year display
- `DaysFont` - Font for day numbers
- `DaysHeaderFont` - Font for day headers
- `DaysSelectedFont` - Font for selected day numbers
- `DateFont` - Font for date displays

### Event/Accent Colors
- `AccentColor` - Color for events and highlights
- `PrimaryColor` - Primary accent color
- `SuccessColor` - Color for confirmed events
- `WarningColor` - Color for pending events
- `ErrorColor` - Color for conflicting events

### General Theme Colors
- `BackColor` - Fallback background color
- `ForeColor` - Fallback text color
- `SurfaceColor` - Surface color for calendar cells
- `BorderColor` - Fallback border color

## Enhanced ApplyTheme Implementation
```csharp
public override void ApplyTheme()
{
    base.ApplyTheme();
    if (_currentTheme == null) return;

    // Apply calendar-specific theme colors
    BackColor = _currentTheme.CalendarBackColor;
    ForeColor = _currentTheme.CalendarForeColor;
    
    // Update calendar structure colors
    _calendarBorderColor = _currentTheme.CalendarBorderColor;
    _todayForeColor = _currentTheme.CalendarTodayForeColor;
    _surfaceColor = _currentTheme.SurfaceColor;
    
    // Update date selection colors
    _selectedDateBackColor = _currentTheme.CalendarSelectedDateBackColor;
    _selectedDateForeColor = _currentTheme.CalendarSelectedDateForColor;
    _hoverBackColor = _currentTheme.CalendarHoverBackColor;
    _hoverForeColor = _currentTheme.CalendarHoverForeColor;
    
    // Update header colors
    _titleForeColor = _currentTheme.CalendarTitleForColor;
    _daysHeaderForeColor = _currentTheme.CalendarDaysHeaderForColor;
    
    // Update event colors
    _eventColor = _currentTheme.AccentColor;
    _confirmedEventColor = _currentTheme.SuccessColor;
    _pendingEventColor = _currentTheme.WarningColor;
    _conflictEventColor = _currentTheme.ErrorColor;
    
    InitializePainter();
    Invalidate();
}
```

## Implementation Steps
1. Update BackColor to use `CalendarBackColor` instead of generic `BackColor`
2. Update ForeColor to use `CalendarForeColor` instead of generic `ForeColor`
3. Add calendar structure color properties:
   - `_calendarBorderColor` = `CalendarBorderColor`
   - `_todayForeColor` = `CalendarTodayForeColor`
   - `_surfaceColor` = `SurfaceColor`
4. Add date selection color properties:
   - `_selectedDateBackColor` = `CalendarSelectedDateBackColor`
   - `_selectedDateForeColor` = `CalendarSelectedDateForColor`
   - `_hoverBackColor` = `CalendarHoverBackColor`
   - `_hoverForeColor` = `CalendarHoverForeColor`
5. Add header color properties:
   - `_titleForeColor` = `CalendarTitleForColor`
   - `_daysHeaderForeColor` = `CalendarDaysHeaderForColor`
6. Add event color properties:
   - `_eventColor` = `AccentColor`
   - `_confirmedEventColor` = `SuccessColor`
   - `_pendingEventColor` = `WarningColor`
   - `_conflictEventColor` = `ErrorColor`
7. Ensure InitializePainter() is called to refresh painter with new theme
8. Ensure Invalidate() is called to trigger repaint

## Additional Properties to Add
The BeepCalendarWidget may need additional properties for calendar styling:
- Structure colors: `_calendarBorderColor`, `_todayForeColor`, `_surfaceColor`
- Selection colors: `_selectedDateBackColor`, `_selectedDateForeColor`, `_hoverBackColor`, `_hoverForeColor`
- Header colors: `_titleForeColor`, `_daysHeaderForeColor`
- Event colors: `_eventColor`, `_confirmedEventColor`, `_pendingEventColor`, `_conflictEventColor`

## Testing
- Verify calendar background uses CalendarBackColor
- Verify calendar text uses CalendarForeColor
- Verify calendar borders use CalendarBorderColor
- Verify today's date uses CalendarTodayForeColor
- Verify selected dates use CalendarSelectedDate colors
- Verify hovered dates use CalendarHover colors
- Verify calendar title uses CalendarTitleForColor
- Verify day headers use CalendarDaysHeaderForColor
- Verify events use AccentColor
- Verify confirmed events use SuccessColor
- Verify pending events use WarningColor
- Verify conflicting events use ErrorColor
- Verify theme changes are immediately reflected</content>
<parameter name="filePath">c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Winform\TheTechIdea.Beep.Winform.Controls\Widgets\BeepCalendarWidget.theme.instructions.md