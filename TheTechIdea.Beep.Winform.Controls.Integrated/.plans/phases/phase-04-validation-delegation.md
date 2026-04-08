# Phase 04 — Delegate `BeepDataBlock.Validation.*` to FormsManager

**Repo:** Beep.Winform  
**Scope:** `BeepDataBlock.Validation.cs` + model/helper deletion.  
**Depends on:** Phase 02 (`_formsManager` guaranteed non-null).  
**Build check:** `dotnet build WinFormsApp.sln` — zero errors before moving to Phase 05.

---

## What changes in this phase

| Item | Action |
|---|---|
| `private Dictionary<string, List<ValidationRule>> _validationRules` | Delete field |
| `ValidationBridge` usage | Delete — no translation layer after types are unified |
| Every public method body | Replace with one-line delegate to `_formsManager.Validation.*` |
| `Models/ValidationRule.cs` | Delete file (body was already commented out) |
| `Helpers/ValidationRuleHelpers.cs` | Delete file |

---

## Clean Code Rules

- `ValidateField` and `ValidateRecord` return `IErrorsInfo` directly from FormsManager — no local result assembly.
- `GetValidationRules` returns what FormsManager knows — not a local snapshot.
- No try/catch around FormsManager calls.
- Remove every `_validationRules[...]` / `_validationRules.ContainsKey(...)` expression.

---

## Target: `BeepDataBlock.Validation.cs` (full rewrite)

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor.Forms.Models;   // ValidationRule, ValidationDefinition
using TheTechIdea.Beep.Report;                // IErrorsInfo

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepDataBlock partial — Validation &amp; Business Rules.
    /// All validation state is owned by <see cref="FormsManager"/>.
    /// Every method here is a one-line delegate.
    /// </summary>
    public partial class BeepDataBlock
    {
        #region Registration

        /// <summary>
        /// Registers a validation rule for the given field.
        /// Oracle Forms equivalent: defining a validation trigger or property constraint.
        /// </summary>
        public void RegisterValidationRule(string fieldName, ValidationRule rule)
            => _formsManager.Validation.RegisterRule(Name, fieldName, rule);

        /// <summary>Registers a record-level validation rule (runs on save/commit).</summary>
        public void RegisterRecordValidationRule(ValidationRule rule)
            => _formsManager.Validation.RegisterRule(Name, "*", rule);

        /// <summary>Removes all validation rules for the given field from this block.</summary>
        public void UnregisterValidationRules(string fieldName)
            => _formsManager.Validation.UnregisterItemRules(Name, fieldName);

        /// <summary>Returns all validation rules registered for the given field.</summary>
        public List<ValidationDefinition> GetValidationRules(string fieldName)
            => _formsManager.Validation.GetItemRules(Name, fieldName);

        #endregion

        #region Execution

        /// <summary>
        /// Validates a single field value.
        /// Oracle Forms equivalent: running WHEN-VALIDATE-ITEM.
        /// </summary>
        public Task<IErrorsInfo> ValidateField(string fieldName, object value)
            => _formsManager.Validation.ValidateItemAsync(Name, fieldName, value);

        /// <summary>
        /// Validates all fields in the current record.
        /// Oracle Forms equivalent: running WHEN-VALIDATE-RECORD.
        /// </summary>
        public Task<IErrorsInfo> ValidateRecord()
            => _formsManager.Validation.ValidateRecordAsync(Name, GetCurrentRecordValues());

        /// <summary>Returns true if the current record passes all validation rules.</summary>
        public async Task<bool> IsRecordValid()
        {
            var result = await ValidateRecord();
            return result?.Flag == Errors.Ok;
        }

        #endregion
    }
}
```

---

## `GetCurrentRecordValues()` helper

This is a WinForms-side helper that reads the current component values into a flat dictionary.  
It must remain in `BeepDataBlock.cs` (or a new `BeepDataBlock.Helpers.cs` partial) — it is not delegated.

```csharp
// In BeepDataBlock.cs or BeepDataBlock.Helpers.cs
private Dictionary<string, object> GetCurrentRecordValues()
{
    var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    foreach (var (itemName, component) in UIComponents)
    {
        values[itemName] = component.Value;
    }
    return values;
}
```

---

## Files to delete

```
DataBlocks/Models/ValidationRule.cs          ← body was already commented out
DataBlocks/Helpers/ValidationRuleHelpers.cs
```

Before deleting, grep for usages:
```
grep -r "ValidationRule\|ValidationRuleHelpers\|ValidationBridge" --include="*.cs"
```
Remaining hits of `ValidationRule` will be in examples and test files — update them to use `TheTechIdea.Beep.Editor.Forms.Models.ValidationRule` (the FormsManager type).

---

## Caller update guide

| Old call | New call |
|---|---|
| `ValidationRuleHelpers.EmailRule(field)` | `block.RegisterValidationRule(field, ValidationRuleLibrary.Email(field))` |
| `ValidationRuleHelpers.RequiredRule(field, msg)` | `block.RegisterValidationRule(field, ValidationRuleLibrary.Required(field, msg))` |
| `ValidationRuleHelpers.RangeRule(field, min, max)` | `block.RegisterValidationRule(field, ValidationRuleLibrary.Range(field, min, max))` |
| `block._validationRules["CustomerName"]` | `block.GetValidationRules("CustomerName")` |
| `new ValidationRule { ValidationType = ValidationType.Email ... }` | Same — use FormsManager `ValidationRule` type from `TheTechIdea.Beep.Editor.Forms.Models` |

---

## Checklist

- [ ] Rewrite `BeepDataBlock.Validation.cs` to delegation-only methods
- [ ] Move `GetCurrentRecordValues()` to `BeepDataBlock.cs` or new `BeepDataBlock.Helpers.cs`
- [ ] Remove `ValidationBridge` usages
- [ ] Update `using` directives — remove old model refs, add FormsManager models
- [ ] Grep: `ValidationRuleHelpers|ValidationBridge|_validationRules` — zero hits
- [ ] Delete `Models/ValidationRule.cs`
- [ ] Delete `Helpers/ValidationRuleHelpers.cs`
- [ ] `dotnet build WinFormsApp.sln` — zero errors
