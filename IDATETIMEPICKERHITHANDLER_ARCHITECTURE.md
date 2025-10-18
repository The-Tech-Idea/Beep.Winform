# IDateTimePickerHitHandler Architecture

## Overview

`IDateTimePickerHitHandler` is a painter-specific handler interface that completely manages interaction logic, hit testing, and state synchronization for each DateTimePicker painter mode.

## Responsibilities

### 1. **Painter-Specific Hit Testing**
Each handler understands its painter's layout structure:
- Single-month layouts (Compact, Header, Appointment, Multiple)
- Dual-month layouts (DualCalendar, FlexibleRange, FilteredRange)
- Time slot layouts (Appointment, RangeWithTime)
- Quick button layouts
- Week number layouts

### 2. **State Management**
Handlers maintain their own internal state:
- Current selected date(s)
- Range start/end dates
- Selected time
- Multiple date selections
- Temporary hover/selection state

### 3. **Bidirectional Sync**
Handlers sync data with the control:
- **SyncFromControl()**: Control → Handler (when values change externally)
- **SyncToControl()**: Handler → Control (when user interaction changes values)

### 4. **Click Handling**
Handlers process clicks according to their mode:
- Single date selection
- Range selection (start → end)
- Multiple date toggling
- Time slot selection
- Quick button actions

### 5. **Validation**
Handlers validate selections:
- IsSelectionComplete() checks if required values are set
- For ranges: both start and end must be selected
- For single dates: date must be in valid range

## Interface Methods

```csharp
internal interface IDateTimePickerHitHandler
{
    // Identity
    DatePickerMode Mode { get; }
    
    // Hit Testing (knows painter layout structure)
    DateTimePickerHitTestResult HitTest(
        Point location, 
        DateTimePickerLayout layout, 
        DateTime displayMonth, 
        DateTimePickerProperties properties);
    
    // Click Handling (mode-specific logic)
    bool HandleClick(
        DateTimePickerHitTestResult hitResult, 
        BeepDateTimePicker owner);
    
    // Hover State (visual feedback)
    void UpdateHoverState(
        DateTimePickerHitTestResult hitResult, 
        DateTimePickerHoverState hoverState);
    
    // Sync: Control → Handler
    void SyncFromControl(BeepDateTimePicker owner);
    
    // Sync: Handler → Control
    void SyncToControl(BeepDateTimePicker owner);
    
    // Validation
    bool IsSelectionComplete();
    
    // Reset
    void Reset();
}
```

## Handler Implementations Needed

### Single-Month Handlers

1. **SingleDateHitHandler** (DatePickerMode.Single)
   - Simple date selection
   - Used by: Single, Compact, Header painters

2. **MultipleDateHitHandler** (DatePickerMode.Multiple)
   - Toggle date selection (add/remove from list)
   - Maintains List<DateTime> internally
   - Used by: MultipleDateTimePickerPainter

3. **AppointmentHitHandler** (DatePickerMode.Appointment)
   - Date + time slot selection
   - Handles scrollable time list
   - Used by: AppointmentDateTimePickerPainter

### Dual-Month Handlers

4. **RangeHitHandler** (DatePickerMode.Range, DualCalendar)
   - Two-step selection: start → end
   - Handles both calendar grids
   - Used by: DualCalendarDateTimePickerPainter

5. **FlexibleRangeHitHandler** (DatePickerMode.FlexibleRange)
   - Range with flexible mode toggle
   - Tab selector (exact dates vs flexible)
   - Quick date buttons (±1 day, ±2 days, etc.)
   - Used by: FlexibleRangeDateTimePickerPainter

6. **FilteredRangeHitHandler** (DatePickerMode.FilteredRange)
   - Range with date filtering
   - Disabled/filtered date visualization
   - Sidebar filters
   - Used by: FilteredRangeDateTimePickerPainter

## Event Flow with Handlers

### Hover Flow
```
Mouse Move → BeepDateTimePicker.OnMouseMove()
           → handler.HitTest(location, layout, displayMonth, properties)
           → handler.UpdateHoverState(hitResult, hoverState)
           → Control.Invalidate() for visual feedback
```

