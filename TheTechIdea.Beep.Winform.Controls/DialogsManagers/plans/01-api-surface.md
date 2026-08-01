# 01 — API Surface & Duplicate Entry Points

**Priority P0. Phase 2.** Changes public API, so it lands before anything is built on top.

## Current behaviour

`BeepDialogManager` exposes 60+ public methods across five partial files. Six of them are pairs:

| Pair | Relationship | Evidence |
|---|---|---|
| `Warning` / `ShowWarning` | character-identical bodies; `Show*` is `[Obsolete]` | `Core.cs:598,607` |
| `Error` / `ShowError` | character-identical bodies; `Show*` is `[Obsolete]` | `Core.cs:615,624` |
| `Question` / `ShowQuestion` | character-identical bodies; `Show*` is `[Obsolete]` | `Core.cs:654,663` |
| `Info` / `ShowInfo` | **different behaviour**; `ShowInfo` is *not* `[Obsolete]` | `Core.cs:632,640` |
| `Success` / `ShowSuccess` | to be confirmed against the same pattern | `Core.cs` |
| `Confirm` / `ConfirmSync` | semantically identical; both synchronous | `Core.cs:671,679` |

### The one that matters

```csharp
// Core.cs:632 — goes through the pipeline
public DialogReturn Info(string title, string message)
    => Show(DialogConfig.CreateInformation(title, message));

// Core.cs:640 — "bypasses pipeline for direct BeepMessageDialog construction"
public DialogReturn ShowInfo(string title, string message)
{
    using var dialog = new BeepMessageDialog();
    dialog.Title = title;
    dialog.Message = message;
    dialog.StartPosition = FormStartPosition.CenterParent;
    var owner = _hostForm ?? (Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null);
    var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
    return new DialogReturn { Value = "ok", Submit = result == …DialogResult.OK, … };
}
```

Everything `Show(config)` does is skipped: default theme, default style, show animation, placement
strategy, state persistence, and the `DialogOpened` / `DialogConfirmed` / `DialogCancelled` events.

The trap is the naming. Three `Show*` methods are marked `[Obsolete]` with "Use X instead", teaching
the reader that `Show*` → bare name is a safe rename. `ShowInfo` is the one that is *not* marked, and
it is the only one where that rename changes behaviour — it turns theming, animation and events back
on. A caller who mechanically applies the deprecation guidance gets a different dialog.

`ConfirmSync` is a second naming lie: the name implies `Confirm` is asynchronous. Both are
synchronous, both call `Show(...).Submit`, and neither returns a `Task`.

## What the reference products do

There is one way to open a dialog and one way to configure it.

- **Radix / shadcn / Headless UI** — a single `Dialog` primitive; variants are props, not functions.
- **Material 3 / Ant Design** — `Modal.info()`, `.error()`, `.confirm()` are thin wrappers that all
  funnel through the same `Modal` implementation; none of them bypasses the theme or the lifecycle.
- **DevExpress `XtraDialog` / Telerik `RadDialog`** — one show method plus typed parameter objects.
- **VS Code** — `showInformationMessage` / `showWarningMessage` / `showErrorMessage` differ only in
  severity; the host renders all three identically.

The common rule: **convenience helpers may exist, but they must be pure sugar over the one path.**
A helper that skips the pipeline is not a helper, it is a second implementation.

## Work

1. **Delete the three `[Obsolete]` aliases** — `ShowWarning`, `ShowError`, `ShowQuestion`. Ground
   rule 2; they are character-identical to their replacements.
2. **Make `ShowInfo` sugar or delete it.** It cannot stay as it is. Either it becomes
   `=> Show(DialogConfig.CreateInformation(title, message));` like its siblings, or it goes. If any
   caller genuinely needs a pipeline-free dialog, that is a documented option on `DialogConfig`, not
   a differently-named method.
3. **Delete `ConfirmSync`.** Identical to `Confirm`, and its name asserts an async/sync distinction
   that does not exist.
4. **Audit `Success`/`ShowSuccess`** for the same pattern and resolve it the same way.
5. **State the rule in one place**: every convenience method is one expression that delegates to
   `Show(DialogConfig)`. Anything that cannot be written that way is a defect in `DialogConfig`.

## Verification

- ⬜ Harness asserts every public convenience method's body is a single delegation to `Show(...)`.
- ⬜ Harness asserts no `[Obsolete]` member remains in the directory.
- ⬜ A probe subscribes to `DialogOpened` and asserts it fires for **every** public show method —
  the check that would have caught `ShowInfo`.
