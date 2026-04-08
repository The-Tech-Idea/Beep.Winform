# Phase 09 — Interface Consolidation

**Repos:** Beep.Winform + BeepDM  
**Scope:** `IBeepDataBlock`, `IBeepDataBlockNotifier`, `BeepDataBlockEditorTemplate`, `BeepDataBlockFieldSelection`.  
**Depends on:** Phases 01–08 (all local state removed from BeepDataBlock partials).  
**Build check:** `dotnet build BeepDM.sln` + `dotnet build WinFormsApp.sln` — zero errors.

---

## Why this phase exists

After Phases 01–08 the `BeepDataBlock` implementation delegates everything. This phase aligns the WinForms-facing interfaces (`IBeepDataBlock`, `IBeepDataBlockNotifier`) with the cross-platform contracts created in Phase 01 so that FormsManager can work with blocks without knowing anything about WinForms. It also slims the two designer model classes to inherit from their BeepDM base types.

---

## Clean Code Rules

- `IBeepDataBlock` must not reference any WinForms namespace in its member list — only WinForms-specific additions are allowed.
- `IBeepDataBlockNotifier` only inherits and optionally adds members — no replacements.
- `BeepDataBlockEditorTemplate` and `BeepDataBlockFieldSelection` keep their WinForms attribute decorators but inherit all data properties from BeepDM.
- No duplicated properties in derived classes — if the base has it, remove it from the derived class.

---

## 9.1 — `IBeepDataBlock.cs`

`IBeepDataBlock` now extends `IDataBlockController` (Phase 01.2).  
Remove any member that moved to `IDataBlockController`.  
Keep only WinForms-specific additions.

```csharp
using System.Collections.Generic;
using TheTechIdea.Beep.Editor.Forms.Interfaces;  // IDataBlockController
using TheTechIdea.Beep.Vis.Modules;              // IBeepUIComponent

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// WinForms-specific extension of the cross-platform <see cref="IDataBlockController"/>.
    /// Adds WinForms component management and master-detail wiring.
    /// </summary>
    public interface IBeepDataBlock : IDataBlockController
    {
        // ── WinForms component registry ───────────────────────────────────────

        /// <summary>
        /// WinForms control references keyed by item name.
        /// Not serialised; not visible to FormsManager.
        /// </summary>
        Dictionary<string, IBeepUIComponent> UIComponents { get; }

        // ── Master–detail (WinForms-specific wiring) ──────────────────────────

        /// <summary>Receives a master record object and refreshes the block.</summary>
        void SetMasterRecord(object masterRecord);

        /// <summary>Removes a child block relationship.</summary>
        void RemoveChildBlock(IBeepDataBlock childBlock);

        /// <summary>Removes the parent block relationship.</summary>
        void RemoveParentBlock();
    }
}
```

---

## 9.2 — `IBeepDataBlockNotifier.cs`

Extend `IDataBlockNotifier` (Phase 01.1).  
No new members needed unless WinForms adds something over the base interface.

```csharp
using TheTechIdea.Beep.Editor.Forms.Interfaces;  // IDataBlockNotifier

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// WinForms-specific extension of <see cref="IDataBlockNotifier"/>.
    /// Default implementation: <see cref="MessageBoxBeepDataBlockNotifier"/>.
    /// </summary>
    public interface IBeepDataBlockNotifier : IDataBlockNotifier
    {
        // All base members are inherited.
        // Add WinForms-specific members here only if genuinely needed.
    }
}
```

`MessageBoxBeepDataBlockNotifier` — no body changes required.  
It already satisfies the interface signature; adding `IBeepDataBlockNotifier` to the implements list is enough.

---

## 9.3 — `BeepDataBlockFieldSelection.cs`

Inherit `FieldSelectionInfo` (Phase 01.3).  
Delete all properties that are already on the base class.  
Keep WinForms attribute decorators and the `EditorTypeOverride` computed property.

```csharp
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Editor.Forms.Models;            // FieldSelectionInfo

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// WinForms designer-aware version of <see cref="FieldSelectionInfo"/>.
    /// Inherits all data properties; adds designer attributes and the
    /// <see cref="EditorTypeOverride"/> computed property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepDataBlockFieldSelection : FieldSelectionInfo
    {
        // All data properties (FieldName, IncludeInView, ControlTypeFullName, etc.)
        // are on the base class — do NOT redeclare them here.

        /// <summary>
        /// Resolves <see cref="FieldSelectionInfo.EditorTypeOverrideFullName"/> to a
        /// live <see cref="Type"/> via the current <see cref="AppDomain"/>.
        /// Returns null if the type cannot be resolved.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public Type EditorTypeOverride
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EditorTypeOverrideFullName))
                    return null;

                return Type.GetType(EditorTypeOverrideFullName, throwOnError: false);
            }
        }
    }
}
```

---

## 9.4 — `BeepDataBlockEditorTemplate.cs`

Same pattern as 9.3 — inherit `EditorTemplateInfo` (Phase 01.4).

```csharp
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Editor.Forms.Models;            // EditorTemplateInfo

namespace TheTechIdea.Beep.Winform.Controls.Integrated.Models
{
    /// <summary>
    /// WinForms designer-aware version of <see cref="EditorTemplateInfo"/>.
    /// Inherits all data properties; adds the <see cref="EditorType"/> computed property.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BeepDataBlockEditorTemplate : EditorTemplateInfo
    {
        // All data properties (TemplateId, DisplayName, EditorTypeFullName, etc.)
        // are on the base class — do NOT redeclare them here.

        /// <summary>
        /// Resolves <see cref="EditorTemplateInfo.EditorTypeFullName"/> to a live
        /// <see cref="Type"/> via the current <see cref="AppDomain"/>.
        /// </summary>
        [Browsable(false)]
        [JsonIgnore]
        public Type EditorType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EditorTypeFullName))
                    return null;

                return Type.GetType(EditorTypeFullName, throwOnError: false);
            }
        }
    }
}
```

---

## FormsManager side — switch from `IBeepDataBlock` to `IDataBlockController`

Search BeepDM for all parameter types and return types that still reference `IBeepDataBlock`:
```
grep -r "IBeepDataBlock" --include="*.cs" DataManagementEngineStandard/
```

Any FormsManager internal method that accepted `IBeepDataBlock` should now accept `IDataBlockController`.  
`BlockRelationship.MasterBlock` and `DetailBlock` should be typed as `IDataBlockController` (or just use string names — FormsManager works by block name, not by direct object reference).

---

## Checklist

- [ ] Update `IBeepDataBlock` to extend `IDataBlockController` — remove duplicate members
- [ ] Update `IBeepDataBlockNotifier` to extend `IDataBlockNotifier`
- [ ] Slim `BeepDataBlockFieldSelection` — inherit `FieldSelectionInfo`, delete duplicate properties
- [ ] Slim `BeepDataBlockEditorTemplate` — inherit `EditorTemplateInfo`, delete duplicate properties
- [ ] Add `IBeepDataBlockNotifier` to `MessageBoxBeepDataBlockNotifier : ...` implements clause
- [ ] Grep BeepDM code for `IBeepDataBlock` — replace with `IDataBlockController`
- [ ] `dotnet build BeepDM.sln` — zero errors
- [ ] `dotnet build WinFormsApp.sln` — zero errors
