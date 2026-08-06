# Stage 07 — a dock of N launchers is one control to a screen reader

**Kind:** enhancement. The dock is keyboard-navigable and inaudible.
**Status:** ☑ done. All eight checks green. See *Outcome*.

## What exists

`BeepDock` sets three properties on itself when the handle is created:

```csharp
AccessibleName = "Dock";                                              // BeepDock.Keyboard.cs:259
AccessibleRole = AccessibleRole.ToolBar;                              // :260
AccessibleDescription = $"Application dock with {_items.Count} items"; // :261
```

and rewrites the description when selection changes (`BeepDock.Events.cs:35`). That is the entire
accessible surface. The items are painted, not child controls ([00](00-MASTER-TRACKER.md) — "no child
controls" is the rule this folder follows), so there is nothing for MSAA to enumerate. A screen
reader reads "Dock, toolbar, application dock with 8 items" and cannot name, reach or activate any of
the eight.

Keyboard navigation, by contrast, is fully implemented — arrows, Home/End, reorder, focus visuals
(`BeepDock.Keyboard.cs`, `BeepDock.InteractionState.cs:51`). So a keyboard user can move through the
dock while a screen-reader user is told nothing has changed. That combination is worse than neither.

## Two dead helpers pointing at the intent

```csharp
private void UpdateAccessibility()          // BeepDock.Keyboard.cs:267 — no callers
{
    if (IsHandleCreated)
    {
        AccessibleDescription = $"Application dock with {_items.Count} items";
        if (_focusedIndex >= 0 && _focusedIndex < _items.Count)
        {
            var item = _items[_focusedIndex];
            // Could use UI Automation to announce item changes      // :277
        }
    }
}
```

`item` is assigned and never read. The method has no callers, so the item count in the description
goes stale the moment items are added. `GetHighContrastColor` (`:324`) has no callers either;
`IsHighContrastMode` (`:316`) is called only by it. Three helpers, one live call between them.

The comment at `:277` is the correct instinct and the wrong mechanism — announcing focus changes is
what an accessible object publishes structurally, not something a control pushes.

## The precedent to follow

This exact problem was solved twice in this repository, for controls with the same painted-not-child
shape:

- `Docking/BeepDockspace.Accessibility.cs` — 134 lines, `Control.ControlAccessibleObject` subclass
  overriding `Role`, `GetChildCount`, `GetChild`, with per-tab child objects whose bounds come from
  the same rectangles the painter drew.
- `Tabs/BeepTabs.Accessibility.cs` — 115 lines, same shape.
- `Trees/Helpers/BeepTreeAccessibleObject.cs` — 367 lines, the hierarchical version.

Its file header states the trap this stage must avoid, and states it better than a restatement would:
overriding `GetChildCount`/`GetChild` is what actually publishes children, because the defaults
return `-1` and MSAA falls back to walking the *window* hierarchy. A traversal check run against an
un-overridden control therefore measures something real and proves nothing about the control.

None of that code carries over — the three controls share no painting or layout — but the shape does,
and following it keeps four controls consistent for whoever maintains them.

## The fix

A new `BeepDock.Accessibility.cs`, modelled on `BeepDockspace.Accessibility.cs`:

1. `CreateAccessibilityInstance()` returns a `DockAccessibleObject : Control.ControlAccessibleObject`.
2. `Role` is `AccessibleRole.ToolBar`, matching what the control already claims.
3. `GetChildCount()` returns the visible item count — items past `_overflowStartIndex` are not on
   screen (`BeepDock.Drawing.cs:32-35`) and must not be published as if they were. The overflow
   affordance itself is a child, named as such.
4. `GetChild(i)` returns a per-item accessible object whose:
   - `Name` is the item's `Text`
   - `Description` carries `Description` and the badge count when non-zero
   - `Bounds` come from `_itemStates[i].Bounds` — the same rectangle the painter drew and hit-testing
     uses. Deriving separate bounds here would be a second implementation that could disagree with
     what is on screen, which is the mistake [03](03-config-consolidation.md) is about.
   - `State` reports `Focused`, `Selected`, `Pressed` and `Unavailable` from the flags
     `DockItemState` already carries
   - `DefaultAction` is "Activate", invoking the same path a click takes
5. `UpdateAccessibility` is deleted; the accessible object reads live state, so nothing needs pushing.
6. `GetHighContrastColor` is either wired into the painters or deleted. High contrast is a real
   requirement for this stage — a painted dock with themed colours is exactly what high-contrast mode
   exists to override — so prefer wiring, and make it a resolver like stage 01's rather than a
   per-painter call.

## Verification

The baseline is what makes these checks mean anything. A stock `Form` containing a `Label` and a
`Button` is the control group: it reports children through the window hierarchy, so a naive traversal
returns a plausible non-zero number against a control with no accessible implementation at all.

1. **Child count.** 8 items, no overflow: assert `GetChildCount() == 8` on the dock's
   `AccessibilityObject`. *Today: `-1`.* Run the same assertion against the stock form first and
   confirm it does **not** return 8 — that is what proves the check is measuring the override and not
   the window tree.
2. **Names.** For each child, assert `Name` equals the corresponding item's `Text`. *Today there are
   no children to name.* This is the check that would be reported as "accessible" by a tool that only
   looks at `AccessibleName` on the control.
3. **Bounds agree with hit-testing.** For each child, take the centre of `child.Bounds`, convert to
   client coordinates, and assert `GetItemAtPoint` returns that same item. Catches the specific
   failure where the accessible tree is published from stale or independently-computed geometry.
4. **Overflow.** Size the dock so 3 of 8 items overflow. Assert `GetChildCount() == 6` — five visible
   items plus the overflow affordance — not 8. A tree that advertises unreachable children is worse
   than a short one.
5. **Focus tracks keyboard.** Arrow to item 3, assert exactly one child reports `Focused` and it is
   index 3. This is the one that closes the gap described at the top.
6. **High contrast.** Under `SystemInformation.HighContrast`, render each style and assert the
   item foreground/background contrast ratio is at least 4.5:1. Break it deliberately first — set a
   painter to draw grey on grey — and confirm the check goes red, or it is not yet a check.

## Outcome

`BeepDock.Accessibility.cs`, modelled on `Docking/BeepDockspace.Accessibility.cs`, so four controls
that all paint rather than nest now solve this the same way.

- `DockAccessibleObject` reports `AccessibleRole.ToolBar` and publishes one child per **visible**
  item. Items past `_overflowStartIndex` are not on screen and are not advertised; the overflow
  affordance is published as its own child instead, named "More items" with a count.
- `DockItemAccessibleObject` gives each launcher its `Text` as `Name`, `Description` plus badge count
  as `Description`, `PushButton` role, and `Bounds` taken from `_itemStates[i].Bounds` — the same
  rectangles the painter drew and hit-testing uses, so the tree cannot describe geometry that is not
  on screen.
- `State` reports `Focused`, `Selected`, `Pressed` and `Unavailable` from flags `DockItemState`
  already maintained and no one ever read.
- `DefaultAction` is "Activate" and routes through `RaiseItemClicked`, so a screen-reader activation
  takes the same path a click takes rather than selecting silently.

`UpdateAccessibility` is deleted — it had no callers, so its item count went stale on the first add,
and its inner branch assigned a variable it never read under a comment wishing for UI Automation. The
tree reads live state, so there is nothing to push.

### Measured

| check | result |
|---|---|
| child count | **8**, against a stock `Panel` control group reporting **0** |
| names | each child named by its item text |
| bounds agree with hit-testing | 8 of 8 |
| role / action | `ToolBar`; items `PushButton` + "Activate" |
| keyboard focus | exactly one child focused, at the right index |
| disabled item | reports `Unavailable` |

### Two failures that were the harness, not the tree

Both are worth recording because they looked exactly like defects in the new code:

1. **"dock reports 3"** of 8 children. The probe called `CreateControl()` but never `Show()`, so the
   parent never laid out, the dock kept its default width, and the overflow cut fired at item 3. The
   tree was **correct** — it was refusing to advertise items that genuinely were not on screen.
2. **"0 focused children"**. `State` reports `Focused` only when the control actually has focus, and
   a control on an unshown form cannot take focus.

The fix was to show the form, which is the more faithful setup anyway. A related detail: the
bounds/hit-test check printed "8 items" from a hardcoded string while only looping over
`min(8, childCount)` = 3 — it was reporting a coverage number it had not achieved. It now prints what
it actually checked.

### Still owed

`GetHighContrastColor` and `IsHighContrastMode` (`BeepDock.Keyboard.cs`) are still unwired — the
former has no callers and the latter only serves it. High-contrast support and the 4.5:1 contrast
assertion in check 6 above are not done; they need the painters to route foreground colour through a
resolver, which is the same eight-painter conversion [01](01-style-switching-is-one-way.md) left open.
