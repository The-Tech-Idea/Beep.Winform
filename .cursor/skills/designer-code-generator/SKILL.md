---
name: designer-code-generator
description: Guidance for the DesignerCodeGenerator in the IDE Extensions that generates WinForms Designer.cs code for BeepDataBlock, BeepBlock, BeepForms, and integrated bootstrap registrations. Use when modifying or debugging code generation patterns.
---

# Designer Code Generator Guide

Use this skill when working with the regex-based `Designer.cs` code generator that scaffolds Oracle Forms-style blocks into WinForms projects.

## File Locations
- `Beep.Desktop\TheTechIdea.Beep.Desktop.IDE.Extensions\Helpers\DesignerCodeGenerator.cs` (~5000 lines)
- `Beep.Desktop\TheTechIdea.Beep.Desktop.IDE.Extensions\Helpers\WinFormsScanner.cs` (parser side)
- `Beep.Desktop\TheTechIdea.Beep.Desktop.IDE.Extensions\Helpers\IntegratedFieldScaffoldDefaults.cs` (field defaults)

## Architecture

The code generator uses **regex-based text manipulation** on `*.Designer.cs` files. It does NOT use Roslyn — it operates on raw C# text with marker-based region management.

### Generated Marker Regions

| Marker | Purpose |
|--------|---------|
| `copilot-generated-integrated-block:{name}` | Block definition section |
| `copilot-generated-integrated-bootstrap-shared` | Shared FormsManager references |
| `copilot-generated-integrated-bootstrap-hooks` | Bootstrap method scaffolding |
| `copilot-generated-integrated-bootstrap-registrations` | Block registration calls |
| `copilot-generated-integrated-relationship-registrations` | Master-detail relationship setup |
| `copilot-generated-integrated-lov-registrations` | LOV field registrations |
| `copilot-generated-integrated-validation-registrations` | Validation rule registrations |
| `copilot-generated-integrated-trigger-registrations` | Trigger handler registrations |

### Code Generation Pipeline

```
User Action (IDE Dialog)
    │
    ▼
BlockItemWorkflowCoordinator / ConnectionWorkflowCoordinator
    │
    ▼
DesignerCodeGenerator.EnsureBeepDataConnectionComponentAsync()   ← Connection component
DesignerCodeGenerator.EnsureBeepFormsHostAsync()                 ← BeepForms host
DesignerCodeGenerator.AddBeepBlockDefinitionToBeepFormsAsync()   ← Block definition
DesignerCodeGenerator.AddIntegratedFieldToBeepBlockDefinitionAsync() ← Field definitions
DesignerCodeGenerator.AddBeepFormsRuntimeBootstrapRegistrationAsync() ← Runtime wiring
    │
    ▼
DesignerCodeGenerator.AddBeepFormsRuntimeRelationshipRegistrationAsync() ← Master-detail
DesignerCodeGenerator.AddBeepFormsRuntimeLovRegistrationAsync()          ← LOVs
DesignerCodeGenerator.AddBeepFormsRuntimeValidationRegistrationAsync()   ← Validation
DesignerCodeGenerator.AddBeepFormsRuntimeTriggerRegistrationAsync()      ← Triggers
```

## Key Fully-Qualified Type Names

```csharp
"TheTechIdea.Beep.Winform.Controls.BeepDataBlock"                          // Legacy
"TheTechIdea.Beep.Winform.Controls.Integrated.Forms.BeepForms"             // Modern host
"TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models.BeepFormsDefinition"
"TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models.BeepBlockDefinition"
"TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models.BeepFieldDefinition"
"TheTechIdea.Beep.Winform.Controls.BeepDataConnection"                     // Connection component
"TheTechIdea.Beep.Editor.UOWManager.FormsManager"                          // Runtime engine
```

## Working Rules

1. **All generated code is marker-delimited.** Use `// <copilot-generated-...>` markers for every inserted section. Removal uses these markers exclusively.
2. **Never modify code outside markers.** The generator only inserts/removes within its own marker regions.
3. **Variable names are unique.** Use `BuildUniqueIdentifier()` for every generated variable to avoid collisions.
4. **Whitespace is significant.** The generator preserves surrounding line breaks to keep `Designer.cs` valid.
5. **Cancellation support.** All public methods accept `CancellationToken` and call `ThrowIfCancellationRequested()`.
6. **No post-generation validation.** The generator does NOT verify that generated code compiles. This is a known gap (tracker item I7).

## Integration Points

### WinFormsScanner (Parser Side)
`WinFormsScanner` uses matching regex patterns to parse generated code back. **Any change to generator output patterns must be mirrored in the scanner.** This is the "boomerang coupling" risk (tracker item I4).

### BeepDataBlockConverter (Sync Side)
`BeepDataBlockConverter.SyncToFormDesignerAsync()` uses the generator's `UpdateBlockDefinition*Async()` methods to push changes from the IDE back to `Designer.cs`.

### IntegratedFieldScaffoldDefaults (Field Defaults)
Field control types and binding properties are resolved through `IntegratedFieldScaffoldDefaults` which delegates to `BeepFieldControlTypeRegistry`.

## Pitfalls

1. **Regex fragility.** Unusual `Designer.cs` formatting (custom code styles, comments between property assignments) can break parsing.
2. **No Roslyn validation.** Generated code may not compile. Post-generation build is the responsibility of the caller.
3. **Duplicate generation guard.** Each method checks for existing generated code before inserting — but the check is marker-based, not semantic.
4. **Thread safety.** Methods read/write `Designer.cs` via `File.ReadAllTextAsync`/`WriteAllTextAsync` — concurrent writes to the same file from different threads will corrupt the file.
5. **Large files.** The generator operates on the entire `Designer.cs` as a single string — large forms with many controls can cause OOM.

## Legacy BeepDataBlock Support

The generator also supports legacy `BeepDataBlock` via:
- `AddBeepDataBlockToFormAsync()` / `RemoveBeepDataBlockFromFormAsync()` — field declaration + initialization code
- Legacy blocks use `FullyQualifiedBeepDataBlockTypeName` = `TheTechIdea.Beep.Winform.Controls.BeepDataBlock`

## Related Skills
- [`winform-integrated-ide`](../winform-integrated-ide/SKILL.md) — IDE extension overview
- [`datablock-connection-integration`](../datablock-connection-integration/SKILL.md) — Block-connection wiring
- [`forms`](../forms/SKILL.md) — FormsManager orchestration

## Detailed Reference
Use [`reference.md`](./reference.md) for code generation examples and marker formats.
