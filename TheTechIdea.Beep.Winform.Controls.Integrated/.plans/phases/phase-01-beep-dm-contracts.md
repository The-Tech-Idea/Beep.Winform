# Phase 01 — Add Cross-Platform Contracts to BeepDM

**Repo:** BeepDM  
**Scope:** New files only — zero WinForms changes.  
**Depends on:** nothing (this is phase 1)  
**Blocking:** All other phases depend on these types existing.  
**Build check:** `dotnet build BeepDM.sln` — zero errors before moving to Phase 02.

---

## Why this phase exists

Later phases rewrite BeepDataBlock partials so that every delegate call uses FormsManager types directly (`TriggerDefinition`, `LOVDefinition`, `ItemInfo`, etc.). Those types must exist in BeepDM first. This phase adds the four missing cross-platform contracts without touching any WinForms code.

---

## Clean Code Rules (apply to every file in this phase)

- One public type per file.
- File name matches type name exactly.
- No WinForms, no `System.Windows.Forms`, no `System.Drawing` refs.
- All properties use auto-properties unless a backing field is mandatory.
- XML doc on every public member.
- No business logic in these contract files — pure data / interface definitions.

---

## 1.1 — `IDataBlockNotifier`

**New file:** `DataManagementEngineStandard/Editor/Forms/Interfaces/IDataBlockNotifier.cs`

```csharp
using System;

namespace TheTechIdea.Beep.Editor.Forms.Interfaces
{
    /// <summary>
    /// Platform-neutral notification surface for data block feedback.
    /// WinForms implements this with MessageBox; Blazor with a toast service; etc.
    /// </summary>
    public interface IDataBlockNotifier
    {
        /// <summary>Displays an informational message.</summary>
        void ShowInfo(string message, string caption = "Information");

        /// <summary>Displays a warning message.</summary>
        void ShowWarning(string message, string caption = "Warning");

        /// <summary>Displays an error message.</summary>
        void ShowError(string message, string caption = "Error");

        /// <summary>
        /// Prompts the user for a yes / no decision.
        /// Returns <c>true</c> if the user confirms.
        /// </summary>
        bool Confirm(string message, string caption = "Confirm");
    }
}
```

---

## 1.2 — `IDataBlockController`

**New file:** `DataManagementEngineStandard/Editor/Forms/Interfaces/IDataBlockController.cs`

This is the platform-neutral surface extracted from `IBeepDataBlock`.  
All `IBeepUIComponent` references are replaced by `string componentId`.  
FormsManager uses this interface — it never references WinForms types.

```csharp
using System.Collections.Generic;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor.UOWManager;
using TheTechIdea.Beep.Editor.Forms.Models;

namespace TheTechIdea.Beep.Editor.Forms.Interfaces
{
    /// <summary>
    /// Platform-neutral contract for a data block.
    /// Implemented by BeepDataBlock (WinForms), WpfDataBlock (WPF),
    /// BlazorDataBlock (Blazor), etc.
    /// </summary>
    public interface IDataBlockController
    {
        // ── Identity ──────────────────────────────────────────────────────────

        /// <summary>Unique block name within the form.</summary>
        string Name { get; set; }

        /// <summary>Name of the form this block belongs to.</summary>
        string FormName { get; set; }

        // ── Data source ───────────────────────────────────────────────────────

        /// <summary>Unit of Work backing this block.</summary>
        IUnitofWork Data { get; set; }

        /// <summary>Entity structure metadata.</summary>
        IEntityStructure EntityStructure { get; }

        /// <summary>Entity fields resolved from the data source.</summary>
        List<EntityField> Fields { get; }

        /// <summary>Name of the data source connection.</summary>
        string ConnectionName { get; set; }

        /// <summary>Entity / table name in the data source.</summary>
        string EntityName { get; set; }

        // ── Mode ──────────────────────────────────────────────────────────────

        /// <summary>Current operational mode of the block.</summary>
        DataBlockMode BlockMode { get; set; }

        /// <summary>Returns true while the block is in Enter-Query mode.</summary>
        bool IsInQueryMode { get; }

        /// <summary>Switches to the given mode and fires the appropriate triggers.</summary>
        void SwitchBlockMode(DataBlockMode newMode);

        // ── Master–detail ─────────────────────────────────────────────────────

        /// <summary>Property name on the master record that drives this detail block.</summary>
        string MasterKeyPropertyName { get; set; }

        /// <summary>Property name on this block's entity that is the FK.</summary>
        string ForeignKeyPropertyName { get; set; }

        // ── Notifications ─────────────────────────────────────────────────────

        /// <summary>Called by FormsManager when data changes require UI refresh.</summary>
        void HandleDataChanges();

        /// <summary>Platform-specific notifier (MessageBox, toast, etc.).</summary>
        IDataBlockNotifier Notifier { get; }
    }
}
```

