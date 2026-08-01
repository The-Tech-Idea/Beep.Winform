# 04 — Dead Scaffolding

**Priority P0. Phase 1.** Cheap to remove, and it shrinks the surface everything else is measured against.

## Findings

**`TestDialogForm` ships in the product directory.** `Forms/TestDialogForm.cs` plus a 455-line
`TestDialogForm.Designer.cs`. A search across the whole solution finds **no references** outside its
own two files. It is a manual test harness that was committed into the shipping control library.

**`Forms/BeepDialogForm.resx` is an orphan.** There is no `BeepDialogForm.cs` and no
`BeepDialogForm.Designer.cs` anywhere in the directory. The `.resx` is a leftover from a form that
was renamed or deleted; it still compiles into the assembly as an embedded resource.

**`BeepDialogManager.Notifications.cs:498` is an empty `Dispose()`.**

```csharp
public void Dispose() { }
```

Either the handle owns something and this is a leak, or it owns nothing and the `IDisposable` is
theatre — a `using` that promises cleanup and performs none. Both readings are defects; which one it
is has to be established from the enclosing type before deleting or filling it.

**`DialogPlacementEngine` has zero callers** — covered in [07](07-placement-and-motion.md) because
the fix there is to *use* it, not to delete it.

## Work

1. **Delete `TestDialogForm`** (`.cs`, `.Designer.cs`, `.resx`). If a manual dialog harness is
   wanted, it belongs in a test or sample project, not in the control library. Verify by deleting and
   compiling — the method this codebase has already established as authoritative.
2. **Delete `Forms/BeepDialogForm.resx`.**
3. **Resolve the empty `Dispose`.** Read the enclosing type; either implement the cleanup it implies
   or remove the `IDisposable` claim. Ground rule 1.

## Verification

- ⬜ Solution compiles after each deletion — the proof.
- ⬜ Harness: every `.resx` under `DialogsManagers/` has a matching `.cs`.
- ⬜ Harness: no empty method body without an explicit `// intentionally empty: <reason>`.
