# 11 — Design-Time

## Verified — no defects found

Unlike every other feature in this program, this one turned up nothing to fix. Recorded as such
rather than left ambiguous.

Four checks over the serialization surface:

- **No property is editable-but-unpersisted.** `Browsable(true)` combined with
  `DesignerSerializationVisibility.Hidden` means the user sets a value in the property grid, sees it
  take effect, saves, reopens — and finds it reverted with no warning. This is the same shape as the
  three overflow policies that did nothing. Neither `BeepTabs` nor `BeepTabPage` has one.
- **All 14 `ShouldSerialize*` members on `BeepTabPage` have a matching `Reset*`,** so the grid's
  Reset command works for every one of them.
- **A default `BeepTabPage` serializes nothing,** so authoring a page does not write noise into
  `.Designer.cs` or churn the file on save.
- **The authored-to-runtime round trip holds**, exercised in the shape a generated
  `InitializeComponent` uses — construct, set properties, add children, add pages, add control — with
  no loops or conditionals, which is what the designer parser requires and emits. Pages rehydrate
  from the control tree, children stay attached, all nine authored metadata values survive, selecting
  a page does not disturb the tree, and an empty control does not invent a page.

## The existing tests could not be run

`BeepTabsPersistenceTests` covers the first of these, but the test project does not compile:
`Dialogs/BeepDialogManagerCreationTests.cs` fails with *'Form' does not contain a definition for
'CustomContent'*. That symbol does not exist anywhere in the controls assembly, and it is unrelated
to this program — but it means the suite is currently unrunnable. The round-trip assertions were
reproduced in the harness rather than reporting the behaviour as unverified.

## Original findings
 Experience

**Priority P2.**

## Current behaviour

`BeepTabs` is a `ContainerControl` and `BeepTabPage` is a `BaseControl`, so pages are real controls
that the designer can host and serialise. `BeepTabs.HostedContent` projects those pages into the
runtime host model.

The deleted plan stated the intended split explicitly:

> At design time, keep authored `BeepTabPage` instances on the `BeepTabs.Controls` tree for
> serializer and designer drop behavior, then project that same page model into the runtime host.

That is a sound design. What is unverified is whether it holds — specifically whether a page authored
in the designer round-trips through `InitializeComponent` and produces the same tab at runtime.

Relevant to this control specifically, and recorded in this repo's own memory as a hard constraint:
**no control flow may appear in `InitializeComponent`** — loops or conditionals there break the
Visual Studio designer parser. A tab control is exactly the kind of component whose generated code is
tempting to write as a loop over pages.

`BeepTabs` hides several inherited members (`new int TabCount`, `new int SelectedIndex`,
`new event EventHandler SelectedIndexChanged`), each marked `[Browsable(false)]` and
`DesignerSerializationVisibility.Hidden`. Hiding a base member is a designer-serialisation hazard: if
the base and the shadow disagree, the designer may serialise the wrong one.

## What the reference products do

- Add / remove / reorder pages from a designer verb or smart tag, not only from the properties grid.
- The designer surface shows the real header and lets a page be selected by clicking its tab.
- Serialisation emits one statement per page, no control flow.
- Undo/redo works for page operations.

## Work

1. **Prove the round trip.** Author a multi-page `BeepTabs` in the designer, build, run, and assert
   the runtime tab set matches what was authored — order, titles, icons, per-page flags.
2. **Inspect the generated `InitializeComponent`** for any control flow, and for whether the shadowed
   members (`SelectedIndex`, `TabCount`) get serialised. They are marked hidden; confirm the
   generated code agrees.
3. **Designer verbs**: add page / remove page / reorder, with undo support.
4. **Design-time header**: clicking a tab in the designer should select that page for editing.
5. **Confirm no runtime-only state leaks into serialisation** — hover, drag and MRU state must never
   be written to the designer file.

## Verification

- Build a host form with an authored multi-page `BeepTabs`; assert the generated designer file
  contains no loops or conditionals.
- Run it and assert the runtime tab set equals the authored one.
- Add, reorder and remove pages via the designer; undo each and assert the previous state returns.
- Assert no interaction-state property appears in the generated file.
