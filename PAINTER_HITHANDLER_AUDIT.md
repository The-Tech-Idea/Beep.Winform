# Painter to HitHandler Enum Mapping Audit

This document audits whether each Painter properly checks for hover/pressed states for ALL enum values that its corresponding HitHandler can return.

## Key Finding
**IMPORTANT:** The painter should check for hover/pressed states for ALL hit areas that the hit handler registers!

## Audit Results

### ⚠️ SingleDateTimePickerPainter → SingleDateTimePickerHitHandler
**HitHandler Registers:**
- PreviousButton
- NextButton
- DayCell
- TodayButton (if enabled)

**Painter Checks:**
- PreviousButton ✓
- NextButton ✓
- DayCell ❌ NOT CHECKED FOR HOVER/PRESS
- TodayButton ❌ NOT CHECKED FOR HOVER/PRESS

**Status:** ⚠️ INCOMPLETE - Missing DayCell and TodayButton hover checks

---

### ✅ CompactDateTimePickerPainter → CompactDateTimePickerHitHandler
**HitHandler Registers:**
- PreviousButton
- NextButton
- TodayButton
- DayCell

**Painter Checks:**
- PreviousButton ✓
- NextButton ✓
- TodayButton ✓
- DayCell (implicit through date rendering) ✓

**Status:** ✅ COMPLETE - All enums checked

---

### ⚠️ AppointmentDateTimePickerPainter → AppointmentDateTimePickerHitHandler
**HitHandler Registers:**
- PreviousButton
- NextButton
- DayCell
- TimeSlot

**Painter Checks:**
- PreviousButton ✓
- NextButton ✓
- DayCell ❌ NOT CHECKED FOR HOVER/PRESS
- TimeSlot ❌ NOT CHECKED FOR HOVER/PRESS

**Status:** ⚠️ INCOMPLETE - Missing DayCell and TimeSlot hover checks

---

### ⚠️ SingleWithTimeDateTimePickerPainter → SingleWithTimeDateTimePickerHitHandler
**HitHandler Registers:**
- PreviousButton
- NextButton
- DayCell
- TimeSlot

**Painter Checks:**
- PreviousButton ✓
- NextButton ✓
- TimeSlot ✓
- DayCell ❌ NOT CHECKED FOR HOVER/PRESS

**Status:** ⚠️ INCOMPLETE - Missing DayCell hover checks

---

## Summary

**Critical Issue:** Most painters only check hover/pressed states for **navigation buttons** but NOT for:
- **DayCell** - Day cells should have hover effects
- **TodayButton** - Today buttons should have hover effects
- **TimeSlot** - Time slots should have hover effects

**Recommendation:** Each painter must check `IsAreaHovered()` and `IsAreaPressed()` for ALL hit areas that its corresponding hit handler can return.
