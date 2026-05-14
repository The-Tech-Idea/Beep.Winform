# BeepCloudSyncManager — Extension Contract

`BeepCloudSyncManager` synchronises named workspace layouts between the local
`WorkspaceManager` and a remote `ICloudSyncProvider` (local file share, Azure Blob
Storage, or any custom backend).

---

## Quick Start

### Preferred — manager-based (survives view switches)

```csharp
// 1. Create settings
var settings = new BeepCloudSyncSettings
{
    ProviderType     = "LocalFile",
    ConnectionString = @"\\team-share\beep-workspaces",
    SyncOnSave       = true,
    SyncInterval     = TimeSpan.FromMinutes(5)
};

// 2. Create provider
var provider = BeepCloudSyncManager.CreateProvider(settings);

// 3. Create manager bound to BeepDocumentManager (not the host directly)
//    This way, if the user switches view modes (Tabbed → NativeMdi) the sync
//    manager stays wired without any additional code.
using var cloudSync = new BeepCloudSyncManager(provider, documentManager, settings);

// 4. React to events (optional)
cloudSync.Uploaded        += (s, e) => statusBar.Text = $"Uploaded: {e.WorkspaceName}";
cloudSync.Downloaded      += (s, e) => statusBar.Text = $"Downloaded: {e.WorkspaceName}";
cloudSync.ConflictDetected += (s, e) => e.ResolvedContent = e.LocalContent; // prefer local
cloudSync.SyncError       += (s, e) => Log.Error(e.Message);

// 5. Trigger a manual full sync
await cloudSync.SyncWorkspacesAsync();
```

### Legacy — host-based

```csharp
// Only use when BeepDocumentManager is not available.
// Note: the sync manager becomes stale if the host is replaced during a view switch.
using var cloudSync = new BeepCloudSyncManager(provider, host.Workspaces, settings);
```

---

## ICloudSyncProvider Contract

Implement `ICloudSyncProvider` for any key/value remote store.  Keys are
slash-separated UTF-8 paths (e.g. `workspaces/my-layout.json`); values are UTF-8
text blobs.

```csharp
public interface ICloudSyncProvider
{
    Task           UploadAsync(string key, string content, CancellationToken ct = default);
    Task<string?>  DownloadAsync(string key, CancellationToken ct = default);
    Task<IReadOnlyList<string>> ListAsync(string prefix, CancellationToken ct = default);
    Task           DeleteAsync(string key, CancellationToken ct = default);
}
```

Built-in implementations:

| Class | Notes |
|-------|-------|
| `LocalFileSyncProvider` | Maps keys to file paths under a root directory.  Good for UNC shares and testing. |
| `AzureBlobSyncProvider` | Requires `Azure.Storage.Blobs` NuGet package.  Pass Azure Storage connection string in `BeepCloudSyncSettings.ConnectionString`. |

---

## BeepCloudSyncSettings Reference

| Property | Default | Description |
|----------|---------|-------------|
| `ProviderType` | `"LocalFile"` | `"LocalFile"` or `"AzureBlob"`.  Passed to `CreateProvider()`. |
| `ConnectionString` | `null` | Root directory for `LocalFile`; Azure Storage connection string for `AzureBlob`. |
| `ContainerName` | `"beep-workspaces"` | Blob container name (AzureBlob only). |
| `SyncOnSave` | `false` | When `true`, uploads a workspace automatically after every `WorkspaceSaved` event. |
| `SyncInterval` | `TimeSpan.Zero` | Polling interval for background sync.  Set to `TimeSpan.Zero` to disable the timer. |

---

## Events

| Event | Raised when |
|-------|-------------|
| `Uploaded` | A workspace was successfully pushed to the remote store. |
| `Downloaded` | A remote workspace was pulled and saved locally. |
| `ConflictDetected` | Both local and remote versions differ.  Set `e.ResolvedContent` to pick the winner; the default is last-write-wins (remote). |
| `SyncError` | Any upload, download, or list operation fails. |

---

## How View-Switch Safety Works

`BeepDocumentManager` tracks which `BeepDocumentHost` is currently active.  When a
view switch occurs (Phase 03: Tabbed → NativeMdi or vice-versa), the manager calls
`RefreshWorkspaceHostBinding()`, which:

1. Unsubscribes `WorkspaceSaved` / `WorkspaceDeleted` / `WorkspaceSwitched` handlers
   from the old host.
2. Subscribes them on the new host.
3. Re-raises each event as a manager-level event (`BeepDocumentManager.WorkspaceSaved`,
   etc.).

`BeepCloudSyncManager` subscribes to **manager-level** events, so it automatically
follows the active host without any code change.

---

## Key Layout

Workspaces are stored remotely under the `workspaces/` prefix:

```
workspaces/{WorkspaceDefinition.Id}.json   ← one file per workspace
```

Each file is the full JSON serialisation of `WorkspaceDefinition`.

---

## Conflict Resolution

When a workspace's local and remote content differ, `ConflictDetected` is raised.
If no handler sets `e.ResolvedContent`, the **remote version wins** (last-write-wins).

```csharp
cloudSync.ConflictDetected += (s, e) =>
{
    // Compare timestamps in the payloads if available, else prefer local.
    e.ResolvedContent = e.LocalContent;
};
```

---

## Disposal

Always dispose `BeepCloudSyncManager` when the form closes.  The constructor's
`settings.SyncOnSave` handler is unsubscribed automatically in `Dispose()`.

```csharp
protected override void OnFormClosed(FormClosedEventArgs e)
{
    _cloudSync?.Dispose();
    base.OnFormClosed(e);
}
```
