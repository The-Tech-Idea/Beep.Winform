# 06 — Designer Serialization

**Priority P1. Phase 3.** The root cause is upstream of this directory.

## Every `BeepInputDialog` construction leaks 30 GDI+ handles

`Forms/BeepInputDialog.Designer.cs` is **2,060 lines**, and the reason is visible in its first 40:

```csharp
private void InitializeComponent()
{
    GraphicsPath graphicsPath1 = new GraphicsPath();
    GraphicsPath graphicsPath2 = new GraphicsPath();
    …
    GraphicsPath graphicsPath30 = new GraphicsPath();
```

**30 allocated, 0 disposed.** `TestDialogForm.Designer.cs` adds 6 more on the same pattern.
`GraphicsPath` wraps a native GDI+ handle, so every construction of the dialog leaks 30 of them.

They are assigned to three properties, ten each:

```
.BorderPath    = graphicsPath1, 3, 6, 10, 12, 15, 18, 22, 24, 27
.ContentShape  = graphicsPath2, 4, 7, 11, 13, 16, 19, 23, 25, 28
.InnerShape    = graphicsPath5, 8, 9, 14, 17, 20, 21, 26, 29, 30
```

`BorderPath`, `ContentShape` and `InnerShape` are **runtime-computed geometry** on the Beep control
base — the shapes a control derives from its own bounds during layout or paint. They are
designer-serializable, so the VS designer dutifully serialized them: it wrote out 30 **empty** paths
and assigns each one over the computed geometry at construction.

So the defect is three-in-one:

1. **A handle leak** — 30 undisposed `IDisposable`s per dialog.
2. **Overwritten geometry** — empty paths assigned over values the control computes for itself.
3. **2,060 lines of designer file**, ~90 of them pure `graphicsPath` noise, the rest inflated by the
   property assignments those paths belong to. This file cannot be reviewed.

## Root cause and scope

This is not a `DialogsManagers` bug. Any control exposing a computed `GraphicsPath` as a public,
browsable, serializable property will do this to **every form that hosts it**. The fix belongs on the
control that declares the property:

```csharp
[Browsable(false)]
[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
public GraphicsPath BorderPath { get; set; }
```

That is the same class of annotation the `BeepTabs` program used for `ModeCapabilities`, and the same
class of defect as the "editable in the grid but never persisted" check in that program's design-time
feature — a property that should never have reached the designer at all.

## Work

1. **Find every control declaring `BorderPath` / `ContentShape` / `InnerShape`** and mark them
   non-browsable and non-serializable. Scope this first: the blast radius is every designer file in
   the solution, not just this directory.
2. **Regenerate or hand-clean the affected designer files.** `BeepInputDialog.Designer.cs` should
   lose the 30 allocations and shrink substantially.
3. **Measure the leak before and after** — construct and dispose the dialog in a loop and watch the
   GDI object count. This is the evidence that the fix worked; the line count is not.
4. **Sequence with [05](05-layout-and-composition.md).** That feature rewrites these forms onto a
   `TableLayoutPanel` scaffold, which regenerates the designer files anyway. Doing 06 first means the
   regenerated files are clean; doing 05 first means regenerating twice.

## Verification

- ⬜ Harness: no `new GraphicsPath()` in any `.Designer.cs` under `DialogsManagers/`.
- ⬜ Probe: construct and dispose each dialog 500 times; assert the process GDI handle count returns
  to its starting value. This is the check that proves the leak is gone rather than moved.
- ⬜ Solution-wide: no public `GraphicsPath` property is designer-serializable.
