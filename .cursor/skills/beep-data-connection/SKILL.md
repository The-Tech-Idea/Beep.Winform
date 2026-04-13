---
name: beep-data-connection
description: Guidance for the BeepDataConnection and BeepConnectionRepository components — the design-time and runtime connection management layer for integrated Beep WinForms controls. Use when configuring BeepDataConnection, managing ConnectionProperties scope and profiles, working with BeepConnectionRepository CRUD, promoting or exporting connection packages, customizing storage providers, or handling secret protection.
---

# BeepDataConnection

`BeepDataConnection` is a `Component` (non-visual, dropped in the designer tray) that owns connection management for WinForms forms using the integrated controls path. It wraps a `BeepConnectionRepository` which delegates persistence to an `IConnectionStorageProvider` (default: `JsonConnectionStorageProvider`) backed by `IBeepService`.

## Use this skill when
- Adding `BeepDataConnection` to a form or component
- Configuring `AppRepoName`, `DirectoryPath`, `PersistenceScope`, `ActiveProfileName`
- Loading, adding, updating, or removing `ConnectionProperties` at design time or runtime
- Promoting connections between scopes (Project → User → Machine)
- Exporting or importing connection packages (`ConnectionCatalogPackage`)
- Replacing the default `IConnectionStorageProvider` with a custom one
- Understanding how secrets (passwords, API keys, tokens) are encrypted with DPAPI

## Do not use this skill when
- The task is purely about the Beep `IDataSource` runtime API — use [`idatasource`](../idatasource/SKILL.md)
- The task is about `ConfigEditor` in BeepDM — use [`configeditor`](../configeditor/SKILL.md)

---

## Architecture — Components

### `BeepDataConnection` (Component)
Design-time component dropped on a form. Owns lifecycle of `IBeepService` and `BeepConnectionRepository`.

| Property | Type | Default | Purpose |
| --- | --- | --- | --- |
| `AppRepoName` | string | `"BeepPlatformConnections"` | Shared repository/container name for `BeepService` configuration |
| `DirectoryPath` | string | `""` | Base directory for `BeepService`; empty = `AppContext.BaseDirectory` |
| `PersistenceScope` | `ConnectionStorageScope` | `Project` | Active scope: `Project`, `User`, or `Machine` |
| `ActiveProfileName` | string | `"Default"` | Named connection set; supports multi-environment profiles |
| `DataConnections` | `List<ConnectionProperties>` | `[]` | In-memory snapshot, updated on `ReloadConnections()` |
| `BeepService` | `IBeepService?` | auto-init | Singleton BeepService instance owned by this component |

**Events:**
- `ConnectionsChanged` — fired by `BeepConnectionRepository` whenever a connection is added, updated, removed, saved, imported, or promoted.

### `BeepConnectionRepository` (sealed)
Centralized CRUD and persistence helper backed by `IBeepService.Config_editor`.

| Method | Purpose |
| --- | --- |
| `LoadConnections()` | Returns `IReadOnlyList<ConnectionProperties>` for current scope and profile |
| `AddOrUpdate(conn, persist)` | Upsert by `ConnectionName`; fires `ConnectionsChanged` |
| `Remove(name, persist)` | Remove by name; fires `ConnectionsChanged` |
| `Save(connections)` | Bulk save for current scope/profile |
| `Promote(targetScope, policy, out msg)` | Copy connections to a higher scope with conflict resolution |
| `ExportPackage(scope, profile, path, ...)` | Write `ConnectionCatalogPackage` JSON to file |
| `ImportPackage(targetScope, profile, path, ...)` | Read and merge `ConnectionCatalogPackage` from file |

**Constructor:**
```csharp
new BeepConnectionRepository(beepService, storageProvider?)
```
`storageProvider` defaults to `JsonConnectionStorageProvider`. Inject a custom one for encrypted stores, cloud storage, etc.

---

## Storage Model

### `ConnectionStorageScope` enum
```
Project   — per-project local, highest precedence when UseScopePrecedence = true
User      — per-user, mid precedence
Machine   — machine-wide, lowest precedence
```

### `ConnectionConflictPolicy` enum
Used by `Promote` and `ImportPackage`:
```
Replace      — overwrite existing by name
Rename       — keep old, add promoted with suffix
Skip         — skip if a connection with same name exists
MergeByGuid  — use GUID as identity (Connection.GuidID)
```

