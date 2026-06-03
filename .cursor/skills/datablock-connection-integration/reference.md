# Data Block & Connection Integration — Reference

## Scenario A: Full Bootstrap — Connection → Form → Block

```csharp
// 1. Create connection configuration
var dataConnection = new BeepDataConnection();
dataConnection.AppRepoName = "MyApp";
dataConnection.AddOrUpdateConnection(new ConnectionProperties
{
    ConnectionName = "Northwind",
    DatabaseType = DataSourceType.SqlServer,
    Host = "localhost",
    Database = "Northwind",
    UserID = "sa",
    Password = "***"
});

// 2. Create FormsManager
var formsManager = new FormsManager(editor);
await formsManager.SetupBlockAsync("Customers", "Northwind", "Customers", isMaster: true);

// 3. Create BeepForms host
var beepForms = new BeepForms();
beepForms.BlockName = "MainForm";
beepForms.FormsManager = formsManager;
beepForms.Definition = new BeepFormsDefinition
{
    FormName = "MainForm",
    Blocks =
    {
        new BeepBlockDefinition
        {
            BlockName = "Customers",
            Entity = new BeepBlockEntityDefinition
            {
                ConnectionName = "Northwind",
                EntityName = "Customers"
            }
        }
    }
};

// 4. Initialize (async bootstrap)
await beepForms.InitializeAsync();

// 5. At this point, BeepBlock inside BeepForms is bound to FormsManager
// and can execute queries, navigate records, etc.
```

## Scenario B: Adding a Block at Runtime

```csharp
var block = new BeepBlock
{
    BlockName = "Orders",
    ConnectionName = "Northwind"
};
block.Definition = new BeepBlockDefinition
{
    BlockName = "Orders",
    ManagerBlockName = "Orders",
    Entity = new BeepBlockEntityDefinition
    {
        ConnectionName = "Northwind",
        EntityName = "Orders"
    }
};

// Bind to existing BeepForms host
beepForms.RegisterBlock(block);
block.Bind(beepForms);
```

## Scenario C: Changing Connection at Runtime

```csharp
// Update the block's connection
beepBlock.ConnectionName = "ProductionDB";

// The FormsManager must re-register the block with the new connection
var managerBlockName = beepBlock.ManagerBlockName;
await formsManager.UnregisterBlockAsync(managerBlockName);
await formsManager.SetupBlockAsync(managerBlockName, "ProductionDB", beepBlock.Definition.Entity.EntityName);
beepBlock.SyncFromManager();
```

## Scenario D: Promoting Connections Between Scopes

```csharp
// Promote project-local connections to user-scope (shared across projects)
var result = dataConnection.PromoteConnections(
    ConnectionStoreKind.ProjectLocal,
    ConnectionStoreKind.Shared,
    ConnectionConflictPolicy.MergeByGuid,
    out var message);

// Export for team sharing
dataConnection.ExportEmbeddedDefaults(
    "team-connections.json",
    includeEncryptedSecretsOnly: true,
    out message);
```

## Scenario E: Connection Validation at Block Level

```csharp
// BeepBlock now validates connection existence (Phase 1 hardening)
beepBlock.ConnectionName = "NonExistentDB";
// Debug output: [BeepBlock] ConnectionName 'NonExistentDB' set on block 'Customers'
// but the connection is not in the available list. Runtime binding may fail.

// Check availability before setting:
var available = beepBlock.GetAvailableConnectionNames();
if (available.Contains("Northwind", StringComparer.OrdinalIgnoreCase))
{
    beepBlock.ConnectionName = "Northwind";
}
```

## Scenario F: Async Connection Storage (Phase 2)

```csharp
var storage = new JsonConnectionStorageProvider(beepService);

// Load all connections asynchronously
var connections = await storage.LoadConnectionsAsync(
    ConnectionStorageScope.Project,
    "Default",
    includePrecedenceChain: true,
    cancellationToken);

// Save with scope/profile awareness
await storage.SaveConnectionsAsync(
    ConnectionStorageScope.User,
    "Production",
    connections,
    cancellationToken);
```

## Verification Checklist

- [ ] `ConnectionName` is set on `BeepBlock` (via property or `Definition.Entity.ConnectionName`)
- [ ] Connection exists in `BeepDataConnection.DataConnections`
- [ ] Driver class is loaded by `SharedContextAssemblyHandler`
- [ ] `FormsManager.SetupBlockAsync` succeeds with the connection name
- [ ] `BeepBlock.IsBound` is `true` after `Bind(formsHost)` call
- [ ] `GetAvailableConnectionNames()` returns the expected connections
- [ ] Property change notifications fire on `BeepDataConnection` via `INotifyPropertyChanged`
