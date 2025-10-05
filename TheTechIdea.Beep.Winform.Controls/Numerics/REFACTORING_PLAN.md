# BeepNumericUpDown Refactoring Plan

**Date:** October 4, 2025  
**Objective:** Refactor BeepNumericUpDown to use painter pattern with BeepControlStyle

---

## Current Structure (Before Refactoring)

**Single Monolithic File:**
- `BeepNumericUpDown.cs` (1404 lines)
  - All properties, events, methods
  - Drawing code inline (DrawControl, DrawButton methods)
  - No painter pattern
  - No style switching capability

---

## New Structure (After Refactoring)

### **Partial Classes:**

1. **BeepNumericUpDown.cs** (Main)
   - Public properties (Value, MinimumValue, MaximumValue, etc.)
   - Events (ValueChanged, etc.)
   - Constructor and initialization
   - Public methods

2. **BeepNumericUpDown.Painters.cs**
   - `Style` property (BeepControlStyle enum)
   - `CurrentPainter` property
   - Painter initialization logic (switch statement for 16 styles)
   - Hit area management

3. **BeepNumericUpDown.Drawing.cs**
   - OnPaint override
   - Calls to painter
   - Layout calculations

4. **BeepNumericUpDown.Helpers.cs**
   - Value formatting logic
   - Validation methods
   - TextBox management
   - Timer and repeat logic

5. **BeepNumericUpDown.Events.cs**
   - Mouse event handlers
   - Keyboard event handlers
   - Value change handlers
   - Focus/hover management

### **Painter Infrastructure:**

1. **INumericUpDownPainter.cs** ✅ DONE
   - Interface for all painters
   - Paint(), PaintButtons(), PaintValueText(), UpdateHitAreas()

2. **INumericUpDownPainterContext.cs** ✅ DONE (part of interface file)
   - Context interface exposing control state to painters

3. **BaseNumericUpDownPainter.cs** ✅ DONE
   - Abstract base class
   - Common helper methods (GetButtonWidth, GetTextRect, CreateRoundedPath)
   - Default UpdateHitAreas implementation

### **16 Painter Implementations:**

All inherit from `BaseNumericUpDownPainter`:

1. **Material3NumericUpDownPainter.cs** ✅ DONE
2. **iOS15NumericUpDownPainter.cs** ⏳ TODO
3. **AntDesignNumericUpDownPainter.cs** ⏳ TODO
4. **Fluent2NumericUpDownPainter.cs** ⏳ TODO
5. **MaterialYouNumericUpDownPainter.cs** ⏳ TODO
6. **Windows11MicaNumericUpDownPainter.cs** ⏳ TODO
7. **MacOSBigSurNumericUpDownPainter.cs** ⏳ TODO
8. **ChakraUINumericUpDownPainter.cs** ⏳ TODO
9. **TailwindCardNumericUpDownPainter.cs** ⏳ TODO
10. **NotionMinimalNumericUpDownPainter.cs** ⏳ TODO
11. **MinimalNumericUpDownPainter.cs** ⏳ TODO
12. **VercelCleanNumericUpDownPainter.cs** ⏳ TODO
13. **StripeDashboardNumericUpDownPainter.cs** ⏳ TODO
14. **DarkGlowNumericUpDownPainter.cs** ⏳ TODO
15. **DiscordStyleNumericUpDownPainter.cs** ⏳ TODO
16. **GradientModernNumericUpDownPainter.cs** ⏳ TODO

---

## Refactoring Steps

### ✅ Phase 1: Create Infrastructure (DONE)
1. Create INumericUpDownPainter interface
2. Create BaseNumericUpDownPainter base class
3. Create Material3NumericUpDownPainter implementation

### ⏳ Phase 2: Split Main File into Partials
1. Extract painter logic → BeepNumericUpDown.Painters.cs
2. Extract drawing logic → BeepNumericUpDown.Drawing.cs
3. Extract helper methods → BeepNumericUpDown.Helpers.cs
4. Extract event handlers → BeepNumericUpDown.Events.cs
5. Keep main properties/constructor in BeepNumericUpDown.cs

### ⏳ Phase 3: Implement Remaining Painters (1-15)
1. Copy Material3 painter as template
2. Customize each for specific design system
3. Implement unique visual characteristics per style

### ⏳ Phase 4: Testing & Verification
1. Test all 16 styles visually
2. Verify button interactions
3. Test value changes, validation
4. Check focus/hover states
5. Compile and ensure zero errors

---

## Design Decisions

### **Pattern Consistency:**
- Same pattern as NavBar, SideBar, ProgressBar
- Geometry helpers allowed (CreateRoundedPath)
- NO shared painting helpers - each painter draws everything inline

### **Context Adapter:**
- Private nested class in BeepNumericUpDown.Painters.cs
- Implements INumericUpDownPainterContext
- Exposes control state to painters without breaking encapsulation

### **Button Layout:**
- Down button on LEFT
- Up button on RIGHT
- Consistent across all painters

### **Text Formatting:**
- Helper method in context/main class
- Painters receive pre-formatted string
- Handles: Standard, Percentage, Currency, CustomUnit, ProgressValue modes

---

## Expected Benefits

1. ✅ **16 Visual Styles** - Users can switch at design-time or runtime
2. ✅ **Clean Separation** - Partial classes organize code logically
3. ✅ **Maintainability** - Each painter is independent, ~150-200 lines
4. ✅ **Consistency** - Same pattern across all Beep controls
5. ✅ **Extensibility** - Easy to add new painters without modifying core

---

## Progress Tracking

**Infrastructure:** 3/3 complete (100%) ✅  
**Partial Classes:** 4/5 complete (80%) ✅  
**Painters:** 1/16 complete (6.25%) ⏳  
**Overall:** 8/24 tasks complete (33.3%) ⏳  

### ✅ Completed Partial Classes:
1. **BeepNumericUpDown.Painters.cs** ✅
   - Style property with BeepControlStyle enum
   - Painter initialization for all 16 styles
   - Hit area management
   - NumericUpDownPainterContext adapter class

2. **BeepNumericUpDown.Drawing.cs** ✅
   - OnPaint override using painter
   - OnResize with button area calculations
   - GetButtonWidthForSize helper
   - Legacy Draw() method support

3. **BeepNumericUpDown.Events.cs** ✅
   - Mouse events (Move, Leave, Down, Up, Click, DoubleClick)
   - Keyboard events (KeyDown, arrow keys, Page Up/Down, Home/End, Enter/Escape)
   - Focus events (GotFocus, LostFocus)
   - Timer events for repeat button behavior

4. **BeepNumericUpDown.Helpers.cs** ✅
   - Value increment/decrement internal methods
   - Value validation and range checking
   - Text formatting (FormatValue with all display modes)
   - TextBox management (StartEditing, EndEditing)
   - Geometry helpers (GetRoundedRectPath)
   - ValueValidatingEventArgs class

### ⏳ Remaining Tasks:
1. **Main file modification** - Change `public class` to `public partial class`
2. **Remove duplicate code** from main file (drawing, events already in partials)
3. **Complete 15 remaining painters** (iOS15 through GradientModern)

---

**Last Updated:** October 4, 2025  
**Status:** Phase 2 Nearly Complete (80%) - Need to modify main file + create 15 painters
