# CustomData to KeyValue Migration Plan

## Problem Statement

The `DateTimePickerHitTestResult` class has both `CustomData` (object) and `KeyValue` (string) properties, but the codebase is inconsistently using `CustomData` for string-based button/filter keys instead of the dedicated `KeyValue` property.

### Current Structure (IDateTimePickerPainter.cs, lines 390-406)
```csharp
public string KeyValue { get; set; }        // ✅ Intended for string keys
public object CustomData { get; set; }      // ❌ Being misused for string keys
```

### Why This Is a Problem

1. **Type Safety**: `CustomData` is `object`, requiring casts (`CustomData.ToString()`)
2. **Semantic Clarity**: `KeyValue` clearly indicates a string identifier, `CustomData` suggests arbitrary data
3. **Code Consistency**: Some code uses `QuarterKey`, `FilterName`, `MonthKey` (specific string properties) while others use generic `CustomData`
4. **Maintainability**: `.ToString().ToLower().Replace("_", "")` pattern is repeated across multiple handlers

---

## Affected Files (16 CustomData usages across 3 handlers)

### 1. FilteredRangeDateTimePickerHitHandler.cs (10 usages)

#### Setting CustomData (4 locations):
- **Line 34**: FilterButton - `result.CustomData = filterKeys[i];`
- **Line 102**: TimeInput (from) - `result.CustomData = "from";`
- **Line 111**: TimeInput (to) - `result.CustomData = "to";`
- **Line 161**: QuickButton - `result.CustomData = btn.Key;`

#### Reading CustomData (6 locations):
- **Line 187**: FilterButton handler - `hitResult.CustomData != null`
- **Line 189**: FilterButton handler - `hitResult.CustomData.ToString().ToLower()`
- **Line 305**: QuickButton handler - `hitResult.CustomData != null`
- **Line 308**: QuickButton handler - `hitResult.CustomData.ToString().ToLower().Replace("filter_", "")`
- **Line 314**: Creating new result - `CustomData = key`
- **Line 398**: Hover state - `hoverState.HoveredButton = hitResult.CustomData?.ToString();`

### 2. FlexibleRangeDateTimePickerHitHandler.cs (3 usages)

#### Reading CustomData (3 locations):
- **Line 132**: QuickButton handler - `hitResult.CustomData != null`
- **Line 137**: QuickButton handler - `hitResult.CustomData.ToString().ToLower().Replace("_", "")` ← User's example
- **Line 211**: Hover state - `hoverState.HoveredButton = hitResult.CustomData?.ToString();`

**Note**: This handler does NOT set `CustomData` - it expects the HitTestHelper registration to provide it

### 3. QuarterlyDateTimePickerHitHandler.cs (3 usages)

#### Reading CustomData (3 locations):
- **Line 132**: QuarterButton handler - `hitResult.CustomData != null`
- **Line 134**: QuarterButton handler - `hitResult.CustomData.ToString().ToLower()`
- **Line 209**: Hover state - `hoverState.HoveredButton = hitResult.CustomData?.ToString();`

**Note**: This handler does NOT set `CustomData` - it expects the HitTestHelper registration to provide it

---

## Root Cause Analysis

### Where CustomData Is Actually Set

