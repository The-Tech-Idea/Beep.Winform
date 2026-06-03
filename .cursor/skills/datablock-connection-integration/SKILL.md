---
name: datablock-connection-integration
description: Guidance for integrating BeepBlock data surfaces with BeepDataConnection configuration in the Beep.Winform.Data.Integrated framework. Use when building Oracle Forms-style CRUD forms that connect to datasources, or when wiring connection management into block-level UI.
---

# Data Block & Connection Integration

Use this skill when connecting `BeepBlock` UI surfaces to datasource connections managed by `BeepDataConnection`.

## Architecture Rule

```
BeepBlock (UI-only)
    │
    ├─► IBeepFormsHost (proxy layer)
    │       │
    │       └─► FormsManager (IUnitofWorksManager)
    │               │
    │               └─► IDataSource (actual connection)
    │
    └─► BeepDataConnection (design-time config)
            │
            ├─► BeepConnectionRepository
            │       └─► IConnectionStorageProvider (JSON files)
            │
            └─► ConnectionSecretProtector (DPAPI encryption)
```

**BeepBlock never touches IDataSource directly.** All data access goes through `IBeepFormsHost` → `FormsManager`.

## File Locations
- `Beep.Winform.Data.Integrated\Beep.Winform.Data.Integrated.Controls\Blocks\BeepBlock\`
- `Beep.Winform.Data.Integrated\Beep.Winform.Data.Integrated.Controls\DataConnection\`
- `BeepDM\DataManagementEngineStandard\Editor\Forms\FormsManager*.cs`

## Key Integration Points

### 1. Setting Up a Connection (Design-Time)
```csharp
// Drop BeepDataConnection on form
var dataConnection = new BeepDataConnection();
dataConnection.AppRepoName = "MyApp";
dataConnection.DirectoryPath = AppContext.BaseDirectory;
dataConnection.AddOrUpdateConnection(new ConnectionProperties
{
    ConnectionName = "Northwind",
    DatabaseType = DataSourceType.SqlServer,
    Host = "localhost",
    Database = "Northwind",
    UserID = "sa",
    Password = "..."
});
```

### 2. Binding a Block to a Connection (Runtime)
```csharp
// In form code-behind, after FormsManager setup:
beepForms.FormsManager = formsManager;
beepForms.Definition = new BeepFormsDefinition
{
    Blocks = { new BeepBlockDefinition
    {
        BlockName = "Customers",
        ManagerBlockName = "Customers",
        Entity = new BeepBlockEntityDefinition
        {
            ConnectionName = "Northwind",  // matches BeepDataConnection
            EntityName = "Customers"
        }
    }}
};

await beepForms.InitializeAsync();
```

### 3. Setting Connection on BeepBlock (Designer or Code)
```csharp
// Direct property (new in Phase 1 hardening):
beepBlock.ConnectionName = "Northwind";
// Equivalent to:
beepBlock.Definition.Entity.ConnectionName = "Northwind";
```

### 4. Enumerating Available Connections
```csharp
// In BeepBlock context:
var available = beepBlock.GetAvailableConnectionNames();
// Returns connection names from FormsManager.DMEEditor.ConfigEditor.DataConnections
```

## Connection Scopes and Profiles

`BeepDataConnection` supports three scopes with precedence chains:

| Scope | File | Precedence |
|-------|------|-----------|
| Project | `project.connections.json` | Project → User → Machine |
| User | `user.connections.json` | User → Machine |
| Machine | `machine.connections.json` | Machine only |

Profiles group connections within a scope (default: "Default").

```csharp
dataConnection.PersistenceScope = ConnectionStorageScope.Project;
dataConnection.ActiveProfileName = "Production";
dataConnection.UseScopePrecedence = true;
```

## Async Operations (Phase 2)
```csharp
// Async load:
var connections = await storageProvider.LoadConnectionsAsync(
    ConnectionStorageScope.Project, "Default", includePrecedenceChain: true, ct);

// Async save:
await storageProvider.SaveConnectionsAsync(
    ConnectionStorageScope.Project, "Default", connections, ct);
```

## Pitfalls

1. **ConnectionName must match exactly.** The block's `ConnectionName` is compared case-insensitively against `ConnectionProperties.ConnectionName` in the config.
2. **Driver must be loaded.** The `SharedContextAssemblyHandler` must have loaded the driver class before `FormsManager.SetupBlockAsync` is called.
3. **Secrets are encrypted at rest.** `ConnectionSecretProtector` uses DPAPI (Windows Data Protection API). Passwords are prefixed `__enc__:` when stored.
4. **BeepBlock is UI-only.** Never call `IDataSource` directly from BeepBlock — route through `IBeepFormsHost`.
5. **Design-time vs runtime.** `BeepDataConnection` check `LicenseManager.UsageMode` to detect design-time and avoid heavy initialization.

## Related Skills
- [`forms`](../forms/SKILL.md) — FormsManager orchestration
- [`connection`](../../../../BeepDM/.cursor/connection/SKILL.md) — Connection management in BeepDM
- [`designer-code-generator`](../designer-code-generator/SKILL.md) — Designer.cs code generation
- [`winform-integrated-ide`](../winform-integrated-ide/SKILL.md) — IDE extension overview

## Detailed Reference
Use [`reference.md`](./reference.md) for end-to-end scenarios.