### Click Flow
```
Mouse Click → BeepDateTimePicker.OnMouseClick()
            → handler.HitTest(location, layout, displayMonth, properties)
            → handler.HandleClick(hitResult, owner)
            → handler.SyncToControl(owner)  // Update control values
            → Control.Invalidate()
            → (Optional) Close dropdown if selection complete
```

### External Value Change Flow
```
Control.SelectedDate = newDate
→ handler.SyncFromControl(owner)  // Handler updates internal state
→ Control.Invalidate()
```

## Integration Points

### With BeepDateTimePicker
```csharp
public partial class BeepDateTimePicker
{
    private IDateTimePickerHitHandler _hitHandler;
    
    protected override void OnMouseMove(MouseEventArgs e)
    {
        var hitResult = _hitHandler.HitTest(e.Location, _layout, _displayMonth, GetCurrentProperties());
        _hitHandler.UpdateHoverState(hitResult, _hoverState);
        Invalidate();
    }
    
    protected override void OnMouseClick(MouseEventArgs e)
    {
        var hitResult = _hitHandler.HitTest(e.Location, _layout, _displayMonth, GetCurrentProperties());
        bool shouldClose = _hitHandler.HandleClick(hitResult, this);
        _hitHandler.SyncToControl(this);
        
        if (shouldClose && this.Parent is ToolStripDropDown dropdown)
            dropdown.Close();
    }
    
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            _selectedDate = value;
            _hitHandler?.SyncFromControl(this);
            Invalidate();
        }
    }
}
```

### With BeepDateDropDown
```csharp
private void SyncToCalendar()
{
    // Sync dropdown state to calendar
    _calendarView.SelectedDate = _selectedDate;
    
    // Calendar's handler syncs from control
    // _calendarView._hitHandler.SyncFromControl(_calendarView);
}

private void SyncFromCalendar()
{
    // Calendar's handler has updated control values
    // Just read them back
    _selectedDate = _calendarView.SelectedDate;
    _startDate = _calendarView.RangeStartDate;
    _endDate = _calendarView.RangeEndDate;
}
```

### With BeepDateTimePickerHitTestHelper
```csharp
// The HitTestHelper registers hit areas for BaseControl's system
// But the handler provides the high-level interpretation
public void RegisterHitAreas(DateTimePickerLayout layout, DateTimePickerProperties props, DateTime displayMonth)
{
    // Register areas for BaseControl click routing
    RegisterDayCells(layout, displayMonth, props);
    RegisterNavigationButtons(layout);
    
    // The handler interprets these hits with full context
}
```

## Benefits of This Architecture

1. **Separation of Concerns**
   - Painter: Drawing logic
   - Handler: Interaction logic
   - Helper: Hit area registration for BaseControl

2. **Mode-Specific Logic**
   - Each mode can have completely different interaction patterns
   - Range pickers handle two-step selection
   - Multiple pickers handle toggle logic
   - Single pickers handle simple selection

3. **Bidirectional Sync**
   - Clean sync between control and handler
   - Handler maintains its own state
   - Control can push/pull values

4. **Validation**
   - Handlers know when selection is complete
   - Dropdowns can auto-close on complete selection
   - Incomplete selections can show visual feedback

5. **Testability**
   - Handlers can be tested independently
   - Mock handlers can be injected for testing
   - Each handler's logic is isolated

## Implementation Priority

1. **Phase 1**: Create base handlers
   - SingleDateHitHandler (simplest)
   - RangeHitHandler (most common dual-month)

2. **Phase 2**: Add specialized handlers
   - MultipleDateHitHandler
   - AppointmentHitHandler

3. **Phase 3**: Add complex handlers
   - FlexibleRangeHitHandler
   - FilteredRangeHitHandler

## Next Steps

1. Create `SingleDateHitHandler` as reference implementation
2. Create `RangeHitHandler` for dual-month support
3. Update `BeepDateTimePicker.Events.cs` to use handlers
4. Update `BeepDateTimePicker.Core.cs` to initialize handlers
5. Test each handler independently
6. Integrate with BeepDateDropDown sync
