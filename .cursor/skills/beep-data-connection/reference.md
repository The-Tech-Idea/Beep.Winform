# BeepDataConnection Reference

## Scenario 1 — Minimal Designer.cs setup

```csharp
// Component tray: beepDataConnection1
this.beepDataConnection1 = new BeepDataConnection();

this.beepDataConnection1.AppRepoName       = "MyApp";
this.beepDataConnection1.DirectoryPath     = "";          // application base
this.beepDataConnection1.PersistenceScope  = ConnectionStorageScope.Project;
this.beepDataConnection1.ActiveProfileName = "Default";
```

---

## Scenario 2 — Adding a connection at runtime

```csharp
var conn = new ConnectionProperties
{
    ConnectionName  = "NorthwindDB",
    DriverName      = "SQLite",
    ConnectionString = "Data Source=./data/northwind.db",
    DatabaseType    = DataSourceType.SqlLite,
    Category        = DatasourceCategory.RDBMS
};

// AddOrUpdate persists immediately (persist = true)
bool added = this.beepDataConnection1.ConnectionRepository.AddOrUpdate(conn, persist: true);

if (added)
{
    // In-memory snapshot refreshed automatically
    this.beepDataConnection1.ReloadConnections();
}
```

---

## Scenario 3 — Loading connections with scope precedence

```csharp
// UseScopePrecedence = true merges Project → User → Machine
// Project-level connections override User/Machine entries with the same name.

var repo = this.beepDataConnection1.ConnectionRepository;
repo.UseScopePrecedence = true;
repo.ActiveScope        = ConnectionStorageScope.Project;
repo.ActiveProfileName  = "Production";

// Returns merged list — most specific scope wins per connection name
var connections = repo.LoadConnections();
foreach (var c in connections)
{
    Console.WriteLine($"{c.ConnectionName} ({c.DatabaseType})");
}
```

---

## Scenario 4 — Promoting project connections to user scope

```csharp
// Share developer-tested connections with any user on this machine

var repo = this.beepDataConnection1.ConnectionRepository;
repo.ActiveScope        = ConnectionStorageScope.Project;
repo.ActiveProfileName  = "Default";

bool ok = repo.Promote(
    targetScope:     ConnectionStorageScope.User,
    conflictPolicy:  ConnectionConflictPolicy.MergeByGuid,
    message:         out string message);

Console.WriteLine(ok ? $"Promoted: {message}" : $"Failed: {message}");
```

---

## Scenario 5 — Exporting and importing a connection package

```csharp
// Export current project connections to a shareable package
var repo = this.beepDataConnection1.ConnectionRepository;

bool exported = repo.ExportPackage(
    scope:                      ConnectionStorageScope.Project,
    profileName:                "Default",
    packagePath:                "C:/shared/connections.pkg.json",
    includeEncryptedSecretsOnly: false,  // include plaintext secrets in export
    message:                    out string exportMsg);

// --- On another machine ---
bool imported = repo.ImportPackage(
    targetScope:        ConnectionStorageScope.Project,
    profileName:        "Default",
    packagePath:        "C:/shared/connections.pkg.json",
    conflictPolicy:     ConnectionConflictPolicy.Skip,
    importWhenEmptyOnly: false,
    message:             out string importMsg);
```

---

## Scenario 6 — Subscribing to connection changes

```csharp
// React whenever any connection is added, updated, removed, or synced

this.beepDataConnection1.ConnectionsChanged += (sender, e) =>
{
    // Refresh any UI that lists available connections
    var updated = this.beepDataConnection1.DataConnections;
    this.connectionComboBox.DataSource = updated
        .Select(c => c.ConnectionName)
        .ToList();
};
```

---

## Scenario 7 — Custom storage provider (encrypted cloud store)

```csharp
// Replace the default JsonConnectionStorageProvider with a custom one
// that reads/writes to Azure Key Vault or a shared database.

public class AzureKeyVaultConnectionStorageProvider : IConnectionStorageProvider
{
    public IReadOnlyList<ConnectionProperties> LoadConnections(
        ConnectionStorageScope scope, string profileName, bool includePrecedenceChain)
    {
        // ... retrieve from Key Vault
        return Array.Empty<ConnectionProperties>();
    }

    public bool AddOrUpdate(ConnectionStorageScope scope, string profileName,
        ConnectionProperties connection, bool persist)
    {
        // ... write to Key Vault
        return true;
    }

    // Implement remaining interface members ...
    public bool SaveConnections(...)   { ... }
    public bool Remove(...)           { ... }
    public bool Promote(...)          { ... }
    public bool ExportPackage(...)    { ... }
    public bool ImportPackage(...)    { ... }
}

// Inject into repository
var beepService = this.beepDataConnection1.BeepService!;
var customRepo  = new BeepConnectionRepository(beepService,
    new AzureKeyVaultConnectionStorageProvider());

// Replace the default repository on the component
this.beepDataConnection1.SetRepository(customRepo);
```

---

## Scenario 8 — Removing a connection

```csharp
// Remove a named connection from the current scope/profile

var repo = this.beepDataConnection1.ConnectionRepository;
bool removed = repo.Remove("NorthwindDB", persist: true);

if (removed)
{
    // ConnectionsChanged fires; DataConnections is stale until reload
    this.beepDataConnection1.ReloadConnections();
}
```

---

## Scenario 9 — Multi-profile environment switching

```csharp
// Switch the active profile to load staging connections
var repo = this.beepDataConnection1.ConnectionRepository;
repo.ActiveProfileName = "Staging";

var stagingConnections = repo.LoadConnections();
// stagingConnections contains only connections saved under "Staging" profile.

// Switch back to Default
repo.ActiveProfileName = "Default";
```