1. **FilteredRangeDateTimePickerHitHandler**: Sets it directly in `HitTest()` method
2. **FlexibleRangeDateTimePickerHitHandler**: Expects `HitTestHelper.RegisterHitAreas()` to set it (but doesn't - missing implementation!)
3. **QuarterlyDateTimePickerHitHandler**: Expects `HitTestHelper.RegisterHitAreas()` to set it (but doesn't - missing implementation!)

### The Missing Link

The painters call `_owner.HitTestHelper?.RegisterHitAreas(layout, properties, _owner.DisplayMonth);` but the `RegisterHitAreas` implementation is NOT setting `CustomData` or `KeyValue` for buttons registered in the layout.

**This means**:
- FilteredRange works because it manually sets `CustomData` in `HitTest()`
- FlexibleRange and Quarterly likely **fail silently** because `CustomData` is always null

---

## Migration Strategy

### Phase 1: Add KeyValue Support (Backwards Compatible)

1. ✅ **Keep existing `CustomData` property** (don't break anything)
2. ✅ **Start using `KeyValue` for new code**
3. ✅ **Set BOTH properties during transition** for compatibility

### Phase 2: Migrate Hit Handlers

#### Pattern to Apply:
```csharp
// OLD (brittle):
if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && hitResult.CustomData != null)
{
    var key = hitResult.CustomData.ToString().ToLower().Replace("_", "");
    // ...
}

// NEW (type-safe):
if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && !string.IsNullOrEmpty(hitResult.KeyValue))
{
    var key = hitResult.KeyValue.ToLower().Replace("_", "");
    // ...
}
```

### Phase 3: Fix HitTestHelper Registration

The `RegisterHitAreas` method needs to extract button keys from layout and set them in hit results.

**Example for QuarterlyDateTimePicker**:
```csharp
// In RegisterHitAreas implementation:
if (layout.QuarterButtonRects != null)
{
    string[] quarterKeys = { "q1", "q2", "q3", "q4" };
    for (int i = 0; i < layout.QuarterButtonRects.Count && i < quarterKeys.Length; i++)
    {
        var hitResult = new DateTimePickerHitTestResult
        {
            HitArea = DateTimePickerHitArea.QuarterButton,
            HitBounds = layout.QuarterButtonRects[i],
            KeyValue = quarterKeys[i],  // ✅ NEW
            CustomData = quarterKeys[i]  // ⚠️ Keep for backwards compatibility
        };
        // Register this hit result
    }
}
```

---

## Detailed Fix Plan

### Step 1: FilteredRangeDateTimePickerHitHandler.cs

**Lines to Change: 10**

#### Setting (4 changes):
```csharp
// Line 34 - FilterButton
OLD: result.CustomData = filterKeys[i];
NEW: result.KeyValue = filterKeys[i];
     result.CustomData = filterKeys[i];  // Keep temporarily

// Line 102 - TimeInput (from)
OLD: result.CustomData = "from";
NEW: result.KeyValue = "from";
     result.CustomData = "from";  // Keep temporarily

// Line 111 - TimeInput (to)
OLD: result.CustomData = "to";
NEW: result.KeyValue = "to";
     result.CustomData = "to";  // Keep temporarily

// Line 161 - QuickButton
OLD: result.CustomData = btn.Key;
NEW: result.KeyValue = btn.Key;
     result.CustomData = btn.Key;  // Keep temporarily
```

#### Reading (6 changes):
```csharp
// Line 187 - FilterButton null check
OLD: if (hitResult.HitArea == DateTimePickerHitArea.FilterButton && hitResult.CustomData != null)
NEW: if (hitResult.HitArea == DateTimePickerHitArea.FilterButton && !string.IsNullOrEmpty(hitResult.KeyValue))

// Line 189 - FilterButton key extraction
OLD: var key = hitResult.QuarterKey != null ? hitResult.QuarterKey.ToLower() : hitResult.CustomData.ToString().ToLower();
NEW: var key = hitResult.QuarterKey ?? hitResult.KeyValue ?? string.Empty;
     key = key.ToLower();

// Line 305 - QuickButton null check
OLD: if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && hitResult.CustomData != null)
NEW: if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && !string.IsNullOrEmpty(hitResult.KeyValue))

// Line 308 - QuickButton key extraction
OLD: var key = hitResult.CustomData.ToString().ToLower().Replace("filter_", "");
NEW: var key = (hitResult.KeyValue ?? string.Empty).ToLower().Replace("filter_", "");

// Line 314 - Creating legacy result
OLD: CustomData = key
NEW: KeyValue = key,
     CustomData = key  // Keep temporarily

// Line 398 - Hover state
OLD: hoverState.HoveredButton = hitResult.CustomData?.ToString();
NEW: hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
```

### Step 2: FlexibleRangeDateTimePickerHitHandler.cs

**Lines to Change: 3**

```csharp
// Line 132 - QuickButton null check
OLD: if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && hitResult.CustomData != null)
NEW: if (hitResult.HitArea == DateTimePickerHitArea.QuickButton && !string.IsNullOrEmpty(hitResult.KeyValue))

// Line 137 - QuickButton key extraction (user's example!)
OLD: var key = hitResult.CustomData.ToString().ToLower().Replace("_", "");
NEW: var key = (hitResult.KeyValue ?? string.Empty).ToLower().Replace("_", "");

// Line 211 - Hover state
OLD: hoverState.HoveredButton = hitResult.CustomData?.ToString();
NEW: hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
```

### Step 3: QuarterlyDateTimePickerHitHandler.cs

**Lines to Change: 3**

```csharp
// Line 132 - QuarterButton null check
OLD: if (hitResult.HitArea == DateTimePickerHitArea.QuarterButton && hitResult.CustomData != null)
NEW: if (hitResult.HitArea == DateTimePickerHitArea.QuarterButton && !string.IsNullOrEmpty(hitResult.KeyValue))

// Line 134 - QuarterButton key extraction
OLD: var quarterKey = hitResult.CustomData.ToString().ToLower();
NEW: var quarterKey = (hitResult.KeyValue ?? string.Empty).ToLower();

// Line 209 - Hover state
OLD: hoverState.HoveredButton = hitResult.CustomData?.ToString();
NEW: hoverState.HoveredButton = hitResult.KeyValue ?? hitResult.CustomData?.ToString();
```

### Step 4: HitTestHelper Implementation (Location TBD)

**Find where `RegisterHitAreas` is implemented and add KeyValue population**

This is the critical missing piece that causes FlexibleRange and Quarterly to not work properly.

```csharp
// Need to find: _owner.HitTestHelper?.RegisterHitAreas(layout, properties, displayMonth);
// Implementation needs to:
// 1. Iterate through layout button rectangles
// 2. Create hit results with proper KeyValue set
// 3. Register them for hit testing
```

---

## Testing Checklist

### After Each Handler Fix:

1. ✅ **Compilation**: No errors after string replacement
2. ✅ **Null Safety**: All `KeyValue` accesses use null coalescing (`??`)
3. ✅ **String Processing**: `.ToLower()`, `.Replace()` work on non-null strings
4. ✅ **Backwards Compatibility**: `CustomData` still set during transition

### Functional Testing:

#### FilteredRangeDateTimePicker:
- [ ] Click sidebar filter buttons (Today, Yesterday, etc.)
- [ ] Verify filter key is correctly identified
- [ ] Check hover state shows correct button

#### FlexibleRangeDateTimePicker:
- [ ] Click tolerance preset buttons (Exact dates, ± 1 day, etc.)
- [ ] Verify button key is correctly parsed
- [ ] **Verify CustomData is actually being set** (may be broken now!)

#### QuarterlyDateTimePicker:
- [ ] Click quarter buttons (Q1-Q4)
- [ ] Verify quarter key is correctly identified
- [ ] **Verify CustomData is actually being set** (may be broken now!)

---

## Benefits After Migration

### 1. Type Safety
```csharp
// Before: Runtime cast errors possible
var key = hitResult.CustomData.ToString();  // NullReferenceException if null!

// After: Compile-time type checking
var key = hitResult.KeyValue ?? string.Empty;  // Always safe
```

### 2. Code Clarity
```csharp
// Before: Unclear what CustomData contains
hitResult.CustomData = "q1";  // Is this a quarter? A filter? Who knows?

// After: Self-documenting
hitResult.KeyValue = "q1";  // Clearly a string identifier/key
```

### 3. Consistency
- `KeyValue` for button/filter identifiers
- `QuarterKey`, `MonthKey`, `WeekKey` for specific date-related keys
- `CustomData` only for truly custom/arbitrary data

### 4. Maintainability
- Single source of truth for key extraction
- Easier to add new button types
- Clear pattern for future developers

---

## Rollback Plan

If issues arise:

1. **Revert to CustomData**: Change all `KeyValue` back to `CustomData`
2. **Keep Dual-Setting**: Both properties set ensures compatibility
3. **Gradual Migration**: Fix one handler at a time, test thoroughly

---

## Implementation Order

1. **Start**: FilteredRangeDateTimePickerHitHandler (10 changes)
   - Has most usages
   - Already sets CustomData (won't break existing behavior)

2. **Next**: FlexibleRangeDateTimePickerHitHandler (3 changes)
   - User's reported issue
   - May reveal HitTestHelper bugs

3. **Next**: QuarterlyDateTimePickerHitHandler (3 changes)
   - Similar pattern to FlexibleRange
   - Validate HitTestHelper fixes

4. **Finally**: Investigate and fix HitTestHelper.RegisterHitAreas()
   - This is the root cause for FlexibleRange and Quarterly
   - Need to locate implementation
   - Add KeyValue population logic

---

## Estimated Effort

- **Per Handler**: 15-20 minutes (find/replace with verification)
- **HitTestHelper Fix**: 1-2 hours (need to locate, understand, implement)
- **Testing**: 30 minutes per picker type
- **Total**: ~4-5 hours for complete migration

---

## Open Questions

1. **Where is HitTestHelper.RegisterHitAreas() implemented?**
   - Called in all 18 painters but implementation not found in search
   - May be in BeepDateTimePicker or a helper class

2. **Should we deprecate CustomData entirely?**
   - Mark as `[Obsolete]` after migration complete?
   - Or keep for truly custom non-string data?

3. **Are there other hit result properties being misused?**
   - Audit QuarterKey, MonthKey, WeekKey usage
   - Ensure consistent patterns

---

## Success Criteria

✅ All `CustomData` usages for string keys replaced with `KeyValue`  
✅ Zero compilation errors  
✅ All 3 affected pickers functional  
✅ Hover states working correctly  
✅ Button clicks properly identified  
✅ HitTestHelper properly populates KeyValue  
✅ Code cleaner and more maintainable  

---

## Date: 2025-01-XX
**Status**: 📋 Plan Created - Ready for Implementation  
**Affected Files**: 3 hit handlers (16 total changes)  
**Priority**: Medium (improves code quality, may fix hidden bugs)
