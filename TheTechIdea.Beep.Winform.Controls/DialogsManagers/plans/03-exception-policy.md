# 03 — Exception Policy

**Priority P0. Phase 1.** "No swallowed exceptions" is a stated rule for this codebase.

## Four bare catches

```csharp
// Helpers/DialogStateStore.cs:76 — LoadStates
catch
{
    return new Dictionary<string, DialogStateRecord>(StringComparer.OrdinalIgnoreCase);
}

// Helpers/DialogStateStore.cs:97 — SaveStates
catch
{
    // Silently fail — persistence is non-critical
}

// BeepDialogManager.Input.cs:438
try { initial = ColorTranslator.FromHtml(initialColor); } catch { }

// Models/DialogResult.cs:93 — GetData<T>
try { return (T)value; }
catch { return defaultValue; }
```

Each is a different kind of wrong.

**`LoadStates`** catches everything and returns empty. A corrupt state file, a permissions failure
and a schema change from a future version are indistinguishable, and all three silently discard every
remembered dialog position and size. The user sees dialogs reset to defaults with no explanation and
no way to find out why.

**`SaveStates`** is the same failure in the other direction, and its comment — *"Silently fail —
persistence is non-critical"* — states the intent plainly. But "the feature is optional" and "the
feature failing must be invisible" are different claims. If the directory is read-only, dialog
positions never persist for the entire life of the installation and nothing ever says so.

**`ColorTranslator.FromHtml`** is the closest to legitimate: an invalid colour string is a real,
expected input failure. But `catch { }` also swallows `OutOfMemoryException` and everything else, and
it leaves `initial` at whatever it was, so the caller cannot distinguish "you passed no colour" from
"you passed an invalid one".

**`GetData<T>`** is not error handling at all — it is a **type test written as an exception
handler**. `InvalidCastException` is being used as control flow where `value is T typed` is the
correct construct, and the `catch` additionally swallows anything else the cast could raise. It is
also far slower than the correct form on the failure path.

## What the reference products do

Persistence failures surface through a diagnostic channel; parse failures return a typed
"could not parse" rather than a silent default. Nothing in this class is caught blind.

## Work

1. **`GetData<T>` → pattern match.** `if (UserData.TryGetValue(key, out var v) && v is T typed) return typed;`
   No exception involved. This is a straight correctness and performance fix.
2. **`ColorTranslator.FromHtml` → narrow catch.** Catch only what the API documents for a malformed
   string, and record the reason. An empty `catch { }` cannot stay under rule 3.
3. **`DialogStateStore` → a real channel.** Both catches become narrow (`IOException`,
   `UnauthorizedAccessException`, `JsonException`) and report through an event or the manager's
   diagnostics, in every build configuration. Persistence may degrade silently *to the user*; it must
   not degrade silently *to the developer*.
4. **Distinguish "no saved state" from "state could not be read."** The first is normal on first
   run; the second is a fault. Returning an empty dictionary for both is why this is invisible today.

## Verification

- ⬜ Harness: no bare `catch` and no `catch (Exception)` with an empty or return-only body under
  `DialogsManagers/`.
- ⬜ Probe: point the state store at an unwritable path and assert the failure is observable.
- ⬜ Probe: `GetData<int>("k", 5)` on a key holding a `string` returns `5` without throwing —
  behaviour preserved, mechanism corrected.
