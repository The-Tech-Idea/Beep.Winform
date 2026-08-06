# Stage 05 — Data binding and multi-column return

**Kind:** enhancement · **Files:** `BeepListofValuesBox.cs`, `LovReturnMapping.cs` · **Status: done.**

## Data binding

`BoundProperty` was never set, so `BaseControl` had no idea which property carried the value and the
control could not take part in binding at all — on a control whose entire job is to supply a foreign
key to a bound field. It is now `SelectedKey`.

## Return items

Picking a row returned into the LOV's own field and nowhere else. Oracle Forms returns several columns
into several fields — choose a department and its number lands in one box, its location in another.

`ReturnMappings` is that list. Each entry names a **`LovField`** and a destination:

```csharp
lov.ReturnMappings.Add(new LovReturnMapping(LovField.Description, txtLocation));
lov.ReturnMappings.Add(new LovReturnMapping(LovField.SubText, v => _manager = v as string));
```

A `BaseControl` destination receives the value through `SetValue`; anything else has its `Text` set.

`ItemSelected` was added alongside, carrying the chosen `SimpleItem` — `SelectionChanged` passes
`EventArgs.Empty`, so a caller had to reach back for `SelectedItem` to learn what had happened.

## Why the fields are an enum

**`SimpleItem` is the item type every list control in this library uses**, and it already carries the
columns a LOV needs to return: `Value`, `Text`, `Name`, `Description`, `SubText`, `SubText2`,
`SubText3`, `ImagePath`, `GuidId`, `ID`. A developer maps their query's columns onto those fields when
they build the list, and the LOV reads them straight back out.

The first attempt took a **field-name string** and, failing to find it on `SimpleItem`, reflected into
whatever object sat in `SimpleItem.Item`. That was a second mechanism standing next to the one the
library already has, and it was wrong on its own terms: it invited callers to bypass `SimpleItem`'s
fields rather than populate them.

The enum is better on every axis that matters here — it is a dropdown in the designer, it needs no
reflection, and it cannot name a field that does not exist.

## Verification

A field into a control, a field into a delegate, a secondary field (`SubText`) resolving, `ItemSelected`
carrying the row, and a mapping with no destination being skipped rather than throwing.

The last one matters because mappings are edited in the designer, where a half-configured entry is the
normal intermediate state — it must not break the selection the user just made.