### `ConnectionCatalogPackage`
Portable export format containing `List<ConnectionCatalogRecord>`. Each record carries a full `ConnectionProperties` snapshot with scope/profile metadata and an export timestamp.

---

## Secret Protection (`ConnectionSecretProtector`)
Internal static helper used by `JsonConnectionStorageProvider` during save and load.

Protected fields: `Password`, `ApiKey`, `KeyToken`, `ClientSecret`, `ProxyPassword`, `ClientCertificatePassword`, `OAuthAccessToken`, `OAuthRefreshToken`, `OAuthClientSecret`, `AuthCode`.

- **Encrypt**: DPAPI `ProtectedData.Protect` with `DataProtectionScope.CurrentUser`; stores as `"__enc__:<base64>"`.
- **Decrypt**: Strips prefix, decodes base64, `ProtectedData.Unprotect`. Already-plaintext values are passed through unchanged.
- **Scope**: Protects in-flight `ConnectionProperties` clones only; originals are not mutated.

> Security note: DPAPI is current-user scoped. Machine-scope connections encrypted on one user account will fail to decrypt for another user. Plan accordingly, or replace the storage provider for cross-user scenarios.

---

## `IConnectionStorageProvider`
Implement to replace the default `JsonConnectionStorageProvider`:

```csharp
public interface IConnectionStorageProvider
{
    IReadOnlyList<ConnectionProperties> LoadConnections(
        ConnectionStorageScope scope, string profileName, bool includePrecedenceChain);
    bool SaveConnections(ConnectionStorageScope scope, string profileName,
        IReadOnlyList<ConnectionProperties> connections);
    bool AddOrUpdate(ConnectionStorageScope scope, string profileName,
        ConnectionProperties connection, bool persist);
    bool Remove(ConnectionStorageScope scope, string profileName,
        string connectionName, bool persist);
    bool Promote(ConnectionStorageScope sourceScope, ConnectionStorageScope targetScope,
        string profileName, ConnectionConflictPolicy conflictPolicy, out string message);
    bool ExportPackage(ConnectionStorageScope scope, string profileName, string packagePath,
        bool includeEncryptedSecretsOnly, out string message);
    bool ImportPackage(ConnectionStorageScope targetScope, string profileName,
        string packagePath, ConnectionConflictPolicy conflictPolicy,
        bool importWhenEmptyOnly, out string message);
}
```

---

## Scope Precedence Chain
When `UseScopePrecedence = true`, `LoadConnections` walks: `Project → User → Machine`, merging by connection name (most specific scope wins). This lets machine-level defaults be selectively overridden at user or project level.

---

## Design-Time Workflow
1. Drag `BeepDataConnection` into the component tray of a WinForms form.
2. Set `AppRepoName` and `DirectoryPath` if non-default.
3. Set `PersistenceScope` and `ActiveProfileName` for the deployment context.
4. Use the designer-side connection list editor to add or edit `ConnectionProperties`.
5. Connections are persisted to JSON under `DirectoryPath` keyed by scope/profile.
6. At runtime `BeepBlock.Definition.Entity.ConnectionName` matches `ConnectionProperties.ConnectionName`.

---

## File Locations
```
TheTechIdea.Beep.Winform.Controls.Integrated/
  DataConnection/
    BeepConnectionRepository.cs       ← CRUD + persistence + promote/export/import
    BeepDataConnection.cs             ← Component, design-time surface, owns IBeepService
    ConnectionCatalogModels.cs        ← ConnectionCatalogRecord / ConnectionCatalogPackage
    ConnectionSecretProtector.cs      ← DPAPI encrypt/decrypt helper (internal)
    IConnectionStorageProvider.cs     ← replaceable storage contract
    JsonConnectionStorageProvider.cs  ← default JSON + DPAPI implementation
```

## Related Skills
- [`beep-block`](../beep-block/SKILL.md) — uses `ConnectionName` to resolve entity metadata
- [`beep-forms-integrated`](../beep-forms-integrated/SKILL.md) — form host that owns BeepBlock instances
- [`connection`](../connection/SKILL.md) — lower-level `ConnectionProperties` and `IDataConnection`
- [`configeditor`](../configeditor/SKILL.md) — BeepDM ConfigEditor for server-side connection management

## Detailed Reference
Use [`reference.md`](./reference.md) for usage scenarios and code examples.
