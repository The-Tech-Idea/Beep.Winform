# 01 — Two containers; v1 is the dead one

## Finding

`DisplayContainers/` holds two independent implementations of the same concept.

| | `BeepDisplayContainer` (v1) | `BeepDisplayContainer2` (v2) |
|---|---|---|
| Size | 804 lines, 1 file | ~4,300 lines, 11 partial files |
| Tab strip | wraps `BeepTabs` (`BeepDisplayContainer.cs:25,157`) | paints natively via `TabPaintHelper` |
| Features | whatever `BeepTabs` provides | badges, pinned tabs, drag-reorder, animation, overflow scroll |
| **Production use** | **none** | **5 forms across 3 repos** |

### Who actually instantiates v2

Scanned across every sibling repo under `source/repos/The-Tech-Idea`, not just this one:

- `Beep.Winform.Data.Integrated.Views/MainFrm.Designer.cs:50` — `new BeepDisplayContainer2()`, field at `:660`
- `Beep.Winform.Data.Integrated.Views/MainFrm_SideBar.Designer.cs`
- `Beep.Winform.Data.Integrated.Views/MainFrm_Tree.Designer.cs`
- `Beep.Sample/Beep.Sample.Winform.CRM/Forms/CrmMainForm.cs`
- `Beep.Sample/Beep.Sample.Winform.Features/Forms/MainForm.cs`

### Who still references v1

Only two sites, **both inside the control library itself**:

- `Managers/BeepFormUIManager.cs` — `:26` field, `:173` and `:222` type tests, `:465` public property
- `Styling/TabStylePresets.cs:74` — an `ApplyPreset` overload beside the v2 overload at `:96`

Two further hits are **string literals**, not type usage: `beepDisplayContainer1.ComponentName =
"BeepDisplayContainer"` in `MainFrm_SideBar.Designer.cs:225` and `MainFrm_Tree.Designer.cs:248` —
both on fields that are `BeepDisplayContainer2`. And `Beep.WPF/…/BeepDisplayContainer.cs` is an
unrelated WPF `ContentControl` that merely shares the name.

## Why this is a stalled migration

During the preceding session `DisplayContainers/BeepDisplayContainer.cs` was found **deleted in the
working tree** while `BeepFormUIManager.cs` and `TabStylePresets.cs` still referenced the type — 6
`CS0246` errors. The file was restored only to get a compiling tree.

That deletion was the right instinct. It failed because the two library-internal consumers were never
moved across. v2 is already the production container; v1 is a wrapper nothing ships.

## Work

- [ ] Point `BeepFormUIManager` at `BeepDisplayContainer2`: the `_displayContainer` field, both
      `is BeepDisplayContainer dc` type tests, and the `DisplayContainer` public property
- [ ] Delete `TabStylePresets.ApplyPreset(BeepDisplayContainer, TabStyle)`; keep the v2 overload.
      Check whether the v1 overload sets anything the v2 one does not, and carry it over if so
- [ ] Delete `BeepDisplayContainer.cs` — no shim, no `[Obsolete]` alias
- [ ] Confirm no `DisplayContainers/*.md` or designer resource still names v1

**Note the public break:** `BeepFormUIManager.DisplayContainer` changes type. It is public API, so
any external caller assigning or reading it must move too. The five known consumers all use v2
already, so this is expected to be contained — but it is a signature change, not an internal edit.

## Verification

- Solution builds with 0 errors after deleting v1. Deletion plus a clean compile is the authoritative
  test for deadness; grep is not, because receiver-less internal calls are invisible to a `\.Method(`
  search — a blind spot that produced three wrong conclusions across the preceding programs.
- `Beep.Winform.Data.Integrated` still builds against the changed `DisplayContainer` property type.
- A hosted container renders its tab strip, activates a tab, and closes one.