---

## 1.3 — `FieldSelectionInfo`

**New file:** `DataManagementEngineStandard/Editor/Forms/Models/FieldSelectionInfo.cs`

Pure data model — no `[TypeConverter]`, no `[DesignerSerializationVisibility]`, no WinForms.  
WinForms `BeepDataBlockFieldSelection` will inherit this in Phase 09.

```csharp
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Editor.Forms.Models
{
    /// <summary>
    /// Describes how a single entity field is represented in a data-block view.
    /// Platform-neutral; WinForms adds designer attributes on top of this base.
    /// </summary>
    public class FieldSelectionInfo
    {
        /// <summary>Name of the entity field (must match <see cref="EntityField.fieldname"/>).</summary>
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; } = string.Empty;

        /// <summary>Whether this field appears in the current view.</summary>
        [JsonPropertyName("includeInView")]
        public bool IncludeInView { get; set; } = true;

        /// <summary>
        /// Assembly-qualified name of the preferred editor control type.
        /// Empty means "use the default for this field's data type".
        /// </summary>
        [JsonPropertyName("controlTypeFullName")]
        public string ControlTypeFullName { get; set; } = string.Empty;

        /// <summary>ID of the editor template to apply (references <see cref="EditorTemplateInfo.TemplateId"/>).</summary>
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; } = string.Empty;

        /// <summary>Overrides the template's editor type for this field specifically.</summary>
        [JsonPropertyName("editorTypeOverrideFullName")]
        public string EditorTypeOverrideFullName { get; set; } = string.Empty;

        /// <summary>Serialised JSON blob of inline per-editor settings.</summary>
        [JsonPropertyName("inlineSettingsJson")]
        public string InlineSettingsJson { get; set; } = string.Empty;

        /// <summary>Display label shown to the user. Empty = use field name.</summary>
        [JsonPropertyName("labelText")]
        public string LabelText { get; set; } = string.Empty;

        /// <summary>When true the field is rendered read-only regardless of block mode.</summary>
        [JsonPropertyName("readOnly")]
        public bool ReadOnly { get; set; }
    }
}
```

---

## 1.4 — `EditorTemplateInfo`

**New file:** `DataManagementEngineStandard/Editor/Forms/Models/EditorTemplateInfo.cs`

Pure data model — same rules as `FieldSelectionInfo`.  
WinForms `BeepDataBlockEditorTemplate` will inherit this in Phase 09.

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Editor.Forms.Models
{
    /// <summary>
    /// Reusable editor template that can be applied to fields across multiple data blocks.
    /// </summary>
    public class EditorTemplateInfo
    {
        /// <summary>Unique identifier for this template.</summary>
        [JsonPropertyName("templateId")]
        public string TemplateId { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>Human-readable name shown in designer drop-downs.</summary>
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Assembly-qualified type name of the editor control.</summary>
        [JsonPropertyName("editorTypeFullName")]
        public string EditorTypeFullName { get; set; } = string.Empty;

        /// <summary>
        /// Comma-separated list of <c>FieldCategory</c> names this template supports.
        /// Empty means "supports all categories".
        /// </summary>
        [JsonPropertyName("supportedFieldCategoriesCsv")]
        public string SupportedFieldCategoriesCsv { get; set; } = string.Empty;

        /// <summary>Serialised JSON blob of default editor settings.</summary>
        [JsonPropertyName("settingsJson")]
        public string SettingsJson { get; set; } = string.Empty;

        /// <summary>Serialised JSON blob of default validation rules.</summary>
        [JsonPropertyName("validationJson")]
        public string ValidationJson { get; set; } = string.Empty;

        /// <summary>Schema version for forward-compatibility.</summary>
        [JsonPropertyName("version")]
        public int Version { get; set; } = 1;

        /// <summary>
        /// Returns true if this template advertises support for the given field category.
        /// Always returns true when <see cref="SupportedFieldCategoriesCsv"/> is empty.
        /// </summary>
        public bool SupportsCategory(string fieldCategory)
        {
            if (string.IsNullOrWhiteSpace(SupportedFieldCategoriesCsv))
                return true;

            foreach (var cat in SupportedFieldCategoriesCsv.Split(','))
            {
                if (string.Equals(cat.Trim(), fieldCategory, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
```

---

## Checklist

- [ ] Create `IDataBlockNotifier.cs`
- [ ] Create `IDataBlockController.cs`
- [ ] Create `FieldSelectionInfo.cs`
- [ ] Create `EditorTemplateInfo.cs`
- [ ] Add new files to `DataManagementEngineStandard.csproj` (if not using glob `**/*.cs`)
- [ ] `dotnet build BeepDM.sln` — zero errors
