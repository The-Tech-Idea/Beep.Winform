# 02 — Result & Config Model

**Priority P0. Phase 2.**

## Three types mean "the result of a dialog"

| Type | Where | Status |
|---|---|---|
| `DialogReturn` | `TheTechIdea.Beep.Vis.Modules2.0/IDialogManager.cs:62` | what every public method actually returns |
| `DialogsManagers.Models.DialogResult` | `Models/DialogResult.cs` (243 lines) | referenced by **one** callback signature and two doc comments |
| `System.Windows.Forms.DialogResult` | framework | what the forms themselves set and read |

The local `DialogResult` **shadows the framework type of the same name**. That is why 44 call sites
across 8 files write `System.Windows.Forms.DialogResult` in full:

```
Core.cs 20   Input.cs 9   File.cs 5
BeepCustomDialog.cs 2   BeepInputDialog.cs 2   BeepListDialog.cs 2
BeepMessageDialog.cs 2  BeepQuestionDialog.cs 2
```

Every one of those qualifications exists to work around a name collision with a class that is almost
unused. Its only real reference is:

```csharp
// DialogConfig.cs:404
public Func<DialogResult, bool>? ValidationCallback { get; set; }
```

So a 243-line public model exists to type one delegate, at the cost of shadowing a framework type
across the whole directory.

## `DialogConfig` is 1,221 lines with 32 factories

`DialogConfig` carries **32** `public static DialogConfig Create*` factories, and `DialogPreset`
carries **12** more `public static` members (68 references, so it is genuinely in use). Whether those
two overlap — and whether all 32 factories are reachable — is **not yet audited**. `ToolTips` had six
declared-and-never-read properties on a much smaller config object; a 1,221-line one has not earned
the benefit of the doubt.

## What the reference products do

- **Radix / Headless UI** — the dialog returns nothing; result flows through the caller's own state.
  Not directly portable to a modal WinForms API, but the principle holds: *one* result shape.
- **Ant Design / Material 3** — `Modal.confirm()` resolves a promise with a single typed outcome.
- **DevExpress / Telerik** — return `DialogResult` (the platform enum) plus a typed payload object.
  Two concepts, cleanly separated: *what the user pressed* and *what they produced*.

`DialogReturn` already has that shape (`Submit`, `UserAction`, `Value`). The local `DialogResult`
duplicates it badly and collides with the platform enum.

## Work

1. **Delete `Models/DialogResult.cs`.** Retype `ValidationCallback` as
   `Func<DialogReturn, bool>?`, which is the type the rest of the API already speaks.
2. **Remove the 44 fully-qualified `System.Windows.Forms.DialogResult` usages** once the shadow is
   gone — they become plain `DialogResult` and the code reads as normal WinForms.
3. **Audit `DialogConfig`**: for each of the 32 factories, find a caller; for each property, find a
   *read*. Delete what nothing reads. Reflection over the assembly, not grep — this is exactly the
   check that found six dead properties in `ToolTips` where a manual audit found three.
4. **Resolve `DialogConfig` vs `DialogPreset`.** Both build configured dialogs. Decide which owns
   presets and delete the overlap.

## Verification

- ⬜ `Models/DialogResult.cs` deleted; solution compiles — the proof, not a grep.
- ⬜ Harness: no type in `DialogsManagers` shadows a `System.Windows.Forms` type name.
- ⬜ Harness: every public property on `DialogConfig` is read somewhere in the assembly
  (reflection + `DeclaredOnly`, reported informationally — text matching cannot prove deadness).
